using PetiteParser.Parser;
using PetiteParser.Tokenizer.Matcher;

namespace PetiteParser.Loader;

/// <summary>The hard coded language definition for the petite language for loading a parser.</summary>
static internal class Language {

    /// <summary>The singleton for the parser loader.</summary>
    static private Parser.Parser? parserSingleton;

    /// <summary>Gets the parser for loading tokenizer and grammar definitions.</summary>
    /// <returns>This is the parser for the parser language.</returns>
    static public Parser.Parser LoaderParser =>
        parserSingleton ??= new(GetLoaderGrammar(), GetLoaderTokenizer(), OnConflict.Panic);

    #region Tokenizer...

    /// <summary>A matcher for matching identifiers allowed in the loader language.</summary>
    private static readonly Group idLetter = new Group().Add(Predef.LetterOrDigit).AddSet("_.-");

    /// <summary>A matcher for matching hexadecimal nibbles used for strings and chars.</summary>
    private static readonly Group hexMatcher = new Group().AddRange('0', '9').AddRange('A', 'F').AddRange('a', 'f');

    /// <summary>Gets the tokenizer used for loading a parser definition.</summary>
    /// <returns>The tokenizer of the parser language.</returns>
    static public Tokenizer.Tokenizer GetLoaderTokenizer() {
        Tokenizer.Tokenizer tok = new();
        tok.Start("start");

        tokenizeWhitespace(tok);
        tokenizeComment(tok);
        tokenizeBrackets(tok);
        tokenizeSymbols(tok);
        tokenizeIdentifier(tok);
        tokenizeFeature(tok);
        tokenizeChars(tok);
        tokenizeString(tok);
        return tok;
    }

    /// <summary>Adds whitespace consumption to ignore all whitespace outside of other tokens such as strings.</summary>
    /// <param name="tok">The tokenizer to add to.</param>
    static private void tokenizeWhitespace(Tokenizer.Tokenizer tok) {
        tok.Join("start", "whitespace").Add(Predef.WhiteSpace);
        tok.Join("whitespace", "whitespace").Add(Predef.WhiteSpace);
        tok.SetToken("whitespace", "whitespace").Consume();
    }

    /// <summary>Adds a comment which starts with a hash tag and continues until the end of a line.</summary>
    /// <param name="tok">The tokenizer to add to.</param>
    static private void tokenizeComment(Tokenizer.Tokenizer tok) {
        tok.Join("start", "comment").AddSingle('#');
        tok.Join("comment", "comment.end").AddSingle('\n');
        tok.Join("comment", "comment").AddAll();
        tok.SetToken("comment.end", "comment").Consume();
    }

    /// <summary>Adds all the symbols used mostly to define states, tokens, terms, and prompts.</summary>
    /// <param name="tok">The tokenizer to add to.</param>
    static private void tokenizeBrackets(Tokenizer.Tokenizer tok) {
        // (State)
        tok.JoinToToken("start", "paren.open").AddSingle('(');
        tok.JoinToToken("start", "paren.close").AddSingle(')');
        // [Token]
        tok.JoinToToken("start", "bracket.open").AddSingle('[');
        tok.JoinToToken("start", "bracket.close").AddSingle(']');
        // <Term>
        tok.JoinToToken("start", "angle.open").AddSingle('<');
        tok.JoinToToken("start", "angle.close").AddSingle('>');
        // {Prompt}
        tok.JoinToToken("start", "curly.open").AddSingle('{');
        tok.JoinToToken("start", "curly.close").AddSingle('}');
    }

    /// <summary>Adds the symbols used in the language for defining the tokenizer and grammar.</summary>
    /// <param name="tok">The tokenizer to add to.</param>
    static private void tokenizeSymbols(Tokenizer.Tokenizer tok) {
        tok.JoinToToken("start", "or").AddSingle('|');
        tok.JoinToToken("start", "not").AddSingle('!');
        tok.JoinToToken("start", "consume").AddSingle('^');
        tok.JoinToToken("start", "colon").AddSingle(':');
        tok.JoinToToken("start", "semicolon").AddSingle(';');
        tok.JoinToToken("colon", "assign").AddSingle('=');
        tok.JoinToToken("start", "comma").AddSingle(',');
        tok.JoinToToken("start", "any").AddSingle('*');
        tok.JoinToToken("start", "lambda").AddSingle('_');

        // Arrow symbol (=>)
        tok.Join("start", "equal").AddSingle('=');
        tok.SetToken("equal", "equal");
        tok.Join("equal", "arrow").AddSingle('>');
        tok.SetToken("arrow", "arrow");

        // Range symbol (..)
        tok.Join("start", "startRange").AddSingle('.');
        tok.JoinToToken("startRange", "range").AddSingle('.');
    }

    /// <summary>Adds the identifier to the tokenizer.</summary>
    /// <param name="tok">The tokenizer to add to.</param>
    static private void tokenizeIdentifier(Tokenizer.Tokenizer tok) {
        tok.JoinToToken("start", "id").Add(idLetter);
        tok.Join("id", "id").Add(idLetter);
    }

    /// <summary>Adds the feature identifier for setting parser loader flags.</summary>
    /// <param name="tok">The tokenizer to add to.</param>
    static private void tokenizeFeature(Tokenizer.Tokenizer tok) {
        tok.Join("start", "feature.start").SetConsume(true).AddSingle('$');
        tok.JoinToToken("feature.start", "feature").Add(idLetter);
        tok.Join("feature", "feature").Add(idLetter);
    }

    /// <summary>Adds the character set (single quote string) to the tokenizer.</summary>
    /// <param name="tok">The tokenizer to add to.</param>
    static private void tokenizeChars(Tokenizer.Tokenizer tok) {
        tok.Join("start", "single.quote.body").SetConsume(true).AddSingle('\'');
        tok.Join("single.quote.body", "single.quote").SetConsume(true).AddSingle('\'');

        // Add simple escape \n \\ \r \t \' \"
        tok.Join("single.quote.body", "single.quote.escape").AddSingle('\\');
        tok.Join("single.quote.escape", "single.quote.body").AddSet("n\\rt'\"");

        // Add ASCII escape (2 hex) \xHH
        tok.Join("single.quote.escape", "single.quote.hex1").AddSingle('x');
        tok.Join("single.quote.hex1", "single.quote.hex2").Add(hexMatcher);
        tok.Join("single.quote.hex2", "single.quote.body").Add(hexMatcher);

        // Add Unicode escape (4 hex) \uHHHH
        tok.Join("single.quote.escape", "single.quote.unicode1").AddSingle('u');
        tok.Join("single.quote.unicode1", "single.quote.unicode2").Add(hexMatcher);
        tok.Join("single.quote.unicode2", "single.quote.unicode3").Add(hexMatcher);
        tok.Join("single.quote.unicode3", "single.quote.unicode4").Add(hexMatcher);
        tok.Join("single.quote.unicode4", "single.quote.body").Add(hexMatcher);

        // Add extended Unicode escape (8 hex) \UHHHHHHHH
        tok.Join("single.quote.escape", "single.quote.rune1").AddSingle('U');
        tok.Join("single.quote.rune1", "single.quote.rune2").Add(hexMatcher);
        tok.Join("single.quote.rune2", "single.quote.rune3").Add(hexMatcher);
        tok.Join("single.quote.rune3", "single.quote.rune4").Add(hexMatcher);
        tok.Join("single.quote.rune4", "single.quote.rune5").Add(hexMatcher);
        tok.Join("single.quote.rune5", "single.quote.rune6").Add(hexMatcher);
        tok.Join("single.quote.rune6", "single.quote.rune7").Add(hexMatcher);
        tok.Join("single.quote.rune7", "single.quote.rune8").Add(hexMatcher);
        tok.Join("single.quote.rune8", "single.quote.body").Add(hexMatcher);

        tok.Join("single.quote.body", "single.quote.body").AddAll();
        tok.SetToken("single.quote", "chars");
    }

    /// <summary>Adds the string (double quote string) to the tokenizer.</summary>
    /// <param name="tok">The tokenizer to add to.</param>
    static private void tokenizeString(Tokenizer.Tokenizer tok) {
        tok.Join("start", "double.quote.body").SetConsume(true).AddSingle('"');
        tok.Join("double.quote.body", "double.quote").SetConsume(true).AddSingle('"');

        // Add simple escape \n \\ \r \t \' \"
        tok.Join("double.quote.body", "double.quote.escape").AddSingle('\\');
        tok.Join("double.quote.escape", "double.quote.body").AddSet("n\\rt'\"");

        // Add ASCII escape (2 hex) \xHH
        tok.Join("double.quote.escape", "double.quote.hex1").AddSingle('x');
        tok.Join("double.quote.hex1", "double.quote.hex2").Add(hexMatcher);
        tok.Join("double.quote.hex2", "double.quote.body").Add(hexMatcher);

        // Add Unicode escape (4 hex) \uHHHH
        tok.Join("double.quote.escape", "double.quote.unicode1").AddSingle('u');
        tok.Join("double.quote.unicode1", "double.quote.unicode2").Add(hexMatcher);
        tok.Join("double.quote.unicode2", "double.quote.unicode3").Add(hexMatcher);
        tok.Join("double.quote.unicode3", "double.quote.unicode4").Add(hexMatcher);
        tok.Join("double.quote.unicode4", "double.quote.body").Add(hexMatcher);

        // Add extended Unicode escape (8 hex) \UHHHHHHHH
        tok.Join("double.quote.escape", "double.quote.rune1").AddSingle('U');
        tok.Join("double.quote.rune1", "double.quote.rune2").Add(hexMatcher);
        tok.Join("double.quote.rune2", "double.quote.rune3").Add(hexMatcher);
        tok.Join("double.quote.rune3", "double.quote.rune4").Add(hexMatcher);
        tok.Join("double.quote.rune4", "double.quote.rune5").Add(hexMatcher);
        tok.Join("double.quote.rune5", "double.quote.rune6").Add(hexMatcher);
        tok.Join("double.quote.rune6", "double.quote.rune7").Add(hexMatcher);
        tok.Join("double.quote.rune7", "double.quote.rune8").Add(hexMatcher);
        tok.Join("double.quote.rune8", "double.quote.body").Add(hexMatcher);

        tok.Join("double.quote.body", "double.quote.body").AddAll();
        tok.SetToken("double.quote", "string");
    }

    #endregion
    #region Grammar...

    /// <summary>Gets the grammar used for loading a parser definition.</summary>
    /// <returns>The grammar for the parser language.</returns>
    static public Grammar.Grammar GetLoaderGrammar() {
        Grammar.Grammar gram = new();
        gram.Start("def.set");
        gram.NewRule("def.set", "<def.set> <def> [semicolon]");
        gram.NewRule("def.set");

        grammarForFeatureLoader(gram);
        grammarForTokenizerStateLoader(gram);
        grammarForTokenizerCharMatcher(gram);
        grammarForTokenizerTokenLoader(gram);
        grammarForGrammarLoader(gram);
        return gram;
    }

    /// <summary>Add the grammar rules for setting loader features.</summary>
    /// <param name="gram">The grammar to add to.</param>
    static private void grammarForFeatureLoader(Grammar.Grammar gram) {
        gram.NewRule("def", "{new.def} [feature] {feature.mode} <feature.tail>");

        // Changing a key value pair feature
        gram.NewRule("feature.tail", "[id] [string] {feature.value}");

        // Changing one or more flag features
        gram.NewRule("feature.tail", "[id] {feature.flag} <feature.list.optional>");
        gram.NewRule("feature.list.optional");
        gram.NewRule("feature.list.optional", "[comma] [id] {feature.flag} <feature.list.optional>");
    }

    /// <summary>Add the grammar rules for reading a tokenizer state.</summary>
    /// <param name="gram">The grammar to add to.</param>
    static private void grammarForTokenizerStateLoader(Grammar.Grammar gram) {
        gram.NewRule("def", "{new.def} [angle.close] <stateID> {start.state} <def.state.optional>");
        gram.NewRule("def", "{new.def} <stateID> <def.state>");

        gram.NewRule("def.state.optional");
        gram.NewRule("def.state.optional", "<def.state>");

        gram.NewRule("def.state", "[colon] <matcher.start> [arrow] <def.assign>");
        gram.NewRule("def.assign", "<stateID> {join.state} <def.state.optional>");
        gram.NewRule("def.assign", "<tokenStateID> {join.token} <def.state.optional>");
        gram.NewRule("def.state", "[arrow] <tokenStateID> {assign.token} <def.state.optional>");

        // Add (State), [token], <term>, and {prompt} with different token modes.
        gram.NewRule("stateID", "[paren.open] [id] {new.state} [paren.close]");
        gram.NewRule("tokenStateID", "[bracket.open] [id] {new.token.state} [bracket.close]");
        gram.NewRule("tokenStateID", "[consume] [bracket.open] [id] {new.token.consume} [bracket.close]");
        gram.NewRule("termID", "[angle.open] [id] {new.term} [angle.close]");
        gram.NewRule("tokenItemID", "[bracket.open] [id] {new.token.item} [bracket.close]");
        gram.NewRule("promptID", "[curly.open] [id] {new.prompt} [curly.close]");
    }

    /// <summary>Add the grammar rules for matching character sets.</summary>
    /// <param name="gram">The grammar to add to.</param>
    static private void grammarForTokenizerCharMatcher(Grammar.Grammar gram) {
        gram.NewRule("matcher.start", "[any] {match.any}");
        gram.NewRule("matcher.start", "<matcher>");
        gram.NewRule("matcher.start", "[consume] <matcher> {match.consume}");

        gram.NewRule("matcher", "<charSetRange> <matcher.tail>");
        gram.NewRule("matcher.tail");
        gram.NewRule("matcher.tail", "[comma] <charSetRange> <matcher.tail>");

        gram.NewRule("charSetRange", "[chars] {match.set}");
        gram.NewRule("charSetRange", "[not] [chars] {match.set.not}");
        gram.NewRule("charSetRange", "[chars] [range] [chars] {match.range}");
        gram.NewRule("charSetRange", "[not] [chars] [range] [chars] {match.range.not}");

        gram.NewRule("charSetRange", "[string] {match.set}");
        gram.NewRule("charSetRange", "[not] [string] {match.set.not}");
        gram.NewRule("charSetRange", "[string] [range] [string] {match.range}");
        gram.NewRule("charSetRange", "[not] [string] [range] [string] {match.range.not}");

        gram.NewRule("charSetRange", "[not] [paren.open] {not.group.start} <matcher> [paren.close] {not.group.end}");
    }

    /// <summary>Add the grammar rules for reading a token and replacements.</summary>
    /// <param name="gram">The grammar to add to.</param>
    static private void grammarForTokenizerTokenLoader(Grammar.Grammar gram) {
        gram.NewRule("def", "{new.def} [any] [arrow] <tokenItemID> {set.error}");
        gram.NewRule("def", "{new.def} <tokenStateID> <def.token>");

        gram.NewRule("def.token", "[equal] <def.token.replace>");
        gram.NewRule("def.token.replace", "<replaceText> [arrow] <tokenStateID> {replace.token} <def.token.optional>");
        gram.NewRule("def.token.optional");
        gram.NewRule("def.token.optional", "[or] <def.token.replace>");

        gram.NewRule("replaceText", "[chars] {add.replace.text}");
        gram.NewRule("replaceText", "<replaceText> [comma] [chars] {add.replace.text}");

        gram.NewRule("replaceText", "[string] {add.replace.text}");
        gram.NewRule("replaceText", "<replaceText> [comma] [string] {add.replace.text}");
    }

    /// <summary>Add the grammar rules for reading grammar rules while loading.</summary>
    /// <param name="gram">The grammar to add to.</param>
    static private void grammarForGrammarLoader(Grammar.Grammar gram) {
        gram.NewRule("def", "{new.def} [angle.close] <termID> {start.term} <start.rule.optional>");
        gram.NewRule("def", "{new.def} <termID> [assign] {start.rule} <start.rule> <next.rule.optional>");

        gram.NewRule("start.rule.optional");
        gram.NewRule("start.rule.optional", "[assign] {start.rule} <start.rule> <next.rule.optional>");

        gram.NewRule("next.rule.optional");
        gram.NewRule("next.rule.optional", "<next.rule.optional> [or] {start.rule} <start.rule>");

        gram.NewRule("start.rule", "<tokenItemID> {item.token} <rule.item>");
        gram.NewRule("start.rule", "<termID> {item.term} <rule.item>");
        gram.NewRule("start.rule", "<promptID> {item.prompt} <rule.item>");
        gram.NewRule("start.rule", "[lambda]");

        gram.NewRule("rule.item");
        gram.NewRule("rule.item", "<rule.item> <tokenItemID> {item.token}");
        gram.NewRule("rule.item", "<rule.item> <termID> {item.term}");
        gram.NewRule("rule.item", "<rule.item> <promptID> {item.prompt}");
    }

    #endregion
}
