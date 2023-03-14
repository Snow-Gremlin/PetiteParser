using PetiteParser.Scanner;

namespace PetiteParser.Tokenizer;

/// <summary>A token contains the text and information from a tokenizer.</summary>
/// <remarks>The locations may be null for programmatically defined tokens which have no real location.</remarks>
/// <param name="name">The name of the token type.</param>
/// <param name="text">The text for this token.</param>
/// <param name="start">The start location of the input string.</param>
/// <param name="end">The end location of the input string. In null then start will be used.</param>
public readonly record struct Token(string Name, string Text, Location? Start, Location? End) {

    /// <summary>Gets the string for the token.</summary>
    /// <returns>The token's string.</returns>
    public override string ToString() =>
        this.Name + ":(" + (this.Start?.ToString() ?? "-") + "):"+
            "\"" + Formatting.Text.Escape(this.Text) + "\"";
}
