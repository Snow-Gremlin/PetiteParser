using Microsoft.VisualStudio.TestTools.UnitTesting;
using PetiteParser.Tokenizer;
using System;
using System.Text;

namespace TestPetiteParser {
    [TestClass]
    public class TokenizerUnitTests {

        static private void checkTok(Tokenizer tok, string input, params string[] expected) {
            StringBuilder resultBuf = new();
            foreach (Token token in tok.Tokenize(input))
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
               "[id]:5:\"hello\"",
               "[id]:11:\"world\"");
        }

        [TestMethod]
        public void Tokenizer2() {
            Tokenizer tok = simpleMathTokenizer();
            checkTok(tok, "a + b * c",
               "[id]:1:\"a\"",
               "[add]:3:\"+\"",
               "[id]:5:\"b\"",
               "[mul]:7:\"*\"",
               "[id]:9:\"c\"");
        }

        [TestMethod]
        public void Tokenizer3() {
            Tokenizer tok = simpleMathTokenizer();
            checkTok(tok, "(a + b)",
               "[open]:1:\"(\"",
               "[id]:2:\"a\"",
               "[add]:4:\"+\"",
               "[id]:6:\"b\"",
               "[close]:7:\")\"");
        }

        [TestMethod]
        public void Tokenizer4() {
            Tokenizer tok = simpleMathTokenizer();
            checkTok(tok, "a + (b * c) + d",
               "[id]:1:\"a\"",
               "[add]:3:\"+\"",
               "[open]:5:\"(\"",
               "[id]:6:\"b\"",
               "[mul]:8:\"*\"",
               "[id]:10:\"c\"",
               "[close]:11:\")\"",
               "[add]:13:\"+\"",
               "[id]:15:\"d\"");
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
               "[ab]:2:\"ab\"",
               "[cd]:4:\"cd\"",
               "[e]:5:\"e\"");
        }
    }
}
