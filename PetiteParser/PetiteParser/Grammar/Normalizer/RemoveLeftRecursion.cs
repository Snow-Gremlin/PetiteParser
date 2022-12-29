using PetiteParser.Formatting;
using PetiteParser.Misc;
using System.Collections.Generic;
using System.Linq;

namespace PetiteParser.Grammar.Normalizer;

/// <summary>Removes all direct and indirect left recursion in this grammar.</summary>
sealed internal class RemoveLeftRecursion : IPrecept {

    /// <summary>Performs this action on the given grammar.</summary>
    /// <param name="analyzer">The analyzer to perform this action on.</param>
    /// <param name="log">The log to write notices, warnings, and errors.</param>
    /// <returns>True if the grammar was changed.</returns>
    public bool Perform(Analyzer.Analyzer analyzer, Logger.ILogger? log) {
        List<Term> terms = analyzer.FindFirstLeftRecursion();
        if (terms is null || terms.Count <= 0) return false;

        log?.AddNoticeF("Found first left recursion in [{0}].", terms.Join(", "));
        Term target = terms[^1];
        if (terms.Count > 1)
            removeIndirection(target, terms.GetRange(..^1));
        removeLeftRecursion(analyzer, target);
        return true;
    }

    /// <summary>Removes any indirection from a recursion.</summary>
    /// <param name="target">The target term to remove indirection from.</param>
    /// <param name="rest">The remaining terms creating the recursive path.</param>
    static private void removeIndirection(Term target, List<Term> rest) {
        foreach (Term source in rest) substitute(target, source);
    }

    /// <summary>Substitute a single step in the path of the indirect left recursion.</summary>
    /// <param name="target">The target which is substituted into.</param>
    /// <param name="source">The source to substitute into the target.</param>
    static private void substitute(Term target, Term source) {
        List<Rule> rules = target.Rules.Where(r => ReferenceEquals(r.BasicItems.FirstOrDefault(), source)).ToList();
        foreach (Rule rule in rules) {
            target.Rules.Remove(rule);

            int index = rule.Items.IndexOf(source);
            List<Item> head = rule.Items.GetRange(..index);
            List<Item> tail = rule.Items.GetRange((index + 1)..);

            foreach (Rule other in source.Rules) {
                List<Item> newItems = target.NewRule().Items;
                newItems.AddRange(head);
                newItems.AddRange(other.Items);
                newItems.AddRange(tail);
            }
        }
    }

    /// <summary>Removes the direct left recursion path from the grammar.</summary>
    /// <param name="analyzer">The analyzer the given term belongs to.</param>
    /// <param name="term">The term with the left recursive that needs to be removed.</param>
    static private void removeLeftRecursion(Analyzer.Analyzer analyzer, Term term) {
        Term prime = analyzer.Grammar.AddGeneratedTerm(term.Name);
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
