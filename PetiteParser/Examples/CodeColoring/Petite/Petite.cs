using PetiteParser.Loader;
using PetiteParser.Parser;
using PetiteParser.ParseTree;
using PetiteParser.Tokenizer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Examples.CodeColoring.Petite {

    /// <summary>A colorer for the petite parser language file.</summary>
    public class Petite: IColorer {
        private const string languageFile = "Examples.CodeColoring.Petite.Petite.lang";
        private const string exampleFile = "Examples.Calculator.Calculator.lang";

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
        static private Font italic;

        /// <summary>Creates a new Petite colorizer.</summary>
        public Petite() { }

        /// <summary>Gets the name for this colorizer.</summary>
        /// <returns>The colorizer name.</returns>
        public override string ToString() => "Petite";

        /// <summary>Returns the color formatting for the given input text.</summary>
        /// <param name="input">The input text to colorize.</param>
        /// <returns>The formatting to color the input with.</returns>
        public IEnumerable<Formatting> Colorize(params string[] input) {
            singleton ??= createParser();
            font      ??= new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
            italic    ??= new Font("Consolas", 9F, FontStyle.Italic,  GraphicsUnit.Point);

            Token[] tokens = singleton.Tokenizer.Tokenize(input).ToArray();
            Result result = singleton.Parse(tokens.Where(t => t.Name != "error" && t.Name != "comment"));
            if (result is not null && result.Success) {
                // Run though the resulting tree and output colors.
                // For strings we have to know how it is used via a prompt before we know what color to give it.
                Token priorToken = null;
                foreach (ITreeNode node in result.Tree.Nodes) {
                    if (node is TokenNode tokenNode) priorToken = tokenNode.Token;
                    else if (node is PromptNode prompt) {
                        Formatting formatting = colorize(prompt, priorToken);
                        if (formatting is not null) yield return formatting;
                    }
                }
            }

            foreach (Token token in tokens.Where(t => t.Name == "comment"))
                yield return new Formatting(token, Color.Green, italic);

            foreach (Token token in tokens.Where(t => t.Name == "error"))
                yield return new Formatting(token, Color.Red, font);
        }

        /// <summary>Returns the color formating for the given prompt and token.</summary>
        /// <param name="prompt">The prompt which is called to indicate how to color the given token.</param>
        /// <param name="token">The token was read right before the given prompt.</param>
        /// <returns>The formatting for this prompt and token.</returns>
        static private Formatting colorize(PromptNode prompt, Token token) =>
            prompt.Prompt switch {
                "state"  => new Formatting(token, Color.DarkBlue,   font),
                "token"  => new Formatting(token, Color.DarkCyan,   font),
                "term"   => new Formatting(token, Color.DarkGreen,  font),
                "prompt" => new Formatting(token, Color.DarkOrange, font),
                "symbol" => new Formatting(token, Color.Black,      font),
                "string" => new Formatting(token, Color.DarkRed,    font),
                _ => throw new Exception("Unexpected prompt: "+prompt)
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
