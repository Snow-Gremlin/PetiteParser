using PetiteParser.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetiteParser.Grammar {

    /// <summary>Normalizer is a tool for automatically fixing potential problems found in a grammar.</summary>
    internal class Normalizer {
        private Grammar grammar;

        /// <summary>Creates a new normalizer tool for the given grammar.</summary>
        /// <param name="grammar">The grammar to normalize.</param>
        public Normalizer(Grammar grammar) => this.grammar = grammar;

        /// <summary>Performs the normalization and modifies the grammar.</summary>
        public void Normalize() {
            this.removeUnproductiveRules();
            this.removeDuplicateRules();
            this.removeDuplicateTerms();
            this.removeLeftRecursion();
        }

        /// <summary>Removes any unproductive rules from the grammar.</summary>
        private void removeUnproductiveRules() {
            foreach (Term term in this.grammar.Terms)
                term.Rules.RemoveAll(unproductiveRule);
        }

        /// <summary>True if the rule is a simple left recursion rule which performs no production.</summary>
        /// <example>Look for a rule like "T := T".</example>
        static private bool unproductiveRule(Rule rule) =>
            (rule.BasicItems.Count() == 1) && (rule.BasicItems.First() as Term == rule.Term);

        /// <summary>Removes any rule in a term which is identical to another rule in the same term.</summary>
        private void removeDuplicateRules() {
            foreach (Term term in this.grammar.Terms) {
                for (int i = term.Rules.Count-1; i >= 1; i--) {
                    Rule rule = term.Rules[i];
                    bool duplicate = false;
                    for (int j = i-1; j >= 0; j--) {
                        if (rule == term.Rules[j]) {
                            duplicate = true;
                            break;
                        }
                    }
                    if (duplicate) term.Rules.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Removes any term which has identical rules to another and
        /// updates all rules using the repeat term with the term which is not removed.
        /// </summary>
        private void removeDuplicateTerms() {

            // TODO: Implement

        }

        /// <summary>Removes all direct and indirect left recursion in this grammar.</summary>
        /// <see cref="https://handwiki.org/wiki/Left_recursion"/>
        /// <see cref="https://www.geeksforgeeks.org/removing-direct-and-indirect-left-recursion-in-a-grammar/"/>
        private void removeLeftRecursion() {
            while (true) {
                List<Term> terms = this.findFirstLeftRecursion();
                if (terms is null || terms.Count <= 0) break;
                this.removeLeftRecursion(terms);
            }
        }

        /// <summary>Tries to find the first direct or indirect left recursion.</summary>
        /// <returns>The tokens in the loop for the left recursion or null if none.</returns>
        private List<Term> findFirstLeftRecursion() {

            // TODO: Need to deal with lambdas

            Stack<Term> path = new();
            foreach (Term term in this.grammar.Terms) {
                List<Term> loop = findFirstLeftRecursion(term, path);
                if (loop != null) return loop;
            }
            return new List<Term>();
        }

        /// <summary>Finds the first left recursion reachable from this term.</summary>
        /// <remarks>If a loop is found, it might not include this term.</remarks>
        /// <param name="path">The current path being checked.</param>
        /// <returns>Returns the left recursion loop or null if none was found.</returns>
        static private List<Term> findFirstLeftRecursion(Term term, Stack<Term> path) {
            path.Push(term);
            foreach (Term next in leadingTerms(term)) {

                // Check for loop and leave if one is found.
                if (path.Contains(next)) {
                    List<Term> result = path.TakeWhile(t => t != next).ToList();
                    result.Insert(0, next);
                    return result;
                }

                List<Term> loop = findFirstLeftRecursion(next, path);
                if (loop != null) return loop;
            }
            path.Pop();
            return null;
        }

        /// <summary>If the first base item is a term then it will be returned, null otherwise.</summary>
        static private Term leadingTerm(Rule rule) =>
            rule.BasicItems.FirstOrDefault() as Term;

        /// <summary>Returns all distinct terms which are leading terms in the rules of this term.</summary>
        static private IEnumerable<Term> leadingTerms(Term term) =>
            term.Rules.Select(leadingTerm).NotNull().Distinct();

        /// <summary>Returns all the rules which have the given leading term.</summary>
        /// <param name="term">The term with the rules to filter.</param>
        /// <param name="target">The term the rule has to be leading with.</param>
        /// <returns>All the rules leading with the given term.</returns>
        private IEnumerable<Rule> rulesLeadingWith(Term term, Term target) =>
            term.Rules.Where(r => leadingTerm(r) == target);

        /// <summary>Removes the given left recursion path from the grammar.</summary>
        /// <param name="terms">The left recursion path.</param>
        private void removeLeftRecursion(List<Term> terms) {
            Term start = terms.First();
            Term stop = terms.Last();


            // TODO: FINISH

        }
    }
}
