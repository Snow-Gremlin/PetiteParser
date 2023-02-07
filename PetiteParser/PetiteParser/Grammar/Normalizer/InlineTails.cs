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
    public bool Perform(Analyzer.Analyzer analyzer, ILogger? log) =>
        analyzer.Grammar.Terms.Any(t => inlineTail(analyzer, t));
    
    // TODO: Comment
    static private bool inlineTail(Analyzer.Analyzer analyzer, Term term) {
        // Check that the term is not the start term.
        if (ReferenceEquals(analyzer.Grammar.StartTerm, term)) return false;

        // Check the term is not directly self referencing and has a lambda.
        if (analyzer.HasChild(term, term) && !analyzer.HasLambda(term)) return false;

        // Find a candidate rule and perform the move on it.
        return analyzer.Grammar.Terms.
            Where(t => t != term).
            SelectMany(t => t.Rules).
            Any(r => tryPerformMove(analyzer, term, r));
    }

    // TODO: Comment
    static private bool tryPerformMove(Analyzer.Analyzer analyzer, Term term, Rule rule) {
        // Cut off the tail from the one rule with the term in it.
        int index = rule.Items.IndexOf(term);
        if (index == -1 || index >= rule.Items.Count-1) return false;
        List<Item> tail = rule.Items.GetRange((index+1)..);

        // If there is more than one usage, make a copy of the term first.s
        int count = analyzer.UsageCount(term);
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
}
