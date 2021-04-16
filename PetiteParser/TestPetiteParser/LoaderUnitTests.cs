using Microsoft.VisualStudio.TestTools.UnitTesting;
using PetiteParser.Loader;
using PetiteParser.Parser;
using PetiteParser.Tokenizer;
using System;
using System.Text;

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
                "# Setting just the starting state. This will match nothing.",
                "> (Start);");
            checkTokenizer(tok, ""); // No tokens found
            checkTokenizerError(tok, "a",
                "String is not tokenizable [state: Start, index: 1]: \"a\"");
        }

        [TestMethod]
        public void Loader02() {
            Tokenizer tok = Loader.LoadTokenizer(
                "# Simple matcher to token directly from the start.",
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
                "# Matches any number of 'a's. Has separate token definition.",
                "> (Start): 'a' => (Start);",
                "(Start) => [Done];");
            checkTokenizer(tok, "",
                "Done:0:\"\"");
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
                "# Matches any number of 'a's with the start having a token.",
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
                "# Matches an 'a' followed by a 'b' with any number of 'c's in the middle.",
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
                "# Matchers for 'ab', 'abc', and 'ababd'. This makes the tokenizer have to back up with 'ababc'.",
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
                "# Matches a set of characters.",
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
                "# Matches not any of the characters in the set.",
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
                "# Matches a range.",
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
                "# Matches not a range.",
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
                "# Matches characters by or-ing other matchers.",
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
                "# Matches one range or another.",
                "> (Start): 'b'..'d', 'g'..'k' => [Done];");
            checkTokenizer(tok, "big",
                "Done:1:\"b\"",
                "Done:2:\"i\"",
                "Done:3:\"g\"");
            checkTokenizerError(tok, "e",
                "String is not tokenizable [state: Start, index: 1]: \"e\"");
        }

        [TestMethod]
        public void Loader13() {
            Tokenizer tok = Loader.LoadTokenizer(
                "# Matcher which consumes the 'b' character.",
                "> (Start): 'a' => (Next): ^'b' => (Next): 'c' => [Done];");
            checkTokenizer(tok, "abc",
                "Done:3:\"ac\"");
            checkTokenizer(tok, "abbbcac",
                "Done:5:\"ac\"",
                "Done:7:\"ac\"");
        }

        [TestMethod]
        public void Loader14() {
            Tokenizer tok = Loader.LoadTokenizer(
                "# Combining consumers and not's to make a basic quoted string matcher which drops the quotes.",
                "> (Start): ^'\"' => (String.Part): !'\"' => (String.Part): ^'\"' => [String];");
            checkTokenizer(tok, "\"abc\"",
                "String:5:\"abc\"");
        }

        [TestMethod]
        public void Loader15() {
            Tokenizer tok = Loader.LoadTokenizer(
                "# Same basic quoted string matcher but using the 'any' character matcher after the end quote matcher.",
                "> (Start): ^'\"' => (String.Part);" +
                "(String.Part): ^'\"' => [String];",
                "(String.Part): * => (String.Part);");
            checkTokenizer(tok, "\"abc\"",
                "String:5:\"abc\"");
        }

        [TestMethod]
        public void Loader16() {
            Tokenizer tok = Loader.LoadTokenizer(
                "# This test consumes a token.",
                "> (Start): 'a' => [A];" +
                "(Start): 'b' => ^[B];",
                "(Start): 'c' => [C];");
            checkTokenizer(tok, "abcbbbca",
                "A:1:\"a\"",
                "C:3:\"c\"",
                "C:7:\"c\"",
                "A:8:\"a\"");
        }

        [TestMethod]
        public void Loader17() {
            Tokenizer tok = Loader.LoadTokenizer(
                "# A group of matchers within a not.",
                "> (Start): !('abc', 'ijk', 'xyz') => [Done];");
            checkTokenizer(tok, "defmno",
                "Done:1:\"d\"",
                "Done:2:\"e\"",
                "Done:3:\"f\"",
                "Done:4:\"m\"",
                "Done:5:\"n\"",
                "Done:6:\"o\"");
            checkTokenizerError(tok, "a",
                "String is not tokenizable [state: Start, index: 1]: \"a\"");
            checkTokenizerError(tok, "j",
                "String is not tokenizable [state: Start, index: 1]: \"j\"");
            checkTokenizerError(tok, "z",
                "String is not tokenizable [state: Start, index: 1]: \"z\"");
        }

        [TestMethod]
        public void Loader18() {
            Tokenizer tok = Loader.LoadTokenizer(
                "# Having a consume on a goup. Consume is for the whole group.",
                "> (Start): ^!('abc', 'ijk', 'xyz'), 'j' => [Done];");
            checkTokenizer(tok, "defmno",
                "Done:1:\"\"",
                "Done:2:\"\"",
                "Done:3:\"\"",
                "Done:4:\"\"",
                "Done:5:\"\"",
                "Done:6:\"\"");
            checkTokenizer(tok, "j",
                "Done:1:\"\"");
            checkTokenizerError(tok, "a",
                "String is not tokenizable [state: Start, index: 1]: \"a\"");
            checkTokenizerError(tok, "z",
                "String is not tokenizable [state: Start, index: 1]: \"z\"");
        }

        [TestMethod]
        public void Loader19() {
            Tokenizer tok = Loader.LoadTokenizer(
                "# Having multiple connections between a node and another (in this case, with itself).",
                "> (Start): 'a' => (Part): 'b' => [Done];",
                "(Part): ^'c' => (Part);",
                "(Part): 'd' => (Part);");
            checkTokenizer(tok, "acdcdcdb",
                "Done:8:\"adddb\"");
        }
    }
}
