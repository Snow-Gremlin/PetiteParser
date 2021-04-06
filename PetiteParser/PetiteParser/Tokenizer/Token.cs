namespace PetiteParser.Tokenizer {

    /// <summary>A token contains the text and information from a tokenizer.</summary>
    public class Token {

        /// <summary>The name of the token type.</summary>
        public readonly string Name;

        /// <summary>The text for this token.</summary>
        public readonly string Text;

        /// <summary>The index offset from the start of the input string.</summary>
        public readonly int Index;

        /// <summary>Creates a new token.</summary>
        /// <param name="name">The name of the token type.</param>
        /// <param name="text">The text for this token.</param>
        /// <param name="index">The index offset from the start of the input string.</param>
        public Token(string name, string text, int index) {
            this.Name = name;
            this.Text = text;
            this.Index = index;
        }

        /// <summary>Gets the string for the token.</summary>
        /// <returns>The token's string.</returns>
        public override string ToString() =>
            this.Name+":"+this.Index+":\""+Tokenizer.EscapeText(this.Text)+"\"";
    }
}
