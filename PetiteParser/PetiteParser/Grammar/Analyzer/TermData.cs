using PetiteParser.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PetiteParser.Grammar.Analyzer;

partial class Analyzer {

    /// <summary>This stores the token sets and other analytic data for a term in the grammar.</summary>
    sealed private class TermData {

        /// <summary>This is a lookup function used for this data to find other data for the given term.</summary>
        private readonly Func<Term, TermData> lookup;

        /// <summary>Indicates this data needs to be updated.</summary>
        private bool update;

        /// <summary>The set of first tokens for this term.</summary>
        private readonly HashSet<TokenItem> firsts;

        /// <summary>The other terms which depends directly on this term.</summary>
        private readonly HashSet<TermData> children;

        /// <summary>The other terms which depends in at least one rule on this term.</summary>
        private readonly HashSet<TermData> dependents;

        /// <summary>The other terms which this term depends upon in at least one rule.</summary>
        private readonly HashSet<TermData> ancestors;

        /// <summary>Creates a new term data of first tokens.</summary>
        /// <param name="lookup">This a method for looking up other term's data.</param>
        /// <param name="term">The term this data belongs to.</param>
        public TermData(Func<Term, TermData> lookup, Term term) {
            this.lookup     = lookup;
            this.Term       = term;
            this.update     = true;
            this.HasLambda  = false;
            this.firsts     = new();
            this.children   = new();
            this.dependents = new();
            this.ancestors  = new();
        }
        
        /// <summary>The term this data if for.</summary>
        public Term Term { get; }

        /// <summary>Joins two terms as parent and dependent.</summary>
        /// <param name="dep">The dependent to join to this parent.</param>
        private bool joinTo(TermData dep) {
            bool changed =
            this.children.Add(dep) |
            this.dependents.Add(dep) |
            dep.ancestors.Add(this);

            // Propagate the join up to the grandparents.
            foreach (TermData grandparent in ancestors) {
                changed =
                    grandparent.dependents.Add(dep) |
                    dep.ancestors.Add(grandparent) |
                    changed;
            }

            // Add the tokens forward to the new dependent.
            return this.firsts.ForeachAny(dep.firsts.Add) || changed;
        }

        /// <summary>Propagates the rule information into the given data.</summary>
        /// <param name="rule">The rule to add token firsts into the data.</param>
        /// <returns>True if the data has been changed, false otherwise.</returns>
        private bool propageteRule(Rule rule) {
            bool updated = false;
            foreach (Item item in rule.BasicItems) {

                // Check if token, if so skip the lambda check and just leave.
                if (item is TokenItem tItem)
                    return this.firsts.Add(tItem);

                // If term, then join to the parents.
                if (item is Term term) {
                    TermData parent = this.lookup(term);
                    updated = parent.joinTo(this) || updated;
                    if (!parent.HasLambda) return updated;
                }
            }

            // If the end has been reached with out stopping then set this as having a lambda.
            if (!this.HasLambda) {
                this.HasLambda = true;
                updated = true;
            }
            return updated;
        }

        /// <summary>Propagate all the rules for this term.</summary>
        /// <returns>True if the data has been changed, false otherwise.</returns>
        public bool Propagate() {
            if (!this.update) return false;
            this.update = false;

            // Run through all rules and update them.
            bool updated = Term.Rules.ForeachAny(propageteRule);

            // Mark all dependents as needing updates.
            if (updated) this.dependents.Foreach(d => d.update = true);
            return updated;
        }

        /// <summary>
        /// Indicates if this term has rules such that it can
        /// pass over this term without consuming any tokens.
        /// </summary>
        public bool HasLambda { get; private set; }

        /// <summary>The set of first tokens for this term.</summary>
        public IEnumerable<TokenItem> Firsts => this.firsts;

        /// <summary>Determines if this term has the given token as a first.</summary>
        /// <param name="token">The token to check for in the firsts for this term.</param>
        /// <returns>True if this term has a first, false otherwise.</returns>
        public bool HasFirst(TokenItem token) => this.firsts.Contains(token);

        /// <summary>Indicates if this term is left recursive.</summary>
        /// <returns>True if this term is left recursive.</returns>
        public bool LeftRecursive() => this.dependents.Contains(this);

        /// <summary>Determine if a child is the next part in the path to the target.</summary>
        /// <param name="target">The target to try to find.</param>
        /// <returns>The child in the path to the target or null if none found.</returns>
        public TermData? ChildInPath(TermData target) =>
            this.children.FirstOrDefault(child => child.dependents.Contains(target));

        /// <summary>Gets the sorted term names from this data.</summary>
        /// <param name="terms">The terms to get the names from.</param>
        /// <returns>The sorted names from the given terms.</returns>
        static private string[] termSetNames(HashSet<TermData> terms) {
            string[] results = terms.Select(g => g.Term.Name).ToArray();
            Array.Sort(results);
            return results;
        }

        /// <summary>Gets a string for this data.</summary>
        /// <param name="verbose">Shows the children and parent terms.</param>
        /// <returns>The string for this data.</returns>
        public string ToString(int namePadding = 0, bool verbose = false) {
            StringBuilder result = new();
            result.Append(this.Term.Name.PadRight(namePadding)).Append(" →");

            if (this.firsts.Any()) {
                string[] tokens = this.firsts.ToNames().ToArray();
                Array.Sort(tokens);
                result.Append(verbose ? " Firsts[" : " [").AppendJoin(", ", tokens).Append(']');
            }

            if (verbose) {
                if (this.children.Any())
                    result.Append(" Children[").AppendJoin(", ", termSetNames(this.children)).Append(']');

                if (this.dependents.Any())
                    result.Append(" Dependents[").AppendJoin(", ", termSetNames(this.dependents)).Append(']');

                if (this.ancestors.Any())
                    result.Append(" Ancestors[").AppendJoin(", ", termSetNames(this.ancestors)).Append(']');
            }

            if (this.HasLambda) result.Append(" λ");
            return result.ToString();
        }
    }
}