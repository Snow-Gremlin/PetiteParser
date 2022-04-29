using PetiteParser.Loader;
using PetiteParser.Misc;
using PetiteParser.Parser;
using PetiteParser.ParseTree;
using PetiteParser.Tokenizer;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Reflection;
using System;

namespace Examples.CodeColoring.Json {

    /// <summary>A colorer for JSON, JavaScript Object Notation language.</summary>
    /// <see cref="https://www.json.org/json-en.html"/>
    /// <see cref="https://json.org/example.html"/>
    public class Json: IColorer {
        private const string languageFile = "Examples.CodeColoring.Json.Json.lang";
        private const string exampleFile  = "Examples.CodeColoring.Json.Json.json";

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
        public Json() { }

        /// <summary>Gets the name for this colorizer.</summary>
        /// <returns>The colorizer name.</returns>
        public override string ToString() => "JSON";

        /// <summary>Returns the color formatting for the given input text.</summary>
        /// <param name="input">The input text to colorize.</param>
        /// <returns>The formatting to color the input with.</returns>
        public IEnumerable<Formatting> Colorize(params string[] input) {
            singleton ??= createParser();
            font      ??= new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);

            Result result = singleton.Parse(input.JoinLines());
            if (result is not null) {
                // On error, don't update the colors.
                if (result.Errors.Length > 0) yield break;

                // Run though the resulting tree and output colors.
                // For strings we have to know how it is used via a prompt before we know what color to give it.
                Token pendingStringToken = null;
                foreach (ITreeNode node in result.Tree.Nodes) {
                    if (node is TokenNode tokenNode) {
                        if (tokenNode.Token.Name == "String")
                            pendingStringToken = tokenNode.Token;
                        else yield return colorize(tokenNode.Token);
                    } else if (node is PromptNode promptNode) {
                        if (promptNode.Prompt == "pushString")
                            yield return new Formatting(pendingStringToken, Color.DarkBlue, font);
                        else if (promptNode.Prompt == "memberKey")
                            yield return new Formatting(pendingStringToken, Color.DarkRed, font);
                        pendingStringToken = null;
                    }
                }
            }
        }

        /// <summary>Returns the color formatting for the given token.</summary>
        /// <param name="token">The token to color.</param>
        /// <returns>The color for the given token.</returns>
        static private Formatting colorize(Token token) =>
            token.Name switch {
                "True" or "False" or "Null"
                    => new Formatting(token, Color.Blue, font),
                "Integer" or "Fraction" or "Exponent"
                    => new Formatting(token, Color.DarkGreen, font),
                _ => new Formatting(token, Color.Black, font),
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
