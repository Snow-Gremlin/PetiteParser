using PetiteParser.Loader;
using PetiteParser.Parser;
using PetiteParser.ParseTree;
using PetiteParser.Tokenizer;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Examples.CodeColoring.Json;

/// <summary>A colorer for JSON, JavaScript Object Notation language.</summary>
/// <see cref="https://www.json.org/json-en.html"/>
/// <see cref="https://json.org/example.html"/>
sealed public class Json: IColorer {
    private const string languageFile = "Examples.CodeColoring.Json.Json.lang";
    private const string exampleFile  = "Examples.CodeColoring.Json.Json.json";

    private static readonly Parser singleton;
    private static readonly Font font;

    /// <summary>Loads the JSON parser.</summary>
    static Json() {
        Assembly assembly = Assembly.GetExecutingAssembly();
        using Stream? stream = assembly.GetManifestResourceStream(languageFile) ??
            throw new FileLoadException(languageFile);
        using StreamReader reader = new(stream);
        singleton = Loader.LoadParser(reader.ReadToEnd());
        font      = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
    }

    /// <summary>Gets the name for this colorizer.</summary>
    /// <returns>The colorizer name.</returns>
    public override string ToString() => "JSON";

    /// <summary>Returns the color formatting for the given input text.</summary>
    /// <param name="input">The input text to colorize.</param>
    /// <returns>The formatting to color the input with.</returns>
    public IEnumerable<Formatting> Colorize(params string[] input) {
        Token[] tokens = singleton.Tokenizer.Tokenize(input).ToArray();
        Result result = singleton.Parse(tokens.Where(t => t.Name != "Error"));
        if (result is not null && result.Success) {
            // Run though the resulting tree and output colors.
            // For strings we have to know how it is used via a prompt before we know what color to give it.
            if (result.Tree is not null) {
                Token? pendingStringToken = null;
                foreach (ITreeNode node in result.Tree.Nodes) {
                    if (node is TokenNode tokenNode) {
                        if (tokenNode.Token.Name == "String")
                            pendingStringToken = tokenNode.Token;
                        else yield return colorize(tokenNode.Token);
                    } else if (node is PromptNode promptNode && pendingStringToken is not null) {
                        if (promptNode.Prompt == "pushString")
                            yield return new Formatting(pendingStringToken, Color.DarkBlue, font);
                        else if (promptNode.Prompt == "memberKey")
                            yield return new Formatting(pendingStringToken, Color.DarkRed, font);
                        pendingStringToken = null;
                    }
                }
            }
        }

        foreach (Token token in tokens.Where(t => t.Name == "Error"))
            yield return colorize(token);
    }

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
            using Stream? stream = assembly.GetManifestResourceStream(exampleFile) ??
                throw new FileLoadException(exampleFile);
            using StreamReader reader = new(stream);
            return reader.ReadToEnd();
        }
    }
}
