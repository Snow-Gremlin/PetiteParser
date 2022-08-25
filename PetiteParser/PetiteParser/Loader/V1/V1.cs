using PetiteParser.Grammar;
using PetiteParser.Loader.V1;
using PetiteParser.Matcher;
using PetiteParser.Misc;
using PetiteParser.Parser;
using PetiteParser.ParseTree;
using PetiteParser.Scanner;
using PetiteParser.Tokenizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PetiteParser.Loader.v1 {

    /// <summary>This is version 1 of the parser loader.</summary>
    internal class V1 : IVersion {

        /// <summary>The version number for this loader.</summary>
        public int Version => 1;

        #region Loader Language Definition...

        /// <summary>Gets the tokenizer used for loading a parser definition.</summary>
        /// <returns>The tokenizer of the parser language.</returns>
        static public Tokenizer.Tokenizer GetLoaderTokenizer() {
            Tokenizer.Tokenizer tok = new();
            tok.Start("start");

            tok.Join("start", "whitespace").Add(Predef.WhiteSpace);
            tok.Join("whitespace", "whitespace").Add(Predef.WhiteSpace);
            tok.SetToken("whitespace", "whitespace").Consume();

            tok.JoinToToken("start", "openParen").AddSingle('(');
            tok.JoinToToken("start", "closeParen").AddSingle(')');
            tok.JoinToToken("start", "openBracket").AddSingle('[');
            tok.JoinToToken("start", "closeBracket").AddSingle(']');
            tok.JoinToToken("start", "openAngle").AddSingle('<');
            tok.JoinToToken("start", "closeAngle").AddSingle('>');
            tok.JoinToToken("start", "openCurly").AddSingle('{');
            tok.JoinToToken("start", "closeCurly").AddSingle('}');

            tok.JoinToToken("start", "or").AddSingle('|');
            tok.JoinToToken("start", "not").AddSingle('!');
            tok.JoinToToken("start", "consume").AddSingle('^');
            tok.JoinToToken("start", "colon").AddSingle(':');
            tok.JoinToToken("start", "semicolon").AddSingle(';');
            tok.JoinToToken("colon", "assign").AddSingle('=');
            tok.JoinToToken("start", "comma").AddSingle(',');
            tok.JoinToToken("start", "any").AddSingle('*');
            tok.JoinToToken("start", "lambda").AddSingle('_');

            tok.Join("start", "comment").AddSingle('#');
            tok.Join("comment", "commentEnd").AddSingle('\n');
            tok.Join("comment", "comment").AddAll();
            tok.SetToken("commentEnd", "comment").Consume();

            tok.Join("start", "equal").AddSingle('=');
            tok.SetToken("equal", "equal");
            tok.Join("equal", "arrow").AddSingle('>');
            tok.SetToken("arrow", "arrow");

            tok.Join("start", "startRange").AddSingle('.');
            tok.JoinToToken("startRange", "range").AddSingle('.');

            Group hexMatcher = new Group().AddRange('0', '9').AddRange('A', 'F').AddRange('a', 'f');
            Group idLetter = new Group().Add(Predef.LetterOrDigit).AddSet("_.-");

            tok.JoinToToken("start", "id").Add(idLetter);
            tok.Join("id", "id").Add(idLetter);

            tok.Join("start", "singleQuote.open").SetConsume(true).AddSingle('\'');
            tok.Join("singleQuote.open", "singleQuote.escape").AddSingle('\\');
            tok.Join("singleQuote.open", "singleQuote.body").AddAll();
            tok.Join("singleQuote.body", "singleQuote").SetConsume(true).AddSingle('\'');
            tok.Join("singleQuote.body", "singleQuote.escape").AddSingle('\\');
            tok.Join("singleQuote.escape", "singleQuote.body").AddSet("n\\rt'\"");
            tok.Join("singleQuote.escape", "singleQuote.hex1").AddSingle('x');
            tok.Join("singleQuote.hex1", "singleQuote.hex2").Add(hexMatcher);
            tok.Join("singleQuote.hex2", "singleQuote.body").Add(hexMatcher);
            tok.Join("singleQuote.escape", "singleQuote.unicode1").AddSingle('u');
            tok.Join("singleQuote.unicode1", "singleQuote.unicode2").Add(hexMatcher);
            tok.Join("singleQuote.unicode2", "singleQuote.unicode3").Add(hexMatcher);
            tok.Join("singleQuote.unicode3", "singleQuote.unicode4").Add(hexMatcher);
            tok.Join("singleQuote.unicode4", "singleQuote.body").Add(hexMatcher);
            tok.Join("singleQuote.escape", "singleQuote.rune1").AddSingle('U');
            tok.Join("singleQuote.rune1", "singleQuote.rune2").Add(hexMatcher);
            tok.Join("singleQuote.rune2", "singleQuote.rune3").Add(hexMatcher);
            tok.Join("singleQuote.rune3", "singleQuote.rune4").Add(hexMatcher);
            tok.Join("singleQuote.rune4", "singleQuote.rune5").Add(hexMatcher);
            tok.Join("singleQuote.rune5", "singleQuote.rune6").Add(hexMatcher);
            tok.Join("singleQuote.rune6", "singleQuote.rune7").Add(hexMatcher);
            tok.Join("singleQuote.rune7", "singleQuote.rune8").Add(hexMatcher);
            tok.Join("singleQuote.rune8", "singleQuote.body").Add(hexMatcher);
            tok.Join("singleQuote.body", "singleQuote.body").AddAll();
            tok.SetToken("singleQuote", "string");

            tok.Join("start", "doubleQuote.open").SetConsume(true).AddSingle('"');
            tok.Join("doubleQuote.open", "doubleQuote.escape").AddSingle('\\');
            tok.Join("doubleQuote.open", "doubleQuote.body").AddAll();
            tok.Join("doubleQuote.body", "doubleQuote").SetConsume(true).AddSingle('"');
            tok.Join("doubleQuote.body", "doubleQuote.escape").AddSingle('\\');
            tok.Join("doubleQuote.escape", "doubleQuote.body").AddSet("n\\rt'\"");
            tok.Join("doubleQuote.escape", "doubleQuote.hex1").AddSingle('x');
            tok.Join("doubleQuote.hex1", "doubleQuote.hex2").Add(hexMatcher);
            tok.Join("doubleQuote.hex2", "doubleQuote.body").Add(hexMatcher);
            tok.Join("doubleQuote.escape", "doubleQuote.unicode1").AddSingle('u');
            tok.Join("doubleQuote.unicode1", "doubleQuote.unicode2").Add(hexMatcher);
            tok.Join("doubleQuote.unicode2", "doubleQuote.unicode3").Add(hexMatcher);
            tok.Join("doubleQuote.unicode3", "doubleQuote.unicode4").Add(hexMatcher);
            tok.Join("doubleQuote.unicode4", "doubleQuote.body").Add(hexMatcher);
            tok.Join("doubleQuote.escape", "doubleQuote.rune1").AddSingle('U');
            tok.Join("doubleQuote.rune1", "doubleQuote.rune2").Add(hexMatcher);
            tok.Join("doubleQuote.rune2", "doubleQuote.rune3").Add(hexMatcher);
            tok.Join("doubleQuote.rune3", "doubleQuote.rune4").Add(hexMatcher);
            tok.Join("doubleQuote.rune4", "doubleQuote.rune5").Add(hexMatcher);
            tok.Join("doubleQuote.rune5", "doubleQuote.rune6").Add(hexMatcher);
            tok.Join("doubleQuote.rune6", "doubleQuote.rune7").Add(hexMatcher);
            tok.Join("doubleQuote.rune7", "doubleQuote.rune8").Add(hexMatcher);
            tok.Join("doubleQuote.rune8", "doubleQuote.body").Add(hexMatcher);
            tok.Join("doubleQuote.body", "doubleQuote.body").AddAll();
            tok.SetToken("doubleQuote", "string");
            return tok;
        }

        /// <summary>Gets the grammar used for loading a parser definition.</summary>
        /// <returns>The grammar for the parser language.</returns>
        static public Grammar.Grammar GetLoaderGrammar() {
            Grammar.Grammar gram = new();

            gram.Start("def.set");
            gram.NewRule("def.set").AddItems("<def.set> <def> [semicolon]");
            gram.NewRule("def.set");

            gram.NewRule("def").AddItems("{new.def} [closeAngle] <stateID> {start.state} <def.state.optional>");
            gram.NewRule("def").AddItems("{new.def} <stateID> <def.state>");

            gram.NewRule("def.state.optional");
            gram.NewRule("def.state.optional").AddItems("<def.state>");

            gram.NewRule("def.state").AddItems("[colon] <matcher.start> [arrow] <def.assign>");
            gram.NewRule("def.assign").AddItems("<stateID> {join.state} <def.state.optional>");
            gram.NewRule("def.assign").AddItems("<tokenStateID> {join.token} <def.state.optional>");
            gram.NewRule("def.state").AddItems("[arrow] <tokenStateID> {assign.token} <def.state.optional>");

            gram.NewRule("stateID").AddItems("[openParen] [id] [closeParen] {new.state}");
            gram.NewRule("tokenStateID").AddItems("[openBracket] [id] [closeBracket] {new.token.state}");
            gram.NewRule("tokenStateID").AddItems("[consume] [openBracket] [id] [closeBracket] {new.token.consume}");
            gram.NewRule("termID").AddItems("[openAngle] [id] [closeAngle] {new.term}");
            gram.NewRule("tokenItemID").AddItems("[openBracket] [id] [closeBracket] {new.token.item}");
            gram.NewRule("promptID").AddItems("[openCurly] [id] [closeCurly] {new.prompt}");

            gram.NewRule("matcher.start").AddItems("[any] {match.any}");
            gram.NewRule("matcher.start").AddItems("<matcher>");
            gram.NewRule("matcher.start").AddItems("[consume] <matcher> {match.consume}");

            gram.NewRule("matcher").AddItems("<charSetRange>");
            gram.NewRule("matcher").AddItems("<matcher> [comma] <charSetRange>");

            gram.NewRule("charSetRange").AddItems("[string] {match.set}");
            gram.NewRule("charSetRange").AddItems("[not] [string] {match.set.not}");
            gram.NewRule("charSetRange").AddItems("[string] [range] [string] {match.range}");
            gram.NewRule("charSetRange").AddItems("[not] [string] [range] [string] {match.range.not}");
            gram.NewRule("charSetRange").AddItems("[not] [openParen] {not.group.start} <matcher> [closeParen] {not.group.end}");

            gram.NewRule("def").AddItems("{new.def} [any] [arrow] <tokenItemID> {set.error}");
            gram.NewRule("def").AddItems("{new.def} <tokenStateID> <def.token>");

            gram.NewRule("def.token").AddItems("[equal] <def.token.replace>");
            gram.NewRule("def.token.replace").AddItems("<replaceText> [arrow] <tokenStateID> {replace.token} <def.token.optional>");
            gram.NewRule("def.token.optional");
            gram.NewRule("def.token.optional").AddItems("[or] <def.token.replace>");

            gram.NewRule("replaceText").AddItems("[string] {add.replace.text}");
            gram.NewRule("replaceText").AddItems("<replaceText> [comma] [string] {add.replace.text}");

            gram.NewRule("def").AddItems("{new.def} [closeAngle] <termID> {start.term} <start.rule.optional>");
            gram.NewRule("def").AddItems("{new.def} <termID> [assign] {start.rule} <start.rule> <next.rule.optional>");

            gram.NewRule("start.rule.optional");
            gram.NewRule("start.rule.optional").AddItems("[assign] {start.rule} <start.rule> <next.rule.optional>");

            gram.NewRule("next.rule.optional");
            gram.NewRule("next.rule.optional").AddItems("<next.rule.optional> [or] {start.rule} <start.rule>");

            gram.NewRule("start.rule").AddItems("<tokenItemID> {item.token} <rule.item>");
            gram.NewRule("start.rule").AddItems("<termID> {item.term} <rule.item>");
            gram.NewRule("start.rule").AddItems("<promptID> {item.prompt} <rule.item>");
            gram.NewRule("start.rule").AddItems("[lambda]");

            gram.NewRule("rule.item");
            gram.NewRule("rule.item").AddItems("<rule.item> <tokenItemID> {item.token}");
            gram.NewRule("rule.item").AddItems("<rule.item> <termID> {item.term}");
            gram.NewRule("rule.item").AddItems("<rule.item> <promptID> {item.prompt}");

            return gram;
        }

        /// <summary>The singleton for the parser loader.</summary>
        static private Parser.Parser parserSingleton;

        /// <summary>Gets or creates a new parser for loading tokenizer and grammar definitions.</summary>
        /// <returns>This is the parser for the parser language.</returns>
        static public Parser.Parser LoaderParser() =>
            parserSingleton ??= new (GetLoaderGrammar(), GetLoaderTokenizer());

        #endregion

        /// <summary>The collection of prompt handles to parse the language file with.</summary>
        private readonly Dictionary<string, PromptHandle<V1Args>> handles;

        /// <summary>Creates a new loader.</summary>
        public V1() =>
            this.handles = new Dictionary<string, PromptHandle<V1Args>>() {
                { "new.def",           this.newDef },
                { "start.state",       this.startState },
                { "join.state",        this.joinState },
                { "join.token",        this.joinToken },
                { "assign.token",      this.assignToken },
                { "new.state",         this.newState },
                { "new.token.state",   this.newTokenState },
                { "new.token.consume", this.newTokenConsume },
                { "new.term",          this.newTerm },
                { "new.token.item",    this.newTokenItem },
                { "new.prompt",        this.newPrompt },
                { "match.any",         this.matchAny },
                { "match.consume",     this.matchConsume },
                { "match.set",         this.matchSet },
                { "match.set.not",     this.matchSetNot },
                { "match.range",       this.matchRange },
                { "match.range.not",   this.matchRangeNot },
                { "not.group.start",   this.notGroupStart },
                { "not.group.end",     this.notGroupEnd },
                { "add.replace.text",  this.addReplaceText },
                { "replace.token",     this.replaceToken },
                { "start.term",        this.startTerm },
                { "start.rule",        this.startRule },
                { "item.token",        this.itemToken },
                { "item.term",         this.itemTerm },
                { "item.prompt",       this.itemPrompt },
                { "set.error",         this.setError }
            };

        /// <summary>
        /// Adds several blocks of definitions to the grammar and tokenizer
        /// which are being loaded via a list of characters containing the definition.
        /// </summary>
        /// <param name="grammar">Gets the grammar which is being loaded.</param>
        /// <param name="tokenizer">Gets the tokenizer which is being loaded.</param>
        /// <param name="input">The input language to read.</param>
        /// <returns>This loader so that calls can be chained.</returns>
        public void Load(Grammar.Grammar grammar, Tokenizer.Tokenizer tokenizer, IScanner input) {
            Result result = LoaderParser().Parse(input);
            if (result.Errors.Length > 0)
                throw new Exception("Error in provided language definition:"+
                    Environment.NewLine + "   " + result.Errors.JoinLines("   "));
            result.Tree.Process(this.handles, new V1Args(grammar, tokenizer));
        }

        #region Handlers...

        /// <summary>A prompt handle for starting a new definition block.</summary>
        /// <param name="args">The arguments for handling the prompt.</param>
        private void newDef(V1Args args) {
            args.Clear();
        }

        /// <summary>A prompt handle for setting the starting state of the tokenizer.</summary>
        /// <param name="args">The arguments for handling the prompt.</param>
        private void startState(V1Args args) =>
          this.Tokenizer.Start(this.states[^1].Name);

        /// <summary>A prompt handle for joining two states with the defined matcher.</summary>
        /// <param name="args">The arguments for handling the prompt.</param>
        private void joinState(V1Args args) {
            Tokenizer.State start = this.states[^2];
            Tokenizer.State end   = this.states[^1];
            Transition trans = start.Join(end.Name, this.curTransConsume);
            trans.Matchers.AddRange(this.curTransGroups[0].Matchers);
            this.curTransGroups.Clear();
            this.curTransConsume = false;
        }

        /// <summary>A prompt handle for joining a state to a token with the defined matcher.</summary>
        /// <param name="args">The arguments for handling the prompt.</param>
        private void joinToken(V1Args args) {
            Tokenizer.State start = this.states[^1];
            TokenState end = this.tokenStates[^1];
            Transition trans = start.Join(end.Name, this.curTransConsume);
            trans.Matchers.AddRange(this.curTransGroups[0].Matchers);
            Tokenizer.State endState = this.Tokenizer.State(end.Name);
            endState.SetToken(end.Name);
            this.curTransGroups.Clear();
            this.curTransConsume = false;
            // Put the accept state of the token onto the states stack.
            this.states.Add(endState);
        }

        /// <summary>A prompt handle for assigning a token to a state.</summary>
        /// <param name="args">The arguments for handling the prompt.</param>
        private void assignToken(V1Args args) {
            Tokenizer.State start = this.states[^1];
            TokenState end = this.tokenStates[^1];
            start.SetToken(end.Name);
        }

        /// <summary>A prompt handle for adding a new state to the tokenizer.</summary>
        /// <param name="args">The arguments for handling the prompt.</param>
        private void newState(V1Args args) =>
            this.states.Add(this.Tokenizer.State(args.Recent(1).Text));

        /// <summary>A prompt handle for adding a new token to the tokenizer.</summary>
        /// <param name="args">The arguments for handling the prompt.</param>
        private void newTokenState(V1Args args) =>
            this.tokenStates.Add(this.Tokenizer.Token(args.Recent(1).Text));

        /// <summary>
        /// A prompt handle for adding a new token to the tokenizer
        /// and setting it to consume that token.
        /// </summary>
        /// <param name="args">The arguments for handling the prompt.</param>
        private void newTokenConsume(V1Args args) =>
            this.tokenStates.Add(this.Tokenizer.Token(args.Recent(1).Text).Consume());

        /// <summary>A prompt handle for adding a new term to the grammar.</summary>
        /// <param name="args">The arguments for handling the prompt.</param>
        private void newTerm(V1Args args) =>
            this.terms.Push(this.Grammar.Term(args.Recent(1).Text));

        /// <summary>A prompt handle for adding a new token to the grammar.</summary>
        /// <param name="args">The arguments for handling the prompt.</param>
        private void newTokenItem(V1Args args) =>
            this.tokenItems.Push(this.Grammar.Token(args.Recent(1).Text));

        /// <summary>A prompt handle for adding a new prompt to the grammar.</summary>
        /// <param name="args">The arguments for handling the prompt.</param>
        private void newPrompt(V1Args args) =>
            this.prompts.Push(this.Grammar.Prompt(args.Recent(1).Text));

        /// <summary>A prompt handle for setting the currently building matcher to match any.</summary>
        /// <param name="args">The arguments for handling the prompt.</param>
        private void matchAny(V1Args args) =>
            this.topTransGroup.AddAll();

        /// <summary>A prompt handle for setting the currently building matcher to be consumed.</summary>
        /// <param name="args">The arguments for handling the prompt.</param>
        private void matchConsume(V1Args args) =>
          this.curTransConsume = true;

        /// <summary>A prompt handle for setting the currently building matcher to match to a character set.</summary>
        /// <param name="args">The arguments for handling the prompt.</param>
        private void matchSet(V1Args args) {
            Rune[] match = Misc.Text.Unescape(args.LastText).EnumerateRunes().ToArray();
            if (match.Length == 1)
                this.topTransGroup.AddSingle(match[0]);
            else this.topTransGroup.AddSet(match);
        }

        /// <summary>A prompt handle for setting the currently building matcher to not match to a character set.</summary>
        /// <param name="args">The arguments for handling the prompt.</param>
        private void matchSetNot(V1Args args) {
            this.notGroupStart(args);
            this.matchSet(args);
            this.notGroupEnd(args);
        }

        /// <summary>A prompt handle for setting the currently building matcher to match to a character range.</summary>
        /// <param name="args">The arguments for handling the prompt.</param>
        private void matchRange(V1Args args) {
            Token lowChar  = args.Recent(2);
            Token highChar = args.Recent();
            Rune[] lowText  = Misc.Text.Unescape(lowChar.Text).EnumerateRunes().ToArray();
            Rune[] highText = Misc.Text.Unescape(highChar.Text).EnumerateRunes().ToArray();
            if (lowText.Length != 1)
                throw new Exception("May only have one character for the low char, "+lowChar+", of a range.");
            if (highText.Length != 1)
                throw new Exception("May only have one character for the high char, "+highChar+", of a range.");
            this.topTransGroup.AddRange(lowText[0], highText[0]);
        }

        /// <summary>A prompt handle for setting the currently building matcher to not match to a character range.</summary>
        /// <param name="args">The arguments for handling the prompt.</param>
        private void matchRangeNot(V1Args args) {
            this.notGroupStart(args);
            this.matchRange(args);
            this.notGroupEnd(args);
        }

        /// <summary>A prompt handle for starting a not group of matchers.</summary>
        /// <param name="args">The arguments for handling the prompt.</param>
        private void notGroupStart(V1Args args) {
            Not notGroup = new();
            this.topTransGroup.Add(notGroup);
            this.curTransGroups.Add(notGroup);
        }

        /// <summary>A prompt handle for ending a not group of matchers.</summary>
        /// <param name="args">The arguments for handling the prompt.</param>
        private void notGroupEnd(V1Args args) =>
            this.curTransGroups.RemoveAt(this.curTransGroups.Count-1);

        /// <summary>A prompt handle for adding a new replacement string to the loader.</summary>
        /// <param name="args">The arguments for handling the prompt.</param>
        private void addReplaceText(V1Args args) =>
          this.replaceText.Add(Misc.Text.Unescape(args.LastText));

        /// <summary>
        /// A prompt handle for setting a set of replacements between two
        /// tokens with a previously set replacement string set.
        /// </summary>
        /// <param name="args">The arguments for handling the prompt.</param>
        private void replaceToken(V1Args args) {
            TokenState start = this.tokenStates[^2];
            TokenState end   = this.tokenStates[^1];
            start.Replace(end.Name, this.replaceText);
            this.replaceText.Clear();
            // remove end while keeping the start.
            this.tokenStates.RemoveAt(this.tokenStates.Count-1);
        }

        /// <summary>A prompt handle for starting a grammar definition of a term.</summary>
        /// <param name="args">The arguments for handling the prompt.</param>
        private void startTerm(V1Args args) =>
          this.Grammar.Start(this.terms.Peek().Name);

        /// <summary>A prompt handle for starting defining a rule for the current term.</summary>
        /// <param name="args">The arguments for handling the prompt.</param>
        private void startRule(V1Args args) =>
          this.curRule = this.terms.Peek().NewRule();

        /// <summary>A prompt handle for adding a token to the current rule being built.</summary>
        /// <param name="args">The arguments for handling the prompt.</param>
        private void itemToken(V1Args args) =>
          this.curRule.AddToken(this.tokenItems.Pop().Name);

        /// <summary>A prompt handle for adding a term to the current rule being built.</summary>
        /// <param name="args">The arguments for handling the prompt.</param>
        private void itemTerm(V1Args args) =>
          this.curRule.AddTerm(this.terms.Pop().Name);

        /// <summary>A prompt handle for adding a prompt to the current rule being built.</summary>
        /// <param name="args">The arguments for handling the prompt.</param>
        private void itemPrompt(V1Args args) =>
          this.curRule.AddPrompt(this.prompts.Pop().Name);

        /// <summary>Sets the error token to the tokenizer and parser to use for bad input.</summary>
        /// <param name="args">The arguments for handling the prompt.</param>
        private void setError(V1Args args) {
            string errToken = this.tokenItems.Pop().Name;
            this.Tokenizer.ErrorToken(errToken);
            this.Grammar.Error(errToken);
        }

        #endregion
    }
}
