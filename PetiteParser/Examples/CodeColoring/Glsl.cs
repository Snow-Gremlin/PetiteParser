using PetiteParser.Loader;
using PetiteParser.Tokenizer;
using System;
using System.Drawing;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Examples.CodeColoring {

    /// <summary>A colorer for GLSL, openGL shader language.</summary>
    public class Glsl: IColorer {
        private const string languageFile = "Examples.CodeColoring.Glsl.lang";
        private const string exampleFile = "Examples.CodeColoring.Glsl.txt";

        /// <summary>Loads the GLSL tokenizer.</summary>
        /// <returns>The GLSL tokenizer.</returns>
        static private Tokenizer createTokenizer() {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using Stream stream = assembly.GetManifestResourceStream(languageFile);
            using StreamReader reader = new(stream);
            return Loader.LoadTokenizer(reader.ReadToEnd());
        }

        static private Tokenizer singleton;
        static private Font font;
        static private Font italic;

        /// <summary>Creates a new GLSL colorizer.</summary>
        public Glsl() {}

        /// <summary>Gets the name for this colorizer.</summary>
        /// <returns>The colorizer name.</returns>
        public override string ToString() => "GLSL";

        /// <summary>Returns the color formatting for the given input text.</summary>
        /// <param name="input">The input text to colorize.</param>
        /// <returns>The formatting to color the input with.</returns>
        public IEnumerable<Formatting> Colorize(params string[] input) {
            singleton ??= createTokenizer();
            font ??= new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
            italic ??= new Font("Consolas", 9F, FontStyle.Italic, GraphicsUnit.Point);
            return this.colorize(singleton.Tokenize(string.Join(Environment.NewLine, input)));
        }

        /// <summary>Returns the color formatting for the given tokens.</summary>
        /// <param name="tokens">The tokens to colorize.</param>
        /// <returns>The formatting color for the given tokens.</returns>
        private IEnumerable<Formatting> colorize(IEnumerable<Token> tokens) {
            foreach (Token token in tokens) {
                yield return this.colorize(token);
            }
        }

        /// <summary>Returns the color formatting for the given token.</summary>
        /// <param name="token">The token to color.</param>
        /// <returns>The color for the given token.</returns>
        private Formatting colorize(Token token) {
            switch (token.Name) {
                case "Builtin":    return new Formatting(token, Color.DarkRed,     font);
                case "Comment":    return new Formatting(token, Color.DarkGreen, italic);
                case "Id":         return new Formatting(token, Color.Black,       font);
                case "Num":        return new Formatting(token, Color.Blue,        font);
                case "Preprocess": return new Formatting(token, Color.DarkMagenta, font);
                case "Reserved":   return new Formatting(token, Color.DarkRed,     font);
                case "Symbol":     return new Formatting(token, Color.DarkRed,     font);
                case "Type":       return new Formatting(token, Color.DarkBlue,    font);
                case "Whitespace": return new Formatting(token, Color.Black,       font);
                default:           return new Formatting(token, Color.Black,       font);
            }
        }

        /// <summary>Returns an example text which this will colorize.</summary>
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
