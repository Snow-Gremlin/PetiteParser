using PetiteParser.Misc;
using System.Collections.Generic;
using System.Linq;

namespace PetiteParser.Grammar {

    /// <summary>A rule is a single definition from a grammar.</summary>
    /// <remarks>
    /// For example `<T> → "(" <E> ")"`. The term for the rule is
    /// the left hand side (`T`) while the items are the parts on the right hand size.
    /// The items are made up of tokens (`(`, `)`) and the rule's term or other terms (`E`).
    /// The order of the items defines how this rule in the grammar is to be used.
    /// </remarks>
    public class Rule {

        /// <summary>The grammar this rule belongs too.</summary>
        private readonly Grammar grammar;

        /// <summary>Creates a new rule for the given grammar and term.</summary>
        /// <param name="grammar">The grammar this rule belongs too.</param>
        /// <param name="term">Gets the left hand side term to the rule.</param>
        internal Rule(Grammar grammar, Term term) {
            this.grammar = grammar;
            this.Term = term;
            this.Items = new List<Item>();
        }

        /// <summary>Gets the left hand side term to the rule.</summary>
        public readonly Term Term;

        /// <summary>
        /// Gets all the terms, tokens, and prompts for this rule.
        /// The items are in the order defined by this rule.
        /// </summary>
        public List<Item> Items { get; }

        /// <summary>Adds a term to this rule. This will get or create a new term to the grammar.</summary>
        /// <param name="termName">The name of the term to add.</param>
        /// <returns>This rule so that rule creation can be chained.</returns>
        public Rule AddTerm(string termName) {
            this.Items.Add(this.grammar.Term(termName));
            return this;
        }

        /// <summary> Adds a token to this rule. </summary>
        /// <param name="tokenName">The name of the token.</param>
        /// <returns>This rule so that rule creation can be chained.</returns>
        public Rule AddToken(string tokenName) {
            this.Items.Add(this.grammar.Token(tokenName));
            return this;
        }

        /// <summary>Adds a prompt to this rule.</summary>
        /// <param name="promptName">The name of the prompt</param>
        /// <returns>This rule so that rule creation can be chained.</returns>
        public Rule AddPrompt(string promptName) {
            this.Items.Add(this.grammar.Prompt(promptName));
            return this;
        }

        /// <summary> Gets the set of terms and tokens without the prompts.</summary>
        public IEnumerable<Item> BasicItems =>
            this.Items.Where(item => item is not Prompt);

        /// <summary>Determines if the given rule is equal to this rule.</summary>
        /// <param name="obj">The object to compare against.</param>
        /// <returns>True if they are equal, false otherwise.</returns>
        public override bool Equals(object obj) {
            if (obj is null) return false;
            if (obj is not Rule other) return false;
            if (this.Term != other.Term) return false;
            if (this.Items.Count != other.Items.Count) return false;
            for (int i = this.Items.Count - 1; i >= 0; i--) {
                if (this.Items[i] != other.Items[i]) return false;
            }
            return true;
        }

        /// <summary>This gets the hash code for this rule.</summary>
        /// <returns>The base object's hash code.</returns>
        public override int GetHashCode() => base.GetHashCode();

        /// <summary>
        /// Gets the string for this rule. Has an optional step index
        /// for showing the different states of the parser generator.
        /// </summary>
        /// <returns>The string for this rule.</returns>
        public override string ToString() => this.ToString(-1);

        /// <summary>
        /// Gets the string for this rule. Has an optional step index
        /// for showing the different states of the parser generator.
        /// </summary>
        /// <param name="stepIndex">The index of the current step to show.</param>
        /// <returns>The string for this rule.</returns>
        public string ToString(int stepIndex) {
            List<string> parts = new();
            int index = 0;
            foreach (Item item in this.Items) {
                if (index == stepIndex) {
                    parts.Add("•");
                    stepIndex = -1;
                }
                parts.Add(item.ToString());
                if (item is not Prompt) index++;
            }
            if (index == stepIndex) parts.Add("•");
            return this.Term.ToString() + " → " + parts.Join(" ");
        }
    }
}
