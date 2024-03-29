﻿using PetiteParser.Formatting;
using PetiteParser.Loader;
using PetiteParser.Tokenizer;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Examples.CodeColoring.Glsl;

/// <summary>A colorer for GLSL, openGL shader language.</summary>
/// <see cref="https://www.khronos.org/opengl/wiki/Core_Language_(GLSL)"/>
sealed public class Glsl : IColorer {
    private const string languageFile = "Examples.CodeColoring.Glsl.Glsl.lang";
    private const string exampleFile  = "Examples.CodeColoring.Glsl.Glsl.glsl";

    private static readonly Tokenizer singleton;
    private static readonly Font font;
    private static readonly Font italic;

    /// <summary>Creates a new GLSL colorizer.</summary>
    static Glsl() {
        Assembly assembly = Assembly.GetExecutingAssembly();
        using Stream? stream = assembly.GetManifestResourceStream(languageFile) ??
            throw new FileLoadException(languageFile);
        using StreamReader? reader = new(stream);
        singleton = Loader.LoadTokenizer(reader.ReadToEnd());
        font      = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
        italic    = new Font("Consolas", 9F, FontStyle.Italic,  GraphicsUnit.Point);
    }

    /// <summary>Gets the name for this colorizer.</summary>
    /// <returns>The colorizer name.</returns>
    public override string ToString() => "GLSL";

    /// <summary>Returns the color formatting for the given input text.</summary>
    /// <param name="input">The input text to colorize.</param>
    /// <returns>The formatting to color the input with.</returns>
    public IEnumerable<Formatting> Colorize(params string[] input) =>
        colorize(singleton.Tokenize(input.JoinLines()));

    /// <summary>Returns the color formatting for the given tokens.</summary>
    /// <param name="tokens">The tokens to colorize.</param>
    /// <returns>The formatting color for the given tokens.</returns>
    static private IEnumerable<Formatting> colorize(IEnumerable<Token> tokens) =>
        tokens.Select(colorize);

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
            "Error"      => new Formatting(token, Color.Red,         font),
            _            => new Formatting(token, Color.Black,       font)
        };

    /// <summary>Returns an example text which this will colorize.</summary>
    public string ExampleCode {
        get {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using Stream? stream = assembly.GetManifestResourceStream(exampleFile) ??
                throw new FileLoadException(exampleFile);
            using StreamReader? reader = new(stream);
            return reader.ReadToEnd();
        }
    }
}
