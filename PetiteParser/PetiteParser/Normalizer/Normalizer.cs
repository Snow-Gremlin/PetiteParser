using PetiteParser.Logger;
using System;
using System.Linq;

namespace PetiteParser.Normalizer;

/// <summary>Tool for helping normalize grammars into an CLR format.</summary>
/// <remarks>
/// This performs several precepts such as removing duplicate grammar rules,
/// removing unproductive rules, and removing left recursion.
/// </remarks>
static public class Normalizer {

    /// <summary>Gets all the default precepts in the order to run them in.</summary>
    static private IPrecept[] allPrecepts => new IPrecept[] {
        new RemoveUnproductiveRules(),
        new RemoveMonoproductiveTerms(),
        new RemoveSingleUseRules(),
        new SortRules(),
        new RemoveDuplicateRules(),
        new RemoveDuplicateTerms(),

        // More complex precepts are run last so that unproductive
        // rules and any complications have already been removed.
        new RemoveLeftRecursion(),
        new LeftFactor(),
    };

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
    /// <param name="grammar">The grammar to normalize.</param>
    /// <param name="log">The optional log to output notices to.</param>
    /// <param name="loopLimit">The maximum number of normalization loops are allowed before failing.</param>
    /// <returns>True if the grammar was changed, false otherwise.</returns>
    static public bool Normalize(Grammar.Grammar grammar, ILogger? log = null, int loopLimit = 10000) {
        Buffered bufLog = new(log);
        Analyzer.Analyzer analyzer = new(grammar);
        int steps = Normalize(analyzer, allPrecepts, loopLimit, log);
        if (steps >= loopLimit) {
            Console.WriteLine(bufLog);
            throw new NormalizerException("Normalizing grammar got stuck in a loop. Log dumped to console.");
        }
        return steps > 0;
    }
    
    /// <summary>Performs a maximum number of steps with the given precepts to change the given grammar.</summary>
    /// <param name="analyzer">The grammar's analyzer to use while normalizing.</param>
    /// <param name="precepts">The set of precepts to run on the grammar.</param>
    /// <param name="maxSteps">The maximum number of steps to perform.</param>
    /// <param name="log">The optional log to output notices to.</param>
    /// <returns>The number of steps which were performed.</returns>
    static internal int Normalize(Analyzer.Analyzer analyzer, IPrecept[] precepts, int maxSteps, ILogger? log = null) {
        for (int steps = 1; steps <= maxSteps; ++steps) {
            if (precepts.Any(a => a.Perform(analyzer, log))) {
                analyzer.NeedsToRefresh();
                return steps;
            }
        }
        return maxSteps;
    }
}
