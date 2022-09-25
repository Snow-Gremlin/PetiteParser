namespace PetiteParser.Tokenizer;

/// <summary>A token contains the text and information from a tokenizer.</summary>
/// <remarks>The locations may be null for programmatically defined tokens which have no real location.</remarks>
/// <param name="Name">The name of the token type.</param>
/// <param name="Text">The text for this token.</param>
/// <param name="Start">The index offset from the start of the input string.</param>
/// <param name="End">The index offset from the end of the input string.</param>
public readonly record struct Token(string Name, string Text, Scanner.Location? Start, Scanner.Location? End) {

    /// <summary>Creates a new token with the same start and stop locations.</summary>
    /// <param name="name">The name of the token type.</param>
    /// <param name="text">The text for this token.</param>
    /// <param name="start">The start location of the input string.</param>
    public Token(string name, string text, Scanner.Location? start) : this(name, text, start, start) { }

    /// <summary>Gets the string for the token.</summary>
    /// <returns>The token's string.</returns>
    public override string ToString() =>
        this.Name + ":(" + (this.Start?.ToString() ?? "-") + "):\"" + Misc.Text.Escape(this.Text) + "\"";
}
