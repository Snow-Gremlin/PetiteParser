using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PetiteParser.Parser;
using PetiteParser.Tokenizer;
using PetiteParser.Loader;

namespace TestPetiteParser {

    [TestClass]
    public class LoaderUnitTests {

        /// <summary>Checks the tokenizer will tokenize the given input.</summary>
        static private void checkTokenizer(Tokenizer tok, string input, params string[] expected) {
            StringBuilder resultBuf = new();
            foreach (Token token in tok.Tokenize(input))
                resultBuf.AppendLine(token.ToString());
            string exp = string.Join(Environment.NewLine, expected);
            string result = resultBuf.ToString().Trim();
            Assert.AreEqual(exp, result);
        }

        /// <summary>Checks the tokenizer will fail with the given input.</summary>
        static private void checkTokenizerError(Tokenizer tok, string input, params string[] expected) {
            StringBuilder resultBuf = new();
            try {
                foreach (Token token in tok.Tokenize(input))
                    resultBuf.AppendLine(token.ToString());
                Assert.Fail("Expected an exception but didn't get one.");
            } catch (Exception ex) {
                resultBuf.AppendLine(ex.Message);
            }
            string exp = string.Join(Environment.NewLine, expected);
            string result = resultBuf.ToString().Trim();
            Assert.AreEqual(exp, result);
        }

        /// <summary>Checks the parser will parse the given input.</summary>
        static private void checkParser(Parser parser, string input, params string[] expected) {
            Result parseResult = parser.Parse(input);
            string exp = string.Join(Environment.NewLine, expected);
            string result = parseResult.ToString();
            Assert.AreEqual(exp, result);
        }

        [TestMethod]
        public void Loader01() {
            Tokenizer tok = Loader.LoadTokenizer(
                "> (Start);");
            checkTokenizer(tok, ""); // No tokens found
            checkTokenizerError(tok, "a",
                "String is not tokenizable [state: Start, index: 1]: \"a\"");
        }

        [TestMethod]
        public void Loader02() {
            Tokenizer tok = Loader.LoadTokenizer(
                "> (Start): 'a' => [Done];");
            checkTokenizer(tok, "a",
                "Done:1:\"a\"");
            checkTokenizer(tok, "aa",
                "Done:1:\"a\"",
                "Done:2:\"a\"");
            checkTokenizerError(tok, "ab",
                "Done:1:\"a\"",
                "String is not tokenizable [state: Start, index: 2]: \"b\"");
        }

        [TestMethod]
        public void Loader03() {
            Tokenizer tok = Loader.LoadTokenizer(
                "> (Start): 'a' => (Start);",
                "(Start) => [Done];");
            checkTokenizer(tok, "a",
                "Done:1:\"a\"");
            checkTokenizer(tok, "aa",
                "Done:2:\"aa\"");
            checkTokenizerError(tok, "aab",
                "Done:2:\"aa\"",
                "String is not tokenizable [state: Start, index: 3]: \"b\"");
            checkTokenizer(tok, "aaa",
                "Done:3:\"aaa\"");
        }

        [TestMethod]
        public void Loader04() {
            Tokenizer tok = Loader.LoadTokenizer(
                "> (Start): 'a' => [Start];");
            checkTokenizer(tok, "a",
                "Start:1:\"a\"");
            checkTokenizer(tok, "aa",
                "Start:2:\"aa\"");
            checkTokenizer(tok, "aaa",
                "Start:3:\"aaa\"");
        }

        [TestMethod]
        public void Loader05() {
            Tokenizer tok = Loader.LoadTokenizer(
                "> (Start);",
                "(Start): 'a' => (Not-Done): 'b' => [Done];",
                "(Not-Done): 'c' => (Not-Done);");
            checkTokenizerError(tok, "a",
                "String is not tokenizable at end of input [state: Not-Done, index: 1]: \"a\"");
            checkTokenizer(tok, "ab",
                "Done:2:\"ab\"");
            checkTokenizer(tok, "acccccb",
                "Done:7:\"acccccb\"");
            checkTokenizer(tok, "abab",
                "Done:2:\"ab\"",
                "Done:4:\"ab\"");
            checkTokenizerError(tok, "acccc",
                "String is not tokenizable at end of input [state: Not-Done, index: 5]: \"acccc\"");
            checkTokenizerError(tok, "acccca",
                "String is not tokenizable [state: Not-Done, index: 6]: \"acccca\"");
        }

        [TestMethod]
        public void Loader06() {
            Tokenizer tok = Loader.LoadTokenizer(
                "> (Start): 'a' => (A1): 'b' => [B1];",
                "(B1): 'c' => [C1];",
                "(B1): 'a' => (A2): 'b' => (B2): 'd' => [D1];");
            checkTokenizer(tok, "abcabcababdababc",
                "C1:3:\"abc\"",
                "C1:6:\"abc\"",
                "D1:11:\"ababd\"",
                "B1:13:\"ab\"",
                "C1:16:\"abc\"");
            checkTokenizer(tok, "abababcabababd",
                "B1:2:\"ab\"",
                "B1:4:\"ab\"",
                "C1:7:\"abc\"",
                "B1:9:\"ab\"",
                "D1:14:\"ababd\"");
        }

        [TestMethod]
        public void Loader07() {
            Tokenizer tok = Loader.LoadTokenizer(
                "> (Start): 'bde' => [Done];");
            checkTokenizer(tok, "bed",
                "Done:1:\"b\"",
                "Done:2:\"e\"",
                "Done:3:\"d\"");
            checkTokenizerError(tok, "c",
                "String is not tokenizable [state: Start, index: 1]: \"c\"");
        }

        [TestMethod]
        public void Loader08() {
            Tokenizer tok = Loader.LoadTokenizer(
                "> (Start): !'bde' => [Done];");
            checkTokenizer(tok, "cat",
                "Done:1:\"c\"",
                "Done:2:\"a\"",
                "Done:3:\"t\"");
            checkTokenizerError(tok, "b",
                "String is not tokenizable [state: Start, index: 1]: \"b\"");
        }

        [TestMethod]
        public void Loader09() {
            Tokenizer tok = Loader.LoadTokenizer(
                "> (Start): 'b'..'e' => [Done];");
            checkTokenizer(tok, "bed",
                "Done:1:\"b\"",
                "Done:2:\"e\"",
                "Done:3:\"d\"");
            checkTokenizerError(tok, "a",
                "String is not tokenizable [state: Start, index: 1]: \"a\"");
            checkTokenizerError(tok, "f",
                "String is not tokenizable [state: Start, index: 1]: \"f\"");
        }

        [TestMethod]
        public void Loader10() {
            Tokenizer tok = Loader.LoadTokenizer(
                "> (Start): !'b'..'e' => [Done];");
            checkTokenizer(tok, "mat",
                "Done:1:\"m\"",
                "Done:2:\"a\"",
                "Done:3:\"t\"");
            checkTokenizerError(tok, "d",
                "String is not tokenizable [state: Start, index: 1]: \"d\"");
        }

        [TestMethod]
        public void Loader11() {
            Tokenizer tok = Loader.LoadTokenizer(
                "> (Start): 'b', 'd', 'e' => [Done];");
            checkTokenizer(tok, "bed",
                "Done:1:\"b\"",
                "Done:2:\"e\"",
                "Done:3:\"d\"");
            checkTokenizerError(tok, "c",
                "String is not tokenizable [state: Start, index: 1]: \"c\"");
        }

        [TestMethod]
        public void Loader12() {
            Tokenizer tok = Loader.LoadTokenizer(
                "> (Start): 'b'..'d', 'g'..'k' => [Done];");
            checkTokenizer(tok, "big",
                "Done:1:\"b\"",
                "Done:2:\"i\"",
                "Done:3:\"g\"");
            checkTokenizerError(tok, "e",
                "String is not tokenizable [state: Start, index: 1]: \"e\"");
        }
    }
}
