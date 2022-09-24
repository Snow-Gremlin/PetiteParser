using PetiteParser.Tokenizer;
using System.Drawing;

namespace Examples.CodeColoring;

/// <summary>The formatting to apply to a range of text.</summary>
/// <param name="Token">The token to format.</param>
/// <param name="Color">The color to format the text with.</param>
/// <param name="Font">The font to format the text with.</param>
public readonly record struct Formatting(Token Token, Color Color, Font Font) {

    /// <summary>Determines if the given formatting is the same.</summary>
    /// <remarks>The offset and locations aren't checked.</remarks>
    /// <param name="fmt">The formatting to check against.</param>
    /// <returns>True if they are the same, false otherwise.</returns>
    public bool Same(Formatting fmt) =>
        this.Token.Name == fmt.Token.Name &&
        this.Token.Text == fmt.Token.Text &&
        this.Color      == fmt.Color      &&
        this.Font.Equals(fmt.Font);
}
