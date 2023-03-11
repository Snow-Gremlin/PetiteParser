using PetiteParser.Logger;
using System;
using System.Linq;

namespace PetiteParser.Grammar.Normalizer;

/// <summary>Tool for helping normalize grammars into an CLR format.</summary>
/// <remarks>
/// This performs several precepts such as removing duplicate grammar rules,
/// removing unproductive rules, and removing left recursion.
/// </remarks>
static public class Normalizer {

    /// <summary>The limit length to use if none is given for normalizing.</summary>
    private const int defaultLoopLimit = 100;

    /// <summary>Gets all the default precepts in the order to run them in.</summary>
    static private IPrecept[] allPrecepts => new IPrecept[] {
        // Remove unneeded parts of the grammar and sort the rules to make finding duplicates easier.
        new RemoveUnusedTerms(),
        new RemoveUnproductiveRules(),
        //new RemoveMonoproductiveTerms(), // TODO: Enable
        new SortRules(),
        new RemoveDuplicateRules(),
        new RemoveDuplicateTerms(),

        // Change the rules to be biased towards shifts over reduces.
        //new InlineOneRuleTerms(), // TODO: Enable
        //new InlineTails(), // TODO: Enable

        // More complex precepts are run last so that unproductive
        // rules and any complications have already been removed.
        new RemoveLeftRecursion(),
    };

    /// <summary>Creates a copy of the grammar and normalizes it.</summary>
    /// <param name="grammar">The grammar to copy and normalize.</param>
    /// <param name="log">The optional log to collect warnings and errors with.</param>
    /// <param name="loopLimit">The maximum number of normalization loops are allowed before failing.</param>
    /// <returns>The normalized copy of the given grammar.</returns>
    static public Grammar GetNormal(Grammar grammar, ILogger? log = null, int loopLimit = defaultLoopLimit) {
        Grammar gram2 = grammar.Copy();
        Normalize(gram2, log, loopLimit);
        return gram2;
    }
    
    /// <summary>Creates a copy of the grammar and normalizes it.</summary>
    /// <param name="grammar">The grammar to copy and normalize.</param>
    /// <param name="maxSteps">The maximum number of steps to perform.</param>
    /// <param name="log">The optional log to output notices to.</param>
    /// <param name="precepts">The set of precepts to run on the grammar.</param>
    /// <returns>The normalized copy of the given grammar.</returns>
    static internal Grammar GetNormal(Grammar grammar, int maxSteps, ILogger? log, params IPrecept[] precepts) {
        Grammar gram2 = grammar.Copy();
        Normalize(gram2, maxSteps, log, precepts);
        return gram2;
    }

    /// <summary>Performs a collection of automatic precepts to change the given grammar into a normal CLR form.</summary>
    /// <param name="grammar">The grammar to normalize.</param>
    /// <param name="log">The optional log to output notices to.</param>
    /// <param name="loopLimit">The maximum number of normalization loops are allowed before failing.</param>
    /// <returns>True if the grammar was changed, false otherwise.</returns>
    static public bool Normalize(Grammar grammar, ILogger? log = null, int loopLimit = defaultLoopLimit) {
        Buffered bufLog = new(log);
        int steps = Normalize(grammar, loopLimit, log, allPrecepts);
        if (steps >= loopLimit) {
            Console.WriteLine(bufLog);
            throw new GrammarException("Normalizing grammar got stuck in a loop. Log dumped to console.");
        }
        return steps > 0;
    }

    /// <summary>Performs a maximum number of steps with the given precepts to change the given grammar.</summary>
    /// <param name="grammar">The grammar to normalize.</param>
    /// <param name="maxSteps">The maximum number of steps to perform.</param>
    /// <param name="log">The optional log to output notices to.</param>
    /// <param name="precepts">The set of precepts to run on the grammar.</param>
    /// <returns>The number of steps which were performed.</returns>
    static internal int Normalize(Grammar grammar, int maxSteps, ILogger? log, params IPrecept[] precepts) {
        Analyzer.Analyzer analyzer = new(grammar);
        for (int steps = 1; steps <= maxSteps; ++steps) {
            if (precepts.Any(a => a.Perform(analyzer, log))) {
                analyzer.NeedsToRefresh();
                // Extra fine detail information for debugging small grammar normalization.
                //log?.AddInfo("Normalized Grammar to: ");
                //log?.Indent().AddInfo(analyzer.Grammar.ToString());
            } else return steps;
        }
        return maxSteps;
    }
}
