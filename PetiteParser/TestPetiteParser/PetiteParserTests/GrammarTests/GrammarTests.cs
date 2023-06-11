using Microsoft.VisualStudio.TestTools.UnitTesting;
using PetiteParser.Formatting;
using PetiteParser.Grammar;
using System.Collections.Generic;
using System.Linq;
using TestPetiteParser.Tools;

namespace TestPetiteParser.PetiteParserTests.GrammarTests;

[TestClass]
sealed public class GrammarTests {

    [TestMethod]
    public void AddingTermAndAutoSetStart() {
        Grammar gram = new();
        Assert.IsNull(gram.StartTerm);
        Assert.AreEqual("", gram.Terms.Join(", "));

        Term term1 = gram.Term("Panda");
        Assert.IsNotNull(term1);
        Assert.AreEqual("Panda", term1.Name);

        Assert.IsNotNull(gram.StartTerm);
        Assert.AreSame(term1, gram.StartTerm);
        Assert.AreEqual("<Panda>", gram.Terms.Join(", "));
        
        Term term2 = gram.Term("Red");
        Assert.IsNotNull(term2);
        Assert.AreEqual("Red", term2.Name);
        
        Assert.AreSame(term1, gram.StartTerm);
        Assert.AreEqual("<Panda>, <Red>", gram.Terms.Join(", "));

        Term term3 = gram.Start("Red");
        Assert.AreSame(term2, term3);
        Assert.AreSame(term2, gram.StartTerm);
        Assert.AreEqual("<Panda>, <Red>", gram.Terms.Join(", "));

        Term term4 = gram.Term("Red");
        Assert.AreSame(term2, term4);
        Assert.AreEqual("<Panda>, <Red>", gram.Terms.Join(", "));

        Assert.AreEqual(0, term1.Rules.Count);
        Assert.AreEqual(0, term2.Rules.Count);
    }
    
    [TestMethod]
    public void AddingGeneratedTerms() {
        Grammar gram = new();
        Assert.AreEqual("<gen'0>", gram.AddGeneratedTerm("").ToString());
        Assert.AreEqual("<gen'1>", gram.AddGeneratedTerm().ToString());
        Assert.AreEqual("<gen'2>", gram.AddGeneratedTerm().ToString());
        Assert.AreEqual("<puppy'0>", gram.AddGeneratedTerm("puppy").ToString());
        Assert.AreEqual("<kitty'0>", gram.AddGeneratedTerm("kitty'6").ToString(),
            "\"kitty'6\" isn't in the grammar so it isn't used to determine the maximum suffix.");
        Assert.AreEqual("<gen'3>", gram.AddGeneratedTerm("gen'0").ToString());
        Assert.AreEqual("<puppy'1>", gram.AddGeneratedTerm("puppy'0").ToString());
        Assert.AreEqual("<gen'4>", gram.AddGeneratedTerm("gen'0").ToString());
    }

    [TestMethod]
    public void MoreGeneratedTerms() {
        Grammar gram = new();
        gram.Term("A");
        gram.AddGeneratedTerm("A");
        gram.AddGeneratedTerm("A'0");
        // 42 is not used in max value because the max values only comes from existing terms.
        gram.AddGeneratedTerm("A'42");
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
    
    [TestMethod]
    public void AddingItems() {
        Grammar gram = new();
        Assert.AreEqual("<TermA>", gram.Term("TermA").ToString());
        Assert.AreEqual("<TermB>", gram.Item("<TermB>").ToString());
        Assert.AreEqual("<TermC> → λ", gram.NewRule("TermC").ToString());
        Assert.AreEqual("[TokenA]", gram.Token("TokenA").ToString());
        Assert.AreEqual("[TokenB]", gram.Item("[TokenB]").ToString());
        Assert.AreEqual("{PromptA}", gram.Prompt("PromptA").ToString());
        Assert.AreEqual("{PromptB}", gram.Item("{PromptB}").ToString());

        Assert.AreEqual("<TermA>, <TermB>, <TermC>", gram.Terms.Join(", "));
        Assert.AreEqual("[TokenA], [TokenB]", gram.Tokens.Join(", "));
        Assert.AreEqual("{PromptA}, {PromptB}", gram.Prompts.Join(", "));
    }
    
    [TestMethod]
    public void AddingRules() {
        Grammar gram = new();
        Rule rule = gram.NewRule("A", "<B> [C] {D}");
        Assert.AreEqual("<A> → <B> [C] {D}", rule.ToString());
        rule.AddTerm("E").AddToken("F").AddPrompt("G");

        Assert.AreEqual("<A>, <B>, <E>", gram.Terms.Join(", "));
        Assert.AreEqual("[C], [F]", gram.Tokens.Join(", "));
        Assert.AreEqual("{D}, {G}", gram.Prompts.Join(", "));
        Assert.AreEqual("<A> → <B> [C] {D} <E> [F] {G}", rule.ToString());
    }

    [TestMethod]
    public void IsDirectlyRecursive() {
        Grammar gram = new();
        Term t1 = gram.Term("A");
        Assert.IsFalse(t1.IsDirectlyRecursive);

        Rule r1 = gram.NewRule("A", "<B> <C> <D>");
        Assert.IsFalse(r1.IsDirectlyRecursive);
        Assert.IsFalse(t1.IsDirectlyRecursive);
        
        Rule r2 = gram.NewRule("B", "<A>");
        Assert.IsFalse(r2.IsDirectlyRecursive);
        Assert.IsFalse(r1.IsDirectlyRecursive);
        Assert.IsFalse(t1.IsDirectlyRecursive);

        Rule r3 = gram.NewRule("A", "<B> <A>");
        Assert.IsTrue(r3.IsDirectlyRecursive);
        Assert.IsFalse(r1.IsDirectlyRecursive);
        Assert.IsTrue(t1.IsDirectlyRecursive);
    }

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
            "┌────────────────┬────────────────────────────────────┬───┐",
            "│ Term           │ Firsts                             │ λ │",
            "├────────────────┼────────────────────────────────────┼───┤",
            "│ def            │ closeAngle, openBracket, openParen │   │",
            "│ defBody        │ openBracket, openParen             │   │",
            "│ defSet         │ closeAngle, openBracket, openParen │ x │",
            "│ stateDef       │ closeAngle                         │ x │",
            "│ stateID        │ openParen                          │   │",
            "│ stateOrTokenID │ openBracket, openParen             │   │",
            "│ tokenID        │ openBracket                        │   │",
            "└────────────────┴────────────────────────────────────┴───┘");
    }

    [TestMethod]
    public void BasicCreation() {
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
            "┌──────┬────────┬───┐",
            "│ Term │ Firsts │ λ │",
            "├──────┼────────┼───┤",
            "│ C    │ A, B   │ x │",
            "│ X    │ A, B   │   │",
            "└──────┴────────┴───┘");
    }

    [TestMethod]
    public void SimplifiedCreation() {
        Grammar gram = new();
        gram.NewRule("C");
        gram.NewRule("C", "<X><C>");
        gram.NewRule("X", "[A]");
        gram.NewRule("X", "[B]{P}");

        gram.Check(
            "> <C>",
            "<C> → λ",
            "   | <X> <C>",
            "<X> → [A]",
            "   | [B] {P}");
    }
}
