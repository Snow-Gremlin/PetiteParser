using Microsoft.VisualStudio.TestTools.UnitTesting;
using PetiteParser.Tokenizer;
using System;
using System.Collections.Generic;
using System.Text;

namespace TestPetiteParser {
    [TestClass]
    public class TokenizerUnitTests {

        static private void checkTok(Tokenizer tok, string input, params string[] expected) =>
            checkTok(tok.Tokenize(input), expected);

        static private void checkTok(IEnumerable<Token> tokens, params string[] expected) {
            StringBuilder resultBuf = new();
            foreach (Token token in tokens)
                resultBuf.AppendLine(token.ToString());
            string exp = string.Join(Environment.NewLine, expected);
            string result = resultBuf.ToString().Trim();
            Assert.AreEqual(exp, result);
        }

        static private Tokenizer simpleMathTokenizer() {
            Tokenizer tok = new();
            tok.Start("start");
            tok.Join("start", "id").AddRange("a", "z");
            tok.Join("id", "id").AddRange("a", "z");
            tok.Join("start", "add").AddSet("+");
            tok.Join("start", "mul").AddSet("*");
            tok.Join("start", "open").AddSet("(");
            tok.Join("start", "close").AddSet(")");
            tok.Join("start", "space").AddSet(" ");
            tok.Join("space", "space").AddSet(" ");
            tok.SetToken("add", "[add]");
            tok.SetToken("mul", "[mul]");
            tok.SetToken("open", "[open]");
            tok.SetToken("close", "[close]");
            tok.SetToken("id", "[id]");
            tok.SetToken("space", "[space]").Consume();
            return tok;
        }

        [TestMethod]
        public void Tokenizer1() {
            Tokenizer tok = simpleMathTokenizer();
            checkTok(tok, "hello world",
               "[id]:(Unnamed:1, 1, 1):\"hello\"",
               "[id]:(Unnamed:1, 7, 7):\"world\"");
        }

        [TestMethod]
        public void Tokenizer2() {
            Tokenizer tok = simpleMathTokenizer();
            checkTok(tok, "a + b * c",
               "[id]:(Unnamed:1, 1, 1):\"a\"",
               "[add]:(Unnamed:1, 3, 3):\"+\"",
               "[id]:(Unnamed:1, 5, 5):\"b\"",
               "[mul]:(Unnamed:1, 7, 7):\"*\"",
               "[id]:(Unnamed:1, 9, 9):\"c\"");
        }

        [TestMethod]
        public void Tokenizer3() {
            Tokenizer tok = simpleMathTokenizer();
            checkTok(tok, "(a + b)",
               "[open]:(Unnamed:1, 1, 1):\"(\"",
               "[id]:(Unnamed:1, 2, 2):\"a\"",
               "[add]:(Unnamed:1, 4, 4):\"+\"",
               "[id]:(Unnamed:1, 6, 6):\"b\"",
               "[close]:(Unnamed:1, 7, 7):\")\"");
        }

        [TestMethod]
        public void Tokenizer4() {
            Tokenizer tok = simpleMathTokenizer();
            checkTok(tok, "a + (b * c) + d",
               "[id]:(Unnamed:1, 1, 1):\"a\"",
               "[add]:(Unnamed:1, 3, 3):\"+\"",
               "[open]:(Unnamed:1, 5, 5):\"(\"",
               "[id]:(Unnamed:1, 6, 6):\"b\"",
               "[mul]:(Unnamed:1, 8, 8):\"*\"",
               "[id]:(Unnamed:1, 10, 10):\"c\"",
               "[close]:(Unnamed:1, 11, 11):\")\"",
               "[add]:(Unnamed:1, 13, 13):\"+\"",
               "[id]:(Unnamed:1, 15, 15):\"d\"");
        }

        [TestMethod]
        public void Tokenizer5() {
            Tokenizer tok = new();
            tok.Start("start");
            //         .--a--(a1)--b--(b1)[ab]--c--(c2)--d--(d2)--f--(f1)[abcdf]
            // start--{---c--(c1)--d--(d1)[cd]
            //         '--e--(e1)[e]
            tok.Join("start", "(a1)").AddSet("a");
            tok.Join("(a1)", "(b1)").AddSet("b");
            tok.Join("(b1)", "(c2)").AddSet("c");
            tok.Join("(c2)", "(d2)").AddSet("d");
            tok.Join("(d2)", "(f1)").AddSet("f");
            tok.Join("start", "(c1)").AddSet("c");
            tok.Join("(c1)", "(d1)").AddSet("d");
            tok.Join("start", "(e1)").AddSet("e");
            tok.SetToken("(b1)", "[ab]");
            tok.SetToken("(d1)", "[cd]");
            tok.SetToken("(f1)", "[abcdf]");
            tok.SetToken("(e1)", "[e]");

            checkTok(tok, "abcde",
               "[ab]:(Unnamed:1, 1, 1):\"ab\"",
               "[cd]:(Unnamed:1, 3, 3):\"cd\"",
               "[e]:(Unnamed:1, 5, 5):\"e\"");
        }

        /*
        // Example of whole multiple inputs can be combined into one tokenization.
        private IEnumerable<string> multipleInput(Tokenizer tok) {
            tok.InputName = "First";
            yield return "a\naa\naaa\n";
            tok.InputName = "Not Used";
            tok.InputName = "Second";
            yield return "aa\naaa\na\n";
            tok.InputName = "Third";
            yield return "aaa\na\naa\n";
        }

        [TestMethod]
        public void Tokenizer6() {
            Tokenizer tok = new();
            tok.Start("start");
            tok.Join("start", "a").AddSet("a");
            tok.Join("a", "a").AddSet("a");
            tok.Join("start", "ws").AddSet(" \n");
            tok.Join("ws", "ws").AddSet(" \n");
            tok.SetToken("a", "[a]");
            tok.SetToken("ws", "ws").Consume();

            checkTok(tok.Tokenize(multipleInput(tok)), 
                "[a]:(First:1, 0, 0):\"a\"",
                "[a]:(First:2, 2, 3):\"aa\"",
                "[a]:(First:3, 3, 7):\"aaa\"",
                "[a]:(Second:1, 1, 1):\"aa\"",
                "[a]:(Second:2, 3, 5):\"aaa\"",
                "[a]:(Second:3, 1, 7):\"a\"",
                "[a]:(Third:1, 2, 2):\"aaa\"",
                "[a]:(Third:2, 1, 4):\"a\"",
                "[a]:(Third:3, 2, 7):\"aa\"");
        }
        */
    }
}
