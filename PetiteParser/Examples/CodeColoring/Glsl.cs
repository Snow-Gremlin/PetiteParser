using PetiteParser.Loader;
using PetiteParser.Tokenizer;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Examples.CodeColoring {
    public class Glsl: IColorer {
        private const string languageFile = "Examples.CodeColoring.Glsl.lang";
        private const string exampleFile = "Examples.CodeColoring.ExampleGlsl.txt";

        static private Tokenizer createTokenizer() {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using Stream stream = assembly.GetManifestResourceStream(languageFile);
            using StreamReader reader = new(stream);
            return Loader.LoadTokenizer(reader.ReadToEnd());
        }

        static private Tokenizer singleton;
        static private Font font;

        public Glsl() {}

        public override string ToString() => "GLSL";

        public IEnumerable<Formatting> Colorize(params string[] input) {
            singleton ??= createTokenizer();
            font ??= new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
            return this.colorize(singleton.Tokenize(string.Join(Environment.NewLine, input)));
        }

        private IEnumerable<Formatting> colorize(IEnumerable<Token> tokens) {
            foreach (Token token in tokens) {
                foreach (Formatting fmt in this.colorize(token))
                    yield return fmt;
            }
        }

        private IEnumerable<Formatting> colorize(Token token) {
            switch (token.Name) {
                case "Builtin":    yield return new Formatting(token, Color.FromArgb(0x441111), font); break;
                case "Comment":    yield return new Formatting(token, Color.FromArgb(0x007777), font); break;
                case "Id":         yield return new Formatting(token, Color.FromArgb(0x111111), font); break;
                case "Num":        yield return new Formatting(token, Color.FromArgb(0x119911), font); break;
                case "Preprocess": yield return new Formatting(token, Color.FromArgb(0x773377), font); break;
                case "Reserved":   yield return new Formatting(token, Color.FromArgb(0x111199), font); break;
                case "Symbol":     yield return new Formatting(token, Color.FromArgb(0x661111), font); break;
                case "Type":       yield return new Formatting(token, Color.FromArgb(0x117711), font); break;
                case "Whitespace": yield return new Formatting(token, Color.FromArgb(0x111111), font); break;
            }
        }

        public string ExampleCode {
            get {
                Assembly assembly = Assembly.GetExecutingAssembly();
                using Stream stream = assembly.GetManifestResourceStream(exampleFile);
                using StreamReader reader = new(stream);
                return reader.ReadToEnd();
            }
        }
    }
}
