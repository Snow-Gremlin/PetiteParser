using PetiteParser.Grammar;
using PetiteParser.Misc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PetiteParser.Normalizer;

/// <summary>Removes all direct and indirect left recursion in this grammar.</summary>
/// <see cref="https://handwiki.org/wiki/Left_recursion"/>
sealed internal class RemoveLeftRecursion : IAction {

    /// <summary>Performs this action on the given grammar.</summary>
    /// <param name="analyzer">The analyzer to perform this action on.</param>
    /// <param name="log">The log to write notices, warnings, and errors.</param>
    /// <returns>True if the grammar was changed.</returns>
    public bool Perform(Analyzer.Analyzer analyzer, Logger.ILogger log) {
        List<Term> terms = analyzer.FindFirstLeftRecursion();
        if (terms is null || terms.Count <= 0) return false;

        log?.AddNoticeF("Found first left recursion in [{0}].", terms.Join(", "));

        try {
            Rule? rule = getRuleToChange(analyzer, terms);
            if (rule is null) return false;
            removeIndirection(analyzer, rule, terms);
            removeLeftRecursion(analyzer, terms[0]);
        } catch (Exception e) {
            throw new NormalizerException("Failed to fix left recursion in [" + terms.Join(", ") + "]", e);
        }
        return true;
    }

    /// <summary>Gets the rule from the first term to the next in the loop.</summary>
    /// <param name="analyzer">The analyzer to use to find the rule.</param>
    /// <param name="terms">The terms creating the recursive path.</param>
    /// <returns>The rule between the first and next term in the loop, or null if not found.</returns>
    static private Rule? getRuleToChange(Analyzer.Analyzer analyzer, List<Term> terms) =>
        analyzer.FirstRuleBetween(terms[0], terms[Math.Max(0, terms.Count - 1)]);

    /// <summary>
    /// Replaces a term in the rule's items with new items. Any terms before the replacement
    /// are removed under the assumption that they have a lambda rule.
    /// </summary>
    /// <param name="rule">The rule to get the items from.</param>
    /// <param name="replace">The term to replace in the rule's item.</param>
    /// <param name="newItems">The items to inject into the rule's items.</param>
    /// <returns>The new list of items with the injection in it.</returns>
    static private List<Item> injectIntoRule(Rule rule, Term replace, List<Item> newItems) {
        int index = rule.Items.IndexOf(replace);
        return rule.Items.Take(index - 1).Where(i => i is not Term).
            Concat(newItems).
            Concat(rule.Items.Skip(index)).
            ToList();
    }

    /// <summary>
    /// Replaces a term in the rule's items with new items. Any terms before the replacement
    /// are removed under the assumption that they have a lambda rule.
    /// </summary>
    /// <param name="analyzer">The analyzer to use to find the rule.</param>
    /// <param name="parent">The parent term for the rule to find and inject into.</param>
    /// <param name="child">The child the rule should reach and replace.</param>
    /// <param name="newItems">The items to inject into the rule's items.</param>
    /// <returns>The new list of items with the injection in it.</returns>
    static private List<Item> injectIntoRule(Analyzer.Analyzer analyzer, Term parent, Term child, List<Item> newItems) {
        Rule? rule = analyzer.FirstRuleBetween(parent, child);
        return rule is not null ? injectIntoRule(rule, child, newItems) :
            throw new NormalizerException("Failed to find first rule between " + parent + " and " + child + " while removing left recursion.");
    }

    /// <summary>Removes any indirection from a recursion.</summary>
    /// <param name="analyzer">The analyzer to use to find the rules.</param>
    /// <param name="rule">The first rule in the path that is to be updated.</param>
    /// <param name="terms">The terms creating the recursive path.</param>
    static private void removeIndirection(Analyzer.Analyzer analyzer, Rule rule, List<Term> terms) {
        if (terms.Count <= 1) return;

        List<Item> newItems = new() { rule.Term };
        newItems = injectIntoRule(analyzer, terms[^1], terms[0], newItems);
        for (int i = terms.Count - 1; i > 1; --i)
            newItems = injectIntoRule(analyzer, terms[i - 1], terms[i], newItems);
        newItems = injectIntoRule(rule, terms[1], newItems);

        rule.Items.Clear();
        rule.Items.AddRange(newItems);
    }

    /// <summary>Removes the direct left recursion path from the grammar.</summary>
    /// <param name="terms">The left recursion path.</param>
    static private void removeLeftRecursion(Analyzer.Analyzer analyzer, Term term) {
        Term prime = analyzer.Grammar.AddRandomTerm(term.Name);
        prime.NewRule(); // Add lambda
        for (int i = term.Rules.Count - 1; i >= 0; --i) {
            Rule rule = term.Rules[i];
            Item? firstBasicItem = rule.BasicItems.FirstOrDefault();
            if (firstBasicItem is null || firstBasicItem != term) {
                rule.Items.Add(prime);
            } else {
                term.Rules.RemoveAt(i);
                rule.Items.Remove(term);
                Rule newRule = prime.NewRule();
                newRule.Items.AddRange(rule.Items);
                newRule.Items.Add(prime);
            }
        }
    }
}
