using System;
using System.Collections.Generic;

namespace PetiteParser.Grammar {

    /// <summary>
    /// This is a tool for calculating the firsts tokens for the terms of a grammar.
    /// </summary>
    public class TokenSets {

        /// <summary>This stores the token sets for a term in the grammar.</summary>
        private class TermGroup {

            /// <summary>The term this group if for.</summary>
            public readonly Term term;

            /// <summary>
            /// Indicates if this term has rules such that it can
            /// pass over this term without consuming any tokens.
            /// </summary>
            public bool hasLambda;

            /// <summary>Indicates this group needs to be updated.</summary>
            public bool update;

            /// <summary>The set of first tokens for this term.</summary>
            public HashSet<TokenItem> tokens;
            
            /// <summary>The other terms which depends in at least one rule on this term.</summary>
            public HashSet<TermGroup> dependents;

            /// <summary>The other terms which this term depends upon in at least one rule.</summary>
            public HashSet<TermGroup> parents;

            /// <summary>Creates a new term group of first tokens.</summary>
            /// <param name="term">The term this group belongs too.</param>
            public TermGroup(Term term) {
                this.term       = term;
                this.hasLambda  = false;
                this.update     = true;
                this.tokens     = new HashSet<TokenItem>();
                this.dependents = new HashSet<TermGroup>();
                this.parents    = new HashSet<TermGroup>();
            }
        }

        private Dictionary<Term, TermGroup> terms;

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

        public bool Firsts(Item item, HashSet<TokenItem> tokens) {
            if (item is TokenItem) {
                tokens.Add(item as TokenItem);
                return false;
            } else if (item is Term) {
                TermGroup group = this.terms[item as Term];
                foreach (TokenItem token in group.tokens)
                    tokens.Add(token);
                return group.hasLambda;
            } else return false; // Prompt
        }

        static private void joinGroups(TermGroup parent, TermGroup dep) {
            parent.dependents.Add(dep);
            dep.parents.Add(parent);
        }

        private bool propageteRule(TermGroup group, Rule rule) {
            bool updated = false;
            foreach (Item item in rule.Items) {

                // Check if token, if so skip the lambda check and just leave.
                if (item is TokenItem)
                    return group.tokens.Add(item as TokenItem);

                if (item is Term) {
                    Term term = item as Term;
                    TermGroup parent = this.terms[term];
                    joinGroups(parent, group);
                    foreach (TermGroup grand in parent.parents)
                        joinGroups(grand, group);
                    foreach (TokenItem token in parent.tokens) {
                        if (group.tokens.Add(token)) updated = true;
                    }
                    if (!parent.hasLambda) return updated;
                }

                // else ignore because it is Prompt
            }

            // If the end has been reached with out stopping
            if (!group.hasLambda) {
                group.hasLambda = true;
                updated = true;
            }
            return updated;
        }

        private bool propagate(Term term) {
            TermGroup group = this.terms[term];
            if (!group.update) return false;
            group.update = false;

            // Run through all rules and update with them.
            bool updated = false;
            foreach (Rule rule in term.Rules) {
                if (this.propageteRule(group, rule)) updated = true;
            }

            // Mark all dependents as needing updates.
            if (updated) {
                foreach (TermGroup dep in group.dependents)
                    dep.update = true;
            }
            return updated;
        }

        public override string ToString() {
            int maxWidth = 0;
            foreach (Term term in this.terms.Keys)
                maxWidth = Math.Max(maxWidth, term.Name.Length);

            string[] parts = new string[this.terms.Count];
            int i = 0;
            foreach (TermGroup group in this.terms.Values) {
                string firstStr = "";
                if (group.tokens.Count > 0) {
                    string[] firsts = new string[group.tokens.Count];
                    int j = 0;
                    foreach (TokenItem item in group.tokens) {
                        firsts[j] = item.Name;
                        ++j;
                    }
                    Array.Sort(firsts);
                    firstStr = "["+string.Join(", ", firsts) +"]";
                }
                string lambda = group.hasLambda ? " λ": "";
                parts[i] = group.term.Name.PadRight(maxWidth) + " → " + firstStr + lambda;
                ++i;
            }
            Array.Sort(parts);
            return string.Join(Environment.NewLine, parts);
        }
    }
}
