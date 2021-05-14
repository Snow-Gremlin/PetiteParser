using PetiteParser.Loader;
using PetiteParser.Tokenizer;
using System;
using System.Collections.Generic;
using System.Drawing;
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

        public Glsl() { }

        public override string ToString() => "GLSL";

        public IEnumerable<Formatted> Colorize(params string[] input) {
            singleton ??= createTokenizer();
            return this.colorize(singleton.Tokenize(string.Join(Environment.NewLine, input)));
        }

        private IEnumerable<Formatted> colorize(IEnumerable<Token> tokens) {
            foreach (Token token in tokens) {
                foreach (Formatted fmt in this.colorize(token))
                    yield return fmt;
            }
        }

        private IEnumerable<Formatted> colorize(Token token) {
            switch (token.Name) {
                case "Builtin":    yield return new Formatted(token, 0x441111); break;
                case "Comment":    yield return new Formatted(token, 0x777777); break;
                case "Id":         yield return new Formatted(token, 0x111111); break;
                case "Num":        yield return new Formatted(token, 0x119911); break;
                case "Preprocess": yield return new Formatted(token, 0x773377); break;
                case "Reserved":   yield return new Formatted(token, 0x111199); break;
                case "Symbol":     yield return new Formatted(token, 0x661111); break;
                case "Type":       yield return new Formatted(token, 0x117711); break;
                case "Whitespace": yield return new Formatted(token, 0x111111); break;
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
