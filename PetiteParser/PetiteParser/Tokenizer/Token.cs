namespace PetiteParser.Tokenizer;

/// <summary>A token contains the text and information from a tokenizer.</summary>
sealed public class Token {

    /// <summary>The name of the token type.</summary>
    public readonly string Name;

    /// <summary>The text for this token.</summary>
    public readonly string Text;

    /// <summary>The index offset from the start of the input string.</summary>
    /// <remarks>This may be null for programmatically defined tokens which have no real location.</remarks>
    public readonly Scanner.Location Start;

    /// <summary>The index offset from the end of the input string.</summary>
    /// <remarks>This may be null for programmatically defined tokens which have no real location.</remarks>
    public readonly Scanner.Location End;

    /// <summary>Creates a new token.</summary>
    /// <param name="name">The name of the token type.</param>
    /// <param name="text">The text for this token.</param>
    /// <param name="start">The start location of the input string.</param>
    /// <param name="end">The end location of the input string.</param>
    public Token(string name, string text, Scanner.Location start, Scanner.Location end = null) {
        this.Name  = name;
        this.Text  = text;
        this.Start = start;
        this.End   = end ?? start;
    }

    /// <summary>Gets the string for the token.</summary>
    /// <returns>The token's string.</returns>
    public override string ToString() =>
        this.Name+":("+(this.Start?.ToString() ?? "-")+"):\""+Misc.Text.Escape(this.Text)+"\"";
}
