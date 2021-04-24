using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PetiteParser.Grammar;
using PetiteParser.Tokenizer;

namespace TestPetiteParser {

    [TestClass]
    public class GrammarUnitTests {

        static private void checkGrammar(Grammar grammar, params string[] expected) {
            string exp = string.Join(Environment.NewLine, expected);
            string result = grammar.ToString().Trim();
            Assert.AreEqual(exp, result);
        }

        /// <summary>Checks the grammar term's first tokens results.</summary>
        static private void checkTermFirst(Grammar grammar, string token, params string[] expected) {
            string exp = string.Join(Environment.NewLine, expected);
            IEnumerable<TokenItem> firsts = grammar.Term(token).Firsts;
            string result = string.Join(Environment.NewLine, firsts);
            Assert.AreEqual(exp, result);
        }

        /// <summary>Checks the grammar term's follow tokens results.</summary>
        static private void checkTermFollow(Grammar grammar, string token, params string[] expected) {
            string exp = string.Join(Environment.NewLine, expected);
            IEnumerable<TokenItem> firsts = grammar.Term(token).Follows;
            string result = string.Join(Environment.NewLine, firsts);
            Assert.AreEqual(exp, result);
        }

        /// <summary>Checks if the given rule's string method.</summary>
        static private void checkRuleString(Rule rule, int index, string exp) {
            string result = rule.ToString(index);
            Assert.AreEqual(exp, result);
        }

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

            checkGrammar(gram,
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

            checkTermFirst(gram, "defSet", "[closeAngle]", "[openParen]", "[openBracket]");
            checkTermFollow(gram, "defSet", "[closeAngle]", "[openParen]", "[openBracket]");

            checkTermFirst(gram, "def", "[closeAngle]", "[openParen]", "[openBracket]");
            checkTermFollow(gram, "def", "[closeAngle]", "[openParen]", "[openBracket]");

            checkTermFirst(gram, "stateDef", "[closeAngle]", "[openParen]", "[openBracket]");
            checkTermFollow(gram, "stateDef", "[openParen]", "[openBracket]");

            checkTermFirst(gram, "defBody", "[openParen]", "[openBracket]");
            checkTermFollow(gram, "defBody", "[closeAngle]", "[openParen]", "[openBracket]", "[colon]");

            checkTermFirst(gram, "stateOrTokenID", "[openParen]", "[openBracket]");
            checkTermFollow(gram, "stateOrTokenID", "[closeAngle]", "[openParen]", "[openBracket]", "[colon]");

            checkTermFirst(gram, "stateID", "[openParen]");
            checkTermFollow(gram, "stateID", "[closeAngle]", "[openParen]", "[openBracket]", "[colon]");

            checkTermFirst(gram, "tokenID", "[openBracket]");
            checkTermFollow(gram, "tokenID", "[closeAngle]", "[openParen]", "[openBracket]", "[colon]");
        }

        [TestMethod]
        public void Grammar2() {
            Grammar gram = new();
            Rule rule0 = gram.NewRule("E");
            Rule rule1 = gram.NewRule("E").AddTerm("E").AddToken("+").AddTerm("E");
            Rule rule2 = gram.NewRule("E").AddTerm("E").AddToken("+").AddTerm("E").AddPrompt("add");
            Rule rule3 = gram.NewRule("E").AddTerm("E").AddToken("+").AddPrompt("add").AddTerm("E");
            Rule rule4 = gram.NewRule("E").AddPrompt("add").AddTerm("E").AddToken("+").AddTerm("E");

            checkRuleString(rule0, -1, "<E> → ");
            checkRuleString(rule0, 0, "<E> → •");
            checkRuleString(rule0, 1, "<E> → ");

            checkRuleString(rule1, -1, "<E> → <E> [+] <E>");
            checkRuleString(rule1, 0, "<E> → • <E> [+] <E>");
            checkRuleString(rule1, 1, "<E> → <E> • [+] <E>");
            checkRuleString(rule1, 2, "<E> → <E> [+] • <E>");
            checkRuleString(rule1, 3, "<E> → <E> [+] <E> •");
            checkRuleString(rule1, 4, "<E> → <E> [+] <E>");

            checkRuleString(rule2, -1, "<E> → <E> [+] <E> {add}");
            checkRuleString(rule2, 0, "<E> → • <E> [+] <E> {add}");
            checkRuleString(rule2, 1, "<E> → <E> • [+] <E> {add}");
            checkRuleString(rule2, 2, "<E> → <E> [+] • <E> {add}");
            checkRuleString(rule2, 3, "<E> → <E> [+] <E> • {add}");
            checkRuleString(rule2, 4, "<E> → <E> [+] <E> {add}");

            checkRuleString(rule3, -1, "<E> → <E> [+] {add} <E>");
            checkRuleString(rule3, 0, "<E> → • <E> [+] {add} <E>");
            checkRuleString(rule3, 1, "<E> → <E> • [+] {add} <E>");
            checkRuleString(rule3, 2, "<E> → <E> [+] • {add} <E>");
            checkRuleString(rule3, 3, "<E> → <E> [+] {add} <E> •");
            checkRuleString(rule3, 4, "<E> → <E> [+] {add} <E>");

            checkRuleString(rule4, -1, "<E> → {add} <E> [+] <E>");
            checkRuleString(rule4, 0, "<E> → • {add} <E> [+] <E>");
            checkRuleString(rule4, 1, "<E> → {add} <E> • [+] <E>");
            checkRuleString(rule4, 2, "<E> → {add} <E> [+] • <E>");
            checkRuleString(rule4, 3, "<E> → {add} <E> [+] <E> •");
            checkRuleString(rule4, 4, "<E> → {add} <E> [+] <E>");
        }
    }
}
