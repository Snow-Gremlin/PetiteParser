using PetiteParser.Grammar;
using PetiteParser.Matcher;
using PetiteParser.Misc;
using PetiteParser.Parser;
using PetiteParser.ParseTree;
using PetiteParser.Tokenizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PetiteParser.Loader {

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
        static public Parser.Parser LoadParser(Scanner.IScanner input) =>
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
        static public Grammar.Grammar LoadGrammar(Scanner.IScanner input) =>
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
        static public Tokenizer.Tokenizer LoadTokenizer(Scanner.IScanner input) =>
            new Loader().Load(input).Tokenizer;

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
            gram.NewRule("def.set").AddTerm("def.set").AddTerm("def").AddToken("semicolon");
            gram.NewRule("def.set");

            gram.NewRule("def").AddPrompt("new.def").AddToken("closeAngle").AddTerm("stateID").AddPrompt("start.state").AddTerm("def.state.optional");
            gram.NewRule("def").AddPrompt("new.def").AddTerm("stateID").AddTerm("def.state");

            gram.NewRule("def.state.optional");
            gram.NewRule("def.state.optional").AddTerm("def.state");

            gram.NewRule("def.state").AddToken("colon").AddTerm("matcher.start").AddToken("arrow").AddTerm("def.assign");
            gram.NewRule("def.assign").AddTerm("stateID").AddPrompt("join.state").AddTerm("def.state.optional");
            gram.NewRule("def.assign").AddTerm("tokenStateID").AddPrompt("join.token").AddTerm("def.state.optional");
            gram.NewRule("def.state").AddToken("arrow").AddTerm("tokenStateID").AddPrompt("assign.token").AddTerm("def.state.optional");

            gram.NewRule("stateID").AddToken("openParen").AddToken("id").AddToken("closeParen").AddPrompt("new.state");
            gram.NewRule("tokenStateID").AddToken("openBracket").AddToken("id").AddToken("closeBracket").AddPrompt("new.token.state");
            gram.NewRule("tokenStateID").AddToken("consume").AddToken("openBracket").AddToken("id").AddToken("closeBracket").AddPrompt("new.token.consume");
            gram.NewRule("termID").AddToken("openAngle").AddToken("id").AddToken("closeAngle").AddPrompt("new.term");
            gram.NewRule("tokenItemID").AddToken("openBracket").AddToken("id").AddToken("closeBracket").AddPrompt("new.token.item");
            gram.NewRule("triggerID").AddToken("openCurly").AddToken("id").AddToken("closeCurly").AddPrompt("new.trigger");

            gram.NewRule("matcher.start").AddToken("any").AddPrompt("match.any");
            gram.NewRule("matcher.start").AddTerm("matcher");
            gram.NewRule("matcher.start").AddToken("consume").AddTerm("matcher").AddPrompt("match.consume");

            gram.NewRule("matcher").AddTerm("charSetRange");
            gram.NewRule("matcher").AddTerm("matcher").AddToken("comma").AddTerm("charSetRange");

            gram.NewRule("charSetRange").AddToken("string").AddPrompt("match.set");
            gram.NewRule("charSetRange").AddToken("not").AddToken("string").AddPrompt("match.set.not");
            gram.NewRule("charSetRange").AddToken("string").AddToken("range").AddToken("string").AddPrompt("match.range");
            gram.NewRule("charSetRange").AddToken("not").AddToken("string").AddToken("range").AddToken("string").AddPrompt("match.range.not");
            gram.NewRule("charSetRange").AddToken("not").AddToken("openParen").AddPrompt("not.group.start").AddTerm("matcher").AddToken("closeParen").AddPrompt("not.group.end");

            gram.NewRule("def").AddPrompt("new.def").AddToken("any").AddToken("arrow").AddTerm("tokenItemID").AddPrompt("set.error");
            gram.NewRule("def").AddPrompt("new.def").AddTerm("tokenStateID").AddTerm("def.token");

            gram.NewRule("def.token").AddToken("equal").AddTerm("def.token.replace");
            gram.NewRule("def.token.replace").AddTerm("replaceText").AddToken("arrow").AddTerm("tokenStateID").AddPrompt("replace.token").AddTerm("def.token.optional");
            gram.NewRule("def.token.optional");
            gram.NewRule("def.token.optional").AddToken("or").AddTerm("def.token.replace");

            gram.NewRule("replaceText").AddToken("string").AddPrompt("add.replace.text");
            gram.NewRule("replaceText").AddTerm("replaceText").AddToken("comma").AddToken("string").AddPrompt("add.replace.text");

            gram.NewRule("def").AddPrompt("new.def").AddToken("closeAngle").AddTerm("termID").AddPrompt("start.term").AddTerm("start.rule.optional");
            gram.NewRule("def").AddPrompt("new.def").AddTerm("termID").AddToken("assign").AddPrompt("start.rule").AddTerm("start.rule").AddTerm("next.rule.optional");

            gram.NewRule("start.rule.optional");
            gram.NewRule("start.rule.optional").AddToken("assign").AddPrompt("start.rule").AddTerm("start.rule").AddTerm("next.rule.optional");

            gram.NewRule("next.rule.optional");
            gram.NewRule("next.rule.optional").AddTerm("next.rule.optional").AddToken("or").AddPrompt("start.rule").AddTerm("start.rule");

            gram.NewRule("start.rule").AddTerm("tokenItemID").AddPrompt("item.token").AddTerm("rule.item");
            gram.NewRule("start.rule").AddTerm("termID").AddPrompt("item.term").AddTerm("rule.item");
            gram.NewRule("start.rule").AddTerm("triggerID").AddPrompt("item.trigger").AddTerm("rule.item");
            gram.NewRule("start.rule").AddToken("lambda");

            gram.NewRule("rule.item");
            gram.NewRule("rule.item").AddTerm("rule.item").AddTerm("tokenItemID").AddPrompt("item.token");
            gram.NewRule("rule.item").AddTerm("rule.item").AddTerm("termID").AddPrompt("item.term");
            gram.NewRule("rule.item").AddTerm("rule.item").AddTerm("triggerID").AddPrompt("item.trigger");
            return gram;
        }

        /// <summary>Creates a new parser for loading tokenizer and grammar definitions.</summary>
        /// <returns>This is the parser for the parser language.</returns>
        static public Parser.Parser GetLoaderParser() => new(GetLoaderGrammar(), GetLoaderTokenizer());

        #endregion

        private Dictionary<string, PromptHandle> handles;
        private List<Tokenizer.State> states;
        private List<TokenState> tokenStates;
        private Stack<Term> terms;
        private Stack<TokenItem> tokenItems;
        private Stack<Prompt> prompts;
        private List<Group> curTransGroups;
        private bool curTransConsume;
        private List<string> replaceText;
        private Rule curRule;

        /// <summary>Creates a new loader.</summary>
        public Loader() {
            this.handles = new Dictionary<string, PromptHandle>() {
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
                { "new.trigger",       this.newTrigger },
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
                { "item.trigger",      this.itemTrigger },
                { "set.error",         this.setError }
            };
            
            this.Grammar     = new Grammar.Grammar();
            this.Tokenizer   = new Tokenizer.Tokenizer();
            this.states      = new List<Tokenizer.State>();
            this.tokenStates = new List<TokenState>();
            this.terms       = new Stack<Term>();
            this.tokenItems  = new Stack<TokenItem>();
            this.prompts     = new Stack<Prompt>();

            this.curTransGroups  = new List<Group>();
            this.curTransConsume = false;
            this.replaceText     = new List<string>();
            this.curRule         = null;
        }

        /// <summary>
        /// Adds several blocks of definitions to the grammar and tokenizer
        /// which are being loaded via a string containing the definition.
        /// </summary>
        /// <param name="input">The input language to read.</param>
        /// <returns>This loader so that calls can be chained.</returns>
        public Loader Load(params string[] input) =>
            this.Load(new Scanner.Default(input));

        /// <summary>
        /// Adds several blocks of definitions to the grammar and tokenizer
        /// which are being loaded via a list of characters containing the definition.
        /// </summary>
        /// <param name="iterator">The input language to read.</param>
        /// <returns>This loader so that calls can be chained.</returns>
        public Loader Load(IEnumerable<Rune> input) =>
            this.Load(new Scanner.Default(input));

        /// <summary>
        /// Adds several blocks of definitions to the grammar and tokenizer
        /// which are being loaded via a list of characters containing the definition.
        /// </summary>
        /// <param name="iterator">The input language to read.</param>
        /// <returns>This loader so that calls can be chained.</returns>
        public Loader Load(Scanner.IScanner input) {
            Result result = GetLoaderParser().Parse(input);
            if (result.Errors.Length > 0)
                throw new Exception("Error in provided language definition:"+
                    Environment.NewLine+"   "+result.Errors.JoinLines("   "));
            result.Tree.Process(this.handles);
            return this;
        }

        /// <summary>Gets the grammar which is being loaded.</summary>
        public Grammar.Grammar Grammar { get; }

        /// <summary>Gets the tokenizer which is being loaded.</summary>
        public Tokenizer.Tokenizer Tokenizer { get; }

        /// <summary>Creates a parser with the loaded tokenizer and grammar.</summary>
        public Parser.Parser Parser => new(this.Grammar, this.Tokenizer);

        /// <summary>Gets the top matcher group in the current transitions.</summary>
        /// <remarks>If there are no groups then one is added.</remarks>
        private Group topTransGroup {
            get {
                if (this.curTransGroups.Count <= 0)
                    this.curTransGroups.Add(new Group());
                return this.curTransGroups[^1];
            }
        }

        #region Handlers...

        /// <summary>A trigger handle for starting a new definition block.</summary>
        /// <param name="args">The arguments for handling the prompt.</param>
        private void newDef(PromptArgs args) {
            args.Tokens.Clear();
            this.states.Clear();
            this.tokenStates.Clear();
            this.terms.Clear();
            this.tokenItems.Clear();
            this.prompts.Clear();
            this.curTransGroups.Clear();
            this.curTransConsume = false;
            this.replaceText.Clear();
            this.curRule = null;
        }

        /// <summary>A trigger handle for setting the starting state of the tokenizer.</summary>
        /// <param name="args">The arguments for handling the prompt.</param>
        private void startState(PromptArgs args) =>
          this.Tokenizer.Start(this.states[^1].Name);

        /// <summary>A trigger handle for joining two states with the defined matcher.</summary>
        /// <param name="args">The arguments for handling the prompt.</param>
        private void joinState(PromptArgs args) {
            Tokenizer.State start = this.states[^2];
            Tokenizer.State end   = this.states[^1];
            Transition trans = start.Join(end.Name, this.curTransConsume);
            trans.Matchers.AddRange(this.curTransGroups[0].Matchers);
            this.curTransGroups.Clear();
            this.curTransConsume = false;
        }

        /// <summary>A trigger handle for joining a state to a token with the defined matcher.</summary>
        /// <param name="args">The arguments for handling the prompt.</param>
        private void joinToken(PromptArgs args) {
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

        /// <summary>A trigger handle for assigning a token to a state.</summary>
        /// <param name="args">The arguments for handling the prompt.</param>
        private void assignToken(PromptArgs args) {
            Tokenizer.State start = this.states[^1];
            TokenState end = this.tokenStates[^1];
            start.SetToken(end.Name);
        }

        /// <summary>A trigger handle for adding a new state to the tokenizer.</summary>
        /// <param name="args">The arguments for handling the prompt.</param>
        private void newState(PromptArgs args) =>
            this.states.Add(this.Tokenizer.State(args.Recent(1).Text));

        /// <summary>A trigger handle for adding a new token to the tokenizer.</summary>
        /// <param name="args">The arguments for handling the prompt.</param>
        private void newTokenState(PromptArgs args) =>
            this.tokenStates.Add(this.Tokenizer.Token(args.Recent(1).Text));

        /// <summary>
        /// A trigger handle for adding a new token to the tokenizer
        /// and setting it to consume that token.
        /// </summary>
        /// <param name="args">The arguments for handling the prompt.</param>
        private void newTokenConsume(PromptArgs args) =>
            this.tokenStates.Add(this.Tokenizer.Token(args.Recent(1).Text).Consume());

        /// <summary>A trigger handle for adding a new term to the grammar.</summary>
        /// <param name="args">The arguments for handling the prompt.</param>
        private void newTerm(PromptArgs args) =>
            this.terms.Push(this.Grammar.Term(args.Recent(1).Text));

        /// <summary>A trigger handle for adding a new token to the grammar.</summary>
        /// <param name="args">The arguments for handling the prompt.</param>
        private void newTokenItem(PromptArgs args) =>
            this.tokenItems.Push(this.Grammar.Token(args.Recent(1).Text));

        /// <summary>A trigger handle for adding a new trigger to the grammar.</summary>
        /// <param name="args">The arguments for handling the prompt.</param>
        private void newTrigger(PromptArgs args) =>
            this.prompts.Push(this.Grammar.Prompt(args.Recent(1).Text));

        /// <summary>A trigger handle for setting the currently building matcher to match any.</summary>
        /// <param name="args">The arguments for handling the prompt.</param>
        private void matchAny(PromptArgs args) =>
            this.topTransGroup.AddAll();

        /// <summary>A trigger handle for setting the currently building matcher to be consumed.</summary>
        /// <param name="args">The arguments for handling the prompt.</param>
        private void matchConsume(PromptArgs args) =>
          this.curTransConsume = true;

        /// <summary>A trigger handle for setting the currently building matcher to match to a character set.</summary>
        /// <param name="args">The arguments for handling the prompt.</param>
        private void matchSet(PromptArgs args) {
            Rune[] match = Misc.Text.Unescape(args.LastText).EnumerateRunes().ToArray();
            if (match.Length == 1)
                this.topTransGroup.AddSingle(match[0]);
            else this.topTransGroup.AddSet(match);
        }

        /// <summary>A trigger handle for setting the currently building matcher to not match to a character set.</summary>
        /// <param name="args">The arguments for handling the prompt.</param>
        private void matchSetNot(PromptArgs args) {
            this.notGroupStart(args);
            this.matchSet(args);
            this.notGroupEnd(args);
        }

        /// <summary>A trigger handle for setting the currently building matcher to match to a character range.</summary>
        /// <param name="args">The arguments for handling the prompt.</param>
        private void matchRange(PromptArgs args) {
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

        /// <summary>A trigger handle for setting the currently building matcher to not match to a character range.</summary>
        /// <param name="args">The arguments for handling the prompt.</param>
        private void matchRangeNot(PromptArgs args) {
            this.notGroupStart(args);
            this.matchRange(args);
            this.notGroupEnd(args);
        }

        /// <summary>A trigger handle for starting a not group of matchers.</summary>
        /// <param name="args">The arguments for handling the prompt.</param>
        private void notGroupStart(PromptArgs args) {
            Not notGroup = new();
            this.topTransGroup.Add(notGroup);
            this.curTransGroups.Add(notGroup);
        }

        /// <summary>A trigger handle for ending a not group of matchers.</summary>
        /// <param name="args">The arguments for handling the prompt.</param>
        private void notGroupEnd(PromptArgs args) =>
            this.curTransGroups.RemoveAt(this.curTransGroups.Count-1);

        /// <summary>A trigger handle for adding a new replacement string to the loader.</summary>
        /// <param name="args">The arguments for handling the prompt.</param>
        private void addReplaceText(PromptArgs args) =>
          this.replaceText.Add(Misc.Text.Unescape(args.LastText));

        /// <summary>
        /// A trigger handle for setting a set of replacements between two
        /// tokens with a previously set replacement string set.
        /// </summary>
        /// <param name="args">The arguments for handling the prompt.</param>
        private void replaceToken(PromptArgs args) {
            TokenState start = this.tokenStates[^2];
            TokenState end   = this.tokenStates[^1];
            start.Replace(end.Name, this.replaceText);
            this.replaceText.Clear();
            // remove end while keeping the start.
            this.tokenStates.RemoveAt(this.tokenStates.Count-1);
        }

        /// <summary>A trigger handle for starting a grammar definition of a term.</summary>
        /// <param name="args">The arguments for handling the prompt.</param>
        private void startTerm(PromptArgs args) =>
          this.Grammar.Start(this.terms.Peek().Name);

        /// <summary>A trigger handle for starting defining a rule for the current term.</summary>
        /// <param name="args">The arguments for handling the prompt.</param>
        private void startRule(PromptArgs args) =>
          this.curRule = this.terms.Peek().NewRule();

        /// <summary>A trigger handle for adding a token to the current rule being built.</summary>
        /// <param name="args">The arguments for handling the prompt.</param>
        private void itemToken(PromptArgs args) =>
          this.curRule.AddToken(this.tokenItems.Pop().Name);

        /// <summary>A trigger handle for adding a term to the current rule being built.</summary>
        /// <param name="args">The arguments for handling the prompt.</param>
        private void itemTerm(PromptArgs args) =>
          this.curRule.AddTerm(this.terms.Pop().Name);

        /// <summary>A trigger handle for adding a trigger to the current rule being built.</summary>
        /// <param name="args">The arguments for handling the prompt.</param>
        private void itemTrigger(PromptArgs args) =>
          this.curRule.AddPrompt(this.prompts.Pop().Name);

        /// <summary>Sets the error token to the tokenizer and parser to use for bad input.</summary>
        /// <param name="args">The arguments for handling the prompt.</param>
        private void setError(PromptArgs args) {
            string errToken = this.tokenItems.Pop().Name;
            this.Tokenizer.ErrorToken(errToken);
            this.Grammar.Error(errToken);
        }

        #endregion
    }
}
