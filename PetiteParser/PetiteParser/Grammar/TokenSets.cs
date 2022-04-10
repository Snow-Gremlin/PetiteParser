using PetiteParser.Misc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PetiteParser.Grammar {

    /// <summary>
    /// This is a tool for calculating the firsts tokens for the terms of a grammar.
    /// </summary>
    public class TokenSets {

        /// <summary>This stores the token sets for a term in the grammar.</summary>
        private class TermGroup {

            /// <summary>The term this group if for.</summary>
            public readonly Term Term;

            /// <summary>
            /// Indicates if this term has rules such that it can
            /// pass over this term without consuming any tokens.
            /// </summary>
            public bool HasLambda;

            /// <summary>Indicates this group needs to be updated.</summary>
            public bool Update;

            /// <summary>The set of first tokens for this term.</summary>
            public HashSet<TokenItem> Tokens;
            
            /// <summary>The other terms which depends in at least one rule on this term.</summary>
            public HashSet<TermGroup> Dependents;

            /// <summary>The other terms which this term depends upon in at least one rule.</summary>
            public HashSet<TermGroup> Parents;

            /// <summary>Creates a new term group of first tokens.</summary>
            /// <param name="term">The term this group belongs too.</param>
            public TermGroup(Term term) {
                this.Term       = term;
                this.HasLambda  = false;
                this.Update     = true;
                this.Tokens     = new HashSet<TokenItem>();
                this.Dependents = new HashSet<TermGroup>();
                this.Parents    = new HashSet<TermGroup>();
            }
        }

        /// <summary>The set of groups for all terms in the grammar.</summary>
        private Dictionary<Term, TermGroup> terms;

        /// <summary>Creates a new token set tool.</summary>
        /// <param name="grammar">The grammar to get the firsts from.</param>
        public TokenSets(Grammar grammar) {
            this.terms = new Dictionary<Term, TermGroup>();

            // Setup all group instances
            foreach (Term term in grammar.Terms)
                this.terms.Add(term, new TermGroup(term));

            // Propagate the information into each group and keep updating as needed.
            bool changed = true;
            while (changed) {
                changed = false;
                foreach (Term term in grammar.Terms) {
                    if (this.propagate(term)) changed = true;
                }
            }
        }

        /// <summary>Gets the determined first token sets for the grammar.</summary>
        /// <param name="item">This is the item to get the token set for.</param>
        /// <param name="tokens">The set to add the found tokens to.</param>
        /// <returns>True if the item has a lambda, false otherwise.</returns>
        public bool Firsts(Item item, HashSet<TokenItem> tokens) {
            if (item is TokenItem tItem) {
                tokens.Add(tItem);
                return false;
            }
            
            if (item is Term term) {
                TermGroup group = this.terms[term];
                foreach (TokenItem token in group.Tokens)
                    tokens.Add(token);
                return group.HasLambda;
            }
            
            return false; // Prompt
        }

        /// <summary>Joins these two groups as parent and dependent.</summary>
        /// <param name="parent">The parent to join to a dependent.</param>
        /// <param name="dep">The dependent to join to the parent.</param>
        static private void joinGroups(TermGroup parent, TermGroup dep) {
            parent.Dependents.Add(dep);
            dep.Parents.Add(parent);
        }

        /// <summary>Propagates the rule information into the given group.</summary>
        /// <param name="group">The group to add the rule information to.</param>
        /// <param name="rule">The rule to add token firsts into the group.</param>
        /// <returns>True if the group has been changed, false otherwise.</returns>
        private bool propageteRule(TermGroup group, Rule rule) {
            bool updated = false;
            foreach (Item item in rule.Items) {

                // Check if token, if so skip the lambda check and just leave.
                if (item is TokenItem tItem)
                    return group.Tokens.Add(tItem);

                // If term, then join to all the parents
                if (item is Term term) {
                    TermGroup parent = this.terms[term];
                    joinGroups(parent, group);
                    foreach (TermGroup grand in parent.Parents)
                        joinGroups(grand, group);
                    foreach (TokenItem token in parent.Tokens) {
                        if (group.Tokens.Add(token)) updated = true;
                    }
                    if (!parent.HasLambda) return updated;
                }

                // else ignore because it is Prompt
            }

            // If the end has been reached with out stopping
            if (!group.HasLambda) {
                group.HasLambda = true;
                updated = true;
            }
            return updated;
        }

        /// <summary>Propagate all the rules for the given term.</summary>
        /// <param name="term">The term to propagate.</param>
        /// <returns>True if the group has been changed, false otherwise.</returns>
        private bool propagate(Term term) {
            TermGroup group = this.terms[term];
            if (!group.Update) return false;
            group.Update = false;

            // Run through all rules and update with them.
            bool updated = false;
            foreach (Rule rule in term.Rules) {
                if (this.propageteRule(group, rule)) updated = true;
            }

            // Mark all dependents as needing updates.
            if (updated) {
                foreach (TermGroup dep in group.Dependents)
                    dep.Update = true;
            }
            return updated;
        }

        /// <summary>Gets a string for debugging the grammar's first tokens.</summary>
        /// <returns>The string with the first tokens.</returns>
        public override string ToString() {
            int maxWidth = 0;
            foreach (Term term in this.terms.Keys)
                maxWidth = Math.Max(maxWidth, term.Name.Length);

            string[] parts = new string[this.terms.Count];
            int i = 0;
            foreach (TermGroup group in this.terms.Values) {
                string firstStr = "";
                if (group.Tokens.Count > 0) {
                    string[] firsts = new string[group.Tokens.Count];
                    int j = 0;
                    foreach (TokenItem item in group.Tokens) {
                        firsts[j] = item.Name;
                        ++j;
                    }
                    Array.Sort(firsts);
                    firstStr = "["+firsts.Join(", ") +"]";
                }
                string lambda = group.HasLambda ? " λ": "";
                parts[i] = group.Term.Name.PadRight(maxWidth) + " → " + firstStr + lambda;
                ++i;
            }
            Array.Sort(parts);
            return parts.JoinLines();
        }
    }
}
