using PetiteParser.Tokenizer;
using System.Drawing;

namespace Examples.CodeColoring {

    /// <summary>The formatting to apply to a range of text.</summary>
    public class Formatting {

        /// <summary>The offset from the start of the input to start formatting at.</summary>
        public readonly int Index;

        /// <summary>The number of characters to format.</summary>
        public readonly int Length;

        /// <summary>The color to format the characters with.</summary>
        public readonly Color Color;

        /// <summary>The font to format the characters with.</summary>
        public readonly Font Font;

        /// <summary>Creates a new formatting information.</summary>
        /// <param name="index">The index to start formatting at.</param>
        /// <param name="length">The number of characters to format.</param>
        /// <param name="color">The color to format the text with.</param>
        /// <param name="font">The font to format the text with.</param>
        public Formatting(int index, int length, Color color, Font font) {
            this.Index = index;
            this.Length = length;
            this.Color = color;
            this.Font = font;
        }

        /// <summary>Creates a new formatting information.</summary>
        /// <param name="token">The token to format.</param>
        /// <param name="color">The color to format the text with.</param>
        /// <param name="font">The font to format the text with.</param>
        public Formatting(Token token, Color color, Font font) {
            this.Index = token.Start.Index;
            this.Length = token.Text.Length;
            this.Color = color;
            this.Font = font;
        }
    }
}
