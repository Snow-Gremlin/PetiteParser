using PetiteParser.Analyzer;
using PetiteParser.Formatting;
using PetiteParser.Grammar;
using PetiteParser.Logger;
using PetiteParser.Misc;
using PetiteParser.Normalizer;
using PetiteParser.Parser;
using PetiteParser.Tokenizer;

namespace TestPetiteParser.Tools;

static public class GrammarExt {
    
    /// <summary>Checks the grammar's string.</summary>
    static public void Check(this Grammar grammar, params string[] expected) =>
        TestTools.AreEqual(expected.JoinLines(), grammar.ToString().Trim());

    /// <summary>Checks the grammar term's first tokens results.</summary>
    static public void CheckFirstSets(this Grammar grammar, params string[] expected) =>
        TestTools.AreEqual(expected.JoinLines(), new Analyzer(grammar).ToString().Trim());

    /// <summary>Checks the grammar's first left recursion is as expected.</summary>
    static public void CheckFindFirstLeftRecursion(this Grammar grammar, params string[] expected) =>
        TestTools.AreEqual(expected.JoinLines(), new Analyzer(grammar).FindFirstLeftRecursion().ToNames().JoinLines());

    /// <summary>Checks that an expected error from the parser builder.</summary>
    static public void CheckParserBuildError(this Grammar grammar, Tokenizer tokenizer, params string[] expected) =>
        TestTools.ThrowsException(() => _ = new Parser(grammar, tokenizer, OnConflict.Panic), expected);
    
    /// <summary>Checks if the given rule's string method.</summary>
    /// <param name="rule">The rule to check.</param>
    /// <param name="stepIndex">The index of the current step to show.</param>
    /// <param name="exp">The expected returned string.</param>
    static public void CheckString(this Rule rule, int stepIndex, string exp) =>
        TestTools.AreEqual(exp, rule.ToString(stepIndex));

    /// <summary>Performs a single step of remove left recursion.</summary>
    static public bool StepRemoveLeftRecursion(this Grammar grammar, ILogger? log = null) =>
        Normalizer.Normalize(new Analyzer(grammar), new IPrecept[] { new RemoveLeftRecursion() }, 1, log) > 0;
}
