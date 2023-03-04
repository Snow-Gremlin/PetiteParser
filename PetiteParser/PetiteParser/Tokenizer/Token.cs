using PetiteParser.Scanner;

namespace PetiteParser.Tokenizer;

/// <summary>A token contains the text and information from a tokenizer.</summary>
/// <remarks>The locations may be null for programmatically defined tokens which have no real location.</remarks>
public readonly record struct Token {

    /// <summary>Creates a new token with the same start and stop locations.</summary>
    /// <param name="name">The name of the token type.</param>
    /// <param name="text">The text for this token.</param>
    /// <param name="start">The start location of the input string.</param>
    /// <param name="end">The index offset from the end of the input string. In null then start will be used.</param>
    public Token(string name, string text, Location? start, Location? end) {
        this.Name  = name;
        this.Text  = text;
        this.Start = start;
        this.End   = end ?? start;
    }

    /// <summary>The name of the token type.</summary>
    public string Name { get; }
    
    /// <summary>The text for this token.</summary>
    public string Text { get; }
    
    /// <summary>The index offset from the start of the input string.</summary>
    public Location? Start { get; }
    
    /// <summary>The index offset from the end of the input string.</summary>
    public Location? End { get; }

    /// <summary>Gets the string for the token.</summary>
    /// <returns>The token's string.</returns>
    public override string ToString() =>
        this.Name + ":(" + (this.Start?.ToString() ?? "-") + "):\"" + Formatting.Text.Escape(this.Text) + "\"";
}
