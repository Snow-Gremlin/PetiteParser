using Microsoft.VisualStudio.TestTools.UnitTesting;
using PetiteParser.Grammar;
using PetiteParser.Misc;
using TestPetiteParser.Tools;

namespace TestPetiteParser.ParserTests {

    [TestClass]
    public class GrammarUnitTests {

        [TestMethod]
        public void Grammar1() {
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
               "<defSet> → ",
               "<def> → <stateDef> <defBody>",
               "<stateDef> → [closeAngle]",
               "<stateDef> → ",
               "<defBody> → <stateOrTokenID>",
               "<defBody> → <defBody> [colon] [arrow] <stateOrTokenID>",
               "<stateOrTokenID> → <stateID>",
               "<stateOrTokenID> → <tokenID>",
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
        public void Grammar2() {
            Grammar gram = new();
            Rule rule0 = gram.NewRule("E");
            Rule rule1 = gram.NewRule("E").AddTerm("E").AddToken("+").AddTerm("E");
            Rule rule2 = gram.NewRule("E").AddTerm("E").AddToken("+").AddTerm("E").AddPrompt("add");
            Rule rule3 = gram.NewRule("E").AddTerm("E").AddToken("+").AddPrompt("add").AddTerm("E");
            Rule rule4 = gram.NewRule("E").AddPrompt("add").AddTerm("E").AddToken("+").AddTerm("E");

            rule0.CheckString(-1, "<E> → ");
            rule0.CheckString( 0, "<E> → •");
            rule0.CheckString( 1, "<E> → ");

            rule1.CheckString(-1, "<E> → <E> [+] <E>");
            rule1.CheckString( 0, "<E> → • <E> [+] <E>");
            rule1.CheckString( 1, "<E> → <E> • [+] <E>");
            rule1.CheckString( 2, "<E> → <E> [+] • <E>");
            rule1.CheckString( 3, "<E> → <E> [+] <E> •");
            rule1.CheckString( 4, "<E> → <E> [+] <E>");

            rule2.CheckString(-1, "<E> → <E> [+] <E> {add}");
            rule2.CheckString( 0, "<E> → • <E> [+] <E> {add}");
            rule2.CheckString( 1, "<E> → <E> • [+] <E> {add}");
            rule2.CheckString( 2, "<E> → <E> [+] • <E> {add}");
            rule2.CheckString( 3, "<E> → <E> [+] <E> • {add}");
            rule2.CheckString( 4, "<E> → <E> [+] <E> {add}");

            rule3.CheckString(-1, "<E> → <E> [+] {add} <E>");
            rule3.CheckString( 0, "<E> → • <E> [+] {add} <E>");
            rule3.CheckString( 1, "<E> → <E> • [+] {add} <E>");
            rule3.CheckString( 2, "<E> → <E> [+] • {add} <E>");
            rule3.CheckString( 3, "<E> → <E> [+] {add} <E> •");
            rule3.CheckString( 4, "<E> → <E> [+] {add} <E>");

            rule4.CheckString(-1, "<E> → {add} <E> [+] <E>");
            rule4.CheckString( 0, "<E> → • {add} <E> [+] <E>");
            rule4.CheckString( 1, "<E> → {add} <E> • [+] <E>");
            rule4.CheckString( 2, "<E> → {add} <E> [+] • <E>");
            rule4.CheckString( 3, "<E> → {add} <E> [+] <E> •");
            rule4.CheckString( 4, "<E> → {add} <E> [+] <E>");
        }

        [TestMethod]
        public void Grammar3() {
            Grammar gram = new();
            gram.NewRule("C");
            gram.NewRule("C").AddTerm("X").AddTerm("C");
            gram.NewRule("X").AddToken("A");
            gram.NewRule("X").AddToken("B");

            gram.Check(
                "> <C>",
                "<C> → ",
                "<C> → <X> <C>",
                "<X> → [A]",
                "<X> → [B]");

            gram.CheckFirstSets(
                "C → [A, B] λ",
                "X → [A, B]");
        }
    }
}
