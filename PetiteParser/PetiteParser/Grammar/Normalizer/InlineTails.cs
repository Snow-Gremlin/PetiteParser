using PetiteParser.Grammar.Analyzer;
using PetiteParser.Logger;
using PetiteParser.Misc;
using System.Collections.Generic;
using System.Linq;

namespace PetiteParser.Grammar.Normalizer;

sealed internal class InlineTails : IPrecept {

    /// <summary>Performs this precept on the given grammar.</summary>
    /// <param name="analyzer">The analyzer to perform this precept on.</param>
    /// <param name="log">The log to write notices, warnings, and errors.</param>
    /// <returns>True if the grammar was changed.</returns>
    public bool Perform(Analyzer.Analyzer analyzer, ILogger? log) {
        // Try to find conflict point.
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

        // If there is more than one usage, make a copy of the term first.s
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

        // If this term is only used one, simply update it as is.
        rule.Items.RemoveRange((index+1)..);

        // Append tail to each rule of the term.
        foreach (Rule r2 in term.Rules)
            r2.Items.AddRange(tail);
        return true;
    }

    // TODO: Fix
    // Example of change to this which needs to be determined:
    //    > <S>
    //    <S> → <A>
    //       | [b]
    //    <A> → <A'0> [a]        <= Both follow with [a] creating a reduction
    //       | [b] [d] <A'0> [a] <= ^ this
    //    <A'0> → λ
    //       | [a] [d] <A'0>     <= Shift by [a]
    //       | [c] <A'0>
    // So [a] should be moved to the end of all the <A'0> rules.

    static private bool canInline(RuleOffset fragment) =>
        fragment.NextItem is Term term && !term.Rules.SelectMany(r => r.Items).OfType<Term>().Any(t => t == term);
}
