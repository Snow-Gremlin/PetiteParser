using PetiteParser.Tokenizer;
using System.Drawing;

namespace Examples.CodeColoring;

/// <summary>The formatting to apply to a range of text.</summary>
sealed public class Formatting {

    /// <summary>The token for this format..</summary>
    public readonly Token Token;

    /// <summary>The color to format the characters with.</summary>
    public readonly Color Color;

    /// <summary>The font to format the characters with.</summary>
    public readonly Font Font;

    /// <summary>Creates a new formatting information.</summary>
    /// <param name="token">The token to format.</param>
    /// <param name="color">The color to format the text with.</param>
    /// <param name="font">The font to format the text with.</param>
    public Formatting(Token token, Color color, Font font) {
        this.Token = token;
        this.Color = color;
        this.Font  = font;
    }

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
