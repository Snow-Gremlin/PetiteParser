using Microsoft.VisualStudio.TestTools.UnitTesting;
using PetiteParser.Loader;
using PetiteParser.Tokenizer;
using System;
using System.Text;

namespace TestPetiteParser {

    [TestClass]
    public class TokenizerLoaderUnitTests {

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

        [TestMethod]
        public void TokenizerLoader01() {
            Tokenizer tok = Loader.LoadTokenizer(
                "# Setting just the starting state. This will match nothing.",
                "> (Start);");
            checkTokenizer(tok, ""); // No tokens found
            checkTokenizerError(tok, "a",
                "String is not tokenizable [state: Start, location: (Unnamed:1, 1, 1)]: \"a\"");
        }

        [TestMethod]
        public void TokenizerLoader02() {
            Tokenizer tok = Loader.LoadTokenizer(
                "# Simple matcher to token directly from the start.",
                "> (Start): 'a' => [Done];");
            checkTokenizer(tok, "a",
                "Done:(Unnamed:1, 1, 1):\"a\"");
            checkTokenizer(tok, "aa",
                "Done:(Unnamed:1, 1, 1):\"a\"",
                "Done:(Unnamed:1, 2, 2):\"a\"");
            checkTokenizerError(tok, "ab",
                "Done:(Unnamed:1, 1, 1):\"a\"",
                "String is not tokenizable [state: Start, location: (Unnamed:1, 2, 2)]: \"b\"");
        }

        [TestMethod]
        public void TokenizerLoader03() {
            Tokenizer tok = Loader.LoadTokenizer(
                "# Matches any number of 'a's. Has separate token definition.",
                "> (Start): 'a' => (Start);",
                "(Start) => [Done];");
            checkTokenizer(tok, "",
                "Done:(Unnamed:1, 0, 0):\"\"");
            checkTokenizer(tok, "a",
                "Done:(Unnamed:1, 1, 1):\"a\"");
            checkTokenizer(tok, "aa",
                "Done:(Unnamed:1, 1, 1):\"aa\"");
            checkTokenizerError(tok, "aab",
                "Done:(Unnamed:1, 1, 1):\"aa\"",
                "String is not tokenizable [state: Start, location: (Unnamed:1, 3, 3)]: \"b\"");
            checkTokenizer(tok, "aaa",
                "Done:(Unnamed:1, 1, 1):\"aaa\"");
        }

        [TestMethod]
        public void TokenizerLoader04() {
            Tokenizer tok = Loader.LoadTokenizer(
                "# Matches any number of 'a's with the start having a token.",
                "> (Start): 'a' => [Start];");
            checkTokenizer(tok, "a",
                "Start:(Unnamed:1, 1, 1):\"a\"");
            checkTokenizer(tok, "aa",
                "Start:(Unnamed:1, 1, 1):\"aa\"");
            checkTokenizer(tok, "aaa",
                "Start:(Unnamed:1, 1, 1):\"aaa\"");
        }

        [TestMethod]
        public void TokenizerLoader05() {
            Tokenizer tok = Loader.LoadTokenizer(
                "# Matches an 'a' followed by a 'b' with any number of 'c's in the middle.",
                "> (Start);",
                "(Start): 'a' => (Not-Done): 'b' => [Done];",
                "(Not-Done): 'c' => (Not-Done);");
            checkTokenizerError(tok, "a",
                "String is not tokenizable at end of input [state: Not-Done, location: (Unnamed:1, 1, 1)]: \"a\"");
            checkTokenizer(tok, "ab",
                "Done:(Unnamed:1, 1, 1):\"ab\"");
            checkTokenizer(tok, "acccccb",
                "Done:(Unnamed:1, 1, 1):\"acccccb\"");
            checkTokenizer(tok, "abab",
                "Done:(Unnamed:1, 1, 1):\"ab\"",
                "Done:(Unnamed:1, 3, 3):\"ab\"");
            checkTokenizerError(tok, "acccc",
                "String is not tokenizable at end of input [state: Not-Done, location: (Unnamed:1, 5, 5)]: \"acccc\"");
            checkTokenizerError(tok, "acccca",
                "String is not tokenizable [state: Not-Done, location: (Unnamed:1, 6, 6)]: \"acccca\"");
        }

        [TestMethod]
        public void TokenizerLoader06() {
            Tokenizer tok = Loader.LoadTokenizer(
                "# Matchers for 'ab', 'abc', and 'ababd'. This makes the tokenizer have to back up with 'ababc'.",
                "> (Start): 'a' => (A1): 'b' => [B1];",
                "(B1): 'c' => [C1];",
                "(B1): 'a' => (A2): 'b' => (B2): 'd' => [D1];");
            checkTokenizer(tok, "abcabcababdababc",
                "C1:(Unnamed:1, 1, 1):\"abc\"",
                "C1:(Unnamed:1, 4, 4):\"abc\"",
                "D1:(Unnamed:1, 7, 7):\"ababd\"",
                "B1:(Unnamed:1, 12, 12):\"ab\"",
                "C1:(Unnamed:1, 14, 14):\"abc\"");
            checkTokenizer(tok, "abababcabababd",
                "B1:(Unnamed:1, 1, 1):\"ab\"",
                "B1:(Unnamed:1, 3, 3):\"ab\"",
                "C1:(Unnamed:1, 5, 5):\"abc\"",
                "B1:(Unnamed:1, 8, 8):\"ab\"",
                "D1:(Unnamed:1, 10, 10):\"ababd\"");
        }

        [TestMethod]
        public void TokenizerLoader07() {
            Tokenizer tok = Loader.LoadTokenizer(
                "# Matches a set of characters.",
                "> (Start): 'bde' => [Done];");
            checkTokenizer(tok, "bed",
                "Done:(Unnamed:1, 1, 1):\"b\"",
                "Done:(Unnamed:1, 2, 2):\"e\"",
                "Done:(Unnamed:1, 3, 3):\"d\"");
            checkTokenizerError(tok, "c",
                "String is not tokenizable [state: Start, location: (Unnamed:1, 1, 1)]: \"c\"");
        }

        [TestMethod]
        public void TokenizerLoader08() {
            Tokenizer tok = Loader.LoadTokenizer(
                "# Matches not any of the characters in the set.",
                "> (Start): !'bde' => [Done];");
            checkTokenizer(tok, "cat",
                "Done:(Unnamed:1, 1, 1):\"c\"",
                "Done:(Unnamed:1, 2, 2):\"a\"",
                "Done:(Unnamed:1, 3, 3):\"t\"");
            checkTokenizerError(tok, "b",
                "String is not tokenizable [state: Start, location: (Unnamed:1, 1, 1)]: \"b\"");
        }

        [TestMethod]
        public void TokenizerLoader09() {
            Tokenizer tok = Loader.LoadTokenizer(
                "# Matches a range.",
                "> (Start): 'b'..'e' => [Done];");
            checkTokenizer(tok, "bed",
                "Done:(Unnamed:1, 1, 1):\"b\"",
                "Done:(Unnamed:1, 2, 2):\"e\"",
                "Done:(Unnamed:1, 3, 3):\"d\"");
            checkTokenizerError(tok, "a",
                "String is not tokenizable [state: Start, location: (Unnamed:1, 1, 1)]: \"a\"");
            checkTokenizerError(tok, "f",
                "String is not tokenizable [state: Start, location: (Unnamed:1, 1, 1)]: \"f\"");
        }

        [TestMethod]
        public void TokenizerLoader10() {
            Tokenizer tok = Loader.LoadTokenizer(
                "# Matches not a range.",
                "> (Start): !'b'..'e' => [Done];");
            checkTokenizer(tok, "mat",
                "Done:(Unnamed:1, 1, 1):\"m\"",
                "Done:(Unnamed:1, 2, 2):\"a\"",
                "Done:(Unnamed:1, 3, 3):\"t\"");
            checkTokenizerError(tok, "d",
                "String is not tokenizable [state: Start, location: (Unnamed:1, 1, 1)]: \"d\"");
        }

        [TestMethod]
        public void TokenizerLoader11() {
            Tokenizer tok = Loader.LoadTokenizer(
                "# Matches characters by or-ing other matchers.",
                "> (Start): 'b', 'd', 'e' => [Done];");
            checkTokenizer(tok, "bed",
                "Done:(Unnamed:1, 1, 1):\"b\"",
                "Done:(Unnamed:1, 2, 2):\"e\"",
                "Done:(Unnamed:1, 3, 3):\"d\"");
            checkTokenizerError(tok, "c",
                "String is not tokenizable [state: Start, location: (Unnamed:1, 1, 1)]: \"c\"");
        }

        [TestMethod]
        public void TokenizerLoader12() {
            Tokenizer tok = Loader.LoadTokenizer(
                "# Matches one range or another.",
                "> (Start): 'b'..'d', 'g'..'k' => [Done];");
            checkTokenizer(tok, "big",
                "Done:(Unnamed:1, 1, 1):\"b\"",
                "Done:(Unnamed:1, 2, 2):\"i\"",
                "Done:(Unnamed:1, 3, 3):\"g\"");
            checkTokenizerError(tok, "e",
                "String is not tokenizable [state: Start, location: (Unnamed:1, 1, 1)]: \"e\"");
        }

        [TestMethod]
        public void TokenizerLoader13() {
            Tokenizer tok = Loader.LoadTokenizer(
                "# Matcher which consumes the 'b' character.",
                "> (Start): 'a' => (Next): ^'b' => (Next): 'c' => [Done];");
            checkTokenizer(tok, "abc",
                "Done:(Unnamed:1, 1, 1):\"ac\"");
            checkTokenizer(tok, "abbbcac",
                "Done:(Unnamed:1, 1, 1):\"ac\"",
                "Done:(Unnamed:1, 6, 6):\"ac\"");
        }

        [TestMethod]
        public void TokenizerLoader14() {
            Tokenizer tok = Loader.LoadTokenizer(
                "# Combining consumers and not's to make a basic quoted string matcher which drops the quotes.",
                "> (Start): ^'\"' => (String.Part): !'\"' => (String.Part): ^'\"' => [String];");
            checkTokenizer(tok, "\"abc\"",
                "String:(Unnamed:1, 1, 1):\"abc\"");
        }

        [TestMethod]
        public void TokenizerLoader15() {
            Tokenizer tok = Loader.LoadTokenizer(
                "# Same basic quoted string matcher but using the 'any' character matcher after the end quote matcher.",
                "> (Start): ^'\"' => (String.Part);" +
                "(String.Part): ^'\"' => [String];",
                "(String.Part): * => (String.Part);");
            checkTokenizer(tok, "\"abc\"",
                "String:(Unnamed:1, 1, 1):\"abc\"");
        }

        [TestMethod]
        public void TokenizerLoader16() {
            Tokenizer tok = Loader.LoadTokenizer(
                "# This test consumes a token.",
                "> (Start): 'a' => [A];" +
                "(Start): 'b' => ^[B];",
                "(Start): 'c' => [C];");
            checkTokenizer(tok, "abcbbbca",
                "A:(Unnamed:1, 1, 1):\"a\"",
                "C:(Unnamed:1, 3, 3):\"c\"",
                "C:(Unnamed:1, 7, 7):\"c\"",
                "A:(Unnamed:1, 8, 8):\"a\"");
        }

        [TestMethod]
        public void TokenizerLoader17() {
            Tokenizer tok = Loader.LoadTokenizer(
                "# A group of matchers within a not.",
                "> (Start): !('abc', 'ijk', 'xyz') => [Done];");
            checkTokenizer(tok, "defmno",
                "Done:(Unnamed:1, 1, 1):\"d\"",
                "Done:(Unnamed:1, 2, 2):\"e\"",
                "Done:(Unnamed:1, 3, 3):\"f\"",
                "Done:(Unnamed:1, 4, 4):\"m\"",
                "Done:(Unnamed:1, 5, 5):\"n\"",
                "Done:(Unnamed:1, 6, 6):\"o\"");
            checkTokenizerError(tok, "a",
                "String is not tokenizable [state: Start, location: (Unnamed:1, 1, 1)]: \"a\"");
            checkTokenizerError(tok, "j",
                "String is not tokenizable [state: Start, location: (Unnamed:1, 1, 1)]: \"j\"");
            checkTokenizerError(tok, "z",
                "String is not tokenizable [state: Start, location: (Unnamed:1, 1, 1)]: \"z\"");
        }

        [TestMethod]
        public void TokenizerLoader18() {
            Tokenizer tok = Loader.LoadTokenizer(
                "# Having a consume on a goup. Consume is for the whole group.",
                "> (Start): ^!('abc', 'ijk', 'xyz'), 'j' => [Done];");
            checkTokenizer(tok, "defmno",
                "Done:(Unnamed:1, 1, 1):\"\"",
                "Done:(Unnamed:1, 2, 2):\"\"",
                "Done:(Unnamed:1, 3, 3):\"\"",
                "Done:(Unnamed:1, 4, 4):\"\"",
                "Done:(Unnamed:1, 5, 5):\"\"",
                "Done:(Unnamed:1, 6, 6):\"\"");
            checkTokenizer(tok, "j",
                "Done:(Unnamed:1, 1, 1):\"\"");
            checkTokenizerError(tok, "a",
                "String is not tokenizable [state: Start, location: (Unnamed:1, 1, 1)]: \"a\"");
            checkTokenizerError(tok, "z",
                "String is not tokenizable [state: Start, location: (Unnamed:1, 1, 1)]: \"z\"");
        }

        [TestMethod]
        public void TokenizerLoader19() {
            Tokenizer tok = Loader.LoadTokenizer(
                "# Having multiple connections between a node and another (in this case, with itself).",
                "> (Start): 'a' => (Part): 'b' => [Done];",
                "(Part): ^'c' => (Part);",
                "(Part): 'd' => (Part);");
            checkTokenizer(tok, "acdcdcdb",
                "Done:(Unnamed:1, 1, 1):\"adddb\"");
        }

        [TestMethod]
        public void TokenizerLoader20() {
            Tokenizer tok = Loader.LoadTokenizer(
                "# Having multiple connections between a node and another (in this case, with itself).",
                ">(Start): ^'a' => (Done): ^'b' => [Done];");
            checkTokenizer(tok, "a",
                "Done:(Unnamed:1, 1, 1):\"\"");
            checkTokenizer(tok, "ab",
                "Done:(Unnamed:1, 1, 1):\"\"");
            checkTokenizer(tok, "abbbb",
                "Done:(Unnamed:1, 1, 1):\"\"");
        }

        [TestMethod]
        public void TokenizerLoader21() {
            Tokenizer tok = Loader.LoadTokenizer(
                "# The example used in documentation. Shows token definitions, consumed tokens, and complex transitions.",
                "> (Start);",
                "(Start): ^'\"' => (inString): !'\"' => (inString): ^'\"' => [String];",
                "(Start): '+' => [Concatenate];",
                "(Start): '=' => [Assignment];",
                "(Start): 'a'..'z', 'A'..'Z', '_' => (Identifier): 'a'..'z', 'A'..'Z', '_', '0'..'9' => [Identifier];",
                "(Start): ' \n\r\t' => ^[Whitespace];");
            checkTokenizer(tok, "abcd_EFG_123",
                "Identifier:(Unnamed:1, 1, 1):\"abcd_EFG_123\"");
            checkTokenizer(tok, "Pet = \"Cat\" + \" & \" + \"Dog\"",
                "Identifier:(Unnamed:1, 1, 1):\"Pet\"",
                "Assignment:(Unnamed:1, 5, 5):\"=\"",
                "String:(Unnamed:1, 7, 7):\"Cat\"",
                "Concatenate:(Unnamed:1, 13, 13):\"+\"",
                "String:(Unnamed:1, 15, 15):\" & \"",
                "Concatenate:(Unnamed:1, 21, 21):\"+\"",
                "String:(Unnamed:1, 23, 23):\"Dog\"");
        }

        [TestMethod]
        public void TokenizerLoader22() {
            Tokenizer tok = Loader.LoadTokenizer(
                "# Set tokens and use a token replacer.",
                "> (Start): 'a'..'z', 'A'..'Z', '_' => [Identifier]: 'a'..'z', 'A'..'Z', '_', '0'..'9' => (Identifier);",
                "(Start): ' \n\r\t' => ^[Whitespace];",
                "[Identifier] = 'if'   => [If]",
                "             | 'else' => [Else]",
                "             | 'for'  => ^[For]",
                "             | 'class', 'object', 'thingy' => [Reserved];");
            checkTokenizer(tok, "abcd_EFG_123 class if else for elf iff",
                "Identifier:(Unnamed:1, 1, 1):\"abcd_EFG_123\"",
                "Reserved:(Unnamed:1, 14, 14):\"class\"",
                "If:(Unnamed:1, 20, 20):\"if\"",
                "Else:(Unnamed:1, 23, 23):\"else\"",
                "Identifier:(Unnamed:1, 32, 32):\"elf\"",
                "Identifier:(Unnamed:1, 36, 36):\"iff\"");
        }

        [TestMethod]
        public void TokenizerLoader23() {
            Tokenizer tok = Loader.LoadTokenizer(
                "# Set tokens and use a token replacer.",
                "> (Start): 'a' => (a) => [A]: 'b' => [B]: 'c' => [C];",
                "(Start): ' \n\r\t' => ^[Whitespace];");
            checkTokenizer(tok, "a ab abc",
                "A:(Unnamed:1, 1, 1):\"a\"",
                "B:(Unnamed:1, 3, 3):\"ab\"",
                "C:(Unnamed:1, 6, 6):\"abc\"");
        }

        [TestMethod]
        public void TokenizerLoader24() {
            Tokenizer tok = Loader.LoadTokenizer(
                "# Tokenizer Integer/Float example",
                "> (Start): '0'..'9' => (Integer): '0'..'9' => [Integer];",
                "(Start): '-' => (Integer-Neg): '0'..'9' => (Integer);",
                "(Integer): '.' => (Float-Dec-start): '0'..'9' => (Float-Dec): '0'..'9' => (Float-Dec) => [Float];",
                "(Integer): 'eE' => (Float-Exp-start): '0'..'9' => (Float-Exp): '0'..'9' => (Float-Exp) => [Float];",
                "(Float-Dec): 'eE' => (Float-Exp-start): '-' => (Float-Exp-Neg): '0'..'9' => (Float-Exp);",
                "",
                "(Start): ' \n\r\t' => ^[Whitespace];");
            checkTokenizer(tok, "0 420 -2 3.0 3.12 2e9 2e-9 -2.0e2 -2.0e-23",
                "Integer:(Unnamed:1, 1, 1):\"0\"",
                "Integer:(Unnamed:1, 3, 3):\"420\"",
                "Integer:(Unnamed:1, 7, 7):\"-2\"",
                "Float:(Unnamed:1, 10, 10):\"3.0\"",
                "Float:(Unnamed:1, 14, 14):\"3.12\"",
                "Float:(Unnamed:1, 19, 19):\"2e9\"",
                "Float:(Unnamed:1, 23, 23):\"2e-9\"",
                "Float:(Unnamed:1, 28, 28):\"-2.0e2\"",
                "Float:(Unnamed:1, 35, 35):\"-2.0e-23\"");
        }

        [TestMethod]
        public void TokenizerLoader25() {
            Tokenizer tok = Loader.LoadTokenizer(
                "# Tokenizer Binary, Octal, Decimal, and Hexadecimal example",
                "> (Start);",
                "(Start): '0' => (Num0): 'x' => (HexStart): '0'..'9', 'a'..'f', 'A'..'F'",
                "             => (Hexadecimal): '0'..'9', 'a'..'f', 'A'..'F' => [Hexadecimal];",
                "(Num0) => [Decimal];",
                "(Num0): 'b' => [Binary];",
                "(Num0): 'o' => [Octal];",
                "(Num0): 'd' => [Decimal];",
                "",
                "(Start): '1' => (Num1): '0'..'1' => (Num1): 'b' => [Binary];",
                "(Num0):  '0'..'1' => (Num1);",
                "(Num1) => [Decimal];",
                "(Num1): 'o' => [Octal];",
                "(Num1): 'd' => [Decimal];",
                "",
                "(Start): '2'..'7' => (Num7): '0'..'7' => (Num7): 'o' => [Octal];",
                "(Num0):  '0'..'7' => (Num7);",
                "(Num1):  '2'..'7' => (Num7);",
                "(Num7) => [Decimal];",
                "(Num7): 'd' => [Decimal];",
                "",
                "(Start): '8'..'9' => (Num9): '0'..'9' => (Num9): 'd' => [Decimal];",
                "(Num0):  '0'..'7' => (Num9);",
                "(Num1):  '2'..'7' => (Num9);",
                "(Num7):  '8'..'7' => (Num9);",
                "(Num9) => [Decimal];",
                "",
                "(Start): ' \n\r\t' => ^[Whitespace];");
            checkTokenizer(tok, "0 000 101 101b 101o 101d 0x101 42 777o 0xFF00FF00 10100110101b",
                "Decimal:(Unnamed:1, 1, 1):\"0\"",
                "Decimal:(Unnamed:1, 3, 3):\"000\"",
                "Decimal:(Unnamed:1, 7, 7):\"101\"",
                "Binary:(Unnamed:1, 11, 11):\"101b\"",
                "Octal:(Unnamed:1, 16, 16):\"101o\"",
                "Decimal:(Unnamed:1, 21, 21):\"101d\"",
                "Hexadecimal:(Unnamed:1, 26, 26):\"0x101\"",
                "Decimal:(Unnamed:1, 32, 32):\"42\"",
                "Octal:(Unnamed:1, 35, 35):\"777o\"",
                "Hexadecimal:(Unnamed:1, 40, 40):\"0xFF00FF00\"",
                "Binary:(Unnamed:1, 51, 51):\"10100110101b\"");
            checkTokenizerError(tok, "123b",
                "Decimal:(Unnamed:1, 1, 1):\"123\"",
                "String is not tokenizable [state: Start, location: (Unnamed:1, 4, 4)]: \"b\"");
        }

        [TestMethod]
        public void TokenizerLoader26() {
            Tokenizer tok = Loader.LoadTokenizer(
                ">(Start);",
                "(Start): '=' => [Assign]: '=' => [EqualTo];",
                "(Start): '!' => [Not]: '=' => [NotEqualTo];",
                "(Start): '>' => [GreaterThan]: '=' => [GreaterThanOrEqualTo];",
                "(Start): '<' => [LessThan]: '=' => [LessThanOrEqualTo];",
                "(Start): '|' => [BitwiseOr]: '|' => [BoolOr]: '=' => [BoolOrAssign];",
                "(BitwiseOr): '=' => [BitwiseOrAssign];",
                "(Start): '&' => [BitwiseAnd]: '&' => [BoolAnd]: '=' => [BoolAndAssign];",
                "(BitwiseAnd): '=' => [BitwiseAndAssign];",
                "(Start): '+' => [Add]: '+' => [Increment];",
                "(Add): '=' => [AddAssign];",
                "(Start): '-' => [Sub]: '-' => [Decrement];",
                "(Sub): '=' => [SubAssign];",
                "(Start): '/' => [Div]: '/' => (Comment1): !'\n' => (Comment1): '\n' => [Comment];",
                "(Div): '=' => [DivAssign];",
                "(Div): '*' => (Comment2): !'*' => (Comment2): '*' => (Comment3): '/' => [Comment];",
                "(Comment3): !'/' => (Comment2);",
                "(Start): '*' => [Multiply]: '=' => [MultiplyAssign];",
                "(Start): '%' => [Modulo]: '=' => [ModuloAssign];",
                "(Start): '^' => [BoolXor]: '=' => [BoolXorAssign];",
                "(Start): '~' => [BitwiseXor]: '=' => [BitwiseXorAssign];",
                "(Start): '(' => [OpenPar];",
                "(Start): ')' => [ClosePar];",
                "(Start): ',' => [Comma];",
                "(Start): ';' => [Semicolon];",
                "(Start): ' \n\r\t' => ^[Whitespace];");
            checkTokenizer(tok, "= == ! != >= <= *= /= & && &= &&= + += ++",
                "Assign:(Unnamed:1, 1, 1):\"=\"",
                "EqualTo:(Unnamed:1, 3, 3):\"==\"",
                "Not:(Unnamed:1, 6, 6):\"!\"",
                "NotEqualTo:(Unnamed:1, 8, 8):\"!=\"",
                "GreaterThanOrEqualTo:(Unnamed:1, 11, 11):\">=\"",
                "LessThanOrEqualTo:(Unnamed:1, 14, 14):\"<=\"",
                "MultiplyAssign:(Unnamed:1, 17, 17):\"*=\"",
                "DivAssign:(Unnamed:1, 20, 20):\"/=\"",
                "BitwiseAnd:(Unnamed:1, 23, 23):\"&\"",
                "BoolAnd:(Unnamed:1, 25, 25):\"&&\"",
                "BitwiseAndAssign:(Unnamed:1, 28, 28):\"&=\"",
                "BoolAndAssign:(Unnamed:1, 31, 31):\"&&=\"",
                "Add:(Unnamed:1, 35, 35):\"+\"",
                "AddAssign:(Unnamed:1, 37, 37):\"+=\"",
                "Increment:(Unnamed:1, 40, 40):\"++\"");
            checkTokenizer(tok, "// single-line comment \n /* *multi-line \n\n comment */",
                "Comment:(Unnamed:1, 1, 1):\"// single-line comment \\n\"",
                "Comment:(Unnamed:2, 2, 26):\"/* *multi-line \\n\\n comment */\"");
        }

        [TestMethod]
        public void TokenizerLoader27() {
            Tokenizer tok = Loader.LoadTokenizer(
                ">(Start);",
                "(Start): 'a'..'z', 'A'..'Z', '_' => (Identifier): 'a'..'z', 'A'..'Z', '0'..'9', '_' => [Identifier];",
                "[Identifier] = 'catch'   => [Catch]",
                "             | 'class'   => [Class]",
                "             | 'do'      => [Do]",
                "             | 'double'  => [Double]",
                "             | 'else'    => [Else]",
                "             | 'extend'  => [Extend]",
                "             | 'float'   => [Float]",
                "             | 'for'     => [For]",
                "             | 'foreach' => [Foreach]",
                "             | 'if'      => [If]",
                "             | 'int'     => [Int]",
                "             | 'object'  => [Object]",
                "             | 'string'  => [String]",
                "             | 'try'     => [Try]",
                "             | 'while'   => [While];",
                "[Identifier] = 'end', 'goto', 'label', 'then' => [Reserved];",
                "(Start): ' \n\r\t' => ^[Whitespace];");
            checkTokenizer(tok, "if else then do while",
                "If:(Unnamed:1, 1, 1):\"if\"",
                "Else:(Unnamed:1, 4, 4):\"else\"",
                "Reserved:(Unnamed:1, 9, 9):\"then\"",
                "Do:(Unnamed:1, 14, 14):\"do\"",
                "While:(Unnamed:1, 17, 17):\"while\"");
        }
    }
}
