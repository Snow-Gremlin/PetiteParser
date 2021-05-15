using PetiteParser.Tokenizer;
using System.Drawing;

namespace Examples.CodeColoring {
    public class Formatting {
        public readonly int Index;
        public readonly int Length;
        public readonly Color Color;
        public readonly Font Font;

        public Formatting(int index, int length, Color color, Font font) {
            this.Index = index;
            this.Length = length;
            this.Color = color;
            this.Font = font;
        }

        public Formatting(Token token, Color color, Font font) {
            this.Index = token.Location.Index;
            this.Length = token.Text.Length;
            this.Color = color;
            this.Font = font;
        }
    }
}
