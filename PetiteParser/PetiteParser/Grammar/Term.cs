using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetiteParser.Grammar {

    /// <summary>
    /// A term is a group of rules and part of a rule which defines part of the grammar language.
    /// </summary>
    /// <remarks>
    /// For example the term `<T>` with the rules `<T> => "(" <E> ")"`,
    /// `<T> => <E> * <E>`, and `<T> => <E> + <E>`.
    /// </remarks>
    public class Term: Item {

        /// <summary>The grammar this term belongs to.</summary>
        private readonly Grammar grammar;

        /// <summary>Creates a new rule for the the given grammar and term.</summary>
        /// <param name="grammar">The grammar for this term.</param>
        /// <param name="name">The name of this term.</param>
        internal Term(Grammar grammar, string name): base(name) {
            this.grammar = grammar;
            this.Rules = new List<Rule>();
        }

        /// <summary>Gets the list of rules starting with this term.</summary>
        public List<Rule> Rules { get; }

        /// <summary>Adds a new rule to this term.</summary>
        /// <returns>The newly added rule.</returns>
        public Rule NewRule() {
            Rule rule = new(this.grammar, this);
            this.Rules.Add(rule);
            return rule;
        }

        /// <summary>Gets the string for this term.</summary>
        /// <returns>This is the name of the term.</returns>
        public override string ToString() => "<"+base.ToString()+">";

        /// <summary>Determines the first tokens that can be reached from the rules of this term.</summary>
        /// <returns>The first tokens in this term.</returns>
        public List<TokenItem> DetermineFirsts() {
            HashSet<TokenItem> tokens = new();
            determineFirsts(this, tokens, new HashSet<Term>());
            return tokens.ToList();
        }

        /// <summary>
        /// Determines the follow tokens that can be reached from the reduction of all the rules,
        /// i.e. the tokens which follow after the term and any first term in all the rules.
        /// </summary>
        /// <returns>The follow tokens in this term.</returns>
        public List<TokenItem> DetermineFollows() {
            HashSet<TokenItem> tokens = new();
            determineFollows(this, tokens, new HashSet<Term>());
            return tokens.ToList();
        }

        /// <summary>
        /// This is the recursive part of the determination of the first token sets which
        /// allows for terms which have already been checked to not be checked again.
        /// </summary>
        /// <param name="tokens">The tokens set to add to.</param>
        /// <param name="checkedTerms">The terms which have already been checked.</param>
        static private void determineFirsts(Term term, HashSet<TokenItem> tokens, HashSet<Term> checkedTerms) {
            if (checkedTerms.Contains(term)) return;
            checkedTerms.Add(term);
            bool needFollows = false;
            foreach (Rule rule in term.Rules) {
                if (determineRuleFirsts(rule, tokens, checkedTerms)) needFollows = true;
            }
            if (needFollows) determineFollows(term, tokens, new HashSet<Term>());
        }

        /// <summary>
        /// This determines the firsts for the given rule.
        /// If the rule has no tokens or terms this will return true
        /// indicating that the rule needs follows to be added.
        /// </summary>
        /// <param name="rule">The rule to get the firsts from.</param>
        /// <param name="tokens">The set of tokens to add to.</param>
        /// <param name="checkedTerms">The terms which have already been checked.</param>
        /// <returns>True to follows are needed, false if a term or token was reached.</returns>
        static private bool determineRuleFirsts(Rule rule, HashSet<TokenItem> tokens, HashSet<Term> checkedTerms) {
            foreach (Item item in rule.Items) {
                if (item is Term) {
                    determineFirsts(item as Term, tokens, checkedTerms);
                    return false;
                } else if (item is TokenItem) {
                    tokens.Add(item as TokenItem);
                    return false;
                }
                // else if Prompt continue.
            }
            return true;
        }

        /// This is the recursive part of the determination of the follow token sets which
        /// allows for terms which have already been checked to not be checked again.
        static private void determineFollows(Term term, HashSet<TokenItem> tokens, HashSet<Term> checkedTerms) {
            if (checkedTerms.Contains(term)) return;
            checkedTerms.Add(term);
            foreach (Term other in term.grammar.Terms) {
                foreach (Rule rule in other.Rules) {
                    List<Item> items = rule.BasicItems;

                    int count = items.Count;
                    for (int i = 0; i < count-1; i++) {
                        if (items[i] == term) {
                            Item item = items[i+1];
                            if (item is Term)
                                determineFirsts(item as Term, tokens, new HashSet<Term>());
                            else tokens.Add(item as TokenItem);
                        }
                    }

                    if ((items.Count > 0) && (items[count-1] == term))
                        determineFollows(other, tokens, checkedTerms);
                }
            }
        }
    }
}
