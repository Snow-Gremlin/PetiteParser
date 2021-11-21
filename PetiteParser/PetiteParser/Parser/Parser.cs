using PetiteParser.Misc;
using PetiteParser.Tokenizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PetiteParser.Parser {

    /// <summary>
    /// This is a parser for running tokens against a grammar to see
    /// if the tokens are part of that grammar.
    /// </summary>
    public class Parser {

        /// <summary>Gets the debug string for the states used for generating the parse table.</summary>
        /// <param name="grammar">The grammar to get the states for.</param>
        /// <returns>The debug string for the parser states.</returns>
        static public string GetDebugStateString(Grammar.Grammar grammar) {
            Builder builder = new(grammar.Copy());
            builder.DetermineStates();
            StringBuilder buf = new();
            foreach (State state in builder.States)
                buf.Append(state.ToString());
            return buf.ToString();
        }

        /// <summary>The parse table to use while parsing.</summary>
        private Table.Table table;

        /// <summary>Creates a new parser with the given grammar.</summary>
        /// <param name="grammar">The grammar for this parser.</param>
        /// <param name="tokenizer">The tokenizer for this parser.</param>
        public Parser(Grammar.Grammar grammar, Tokenizer.Tokenizer tokenizer) {
            string errors = grammar.Validate();
            if (errors.Length > 0)
                throw new Misc.Exception("Parser can not use invalid grammar: "+errors);

            grammar = grammar.Copy();
            Builder builder = new(grammar);
            builder.DetermineStates();
            builder.FillTable();
            string errs = builder.BuildErrors;
            if (errs.Length > 0)
                throw new Misc.Exception("Errors while building parser:"+
                    Environment.NewLine+builder.ToString(showTable: false));

            this.table = builder.Table;
            this.Grammar = grammar;
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
        /// <returns>The result of a parse.</returns>
        public Result Parse(string input, int errorCap = 0) =>
            this.Parse(this.Tokenizer.Tokenize(input), errorCap);

        /// <summary>This parses the given string and returns the results.</summary>
        /// <param name="input">The input to parse.</param>
        /// <param name="errorCap">The number of errors to allow before failure.</param>
        /// <returns>The result of a parse.</returns>
        public Result Parse(IEnumerable<string> input, int errorCap = 0) =>
            this.Parse(this.Tokenizer.Tokenize(input), errorCap);

        /// <summary>This parses the given characters and returns the results.</summary>
        /// <param name="input">The input to parse.</param>
        /// <param name="errorCap">The number of errors to allow before failure.</param>
        /// <returns>The result to parse.</returns>
        public Result Parse(IEnumerable<Rune> input, int errorCap = 0) =>
          this.Parse(this.Tokenizer.Tokenize(input), errorCap);

        /// <summary>This parses the given characters and returns the results.</summary>
        /// <param name="input">The input to parse.</param>
        /// <param name="errorCap">The number of errors to allow before failure.</param>
        /// <returns>The result to parse.</returns>
        public Result Parse(Scanner.IScanner input, int errorCap = 0) =>
          this.Parse(this.Tokenizer.Tokenize(input), errorCap);

        /// <summary>This parses the given tokens and returns the results.</summary>
        /// <param name="tokens">The input to parse.</param>
        /// <param name="errorCap">The number of errors to allow before failure.</param>
        /// <returns>The result to parse.</returns>
        public Result Parse(IEnumerable<Token> tokens, int errorCap = 0) {
            Runner runner = new(this.table, errorCap);
            foreach (Token token in tokens) {
                if (!runner.Add(token)) return runner.Result;
            }
            runner.Add(new Token(Builder.EofTokenName, Builder.EofTokenName, null, null));
            return runner.Result;
        }
    }
}
