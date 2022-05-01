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

namespace Examples.CodeColoring.Patite {

    /// <summary>A colorer for the patite parser language file.</summary>
    public class Patite: IColorer {
        private const string languageFile = "Examples.CodeColoring.Patite.Patite.lang";
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

        /// <summary>Creates a new Patite colorizer.</summary>
        public Patite() { }

        /// <summary>Gets the name for this colorizer.</summary>
        /// <returns>The colorizer name.</returns>
        public override string ToString() => "Patite";

        /// <summary>Returns the color formatting for the given input text.</summary>
        /// <param name="input">The input text to colorize.</param>
        /// <returns>The formatting to color the input with.</returns>
        public IEnumerable<Formatting> Colorize(params string[] input) {
            singleton ??= createParser();
            font      ??= new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);

            Token[] tokens = singleton.Tokenizer.Tokenize(input).ToArray();
            Token[] parserTokens = tokens.Where(t => t.Name != "Error").ToArray();
            Result result = singleton.Parse(parserTokens);
            if (result is not null && result.Success) {
                // Run though the resulting tree and output colors.
                // For strings we have to know how it is used via a prompt before we know what color to give it.
                int tokenIndex = 0;
                foreach (ITreeNode node in result.Tree.Nodes) {
                    if (node is TokenNode) tokenIndex++;
                    else if (node is PromptNode prompt) {
                        Formatting formatting = colorize(prompt, parserTokens, tokenIndex);
                        if (formatting is not null) yield return formatting;
                    }
                }
            }

            foreach (Token token in tokens.Where(t => t.Name == "Error"))
                yield return colorize(token);
        }

        static private Formatting colorize(PromptNode prompt, Token[] passTokens, int tokenIndex) =>
            prompt.Prompt switch {
                "new.def" => null,
                "start.state" => null,
                "join.state" => null,
                "join.token" => null,
                "assign.token" => null,
                "new.state" => null,
                "new.token.state" => null,
                "new.token.consume" => null,
                "new.term" => null,
                "new.token.item" => null,
                "new.trigger" => null,
                "match.any" => null,
                "match.consume" => null,
                "match.set" => null,
                "match.set.not" => null,
                "match.range" => null,
                "match.range.not" => null,
                "not.group.start" => null,
                "not.group.end" => null,
                "add.replace.text" => null,
                "replace.token" => null,
                "start.term" => null,
                "start.rule" => null,
                "item.token" => null,
                "item.term" => null,
                "item.trigger" => null,
                "set.error" => null,
                _ => throw new Exception("Unexpected prompt: "+prompt)
            };

        /// <summary>Returns the color formatting for the given token.</summary>
        /// <param name="token">The token to color.</param>
        /// <returns>The color for the given token.</returns>
        static private Formatting colorize(Token token) =>
            token.Name switch {
                "Error"
                    => new Formatting(token, Color.Red, font),
                "String"
                    => new Formatting(token, Color.DarkBlue, font),
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
