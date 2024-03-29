﻿using PetiteParser.Misc;

namespace PetiteParser.Grammar.Normalizer;

/// <summary>A precept to sort the rules in the given term.</summary>
sealed internal class SortRules : IPrecept {

    /// <summary>The identifier name of this precept.</summary>
    public string Name => nameof(SortRules);

    /// <summary>Performs this precept on the given grammar.</summary>
    /// <param name="analyzer">The analyzer to perform this precept on.</param>
    /// <param name="log">The log to write notices, warnings, and errors.</param>
    /// <returns>True if the grammar was changed.</returns>
    public bool Perform(Analyzer.Analyzer analyzer, Logger.ILogger? log) =>
        analyzer.Grammar.Terms.ForeachAny(t => sortRules(t, log));

    /// <summary>Sorts the rules in the given term.</summary>
    /// <param name="term">The term to sort the rules in.</param>
    /// <param name="log">The log to write notices, warnings, and errors.</param>
    /// <returns>True if the rules were sorted, false if they were already in sort order.</returns>
    static private bool sortRules(Term term, Logger.ILogger? log) {
        if (term.Rules.IsSorted()) return false;
        term.Rules.Sort();
        log?.AddNoticeF("Sorted the rules for {0}.", term);
        return true;
    }
}
