using Microsoft.VisualStudio.TestTools.UnitTesting;
using PetiteParser.Formatting;
using PetiteParser.Grammar;
using System.Collections.Generic;
using TestPetiteParser.Tools;

namespace TestPetiteParser.GrammarTests;

[TestClass]
sealed public class GrammarTests {

    [TestMethod]
    public void Grammar01BasicCreation() {
        Grammar gram = new();
        gram.Start("defSet");
        gram.NewRule("defSet").AddTerm("defSet").AddTerm("def");
        gram.NewRule("defSet");

        gram.NewRule("def").AddTerm("stateDef").AddTerm("defBody");
        gram.NewRule("stateDef").AddToken("closeAngle");
        gram.NewRule("stateDef");
        gram.NewRule("defBody").AddTerm("stateOrTokenID");
        gram.NewRule("defBody").AddTerm("defBody").AddToken("colon").AddToken("arrow").AddTerm("stateOrTokenID");

        gram.NewRule("stateOrTokenID").AddTerm("stateID");
        gram.NewRule("stateOrTokenID").AddTerm("tokenID");
        gram.NewRule("stateID").AddToken("openParen").AddToken("id").AddToken("closeParen");
        gram.NewRule("tokenID").AddToken("openBracket").AddToken("id").AddToken("closeBracket");

        gram.Check(
            "> <defSet>",
            "<defSet> → <defSet> <def>",
            "   | λ",
            "<def> → <stateDef> <defBody>",
            "<stateDef> → [closeAngle]",
            "   | λ",
            "<defBody> → <stateOrTokenID>",
            "   | <defBody> [colon] [arrow] <stateOrTokenID>",
            "<stateOrTokenID> → <stateID>",
            "   | <tokenID>",
            "<stateID> → [openParen] [id] [closeParen]",
            "<tokenID> → [openBracket] [id] [closeBracket]");

        gram.CheckFirstSets(
            "def            → [closeAngle, openBracket, openParen]",
            "defBody        → [openBracket, openParen]",
            "defSet         → [closeAngle, openBracket, openParen] λ",
            "stateDef       → [closeAngle] λ",
            "stateID        → [openParen]",
            "stateOrTokenID → [openBracket, openParen]",
            "tokenID        → [openBracket]");
    }

    [TestMethod]
    public void Grammar02BasicCreation() {
        Grammar gram = new();
        gram.NewRule("C");
        gram.NewRule("C").AddTerm("X").AddTerm("C");
        gram.NewRule("X").AddToken("A");
        gram.NewRule("X").AddToken("B");

        gram.Check(
            "> <C>",
            "<C> → λ",
            "   | <X> <C>",
            "<X> → [A]",
            "   | [B]");

        gram.CheckFirstSets(
            "C → [A, B] λ",
            "X → [A, B]");
    }

    [TestMethod]
    public void Grammar03GeneratedTerms() {
        Grammar gram = new();
        gram.Term("A");
        gram.AddGeneratedTerm("A");
        gram.AddGeneratedTerm("A'0");
        gram.AddGeneratedTerm("A'42"); // 42 is ignored on purpose and not used in max value
        gram.AddGeneratedTerm("B");
        gram.AddGeneratedTerm("B");
        TestTools.AreEqual(new List<string>() {
            "<A>",
            "<A'0>",
            "<A'1>",
            "<A'2>",
            "<B'0>",
            "<B'1>",
            }.JoinLines(), gram.Terms.JoinLines());
    }
}
