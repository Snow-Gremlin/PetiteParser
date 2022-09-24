using PetiteParser.Logger;
using PetiteParser.Misc;
using PetiteParser.Parser.States;
using PetiteParser.Tokenizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PetiteParser.Parser;

/// <summary>
/// This is a parser for running tokens against a grammar to see
/// if the tokens are part of that grammar.
/// </summary>
sealed public class Parser {

    /// <summary>The parse table to use while parsing.</summary>
    private readonly Table.Table table;

    /// <summary>Creates a new parser with the given grammar.</summary>
    /// <param name="grammar">The grammar for this parser.</param>
    /// <param name="tokenizer">The tokenizer for this parser.</param>
    /// <param name="log">
    /// Optional log to write notices and warnings about the parser build.
    /// Any errors which occurred while building the parser should be thrown.
    /// </param>
    public Parser(Grammar.Grammar grammar, Tokenizer.Tokenizer tokenizer, ILogger log = null) {
        Buffered bufLog = new(log);
        Analyzer.Analyzer.Validate(grammar, bufLog);
        grammar = Analyzer.Analyzer.Normalize(grammar, bufLog);
        ParserStates states = new(grammar, bufLog);

        if (bufLog.Failed)
            throw new Exception("Errors while building parser:" + Environment.NewLine + bufLog.ToString());

        this.table     = states.CreateTable();
        this.Grammar   = grammar;
        this.Tokenizer = tokenizer;
    }

    /// <summary>Create a new parser with a pre-created table instance.</summary>
    /// <param name="table">The table to create this parser for.</param>
    /// <param name="grammar">The grammar for the parser.</param>
    /// <param name="tokenizer">The tokenizer for the parser.</param>
    internal Parser(Table.Table table, Grammar.Grammar grammar, Tokenizer.Tokenizer tokenizer) {
        this.table     = table;
        this.Grammar   = grammar;
        this.Tokenizer = tokenizer;
    }

    /// <summary>
    /// Gets the grammar for this parser.
    /// This should be treated as a constant, modifying it could cause the parser to fail.
    /// </summary>
    public readonly Grammar.Grammar Grammar;

    /// <summary>
    /// Gets the tokenizer for this parser.
    /// This should be treated as a constant, modifying it could cause the parser to fail.
    /// </summary>
    public readonly Tokenizer.Tokenizer Tokenizer;

    /// <summary>This gets all the prompt names not defined in the given prompts.</summary>
    /// <remarks>
    /// This is useful for checking that your prompts have all the prompt handlers needed
    /// for processing a parse tree which can be created by this parser's grammar.
    /// </remarks>
    /// <param name="prompts">The prompts used for processing, which need to be checked.</param>
    /// <returns>The names of the prompts which are missing from the given prompts.</returns>
    public string[] MissingPrompts(Dictionary<string, ParseTree.PromptHandle> prompts) {
        HashSet<string> remaining = new(this.Grammar.Prompts.ToNames());
        return prompts.Keys.Where(name => !remaining.Contains(name)).ToArray();
    }

    /// <summary>This gets all the prompt names not defined in this parser's grammar.</summary>
    /// <remarks>
    /// This is useful for checking that you don't have prompt handlers which will
    /// never be used because they don't exist in this parser's grammar.
    /// </remarks>
    /// <param name="prompts">The prompts used for processing, which need to be checked.</param>
    /// <returns>The names of the prompts which are unneeded in the given prompts.</returns>
    public string[] UnneededPrompts(Dictionary<string, ParseTree.PromptHandle> prompts) {
        HashSet<string> remaining = new(this.Grammar.Prompts.ToNames());
        prompts.Keys.Where(name => remaining.Contains(name)).Foreach(remaining.Remove);
        return remaining.ToArray();
    }

    /// <summary>This parses the given string and returns the results.</summary>
    /// <param name="input">The input to parse.</param>
    /// <param name="errorCap">The number of errors to allow before failure.</param>
    /// <param name="log">Optional logger for logging the parse process.</param>
    /// <returns>The result of a parse.</returns>
    public Result Parse(string input, int errorCap = 0, ILogger log = null) =>
        this.Parse(this.Tokenizer.Tokenize(input), errorCap, log);

    /// <summary>This parses the given string and returns the results.</summary>
    /// <param name="input">The input to parse.</param>
    /// <param name="errorCap">The number of errors to allow before failure.</param>
    /// <param name="log">Optional logger for logging the parse process.</param>
    /// <returns>The result of a parse.</returns>
    public Result Parse(IEnumerable<string> input, int errorCap = 0, ILogger log = null) =>
        this.Parse(this.Tokenizer.Tokenize(input), errorCap, log);

    /// <summary>This parses the given characters and returns the results.</summary>
    /// <param name="input">The input to parse.</param>
    /// <param name="errorCap">The number of errors to allow before failure.</param>
    /// <param name="log">Optional logger for logging the parse process.</param>
    /// <returns>The result to parse.</returns>
    public Result Parse(IEnumerable<Rune> input, int errorCap = 0, ILogger log = null) =>
      this.Parse(this.Tokenizer.Tokenize(input), errorCap, log);

    /// <summary>This parses the given characters and returns the results.</summary>
    /// <param name="input">The input to parse.</param>
    /// <param name="errorCap">The number of errors to allow before failure.</param>
    /// <param name="log">Optional logger for logging the parse process.</param>
    /// <returns>The result to parse.</returns>
    public Result Parse(Scanner.IScanner input, int errorCap = 0, ILogger log = null) =>
      this.Parse(this.Tokenizer.Tokenize(input), errorCap, log);

    /// <summary>This parses the given tokens and returns the results.</summary>
    /// <param name="tokens">The input to parse.</param>
    /// <param name="errorCap">The number of errors to allow before failure.</param>
    /// <param name="log">Optional logger for logging the parse process.</param>
    /// <returns>The result to parse.</returns>
    public Result Parse(IEnumerable<Token> tokens, int errorCap = 0, ILogger log = null) {
        Runner runner = new(this.table, this.Grammar.ErrorToken, errorCap, log);
        if (!tokens.All(runner.Add)) return runner.Result;
        runner.Add(new Token(ParserStates.EofTokenName, ParserStates.EofTokenName, null, null));
        return runner.Result;
    }
}
