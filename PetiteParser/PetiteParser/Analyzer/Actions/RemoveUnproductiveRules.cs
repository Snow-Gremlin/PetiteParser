using PetiteParser.Misc;
using System.Linq;

namespace PetiteParser.Analyzer.Actions;

/// <summary>An action to remove any unproductive rules from the grammar.</summary>
sealed internal class RemoveUnproductiveRules : IAction {

    /// <summary>Performs this action on the given grammar.</summary>
    /// <param name="analyzer">The analyzer to perform this action on.</param>
    /// <param name="log">The log to write notices, warnings, and errors.</param>
    /// <returns>True if the grammar was changed.</returns>
    public bool Perform(Analyzer analyzer, Logger.ILogger log) =>
        analyzer.Grammar.Terms.ForeachAny(term => removeUnproductiveRules(term, log));

    /// <summary>Removes unproductive rules from the given term.</summary>
    /// <param name="term">The term to remove unproductive rules from.</param>
    /// <param name="log">The log to write notices, warnings, and errors.</param>
    /// <returns>True if any rules were removed, false if no rules were removed.</returns>
    static private bool removeUnproductiveRules(Grammar.Term term, Logger.ILogger log) {
        int count = term.Rules.RemoveAll(unproductiveRule);
        if (count <= 0) return false;
        log?.AddNoticeF("Removed {0} unproductive rules from {1}.", count, term);
        return true;
    }

    /// <summary>True if the rule is a simple left recursion rule which performs no production.</summary>
    /// <param name="rule">The rule to check.</param>
    /// <example>Look for a rule like "T := T".</example>
    static private bool unproductiveRule(Grammar.Rule rule) =>
        (rule.BasicItems.Count() == 1) && (rule.BasicItems.First() as Grammar.Term == rule.Term);
}
