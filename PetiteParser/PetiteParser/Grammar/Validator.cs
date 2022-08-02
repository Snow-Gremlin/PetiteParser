using PetiteParser.Misc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PetiteParser.Grammar {

    /// <summary>The validator for checking if the grammar has any problems.</summary>
    internal class Validator {
        private readonly Grammar grammar;
        private readonly List<string> errors;
        private readonly HashSet<string> termUnreached;
        private readonly HashSet<string> tokenUnreached;
        private readonly HashSet<string> promptUnreached;

        /// <summary>Creates a new grammar validator.</summary>
        /// <param name="grammar">This is the grammar to validate.</param>
        public Validator(Grammar grammar) {
            this.grammar = grammar;
            this.errors  = new();
            this.termUnreached   = new();
            this.tokenUnreached  = new();
            this.promptUnreached = new();
        }

        /// <summary>Performs validation of the grammar.</summary>
        /// <returns>The errors which were found or empty on success.</returns>
        public string Validate() {
            this.errors.Clear();
            this.checkForEmptyDefinitions();
            this.checkStartTerm();
            this.checkErrorToken();
            this.checkForDuplicateTerms();
            this.checkTerms();
            this.checkReachability();
            return this.errors.JoinLines();
        }

        /// <summary>Adds a new error message to the results.</summary>
        /// <param name="message">The message of the error to add.</param>
        private void error(string message) =>
            this.errors.Add(message);

        /// <summary>Check for grammars which have nothing defined.</summary>
        private void checkForEmptyDefinitions() {
            if (!this.grammar.Terms.Any())
                this.error("No terms are defined.");
            if (!this.grammar.Tokens.Any())
                this.error("No tokens are defined.");
        }

        /// <summary>Checks that the grammar has a valid start term.</summary>
        private void checkStartTerm() {
            Term start = this.grammar.StartTerm;
            if (start is null)
                this.error("The start term is not set.");
            else if (!this.grammar.Terms.Contains(start))
                this.error("The start term, "+start+", was not found in the set of terms.");
        }

        /// <summary>Checks that, if an error token is set, then it is valid.</summary>
        private void checkErrorToken() {
            TokenItem errorTok = this.grammar.ErrorToken;
            if (errorTok is not null && !this.grammar.Tokens.Contains(errorTok))
                this.error("The error term, "+errorTok+", was not found in the set of tokens.");
        }

        /// <summary>Checks for duplicate terms.</summary>
        private void checkForDuplicateTerms() {
            List<Term> termList = this.grammar.Terms.ToList();
            for (int i = termList.Count - 1; i >= 0; i--) {
                string termName = termList[i].Name;
                for (int j = i - 1; j >= 0; j--) {
                    if (termName == termList[j].Name)
                        this.error("There exists two terms with the same name, "+termName+".");
                }
            }
        }

        /// <summary>Checks all the terms are valid.</summary>
        private void checkTerms() {
            foreach (Term term in this.grammar.Terms) {
                this.checkTermNames(term);
                this.checkForDuplicateRules(term);
                foreach (Rule rule in term.Rules) {
                    this.checkTermRuleTerm(term, rule);
                    this.checkForLoopingRule(term, rule);
                    foreach (Item item in rule.Items)
                        this.checkTermRuleItem(term, item);
                }
            }
        }

        /// <summary>Checks the names of the terms.</summary>
        /// <param name="term">The term to check the name of.</param>
        private void checkTermNames(Term term) {
            if (string.IsNullOrWhiteSpace(term.Name))
                this.error("There exists a term which has a whitespace or empty name.");

            if (term.Rules.Count <= 0)
                this.error("The term, "+term+", has no rules defined for it.");
        }

        /// <summary>Checks for duplicate rules in a term.</summary>
        /// <param name="term">The term to check for duplicate rules within.</param>
        private void checkForDuplicateRules(Term term) {
            for (int i = term.Rules.Count - 1; i >= 0; i--) {
                Rule rule = term.Rules[i];
                for (int j = i - 1; j >= 0; j--) {
                    if (rule == term.Rules[j])
                        this.error("There exists two rules which are the same, "+rule+".");
                }
            }
        }

        /// <summary>Checks the term in a rule of a term are the same.</summary>
        /// <param name="term">The term to check is at the start of the given rule.</param>
        /// <param name="rule">The rule to check the term of.</param>
        private void checkTermRuleTerm(Term term, Rule rule) {
            if (rule.Term is null)
                this.error("The rule for "+term+" has a nil term.");
            else if (rule.Term != term)
                this.error("The rule for "+term+" says it is for "+rule.Term+".");
        }

        /// <summary>Checks for rules which loop because they are non-productive.</summary>
        /// <param name="term">The term the given rule is from.</param>
        /// <param name="rule">The rule to check is productive.</param>
        private void checkForLoopingRule(Term term, Rule rule) {
            int count = rule.BasicItems.Count();
            if (count == 1) {
                Item item = rule.BasicItems.First();
                if (item is Term other && other == term)
                    this.error("There exists a rule for "+term+" which is nonproductive, "+rule+".");
            }
        }

        /// <summary>Checks an item from a term in a rule is valid.</summary>
        /// <param name="term">The term to check the item for.</param>
        /// <param name="item">The item from a rule in the given term to check.</param>
        private void checkTermRuleItem(Term term, Item item) {
            if (string.IsNullOrWhiteSpace(item.Name))
                this.error("There exists an item in rule for "+term+" which is all whitespace or empty.");

            if (item is Term) {
                if (!this.grammar.Terms.Contains(item))
                    this.error("The term, "+item+", in a rule for "+term+", was not found in the set of terms.");
            } else if (item is TokenItem) {
                if (!this.grammar.Tokens.Contains(item))
                    this.error("The token, "+item+", in a rule for "+term+", was not found in the set of tokens.");
            } else if (item is Prompt) {
                if (!this.grammar.Prompts.Contains(item))
                    this.error("The prompt, "+item+", in a rule for "+term+", was not found in the set of prompts.");
            } else this.error("Unknown item type in "+term+", "+item+".");
        }

        /// <summary>Check that all terms, tokens, and prompts are used in the grammar.</summary>
        private void checkReachability() {
            this.termUnreached.Clear();
            this.tokenUnreached.Clear();
            this.promptUnreached.Clear();

            this.termUnreached.UnionWith(this.grammar.Terms.ToNames());
            this.tokenUnreached.UnionWith(this.grammar.Tokens.ToNames());
            this.promptUnreached.UnionWith(this.grammar.Prompts.ToNames());

            this.touch(this.grammar.StartTerm);

            if (this.grammar.ErrorToken is not null)
                tokenUnreached.Remove(this.grammar.ErrorToken.Name);

            if (termUnreached.Count > 0)
                this.error("The following terms are unreachable: " + termUnreached.Join(", "));

            if (tokenUnreached.Count > 0)
                this.error("The following tokens are unreachable: " + tokenUnreached.Join(", "));

            if (promptUnreached.Count > 0)
                this.error("The following prompts are unreachable:" + promptUnreached.Join(", "));
        }

        /// <summary>This indicates that the given item has been reached and will recursively touch its own items.</summary>
        /// <param name="item">The item to mark as reachable.</param>
        private void touch(Item item) {
            if (item is Term) {
                Term term = item as Term;
                if (this.termUnreached.Contains(term.Name)) {
                    this.termUnreached.Remove(term.Name);
                    foreach (Rule r in term.Rules)
                        foreach (Item innerItem in r.Items)
                            this.touch(innerItem);
                }
            } else if (item is TokenItem) this.tokenUnreached.Remove(item.Name);
            else if (item is Prompt) this.promptUnreached.Remove(item.Name);
        }
    }
}
