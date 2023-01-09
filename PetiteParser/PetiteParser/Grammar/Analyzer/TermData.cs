using PetiteParser.Formatting;
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
        /// <remarks>
        /// The first tokens are any token reachable from the beginning of all the rules of this term.
        /// If a term is reached in any rule then the firsts of that term are the firsts of this term too.
        /// </remarks>
        private readonly HashSet<TokenItem> firsts;

        /// <summary>These are first token for this term which are only defined within the terms rules.</summary>
        private readonly HashSet<TokenItem> directFirsts;

        /// <summary>The terms which this term directly depends upon.</summary>
        /// <remarks>
        /// For the rule `A → B C A n D`, where `A`, `B`, `C`, and `D` are terms and `n` is a token,
        /// the children for `A` are `B` and, if `B` has a lambda, `C` and if both `B` and `C` have a lambda, `A` itself.
        /// `A` can be treated as a child of itself, since if any rule of `A` is found to have a lambda and
        /// `A` has itself as a child, then that one lambda rule in `A` effects any rule with `A` as a child.
        /// In the given example `D` is after a token can not effect firsts and therefore is not a child.
        /// </remarks>
        private readonly HashSet<TermData> children;

        /// <summary>The terms which are children, grandchildren, and so on of this term.</summary>
        private readonly HashSet<TermData> descendants;

        /// <summary>The terms which depends directly in at least one rule on this term.<</summary>
        private readonly HashSet<TermData> parents;

        /// <summary>The terms which are parents, grandparents, and so on of this term.</summary>
        private readonly HashSet<TermData> ancestors;

        /// <summary>Creates a new term data of first tokens.</summary>
        /// <param name="lookup">This a method for looking up other term's data.</param>
        /// <param name="term">The term this data belongs to.</param>
        public TermData(Func<Term, TermData> lookup, Term term) {
            this.lookup       = lookup;
            this.Term         = term;
            this.firsts       = new();
            this.directFirsts = new();
            this.children     = new();
            this.descendants  = new();
            this.parents      = new();
            this.ancestors    = new();
            this.update       = true;
            this.HasLambda    = false;
        }
        
        /// <summary>The term this data if for.</summary>
        public Term Term { get; }
        
        /// <summary>
        /// Indicates if this term has rules such that it can
        /// pass over this term without consuming any tokens.
        /// </summary>
        public bool HasLambda { get; private set; }

        /// <summary>Joins two terms as parent and dependent child.</summary>
        /// <param name="child">The child to join to this parent.</param>
        private bool addChildTerm(TermData child) {
            bool changed =
                this.children.Add(child) |
                this.descendants.Add(child) |
                child.parents.Add(this) |
                child.ancestors.Add(this);

            // Propagate the join up to the grandchildren.
            foreach (TermData grandchild in child.descendants) {
                changed =
                    grandchild.ancestors.Add(this) |
                    this.descendants.Add(grandchild) |
                    changed;
            }

            // Propagate the join up to the grandparents.
            foreach (TermData grandparent in this.ancestors) {
                changed =
                    grandparent.descendants.Add(child) |
                    child.ancestors.Add(grandparent) |
                    changed;
            }
            
            // Add the tokens forward from the new ancestor to this term.
            return child.firsts.ForeachAny(this.firsts.Add) || changed;
        }

        /// <summary>Propagates the rule information into the given data.</summary>
        /// <param name="rule">The rule to add token firsts into the data.</param>
        /// <returns>True if the data has been changed, false otherwise.</returns>
        private bool propageteRule(Rule rule) {
            bool updated = false;
            foreach (Item item in rule.BasicItems) {

                // Check if token, if so skip the lambda check and just leave.
                if (item is TokenItem tItem)
                    return (this.firsts.Add(tItem) |
                        this.directFirsts.Add(tItem)) || updated;

                // If term, then join to the term for this item (a child term).
                if (item is Term term) {
                    TermData child = this.lookup(term);
                    updated = this.addChildTerm(child) || updated;
                    if (!child.HasLambda) return updated;
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

            // Mark all parents as needing updates,
            // i.e. the child has changed so the parents should be updated.
            if (updated) this.parents.Foreach(p => p.update = true);
            return updated;
        }
        
        /// <summary>Gets the first token sets for this grammar item.</summary>
        /// <param name="tokens">The set to add the found tokens to.</param>
        /// <returns>True if the item has a lambda, false otherwise.</returns>
        public bool Firsts(HashSet<TokenItem> tokens) {
            this.firsts.Foreach(tokens.Add);
            return this.HasLambda;
        }

        /// <summary>Determines if this term has the given token as a first.</summary>
        /// <param name="token">The token to check for in the firsts for this term.</param>
        /// <returns>True if this term has a first, false otherwise.</returns>
        public bool HasFirst(TokenItem token) => this.firsts.Contains(token);

        /// <summary>Determines if the given term is a direct child of this term.</summary>
        /// <param name="term">The term to determine to be a child or not of this term.</param>
        /// <returns>True if direct child, false otherwise.</returns>
        public bool HasChild(TermData term) => this.children.Contains(term);

        /// <summary>Indicates if this term is left recursive.</summary>
        /// <returns>True if this term is left recursive.</returns>
        public bool LeftRecursive() => this.descendants.Contains(this);

        /// <summary>Determine if a child is the next part in the path to the target.</summary>
        /// <remarks>This will not allow self-looping terms to be returns.</remarks>
        /// <param name="target">The target to try to find.</param>
        /// <param name="touched">These are terms already in the path, so may not be used.</param>
        /// <returns>The child in the path to the target or null if none found.</returns>
        public TermData? ChildInPath(TermData target, HashSet<TermData> touched ) =>
            this.children.WhereNot(touched.Contains).WhereNot(this.Equals).
                FirstOrDefault(child => child.ancestors.Contains(target));

        /// <summary>Gets the sorted term names for the given set.</summary>
        /// <param name="terms">The terms to get the names from.</param>
        /// <returns>The sorted names from the given terms.</returns>
        static private string[] termSetNames(HashSet<TermData> terms) {
            string[] results = terms.Select(g => g.Term.Name).ToArray();
            Array.Sort(results);
            return results;
        }

        /// <summary>Gets the sorted token names for the given set.</summary>
        /// <param name="tokens">The tokens to get the names from.</param>
        /// <returns>The sorted names from the given tokens.</returns>
        static private string[] tokenSetNames(HashSet<TokenItem> tokens) {
            string[] results = tokens.ToNames().ToArray();
            Array.Sort(results);
            return results;
        }

        /// <summary>Adds a row of strings into the given string table for this term data.</summary>
        /// <param name="st">The state table to write a rule to.</param>
        /// <param name="row">The index of the row to set.</param>
        /// <param name="verbose">
        /// Indicates that the parents and children sets should be added,
        /// otherwise only firsts and lambdas are outputted.
        /// </param>
        internal void AddRow(StringTable st, int row, bool verbose) {
            st.Data[row, 0] = this.Term.Name;
            st.Data[row, 1] = tokenSetNames(this.firsts).Join(", ");
            st.Data[row, 2] = this.HasLambda ? "x" : "";
            if (verbose) {
                st.Data[row, 3] = tokenSetNames(this.directFirsts).Join(", ");
                st.Data[row, 4] = termSetNames(this.children).Join(", ");
                st.Data[row, 5] = termSetNames(this.parents).Join(", ");
                st.Data[row, 6] = termSetNames(this.descendants).Join(", ");
                st.Data[row, 7] = termSetNames(this.ancestors).Join(", ");
            }
        }

        /// <summary>Checks if the given object is equal to this term data.</summary>
        /// <remarks>
        /// This only checks the terms assuming there is only one term with a specific name per analyzer
        /// and term data is not shared across multiple analyzers and grammars.
        /// </remarks>
        /// <param name="obj">The other object to check against.</param>
        /// <returns>True of the given object is equal to this term data, false otherwise.</returns>
        public override bool Equals(object? obj) => 
            obj is not null and TermData other && this.Term == other.Term;

        /// <summary>Gets the hash code for the term in this term data.</summary>
        /// <returns>The term's hash code.</returns>
        public override int GetHashCode() => this.Term.GetHashCode();

        /// <summary>Gets a string for this data.</summary>
        /// <returns>The string for this data.</returns>
        public override string ToString() {
            StringBuilder result = new();
            result.Append(this.Term.Name).Append(" →");

            if (this.firsts.Any()) {
                string[] tokens = this.firsts.ToNames().ToArray();
                Array.Sort(tokens);
                result.Append(" Firsts[").AppendJoin(", ", tokens).Append(']');
            }

            if (this.children.Any())
                result.Append(" Children[").AppendJoin(", ", termSetNames(this.children)).Append(']');

            if (this.parents.Any())
                result.Append(" Parents[").AppendJoin(", ", termSetNames(this.parents)).Append(']');

            if (this.descendants.Any())
                result.Append(" Descendants[").AppendJoin(", ", termSetNames(this.descendants)).Append(']');

            if (this.ancestors.Any())
                result.Append(" Ancestors[").AppendJoin(", ", termSetNames(this.ancestors)).Append(']');

            if (this.HasLambda) result.Append(" λ");
            return result.ToString();
        }
    }
}