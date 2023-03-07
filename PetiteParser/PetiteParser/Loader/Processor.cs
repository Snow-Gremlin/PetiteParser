using PetiteParser.Formatting;
using PetiteParser.ParseTree;
using PetiteParser.Tokenizer;
using PetiteParser.Tokenizer.Matcher;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PetiteParser.Loader;

/// <summary>The prompt handlers for processing a loader's result tree.</summary>
static internal class Processor {

    /// <summary>The collection of prompt handles to parse the language file with.</summary>
    static public Dictionary<string, PromptHandle<LoaderArgs>> Handles { get; }

    /// <summary>Prepares the handles used for processing a parse.</summary>
    static Processor() => Handles = new Dictionary<string, PromptHandle<LoaderArgs>>() {
            { "new.def",           newDef },
            { "start.state",       startState },
            { "join.state",        joinState },
            { "join.token",        joinToken },
            { "assign.token",      assignToken },
            { "new.state",         newState },
            { "new.token.state",   newTokenState },
            { "new.token.consume", newTokenConsume },
            { "new.term",          newTerm },
            { "new.token.item",    newTokenItem },
            { "new.prompt",        newPrompt },
            { "match.any",         matchAny },
            { "match.consume",     matchConsume },
            { "match.set",         matchSet },
            { "match.set.not",     matchSetNot },
            { "match.range",       matchRange },
            { "match.range.not",   matchRangeNot },
            { "not.group.start",   notGroupStart },
            { "not.group.end",     notGroupEnd },
            { "add.replace.text",  addReplaceText },
            { "replace.token",     replaceToken },
            { "feature.mode",      featureMode },
            { "feature.flag",      featureFlag },
            { "feature.value",     featureValue },
            { "start.term",        startTerm },
            { "start.rule",        startRule },
            { "item.token",        itemToken },
            { "item.term",         itemTerm },
            { "item.prompt",       itemPrompt },
            { "set.error",         setError }
        };

    /// <summary>A prompt handle for starting a new definition block.</summary>
    /// <param name="args">The arguments for handling the prompt.</param>
    static private void newDef(LoaderArgs args) => args.Clear();

    /// <summary>A prompt handle for setting the starting state of the tokenizer.</summary>
    /// <param name="args">The arguments for handling the prompt.</param>
    static private void startState(LoaderArgs args) {
        State start = args.CurState ??
            throw new LoaderException("Expected a current state when setting the start state.");
        args.Tokenizer.Start(start.Name);
    }

    /// <summary>A prompt handle for joining two states with the defined matcher.</summary>
    /// <param name="args">The arguments for handling the prompt.</param>
    static private void joinState(LoaderArgs args) {
        State start = args.PrevState ??
            throw new LoaderException("Expected a previous state when joining to a state.");

        State end = args.CurState ??
            throw new LoaderException("Expected a current state when joining to a state.");

        Transition trans = start.Join(end.Name, args.CurTransConsume);
        trans.Matchers.AddRange(args.CurTransGroups[0].Matchers);
        args.CurTransGroups.Clear();
        args.CurTransConsume = false;
    }

    /// <summary>A prompt handle for joining a state to a token with the defined matcher.</summary>
    /// <param name="args">The arguments for handling the prompt.</param>
    static private void joinToken(LoaderArgs args) {
        State start = args.CurState ??
            throw new LoaderException("Expected a current state when joining to a token.");

        TokenState end = args.TokenStates[^1];
        Transition trans = start.Join(end.Name, args.CurTransConsume);
        trans.Matchers.AddRange(args.CurTransGroups[0].Matchers);
        State endState = args.Tokenizer.State(end.Name);
        endState.SetToken(end.Name);
        args.CurTransGroups.Clear();
        args.CurTransConsume = false;
        // Put the accept state of the token onto the states stack.
        args.PushState(endState);
    }

    /// <summary>A prompt handle for assigning a token to a state.</summary>
    /// <param name="args">The arguments for handling the prompt.</param>
    static private void assignToken(LoaderArgs args) {
        State start = args.CurState ??
            throw new LoaderException("Expected a current state when assigning a token.");

        TokenState end = args.TokenStates[^1];
        start.SetToken(end.Name);
    }

    /// <summary>A prompt handle for adding a new state to the tokenizer.</summary>
    /// <param name="args">The arguments for handling the prompt.</param>
    static private void newState(LoaderArgs args) =>
        args.PushState(args.Tokenizer.State(args.LastText));

    /// <summary>A prompt handle for adding a new token to the tokenizer.</summary>
    /// <param name="args">The arguments for handling the prompt.</param>
    static private void newTokenState(LoaderArgs args) =>
        args.TokenStates.Add(args.Tokenizer.Token(args.LastText));

    /// <summary>
    /// A prompt handle for adding a new token to the tokenizer
    /// and setting it to consume that token.
    /// </summary>
    /// <param name="args">The arguments for handling the prompt.</param>
    static private void newTokenConsume(LoaderArgs args) =>
        args.TokenStates.Add(args.Tokenizer.Token(args.LastText).Consume());

    /// <summary>A prompt handle for adding a new term to the grammar.</summary>
    /// <param name="args">The arguments for handling the prompt.</param>
    static private void newTerm(LoaderArgs args) =>
        args.Terms.Push(args.Grammar.Term(args.LastText));

    /// <summary>A prompt handle for adding a new token to the grammar.</summary>
    /// <param name="args">The arguments for handling the prompt.</param>
    static private void newTokenItem(LoaderArgs args) =>
        args.TokenItems.Push(args.Grammar.Token(args.LastText));

    /// <summary>A prompt handle for adding a new prompt to the grammar.</summary>
    /// <param name="args">The arguments for handling the prompt.</param>
    static private void newPrompt(LoaderArgs args) =>
        args.Prompts.Push(args.Grammar.Prompt(args.LastText));

    /// <summary>A prompt handle for setting the currently building matcher to match any.</summary>
    /// <param name="args">The arguments for handling the prompt.</param>
    static private void matchAny(LoaderArgs args) =>
        args.TopTransGroup.AddAll();

    /// <summary>A prompt handle for setting the currently building matcher to be consumed.</summary>
    /// <param name="args">The arguments for handling the prompt.</param>
    static private void matchConsume(LoaderArgs args) =>
        args.CurTransConsume = true;

    /// <summary>A prompt handle for setting the currently building matcher to match to a character set.</summary>
    /// <param name="args">The arguments for handling the prompt.</param>
    static private void matchSet(LoaderArgs args) {
        Token chars = args.Recent() ??
            throw new LoaderException("Expected a recent token for characters in match set.");

        Rune[] match = Text.Unescape(chars.Text).EnumerateRunes().ToArray();
        if (match.Length <= 0)
            throw new LoaderException("Must have at least one char, " + chars + ", in a char set.");

        if (match.Length == 1)
            args.TopTransGroup.AddSingle(match[0]);
        else args.TopTransGroup.AddSet(match);
    }

    /// <summary>A prompt handle for setting the currently building matcher to not match to a character set.</summary>
    /// <param name="args">The arguments for handling the prompt.</param>
    static private void matchSetNot(LoaderArgs args) {
        notGroupStart(args);
        matchSet(args);
        notGroupEnd(args);
    }

    /// <summary>A prompt handle for setting the currently building matcher to match to a character range.</summary>
    /// <param name="args">The arguments for handling the prompt.</param>
    static private void matchRange(LoaderArgs args) {
        Token lowChar  = args.Recent(2) ??
            throw new LoaderException("Expected a recent token for the lower character in match range.");

        Token highChar = args.Recent() ??
            throw new LoaderException("Expected a recent token for the higher character in match range.");

        Rune[] lowText  = Text.Unescape(lowChar.Text).EnumerateRunes().ToArray();
        if (lowText.Length != 1)
            throw new LoaderException("May only have one character for the low char, " + lowChar + ", of a range.");

        Rune[] highText = Text.Unescape(highChar.Text).EnumerateRunes().ToArray();
        if (highText.Length != 1)
            throw new LoaderException("May only have one character for the high char, " + highChar + ", of a range.");

        args.TopTransGroup.AddRange(lowText[0], highText[0]);
    }

    /// <summary>A prompt handle for setting the currently building matcher to not match to a character range.</summary>
    /// <param name="args">The arguments for handling the prompt.</param>
    static private void matchRangeNot(LoaderArgs args) {
        notGroupStart(args);
        matchRange(args);
        notGroupEnd(args);
    }

    /// <summary>A prompt handle for starting a not group of matchers.</summary>
    /// <param name="args">The arguments for handling the prompt.</param>
    static private void notGroupStart(LoaderArgs args) {
        NotGroup notGroup = new();
        args.TopTransGroup.Add(notGroup);
        args.CurTransGroups.Add(notGroup);
    }

    /// <summary>A prompt handle for ending a not group of matchers.</summary>
    /// <param name="args">The arguments for handling the prompt.</param>
    static private void notGroupEnd(LoaderArgs args) =>
        args.CurTransGroups.RemoveAt(args.CurTransGroups.Count-1);

    /// <summary>A prompt handle for adding a new replacement string to the loader.</summary>
    /// <param name="args">The arguments for handling the prompt.</param>
    static private void addReplaceText(LoaderArgs args) =>
        args.ReplaceText.Add(Text.Unescape(args.LastText));

    /// <summary>
    /// A prompt handle for setting a set of replacements between two
    /// tokens with a previously set replacement string set.
    /// </summary>
    /// <param name="args">The arguments for handling the prompt.</param>
    static private void replaceToken(LoaderArgs args) {
        TokenState start = args.TokenStates[^2];
        TokenState end   = args.TokenStates[^1];
        start.Replace(end.Name, args.ReplaceText);
        args.ReplaceText.Clear();
        // remove end while keeping the start.
        args.TokenStates.RemoveAt(args.TokenStates.Count-1);
    }

    /// <summary>A prompt handle for setting the feature mode.</summary>
    /// <param name="args">The arguments for handling the prompt.</param>
    static private void featureMode(LoaderArgs args) =>
        args.FeatureFlagMode = args.LastText;

    /// <summary>A prompt handle for setting a feature with the current mode and key without a value.</summary>
    /// <param name="args">The arguments for handling the prompt.</param>
    static private void featureFlag(LoaderArgs args) {
        switch (args.FeatureFlagMode) {
            case "enable":  args.EnableFeatureValue(args.LastText, true);  return;
            case "disable": args.EnableFeatureValue(args.LastText, false); return;
            default: throw new LoaderException("May not change a feature flag with \"" + args.FeatureFlagMode + "\".");
        }
    }

    /// <summary>A prompt handle for setting a feature with the current mode, key, and value.</summary>
    /// <param name="args">The arguments for handling the prompt.</param>
    static private void featureValue(LoaderArgs args) {
        Token value = args.Recent(1) ??
            throw new LoaderException("Expected a recent token for the feature value.");

        switch (args.FeatureFlagMode) {
            case "set": args.SetFeatureValue(value.Text, args.LastText); return;
            default: throw new LoaderException("May not change a feature with \"" + args.FeatureFlagMode + "\".");
        }
    }

    /// <summary>A prompt handle for starting a grammar definition of a term.</summary>
    /// <param name="args">The arguments for handling the prompt.</param>
    static private void startTerm(LoaderArgs args) =>
        args.Grammar.Start(args.Terms.Peek().Name);

    /// <summary>A prompt handle for starting defining a rule for the current term.</summary>
    /// <param name="args">The arguments for handling the prompt.</param>
    static private void startRule(LoaderArgs args) =>
        args.CurRule = args.Terms.Peek().NewRule();

    /// <summary>A prompt handle for adding a token to the current rule being built.</summary>
    /// <param name="args">The arguments for handling the prompt.</param>
    static private void itemToken(LoaderArgs args) =>
        args.CurRule?.AddToken(args.TokenItems.Pop().Name);

    /// <summary>A prompt handle for adding a term to the current rule being built.</summary>
    /// <param name="args">The arguments for handling the prompt.</param>
    static private void itemTerm(LoaderArgs args) =>
        args.CurRule?.AddTerm(args.Terms.Pop().Name);

    /// <summary>A prompt handle for adding a prompt to the current rule being built.</summary>
    /// <param name="args">The arguments for handling the prompt.</param>
    static private void itemPrompt(LoaderArgs args) =>
        args.CurRule?.AddPrompt(args.Prompts.Pop().Name);

    /// <summary>Sets the error token to the tokenizer and parser to use for bad input.</summary>
    /// <param name="args">The arguments for handling the prompt.</param>
    static private void setError(LoaderArgs args) {
        string errToken = args.TokenItems.Pop().Name;
        args.Tokenizer.ErrorToken(errToken);
        args.Grammar.Error(errToken);
    }
}
