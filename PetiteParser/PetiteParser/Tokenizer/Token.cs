namespace PetiteParser.Tokenizer {

    /// <summary>A token contains the text and information from a tokenizer.</summary>
    public class Token {

        /// <summary>The name of the token type.</summary>
        public readonly string Name;

        /// <summary>The text for this token.</summary>
        public readonly string Text;

        /// <summary>The index offset from the start of the input string.</summary>
        /// <remarks>This may be null for programmatically defined tokens which have no real location.</remarks>
        public readonly Location Location;

        /// <summary>Creates a new token.</summary>
        /// <param name="name">The name of the token type.</param>
        /// <param name="text">The text for this token.</param>
        /// <param name="location">The location of the input string.</param>
        public Token(string name, string text, Location location) {
            this.Name = name;
            this.Text = text;
            this.Location = location;
        }

        /// <summary>Gets the string for the token.</summary>
        /// <returns>The token's string.</returns>
        public override string ToString() =>
            this.Name+":("+(this.Location?.ToString() ?? "-")+"):\""+Misc.Text.Escape(this.Text)+"\"";
    }
}
