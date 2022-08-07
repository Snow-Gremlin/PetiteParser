using PetiteParser.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PetiteParser.Grammar {

    /// <summary>This is a tool for calculating the firsts tokens and term sets for a grammar.</summary>
    /// <remarks>
    /// This performs analysis of the given grammar as it is, any changes to the grammar
    /// will require reanalysis. This analysis required propagation through the rules of the grammar
    /// meaning that this may be slow for large complex grammars.
    /// </remarks>
    public class Analyzer {

        /// <summary>This stores the token sets for a term in the grammar.</summary>
        private class TermGroup {

            /// <summary>The sets of all terms this group belongs to.</summary>
            private readonly Analyzer sets;

            /// <summary>The term this group if for.</summary>
            public readonly Term Term;

            /// <summary>Indicates this group needs to be updated.</summary>
            private bool update;

            /// <summary>
            /// Indicates if this term has rules such that it can
            /// pass over this term without consuming any tokens.
            /// </summary>
            public bool HasLambda;

            /// <summary>The set of first tokens for this term.</summary>
            public readonly HashSet<TokenItem> Tokens;

            /// <summary>The other terms which depends directly on this term.</summary>
            public readonly HashSet<TermGroup> Children;

            /// <summary>The other terms which depends in at least one rule on this term.</summary>
            public readonly HashSet<TermGroup> Dependents;

            /// <summary>The other terms which this term depends upon in at least one rule.</summary>
            public readonly HashSet<TermGroup> Ancestors;

            /// <summary>Creates a new term group of first tokens.</summary>
            /// <param name="sets">The sets of all terms this group belongs to.</param>
            /// <param name="term">The term this group belongs to.</param>
            public TermGroup(Analyzer sets, Term term) {
                this.sets       = sets;
                this.Term       = term;
                this.update     = true;
                this.HasLambda  = false;
                this.Tokens     = new HashSet<TokenItem>();
                this.Children   = new HashSet<TermGroup>();
                this.Dependents = new HashSet<TermGroup>();
                this.Ancestors  = new HashSet<TermGroup>();
            }

            /// <summary>Joins two groups as parent and dependent.</summary>
            /// <param name="dep">The dependent to join to this parent.</param>
            private bool joinTo(TermGroup dep) {
                bool changed =
                    this.Children.Add(dep) |
                    this.Dependents.Add(dep) |
                    dep.Ancestors.Add(this);

                // Propagate the join up to the grandparents.
                foreach (TermGroup grandparent in this.Ancestors) {
                    changed =
                        grandparent.Dependents.Add(dep) |
                        dep.Ancestors.Add(grandparent) |
                        changed;
                }

                // Add the tokens forward to the new dependent.
                return this.Tokens.ForeachAny(dep.Tokens.Add) || changed;
            }

            /// <summary>Propagates the rule information into the given group.</summary>
            /// <param name="group">The group to add the rule information to.</param>
            /// <param name="rule">The rule to add token firsts into the group.</param>
            /// <returns>True if the group has been changed, false otherwise.</returns>
            private bool propageteRule(Rule rule) {
                bool updated = false;
                foreach (Item item in rule.BasicItems) {

                    // Check if token, if so skip the lambda check and just leave.
                    if (item is TokenItem tItem)
                        return this.Tokens.Add(tItem);

                    // If term, then join to the parents.
                    if (item is Term term) {
                        TermGroup parent = this.sets.terms[term];
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
            /// <returns>True if the group has been changed, false otherwise.</returns>
            public bool Propagate() {
                if (!this.update) return false;
                this.update = false;

                // Run through all rules and update them.
                bool updated = this.Term.Rules.ForeachAny(this.propageteRule);

                // Mark all dependents as needing updates.
                if (updated) this.Dependents.Foreach(d => d.update = true);
                return updated;
            }

            /// <summary>Indicates if this term is left recursive.</summary>
            /// <returns>True if this term is left recursive.</returns>
            public bool LeftRecursive() => this.Dependents.Contains(this);

            /// <summary>Determine if a child is the next part in the path to the target.</summary>
            /// <param name="target">The target to try to find.</param>
            /// <returns>The child in the path to the target or null if none found.</returns>
            public TermGroup ChildInPath(TermGroup target) =>
                this.Children.FirstOrDefault(child => child.Dependents.Contains(target));

            /// <summary>Gets the sorted term names from the given groups.</summary>
            /// <param name="terms">The terms to get the names from.</param>
            /// <returns>The sorted names from the given terms.</returns>
            static private string[] termGroupsNames(HashSet<TermGroup> terms) {
                string[] results = terms.Select(g =>  g.Term.Name).ToArray();
                Array.Sort(results);
                return results;
            }

            /// <summary>Gets a string for this group.</summary>
            /// <param name="namePadding">The padding for the name to align several groups.</param>
            /// <param name="verbose">Shows the children and parent terms.</param>
            /// <returns>The string for this group.</returns>
            public string ToString(int namePadding = 0, bool verbose = false) {
                StringBuilder result = new();
                result.Append(this.Term.Name.PadRight(namePadding)).Append(" →");

                if (this.Tokens.Any()) {
                    string[] tokens = this.Tokens.ToNames().ToArray();
                    Array.Sort(tokens);
                    result.Append(verbose ? " Tokens[" : " [").AppendJoin(", ", tokens).Append(']');
                }

                if (verbose) {
                    if (this.Children.Any())
                        result.Append(" Children[").AppendJoin(", ", termGroupsNames(this.Children)).Append(']');

                    if (this.Dependents.Any())
                        result.Append(" Dependents[").AppendJoin(", ", termGroupsNames(this.Dependents)).Append(']');

                    if (this.Ancestors.Any())
                        result.Append(" Ancestors[").AppendJoin(", ", termGroupsNames(this.Ancestors)).Append(']');
                }

                if (this.HasLambda) result.Append(" λ");
                return result.ToString();
            }
        }

        /// <summary>The set of groups for all terms in the grammar.</summary>
        private readonly Dictionary<Term, TermGroup> terms;

        /// <summary>Creates a new token set tool.</summary>
        /// <param name="grammar">The grammar to get the firsts from.</param>
        public Analyzer(Grammar grammar) {
            this.terms = grammar.Terms.ToDictionary(term => term, term => new TermGroup(this, term));

            // Propagate the information into each group and keep updating as needed.
            while (this.propagate(grammar)) ;
        }

        /// <summary>Gets the determined first token sets for the grammar.</summary>
        /// <param name="item">This is the item to get the token set for.</param>
        /// <param name="tokens">The set to add the found tokens to.</param>
        /// <returns>True if the item has a lambda, false otherwise.</returns>
        public bool Firsts(Item item, HashSet<TokenItem> tokens) {
            if (item is TokenItem token) {
                tokens.Add(token);
                return false;
            }
            
            if (item is Term term) {
                TermGroup group = this.terms[term];
                group.Tokens.Foreach(tokens.Add);
                return group.HasLambda;
            }
            
            return false; // Prompt
        }

        /// <summary>Tries to find the first direct or indirect left recursion.</summary>
        /// <returns>The tokens in the loop for the left recursion or null if none.</returns>
        /// <see cref="https://handwiki.org/wiki/Left_recursion"/>
        public List<Term> FindFirstLeftRecursion() {
            TermGroup target = this.terms.Values.FirstOrDefault(g => g.LeftRecursive());
            if (target is null) return new List<Term>();

            List<Term> path = new() { target.Term };
            TermGroup group = target;
            while (true) {
                TermGroup next = group.ChildInPath(target);
                if (next is null)
                    throw new Exception("No children found in path from " + group.Term +
                        " to " + target.Term + " when left recursive found.");

                if (next == target) return path;
                path.Add(next.Term);
                group = next;
            }
        }

        /// <summary>Propagates all the terms in the given grammar.</summary>
        /// <param name="grammar">The grammar to propagate.</param>
        /// <returns>True if any term had a change during propagation.</returns>
        private bool propagate(Grammar grammar) =>
            this.terms.Values.ForeachAny(group => group.Propagate());

        /// <summary>Gets a string for debugging the grammar's first tokens.</summary>
        /// <param name="verbose">Shows the children and parent terms.</param>
        /// <returns>The string with the first tokens.</returns>
        public string ToString(bool verbose = false) {
            int maxWidth = this.terms.Keys.Select(term => term.Name.Length).Aggregate(Math.Max);
            string[] parts = this.terms.Values.Select(g => g.ToString(maxWidth, verbose)).ToArray();
            Array.Sort(parts);
            return parts.JoinLines();
        }
    }
}
