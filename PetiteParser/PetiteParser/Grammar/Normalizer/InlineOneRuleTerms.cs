using PetiteParser.Logger;
using PetiteParser.Misc;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace PetiteParser.Grammar.Normalizer;

/// <summary>
/// In-lines any term which has only one rule
/// by replacing the one usage with all the items for the rule.
/// </summary>
/// <remarks>
/// The point is to make the longest rules such that" shift" is used more than "reduce:.
/// This may create more states but helps with reducing conflicts since
/// shifts are performed in parallel while working in states where as reduce is not.
/// </remarks>
sealed internal class InlineOneRuleTerms : IPrecept {

    /// <summary>The identifier name of this precept.</summary>
    public string Name => nameof(InlineOneRuleTerms);

    /// <summary>Performs this precept on the given grammar.</summary>
    /// <param name="analyzer">The analyzer to perform this precept on.</param>
    /// <param name="log">The log to write notices, warnings, and errors.</param>
    /// <returns>True if the grammar was changed.</returns>
    public bool Perform(Analyzer.Analyzer analyzer, ILogger? log) {
        Grammar grammar = analyzer.Grammar;
        List<Rule> candidates = grammar.Terms.Where(t => isCandidate(grammar, t)).Select(t => t.Rules[0]).ToList();
        return candidates.ForeachAny(rule => replaceAll(analyzer.Grammar, rule, log));
    }

    /// <summary>Determines if the given term is a candidate to be replaced.</summary>
    /// <param name="grammar">The grammar this term belongs to.</param>
    /// <param name="term">The term to check if a candidate.</param>
    /// <returns>True if it is a candidate, false otherwise.</returns>
    static private bool isCandidate(Grammar grammar, Term term) {
        // Check that the term is not the start term.
        if (ReferenceEquals(grammar.StartTerm, term)) return false;

        // Check that there is one and only one rule.
        if (term.Rules.Count != 1) return false;

        // Check that the rule isn't initially directly recursive within itself.
        return !term.Rules[0].IsDirectlyRecursive;
    }
    
    /// <summary>
    /// Replaces all instance of the term from the given rule with the items of the given rule
    /// in all locations in the grammar that the term has been used.
    /// </summary>
    /// <param name="grammar">The grammar to replace all the instances of the term in.</param>
    /// <param name="insert">The rule with the term to replace and the items to replace it with.</param>
    /// <param name="log">The log to write notices, warnings, and errors.</param>
    /// <returns>True if all replacements were made, otherwise false.</returns>
    static private bool replaceAll(Grammar grammar, Rule insert, ILogger? log) {
        // Check if another replacement has caused this rule to become directly recursive.
        if (insert.IsDirectlyRecursive) return false;
        log?.AddNoticeF("Removing one rule term, {0}.", insert.Term);
        foreach (Term otherTerm in grammar.Terms) {
            if (ReferenceEquals(otherTerm, insert.Term)) continue;
            foreach (Rule otherRule in otherTerm.Rules) {
                replaceInOneRule(otherRule, insert);
            }
        }
        grammar.RemoveTerm(insert.Term);
        return true;
    }

    /// <summary>
    /// Replaces the instances of the term from the given insertion rule that exist in the
    /// given target rule. The term is replaced by the items from the insertion rule.
    /// </summary>
    /// <param name="target">The rule to perform the replacements inside of.</param>
    /// <param name="insert">The rule with the term to replace with it's items.</param>
    static private void replaceInOneRule(Rule target, Rule insert) {
        for (int i = target.Items.Count-1; i >= 0; --i) {
            if (ReferenceEquals(target.Items[i], insert.Term)) {
                target.Items.RemoveAt(i);
                target.Items.InsertRange(i, insert.Items);
            }
        }
    }
}
