using PetiteParser.Formatting;
using PetiteParser.Grammar;
using PetiteParser.Grammar.Analyzer;
using PetiteParser.Logger;
using PetiteParser.Misc;
using PetiteParser.Parser;
using PetiteParser.Parser.States;
using PetiteParser.Tokenizer;
using System;
using TestPetiteParser.Tools;

namespace TestPetiteParser.GrammarTests;

static internal class GrammarExt {
    #region Grammar

    /// <summary>Checks the grammar's string.</summary>
    static public void Check(this Grammar grammar, params string[] expected) =>
        TestTools.AreEqual(expected.JoinLines(), grammar.ToString().Trim());

    /// <summary>Checks that an expected error from the parser builder.</summary>
    static public void CheckParserBuildError(this Grammar grammar, Tokenizer tokenizer, params string[] expected) =>
        TestTools.ThrowsException(() => _ = new Parser(grammar, tokenizer), expected);
    
    /// <summary>Checks that no conflicts (or any other exceptions) occur when creating states for this grammar.</summary>
    /// <remarks>This will throw an exception if one occurs as the way to indicate an unexpected exception occurred.</remarks>
    /// <param name="ignoreConflicts">This indicates that as many conflicts in state actions as possible should be ignored.</param>
    static public void CheckNoStateConflicts(this Grammar grammar, bool ignoreConflicts = true) =>
        new ParserStates().DetermineStates(grammar, new Writer(), ignoreConflicts);

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
    static public void CheckFindFirstLeftRecursion(this Grammar grammar, params string[] expected) {
        Analyzer analyzer = new(grammar);
        Console.WriteLine(grammar);
        Console.WriteLine(analyzer.ToString());
        Console.WriteLine();
        TestTools.AreEqual(expected.JoinLines(), analyzer.FindFirstLeftRecursion().ToNames().JoinLines());
    }

    /*
    // TODO: Update or remove
    /// <summary>Checks the follows found by the analyzer for the fragment of the given rule and offset index.</summary>
    public static void CheckFollows(this Analyzer analyzer, Rule rule, int index,
        bool expectedEndReached, string expectedLookaheads, string expectedRuleString) {
        HashSet<TokenItem> lookahead = new();
        bool endReached = analyzer.Follows(rule, index, lookahead);
        Assert.AreEqual(endReached, expectedEndReached);
        Assert.AreEqual(expectedLookaheads, lookahead.Join(" ").Trim());
        Assert.AreEqual(rule.ToString(index), expectedRuleString);
    }
    */

    #endregion
}
