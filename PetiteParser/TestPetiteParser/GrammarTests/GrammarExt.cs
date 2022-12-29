using Microsoft.VisualStudio.TestTools.UnitTesting;
using PetiteParser.Formatting;
using PetiteParser.Grammar;
using PetiteParser.Grammar.Analyzer;
using PetiteParser.Logger;
using PetiteParser.Misc;
using PetiteParser.Parser;
using PetiteParser.Parser.States;
using PetiteParser.Tokenizer;
using System.Collections.Generic;
using TestPetiteParser.Tools;

namespace TestPetiteParser.GrammarTests;

static internal class GrammarExt {
    #region Grammar

    /// <summary>Checks the grammar's string.</summary>
    static public void Check(this Grammar grammar, params string[] expected) =>
        TestTools.AreEqual(expected.JoinLines(), grammar.ToString().Trim());

    /// <summary>Checks that an expected error from the parser builder.</summary>
    static public void CheckParserBuildError(this Grammar grammar, Tokenizer tokenizer, params string[] expected) =>
        TestTools.ThrowsException(() => _ = new Parser(grammar, tokenizer, OnConflict.Panic), expected);
    
    /// <summary>Checks that no conflicts (or any other exceptions) occur when creating states for this grammar.</summary>
    /// <remarks>This will throw an exception if one occurs as the way to indicate an unexpected exception occurred.</remarks>
    static public void CheckNoStateConflicts(this Grammar grammar) =>
        new ParserStates().DetermineStates(grammar, OnConflict.Panic, new Writer());

    #endregion
    #region Rule

    /// <summary>Checks if the given rule's string method.</summary>
    /// <param name="rule">The rule to check.</param>
    /// <param name="stepIndex">The index of the current step to show.</param>
    /// <param name="exp">The expected returned string.</param>
    static public void CheckString(this Rule rule, int stepIndex, string exp) =>
        TestTools.AreEqual(exp, rule.ToString(stepIndex));

    #endregion
    #region Analyzer

    /// <summary>Checks the grammar term's first tokens results.</summary>
    static public void CheckFirstSets(this Grammar grammar, params string[] expected) =>
        TestTools.AreEqual(expected.JoinLines(), new Analyzer(grammar).ToString().Trim());

    /// <summary>Checks the grammar's first left recursion is as expected.</summary>
    static public void CheckFindFirstLeftRecursion(this Grammar grammar, params string[] expected) =>
        TestTools.AreEqual(expected.JoinLines(), new Analyzer(grammar).FindFirstLeftRecursion().ToNames().JoinLines());

    /// <summary>Checks the follows found by the analyzer for the fragment of the given rule and offset index.</summary>
    public static void CheckFollows(this Analyzer analyzer, Rule rule, int index, string parentToken, string expected) {
        List<TokenItem> parentLookahead = new();
        if (!string.IsNullOrEmpty(parentToken)) parentLookahead.Add(new TokenItem(parentToken));

        TokenItem[] lookahead = analyzer.Follows(rule, index, parentLookahead.ToArray());
        Assert.AreEqual(expected, lookahead.Join(" ").Trim());
    }

    #endregion
}
