using PetiteParser.Loader;
using PetiteParser.Parser;
using PetiteParser.Tokenizer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Examples.CodeColoring.Json {

    /// <summary>A colorer for JSON, JavaScript Object Notation language.</summary>
    /// <see cref="https://www.json.org/json-en.html"/>
    /// <see cref="https://json.org/example.html"/>
    public class Json: IColorer {
        private const string languageFile = "Examples.CodeColoring.Json.Json.lang";
        private const string exampleFile  = "Examples.CodeColoring.Json.Json.txt";

        /// <summary>Loads the JSON parser.</summary>
        /// <returns>The JSON parser.</returns>
        static private Parser createParser() {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using Stream stream = assembly.GetManifestResourceStream(languageFile);
            using StreamReader reader = new(stream);
            return Loader.LoadParser(reader.ReadToEnd());
        }
        
        static private Parser singleton;
        static private Font font;

        /// <summary>Creates a new JSON colorizer.</summary>
        public Json() {}

        /// <summary>Gets the name for this colorizer.</summary>
        /// <returns>The colorizer name.</returns>
        public override string ToString() => "JSON";

        /// <summary>Returns the color formatting for the given input text.</summary>
        /// <param name="input">The input text to colorize.</param>
        /// <returns>The formatting to color the input with.</returns>
        public IEnumerable<Formatting> Colorize(params string[] input) {
            singleton ??= createParser();
            font      ??= new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
            return colorize(singleton.Tokenize(string.Join(Environment.NewLine, input)));
        }

        /// <summary>Returns the color formatting for the given tokens.</summary>
        /// <param name="tokens">The tokens to colorize.</param>
        /// <returns>The formatting color for the given tokens.</returns>
        static private IEnumerable<Formatting> colorize(IEnumerable<Token> tokens) =>
            tokens.Select((token) => colorize(token));

        /// <summary>Returns the color formatting for the given token.</summary>
        /// <param name="token">The token to color.</param>
        /// <returns>The color for the given token.</returns>
        static private Formatting colorize(Token token) =>
            token.Name switch {
                "Builtin"    => new Formatting(token, Color.DarkRed,     font),
                "Comment"    => new Formatting(token, Color.DarkGreen,   italic),
                "Id"         => new Formatting(token, Color.Black,       font),
                "Num"        => new Formatting(token, Color.Blue,        font),
                "Preprocess" => new Formatting(token, Color.DarkMagenta, font),
                "Reserved"   => new Formatting(token, Color.DarkRed,     font),
                "Symbol"     => new Formatting(token, Color.DarkRed,     font),
                "Type"       => new Formatting(token, Color.DarkBlue,    font),
                "Whitespace" => new Formatting(token, Color.Black,       font),
                _            => new Formatting(token, Color.Black,       font),
            };

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
