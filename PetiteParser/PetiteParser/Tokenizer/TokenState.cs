using System.Collections.Generic;

namespace PetiteParser.Tokenizer {

    /// <summary>
    /// A token state is added to a state to indicate that the state is acceptance for a token.
    /// </summary>
    public class TokenState {

        /// <summary>The tokenizer for this token state.</summary>
        private readonly Tokenizer tokenizer;

        /// <summary>The map from text to replacement token name.</summary>
        private Dictionary<string, string> replace;

        /// <summary> Creates a new token state for the given tokenizer. </summary>
        /// <param name="tokenizer">The tokenizer for this token state.</param>
        /// <param name="name">The name of this token.</param>
        public TokenState(Tokenizer tokenizer, string name) {
            this.tokenizer = tokenizer;
            this.Name = name;
            this.replace = new Dictionary<string, string>();
        }

        /// <summary>Gets the name of this token.</summary>
        public string Name { get; }

        /// <summary>
        /// Adds a replacement which replaces this token's name with the given token name
        /// when the accepted text is the same as any of the given text.
        /// </summary>
        /// <param name="tokenName">The name of the token to use.</param>
        /// <param name="text">The text to use the given token name instead of this states name.</param>
        public void Replace(string tokenName, params string[] text) =>
            this.Replace(tokenName, text as IEnumerable<string>);

        /// <summary>
        /// Adds a replacement which replaces this token's name with the given token name
        /// when the accepted text is the same as any of the given text.
        /// </summary>
        /// <param name="tokenName">The name of the token to use.</param>
        /// <param name="text">The text to use the given token name instead of this states name.</param>
        public void Replace(string tokenName, IEnumerable<string> text) {
            foreach (string t in text) this.replace.Add(t, tokenName);
        }

        /// <summary>
        /// Indicates that tokens with this name should not be emitted but quietly consumed.
        /// </summary>
        public void Consume() => this.tokenizer.Consume(this.Name);

        /// <summary>
        /// Creates a token for this token state and the given text.
        /// If the text matches a replacement's text the replacement token is used instead.
        /// </summary>
        /// <param name="text">The text for the token.</param>
        /// <param name="index">The index the token was read from.</param>
        /// <returns>The new token from this token state.</returns>
        public Token GetToken(string text, int index) =>
            new Token(this.replace.ContainsKey(text) ? this.replace[text] : this.Name, text, index);

        /// <summary>Gets the name for this token state.</summary>
        /// <returns>The token state's string.</returns>
        public override string ToString() => this.Name;
    }
}
