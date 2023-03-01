using PetiteParser.Formatting;
using PetiteParser.Parser;
using PetiteParser.Scanner;
using System;
using System.Collections.Generic;
using System.Text;

namespace PetiteParser.Loader;

/// <summary>
/// Loader is a parser and interpreter for reading a tokenizer
/// and grammar definition from a string to create a parser.
/// </summary>
public class Loader {

    /// <summary>Creates a parser from one or more parser definition strings.</summary>
    /// <param name="input">The parser definition.</param>
    static public Parser.Parser LoadParser(params string[] input) =>
        new Loader().Load(input).Parser;

    /// <summary>Creates a parser from a parser definition runes.</summary>
    /// <param name="input">The parser definition.</param>
    static public Parser.Parser LoadParser(IEnumerable<Rune> input) =>
        new Loader().Load(input).Parser;

    /// <summary>Creates a parser from a parser definition runes.</summary>
    /// <param name="input">The parser definition.</param>
    static public Parser.Parser LoadParser(IScanner input) =>
        new Loader().Load(input).Parser;

    /// <summary>Creates a grammar from one or more parser definition strings.</summary>
    /// <remarks>Any tokenizer information in the definition is ignored.</remarks>
    /// <param name="input">The grammar definition.</param>
    static public Grammar.Grammar LoadGrammar(params string[] input) =>
        new Loader().Load(input).Grammar;

    /// <summary>Creates a grammar from a parser definition runes.</summary>
    /// <remarks>Any tokenizer information in the definition is ignored.</remarks>
    /// <param name="input">The grammar definition.</param>
    static public Grammar.Grammar LoadGrammar(IEnumerable<Rune> input) =>
        new Loader().Load(input).Grammar;

    /// <summary>Creates a grammar from a parser definition runes.</summary>
    /// <remarks>Any tokenizer information in the definition is ignored.</remarks>
    /// <param name="input">The grammar definition.</param>
    static public Grammar.Grammar LoadGrammar(IScanner input) =>
        new Loader().Load(input).Grammar;

    /// <summary>Creates a tokenizer from one or more parser definition strings.</summary>
    /// <remarks>Any parser information in the definition is ignored.</remarks>
    /// <param name="input">The tokenizer definition.</param>
    static public Tokenizer.Tokenizer LoadTokenizer(params string[] input) =>
        new Loader().Load(input).Tokenizer;

    /// <summary>Creates a tokenizer from a parser definition runes.</summary>
    /// <remarks>Any parser information in the definition is ignored.</remarks>
    /// <param name="input">The tokenizer definition.</param>
    static public Tokenizer.Tokenizer LoadTokenizer(IEnumerable<Rune> input) =>
        new Loader().Load(input).Tokenizer;

    /// <summary>Creates a tokenizer from a parser definition runes.</summary>
    /// <remarks>Any parser information in the definition is ignored.</remarks>
    /// <param name="input">The tokenizer definition.</param>
    static public Tokenizer.Tokenizer LoadTokenizer(IScanner input) =>
        new Loader().Load(input).Tokenizer;

    /// <summary>The loader arguments to use while parsing the loader language.</summary>
    private readonly LoaderArgs args;

    /// <summary>Creates a new loader.</summary>
    /// <param name="features">
    /// The optional features object for the features to use when parsing.
    /// This allows features to be preset or non parsing features to be passed
    /// in and set via the language loader.
    /// </param>
    public Loader(Features features = null) =>
        this.args = new LoaderArgs(new Grammar.Grammar(), new Tokenizer.Tokenizer(), features);

    /// <summary>
    /// Adds several blocks of definitions to the grammar and tokenizer
    /// which are being loaded via a string containing the definition.
    /// </summary>
    /// <param name="input">The input language to read.</param>
    /// <returns>This loader so that calls can be chained.</returns>
    public Loader Load(params string[] input) =>
        this.Load(new Default(input));

    /// <summary>
    /// Adds several blocks of definitions to the grammar and tokenizer
    /// which are being loaded via a list of characters containing the definition.
    /// </summary>
    /// <param name="iterator">The input language to read.</param>
    /// <returns>This loader so that calls can be chained.</returns>
    public Loader Load(IEnumerable<Rune> input) =>
        this.Load(new Default(input));

    /// <summary>
    /// Adds several blocks of definitions to the grammar and tokenizer
    /// which are being loaded via a list of characters containing the definition.
    /// </summary>
    /// <param name="iterator">The input language to read.</param>
    /// <returns>This loader so that calls can be chained.</returns>
    public Loader Load(IScanner input) {
        Result result = Language.LoaderParser.Parse(input);
        if (result.Errors.Length > 0)
            throw new Exception("Error in provided language definition:"+
                Environment.NewLine + "   " + result.Errors.JoinLines("   "));

        this.args.Clear();
        result.Tree.Process(Processor.Handles, this.args);
        return this;
    }

    /// <summary>Gets the grammar which is being loaded.</summary>
    public Grammar.Grammar Grammar => this.args.Grammar;

    /// <summary>Gets the tokenizer which is being loaded.</summary>
    public Tokenizer.Tokenizer Tokenizer => this.args.Tokenizer;

    /// <summary>Creates a parser with the loaded tokenizer and grammar.</summary>
    public Parser.Parser Parser => new(this.Grammar, this.Tokenizer);
}
