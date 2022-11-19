using Microsoft.VisualStudio.TestTools.UnitTesting;
using PetiteParser.Analyzer;
using PetiteParser.Formatting;
using PetiteParser.Grammar;
using PetiteParser.Parser.States;
using System.Collections.Generic;

namespace TestPetiteParser.Tools;

static public class AnalyzerExt {

    public static void CheckFirsts(this Analyzer analyzer, string item, bool expHasLambda, string expected) {
        HashSet<TokenItem> tokens = new();
        bool hasLambda = analyzer.Firsts(analyzer.Grammar.Item(item), tokens);
        Assert.AreEqual(expHasLambda, hasLambda, "Has Lambda");
        Assert.AreEqual(expected, tokens.Join(" ").Trim());
    }

    public static void CheckClosureLookAheads(this Analyzer analyzer, Rule rule, int index, string parentToken, string expected) {
        List<TokenItem> parentLookahead = new();
        if (!string.IsNullOrEmpty(parentToken)) parentLookahead.Add(new TokenItem(parentToken));

        TokenItem[] lookahead = analyzer.ClosureLookAheads(rule, index, parentLookahead.ToArray());
        Assert.AreEqual(expected, lookahead.Join(" ").Trim());
    }
}
