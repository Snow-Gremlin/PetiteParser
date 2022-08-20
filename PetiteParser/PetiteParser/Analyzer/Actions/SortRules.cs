using PetiteParser.Grammar;
using PetiteParser.Misc;

namespace PetiteParser.Analyzer.Actions {

    /// <summary>An action to sort the rules in the given term.</summary>
    internal class SortRules : IAction {

        /// <summary>Performs this action on the given grammar.</summary>
        /// <param name="analyzer">The analyzer to perform this action on.</param>
        /// <returns>True if the grammar was changed.</returns>
        public bool Perform(Analyzer analyzer) =>
            analyzer.Grammar.Terms.ForeachAny(sortRules);

        /// <summary>Sorts the rules in the given term.</summary>
        /// <param name="term">The term to sort the rules in.</param>
        /// <returns>True if the rules were sorted, false if they were already in sort order.</returns>
        private bool sortRules(Term term) {
            if (term.Rules.IsSorted()) return false;
            term.Rules.Sort();
            return true;
        }
    }
}
