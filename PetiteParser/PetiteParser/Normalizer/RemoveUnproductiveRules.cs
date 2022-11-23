using PetiteParser.Grammar;
using PetiteParser.Misc;

namespace PetiteParser.Normalizer;

/// <summary>A precept to remove any unproductive rules from the grammar.</summary>
sealed internal class RemoveUnproductiveRules : IPrecept {

    /// <summary>Performs this precept on the given grammar.</summary>
    /// <param name="analyzer">The analyzer to perform this precept on.</param>
    /// <param name="log">The log to write notices, warnings, and errors.</param>
    /// <returns>True if the grammar was changed.</returns>
    public bool Perform(Analyzer.Analyzer analyzer, Logger.ILogger? log) =>
        analyzer.Grammar.Terms.ForeachAny(term => removeUnproductiveRules(term, log));

    /// <summary>Removes unproductive rules from the given term.</summary>
    /// <param name="term">The term to remove unproductive rules from.</param>
    /// <param name="log">The log to write notices, warnings, and errors.</param>
    /// <returns>True if any rules were removed, false if no rules were removed.</returns>
    static private bool removeUnproductiveRules(Term term, Logger.ILogger? log) {
        int count = term.Rules.RemoveAll(unproductiveRule);
        if (count <= 0) return false;
        log?.AddNoticeF("Removed {0} unproductive rules from {1}.", count, term);
        return true;
    }

    /// <summary>True if the rule is a simple left recursion rule which performs no production.</summary>
    /// <param name="rule">The rule to check.</param>
    /// <example>Look for a rule like "T := T".</example>
    static private bool unproductiveRule(Rule rule) {
        Term? term = rule.BasicItems.OnlyOne() as Term;
        return term is not null && term == rule.Term;
    }
}
