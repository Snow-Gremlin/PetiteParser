using S = System;
using System.Collections.Generic;
using System.Linq;

namespace PetiteParser.Analyzer.Actions {

    /// <summary>Removes all direct and indirect left recursion in this grammar.</summary>
    /// <see cref="https://handwiki.org/wiki/Left_recursion"/>
    internal class RemoveLeftRecursion : IAction {

        /// <summary>Performs this action on the given grammar.</summary>
        /// <param name="analyzer">The analyzer to perform this action on.</param>
        /// <returns>True if the grammar was changed.</returns>
        public bool Perform(Analyzer analyzer) {
            List<Grammar.Term> terms = analyzer.FindFirstLeftRecursion();
            if (terms is null || terms.Count <= 0) return false;

            Grammar.Rule rule = getRuleToChange(analyzer, terms);
            removeIndirection(analyzer, rule, terms);
            removeLeftRecursion(analyzer, rule);
            return true;
        }

        /// <summary>Gets the rule from the first term to the next in the loop.</summary>
        /// <param name="analyzer">The analyzer to use to find the rule.</param>
        /// <param name="terms">The terms creating the recursive path.</param>
        /// <returns>The rule between the first and next term in the loop, or null if not found.</returns>
        static private Grammar.Rule getRuleToChange(Analyzer analyzer, List<Grammar.Term> terms) =>
            analyzer.FirstRuleBetween(terms[0], terms[S.Math.Max(1, terms.Count - 1)]);

        /// <summary>Removes any indirection from a recursion.</summary>
        /// <param name="analyzer">The analyzer to use to find the rules.</param>
        /// <param name="rule">The first rule in the path that is to be updated.</param>
        /// <param name="terms">The terms creating the recursive path.</param>
        static private void removeIndirection(Analyzer analyzer, Grammar.Rule rule, List<Grammar.Term> terms) {
            if (terms.Count <= 1) return;



            // TODO: FINISH


        }

        /// <summary>Removes the direct left recursion path from the grammar.</summary>
        /// <param name="terms">The left recursion path.</param>
        static private void removeLeftRecursion(Analyzer analyzer, Grammar.Rule rule) {


            // TODO: FINISH

        }
    }
}
