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
    internal readonly Table.Table table;

    /// <summary>Creates a new parser with the given grammar.</summary>
    /// <param name="grammar">The grammar for this parser.</param>
    /// <param name="tokenizer">The tokenizer for this parser.</param>
    /// <param name="log">
    /// Optional log to write notices and warnings about the parser build.
    /// Any errors which occurred while building the parser should be thrown.
    /// </param>
    /// <param name="ignoreConflicts">
    /// This indicates that as many conflicts in state actions as possible should be ignored.
    /// Typically this is only when there is a reduce or shift, but multiple shifts or multiple reduce can't be ignored.
    /// </param>
    public Parser(Grammar.Grammar grammar, Tokenizer.Tokenizer tokenizer, ILogger? log = null, bool ignoreConflicts = true) {
        Buffered bufferedLog = new(log);
        PetiteParser.Grammar.Inspector.Inspector.Validate(grammar, bufferedLog);
        grammar = PetiteParser.Grammar.Normalizer.Normalizer.GetNormal(grammar, bufferedLog);
        ParserStates states = new();
        states.DetermineStates(grammar, bufferedLog, ignoreConflicts);

        if (bufferedLog.Failed)
            throw new ParserException("Errors while building parser:" + Environment.NewLine + bufferedLog.ToString());

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
    public Grammar.Grammar Grammar { get; }

    /// <summary>
    /// Gets the tokenizer for this parser.
    /// This should be treated as a constant, modifying it could cause the parser to fail.
    /// </summary>
    public Tokenizer.Tokenizer Tokenizer { get; }

    #region MissingPrompts...

    /// <summary>This gets all the prompt names not defined in the given prompts.</summary>
    /// <remarks>
    /// This is useful for checking that your prompts have all the prompt handlers needed
    /// for processing a parse tree which can be created by this parser's grammar.
    /// </remarks>
    /// <typeparam name="T">The prompt handler which is not used in this method.</typeparam>
    /// <param name="prompts">The prompts used for processing, which need to be checked.</param>
    /// <returns>The names of the prompts which are missing from the given prompts.</returns>
    public string[] MissingPrompts<T>(Dictionary<string, T> prompts) =>
        this.MissingPrompts(prompts.Keys);

    /// <summary>This gets all the prompt names not defined in the given prompts.</summary>
    /// <remarks>
    /// This is useful for checking that your prompts have all the prompt handlers needed
    /// for processing a parse tree which can be created by this parser's grammar.
    /// </remarks>
    /// <param name="promptsKeys">The keys of the prompts used for processing, which need to be checked.</param>
    /// <returns>The names of the prompts which are missing from the given prompts.</returns>
    public string[] MissingPrompts(params string[] promptsKeys) =>
        this.MissingPrompts(promptsKeys as IEnumerable<string>);

    /// <summary>This gets all the prompt names not defined in the given prompts.</summary>
    /// <remarks>
    /// This is useful for checking that your prompts have all the prompt handlers needed
    /// for processing a parse tree which can be created by this parser's grammar.
    /// </remarks>
    /// <param name="promptsKeys">The keys of the prompts used for processing, which need to be checked.</param>
    /// <returns>The names of the prompts which are missing from the given prompts.</returns>
    public string[] MissingPrompts(IEnumerable<string> promptsKeys) {
        HashSet<string> remaining = new(this.Grammar.Prompts.ToNames());
        return promptsKeys.WhereNot(remaining.Contains).ToArray();
    }

    #endregion
    #region UnneededPrompts...

    /// <summary>This gets all the prompt names not defined in this parser's grammar.</summary>
    /// <remarks>
    /// This is useful for checking that you don't have prompt handlers which will
    /// never be used because they don't exist in this parser's grammar.
    /// </remarks>
    /// <typeparam name="T">The prompt handler which is not used in this method.</typeparam>
    /// <param name="prompts">The prompts used for processing, which need to be checked.</param>
    /// <returns>The names of the prompts which are unneeded in the given prompts.</returns>
    public string[] UnneededPrompts<T>(Dictionary<string, T> prompts) =>
        this.UnneededPrompts(prompts.Keys);

    /// <summary>This gets all the prompt names not defined in this parser's grammar.</summary>
    /// <remarks>
    /// This is useful for checking that you don't have prompt handlers which will
    /// never be used because they don't exist in this parser's grammar.
    /// </remarks>
    /// <param name="promptsKeys">The keys of the prompts used for processing, which need to be checked.</param>
    /// <returns>The names of the prompts which are unneeded in the given prompts.</returns>
    public string[] UnneededPrompts(params string[] promptsKeys) =>
        this.UnneededPrompts(promptsKeys as IEnumerable<string>);

    /// <summary>This gets all the prompt names not defined in this parser's grammar.</summary>
    /// <remarks>
    /// This is useful for checking that you don't have prompt handlers which will
    /// never be used because they don't exist in this parser's grammar.
    /// </remarks>
    /// <param name="promptsKeys">The keys of the prompts used for processing, which need to be checked.</param>
    /// <returns>The names of the prompts which are unneeded in the given prompts.</returns>
    public string[] UnneededPrompts(IEnumerable<string> promptsKeys) {
        HashSet<string> remaining = new(this.Grammar.Prompts.ToNames());
        promptsKeys.Where(remaining.Contains).Foreach(remaining.Remove);
        return remaining.ToArray();
    }
    
    #endregion
    #region Parse...

    /// <summary>This parses the given string and returns the results.</summary>
    /// <param name="input">The input to parse.</param>
    /// <param name="errorCap">The number of errors to allow before failure.</param>
    /// <param name="log">Optional logger for logging the parse process.</param>
    /// <returns>The result of a parse.</returns>
    public Result Parse(string input, int errorCap = 0, ILogger? log = null) =>
        this.Parse(this.Tokenizer.Tokenize(input), errorCap, log);

    /// <summary>This parses the given string and returns the results.</summary>
    /// <param name="input">The input to parse.</param>
    /// <param name="errorCap">The number of errors to allow before failure.</param>
    /// <param name="log">Optional logger for logging the parse process.</param>
    /// <returns>The result of a parse.</returns>
    public Result Parse(IEnumerable<string> input, int errorCap = 0, ILogger? log = null) =>
        this.Parse(this.Tokenizer.Tokenize(input), errorCap, log);

    /// <summary>This parses the given characters and returns the results.</summary>
    /// <param name="input">The input to parse.</param>
    /// <param name="errorCap">The number of errors to allow before failure.</param>
    /// <param name="log">Optional logger for logging the parse process.</param>
    /// <returns>The result to parse.</returns>
    public Result Parse(IEnumerable<Rune> input, int errorCap = 0, ILogger? log = null) =>
      this.Parse(this.Tokenizer.Tokenize(input), errorCap, log);

    /// <summary>This parses the given characters and returns the results.</summary>
    /// <param name="input">The input to parse.</param>
    /// <param name="errorCap">The number of errors to allow before failure.</param>
    /// <param name="log">Optional logger for logging the parse process.</param>
    /// <returns>The result to parse.</returns>
    public Result Parse(Scanner.IScanner input, int errorCap = 0, ILogger? log = null) =>
      this.Parse(this.Tokenizer.Tokenize(input), errorCap, log);

    /// <summary>This parses the given tokens and returns the results.</summary>
    /// <param name="tokens">The input to parse.</param>
    /// <param name="errorCap">The number of errors to allow before failure.</param>
    /// <param name="log">Optional logger for logging the parse process.</param>
    /// <returns>The result to parse.</returns>
    public Result Parse(IEnumerable<Token> tokens, int errorCap = 0, ILogger? log = null) {
        Runner runner = new(this.table, this.Grammar.ErrorToken, errorCap, log);
        if (!tokens.All(runner.Add)) return runner.Result;
        runner.Add(new Token(ParserStates.EOfTokenName, ParserStates.EOfTokenName, null, null));
        return runner.Result;
    }
    
    #endregion
}
