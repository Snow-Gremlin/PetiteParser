using PetiteParser.Misc;
using System.Linq;

namespace PetiteParser.Analyzer.Actions {

    /// <summary>An action to remove any unproductive rules from the grammar.</summary>
    internal class RemoveUnproductiveRules : IAction {

        /// <summary>Performs this action on the given grammar.</summary>
        /// <param name="analyzer">The analyzer to perform this action on.</param>
        /// <returns>True if the grammar was changed.</returns>
        public bool Perform(Analyzer analyzer) =>
            analyzer.Grammar.Terms.ForeachAny(term => term.Rules.RemoveAll(unproductiveRule) > 0);

        /// <summary>True if the rule is a simple left recursion rule which performs no production.</summary>
        /// <param name="rule">The rule to check.</param>
        /// <example>Look for a rule like "T := T".</example>
        static private bool unproductiveRule(Grammar.Rule rule) =>
            (rule.BasicItems.Count() == 1) && (rule.BasicItems.First() as Grammar.Term == rule.Term);
    }
}
