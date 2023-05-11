using PetiteParser.Grammar.Analyzer;
using PetiteParser.Logger;
using PetiteParser.Misc;
using System.Collections.Generic;
using System.Linq;

namespace PetiteParser.Grammar.Normalizer;

/// <summary>
/// A precept that puts short rules for terms found near the end of a rule
/// in place of the term such that there are less rules and the rules are longer.
/// This specifically looks for terms which could cause a shift or a reduce conflict.
/// This encourage "shift"s which can work in parallel over "reduce"s which can not.
/// </summary>
sealed internal class InlineTails : IPrecept {
    
    /// <summary>The identifier name of this precept.</summary>
    public string Name => nameof(InlineTails);

    /// <summary>Performs this precept on the given grammar.</summary>
    /// <param name="analyzer">The analyzer to perform this precept on.</param>
    /// <param name="log">The log to write notices, warnings, and errors.</param>
    /// <returns>True if the grammar was changed.</returns>
    public bool Perform(Analyzer.Analyzer analyzer, ILogger? log) {
        // Try to find a conflict point.
        RuleOffset? fragment = analyzer.FindConflictPoint().
            Where(canInline).
            FirstOrDefault();
        if (fragment is null) return false;

        log?.AddInfo("Inlining tail for "+fragment);

        Rule rule = fragment.Rule;
        int index = fragment.Index;
        Item? item = fragment.NextItem;
        if (item is not Term term) return false;

        // Cut off the tail from the one rule with the term in it.
        List<Item> tail = fragment.FollowingItems.ToList();

        // If there is more than one usage, make a copy of the term first.
        int count = analyzer.UsageCount(item);
        if (count > 1) {
            Term t2 = analyzer.Grammar.AddGeneratedTerm(term.Name);
            rule.Items.RemoveRange(index..);
            rule.Items.Add(t2);

            // Append tail to each rule of the term.
            foreach (Rule head in term.Rules) {
                Rule r2 = t2.NewRule();
                r2.Items.AddRange(head.Items);
                r2.Items.AddRange(tail);
            }
            return true;
        }

        // If this term is only used once, simply update it as is.
        rule.Items.RemoveRange((index+1)..);

        // Append tail to each rule of the term.
        foreach (Rule r2 in term.Rules)
            r2.Items.AddRange(tail);
        return true;
    }

    /// <summary>Determines if the fragment represents a term which can be inlined.</summary>
    /// <param name="fragment">The fragment to check.</param>
    /// <returns>True if the term can be inlined, false otherwise.</returns>
    static private bool canInline(RuleOffset fragment) =>
        fragment.NextItem is Term term && !term.IsDirectlyRecursive;
}
