using PetiteParser.Logger;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PetiteParser.Normalizer;

/// <summary>Tool for helping normalize grammars into an CLR format.</summary>
/// <remarks>
/// This performs several precepts such as removing duplicate grammar rules,
/// removing unproductive rules, and removing left recursion.
/// </remarks>
static public class Normalizer {

    /// <summary>Creates a copy of the grammar and normalizes it.</summary>
    /// <param name="grammar">The grammar to copy and normalize.</param>
    /// <param name="log">The optional log to collect warnings and errors with.</param>
    /// <param name="loopLimit">The maximum number of normalization loops are allowed before failing.</param>
    /// <returns>The normalized copy of the given grammar.</returns>
    static public Grammar.Grammar GetNormal(Grammar.Grammar grammar, ILogger? log = null, int loopLimit = 10000) {
        Grammar.Grammar gram2 = grammar.Copy();
        Normalize(gram2, log, loopLimit);
        return gram2;
    }

    /// <summary>Performs a collection of automatic precepts to change the given grammar into a normal CLR form.</summary>
    /// <param name="grammar">The grammar to analyze.</param>
    /// <param name="log">The optional log to output notices to.</param>
    /// <param name="loopLimit">The maximum number of normalization loops are allowed before failing.</param>
    /// <returns>True if the grammar was changed, false otherwise.</returns>
    static public bool Normalize(Grammar.Grammar grammar, ILogger? log = null, int loopLimit = 10000) {
        List<IPrecept> precepts = new() {
            new RemoveUnproductiveRules(),
            new RemoveUnproductiveTerms(),
            new RemoveSingleUseRules(),
            new SortRules(),
            new RemoveDuplicateRules(),
            new RemoveDuplicateTerms(),

            // Left recursion should be last precept so that unproductive
            // rules and any complications have already been removed.
            new RemoveLeftRecursion(),
        };

        Analyzer.Analyzer analyzer = new(grammar);
        Buffered bufLog = new(log);
        bool changed = false;
        int loopCount = 0;
        while (precepts.Any(a => a.Perform(analyzer, bufLog))) {
            changed = true;
            analyzer.NeedsToRefresh();
            ++loopCount;
            if (loopCount > loopLimit) {
                Console.WriteLine(bufLog);
                throw new NormalizerException("Normalizing grammar got stuck in a loop. Log dumped to console.");
            }
        }
        return changed;
    }
}
