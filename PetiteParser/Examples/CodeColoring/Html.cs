using PetiteParser.Loader;
using PetiteParser.Tokenizer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Examples.CodeColoring {
    public class Html: IColorer {
        private const string languageFile = "Examples.CodeColoring.Html.lang";
        private const string exampleFile = "Examples.CodeColoring.Html.txt";

        /*
        public Html() : base("HTML", createTokenizer()) { }

        static private Tokenizer createTokenizer() =>
            Loader.LoadTokenizer(
                "> (Start);",
                "(Start): 'a'..'z', 'A'..'Z', '_' => (Id): 'a'..'z', 'A'..'Z', '_', '0'..'9' => [Id];'",
                "(Id): '=' => [Attribute];",
                "(Start): '<' => (Start-Tag):",
                "(Start): '/\\-!>=' => (Symbol): '</\\-!>=' => [Symbol];",
                "(Start): '\"' => (String-Open): '\"' => [String];",
                "(String-Open): '\\' => (String-Escape): * => (String-Open);",
                "(Start): * => (Other): !('</\\-!>=_', 'a'..'z', 'A'..'Z'): => [Other];",
                "[Id] = 'DOCTYPE', 'html', 'head', 'meta', 'link', 'title', 'body', 'script' => [Reserved];");

        protected override void ProcessToken(Token token) {
            switch (token.Name) {
                case "Attribute":
                    this.appendText(token.Text.Substring(0, token.Text.Length-1), Color.FromArgb(0x991111));
                    this.appendText("=", Color.FromArgb(0x111111));
                    break;
                case "Comment":    this.appendText(token.Text, Color.FromArgb(0x777777)); break;
                case "Id":         this.appendText(token.Text, Color.FromArgb(0x111111)); break;
                case "Num":        this.appendText(token.Text, Color.FromArgb(0x119911)); break;
                case "Preprocess": this.appendText(token.Text, Color.FromArgb(0x773377)); break;
                case "Reserved":   this.appendText(token.Text, Color.FromArgb(0x111199)); break;
                case "Symbol":     this.appendText(token.Text, Color.FromArgb(0x661111)); break;
                case "Type":       this.appendText(token.Text, Color.FromArgb(0x117711)); break;
                case "Whitespace": this.appendText(token.Text, Color.FromArgb(0x111111)); break;
            }
        }
        */
        public string ExampleCode => throw new NotImplementedException();

        public IEnumerable<Formatting> Colorize(params string[] input) => throw new NotImplementedException();
    }
}
