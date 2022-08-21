using PetiteParser.Misc;

namespace PetiteParser.Analyzer.Actions {

    /// <summary>Removes any rule in a term which is identical to another rule in the same term.</summary>
    internal class RemoveDuplicateRules : IAction {

        /// <summary>Performs this action on the given grammar.</summary>
        /// <param name="analyzer">The analyzer to perform this action on.</param>
        /// <param name="log">The log to write notices, warnings, and errors.</param>
        /// <returns>True if the grammar was changed.</returns>
        public bool Perform(Analyzer analyzer, Log.Log log) =>
            analyzer.Grammar.Terms.ForeachAny(t => removeDuplicatesInTerm(t, log));

        /// <summary>Remove duplicate rules in terms.</summary>
        /// <param name="term">The term to look for a duplicate withing</param>
        /// <param name="log">The log to write notices, warnings, and errors.</param>
        /// <returns>True if rules were removed or false if not.</returns>
        static private bool removeDuplicatesInTerm(Grammar.Term term, Log.Log log) {
            bool changed = false;
            for (int i = term.Rules.Count-1; i >= 1; i--) {
                if (term.Rules[i] == term.Rules[i-1]) {
                    term.Rules.RemoveAt(i);
                    log?.AddNotice("Removed duplicate rule: {0}", term.Rules[i-1]);
                    changed = true;
                }
            }
            return changed;
        }
    }
}
