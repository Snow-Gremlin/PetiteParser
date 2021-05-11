using PetiteParser.Tokenizer;
using System.Drawing;

namespace Examples.CodeColoring {
    public class Formatted {
        public readonly int Index;
        public readonly string Token;
        public readonly string Text;
        public readonly Color Color;
        public readonly bool Bold;
        public readonly bool Italic;

        public Formatted(int index, string token, string text, Color color, bool bold = false, bool italic = false) {
            this.Index = index;
            this.Token = token;
            this.Text = text;
            this.Color = color;
            this.Bold = bold;
            this.Italic = italic;
        }

        public Formatted(int index, string token, string text, int color, bool bold = false, bool italic = false) {
            this.Index = index;
            this.Token = token;
            this.Text = text;
            this.Color = Color.FromArgb(color);
            this.Bold = bold;
            this.Italic = italic;
        }

        public Formatted(Token token, Color color, bool bold = false, bool italic = false) {
            this.Index = token.Index;
            this.Token = token.Name;
            this.Text = token.Text;
            this.Color = color;
            this.Bold = bold;
            this.Italic = italic;
        }

        public Formatted(Token token, int color, bool bold = false, bool italic = false) {
            this.Index = token.Index;
            this.Token = token.Name;
            this.Text = token.Text;
            this.Color = Color.FromArgb(color);
            this.Bold = bold;
            this.Italic = italic;
        }
    }
}
