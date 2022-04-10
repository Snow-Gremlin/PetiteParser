using Microsoft.VisualStudio.TestTools.UnitTesting;
using PetiteParser.Loader;
using PetiteParser.Misc;
using PetiteParser.Parser;

namespace TestPetiteParser {

    [TestClass]
    public class ParserLoaderUnitTests {

        /// <summary>Checks the parser will parse the given input.</summary>
        static private void checkParser(Parser parser, string input, params string[] expected) =>
            TestTools.AreEqual(expected.JoinLines(), parser.Parse(input).ToString());

        [TestMethod]
        public void ParserLoader01() {
            Parser parser = Loader.LoadParser(
                "> (Start): 'a' => [A];",
                "(Start): 'b' => [B];",
                "(Start): 'c' => [C];",
                "# Sets the start of the program. Accepts three consecutive tokens only.",
                "> <Program>;",
                "<Program> := [A] [B] [C];");
            checkParser(parser, "abc",
                "─<Program>",
                "  ├─[A:(Unnamed:1, 1, 1):\"a\"]",
                "  ├─[B:(Unnamed:1, 2, 2):\"b\"]",
                "  └─[C:(Unnamed:1, 3, 3):\"c\"]");
            checkParser(parser, "cba",
                "Unexpected item, [C:(Unnamed:1, 1, 1):\"c\"], in state 0. Expected: A.",
                "Unexpected item, [B:(Unnamed:1, 2, 2):\"b\"], in state 0. Expected: A.",
                "Unexpected item, [$EOFToken:(-):\"$EOFToken\"], in state 2. Expected: B.");
        }

        [TestMethod]
        public void ParserLoader02() {
            Parser parser = Loader.LoadParser(
                "> (Start): 'a' => [A];",
                "(Start): 'b' => [B];",
                "(Start): 'c' => [C];",
                "# Language accepts only one of the three tokens.",
                "> <Program>;",
                "<Program> := [A] | [B] | [C];");
            checkParser(parser, "a",
                "─<Program>",
                "  └─[A:(Unnamed:1, 1, 1):\"a\"]");
            checkParser(parser, "b",
                "─<Program>",
                "  └─[B:(Unnamed:1, 1, 1):\"b\"]");
            checkParser(parser, "ab",
                "Unexpected item, [B:(Unnamed:1, 2, 2):\"b\"], in state 2. Expected: $EOFToken.");
        }

        [TestMethod]
        public void ParserLoader03() {
            Parser parser = Loader.LoadParser(
                "> (Start): 'a' => [A];",
                "(Start): 'b' => [B];",
                "(Start): 'c' => [C];",
                "# Matches two of any token.",
                "> <Program>;",
                "<Program> := <Value> <Value>;" +
                "<Value> := [A] | [B] | [C];");

            checkParser(parser, "ab",
                "─<Program>",
                "  ├─<Value>",
                "  │  └─[A:(Unnamed:1, 1, 1):\"a\"]",
                "  └─<Value>",
                "     └─[B:(Unnamed:1, 2, 2):\"b\"]");
            checkParser(parser, "ba",
                "─<Program>",
                "  ├─<Value>",
                "  │  └─[B:(Unnamed:1, 1, 1):\"b\"]",
                "  └─<Value>",
                "     └─[A:(Unnamed:1, 2, 2):\"a\"]");
            checkParser(parser, "cc",
                "─<Program>",
                "  ├─<Value>",
                "  │  └─[C:(Unnamed:1, 1, 1):\"c\"]",
                "  └─<Value>",
                "     └─[C:(Unnamed:1, 2, 2):\"c\"]");
            checkParser(parser, "a",
                "Unexpected item, [$EOFToken:(-):\"$EOFToken\"], in state 3. Expected: A, B, C.");
            checkParser(parser, "abc",
                "Unexpected item, [C:(Unnamed:1, 3, 3):\"c\"], in state 8. Expected: $EOFToken.");
        }

        [TestMethod]
        public void ParserLoader04() {
            Parser parser = Loader.LoadParser(
                "> (Start): 'a' => [A];",
                "(Start): 'b' => [B];",
                "(Start): 'c' => [C];",
                "# Uses a lambda to allow an optional repeating section of Bs between As.",
                "> <Program>;",
                "<Program> := [A] <B> [C];" +
                "<B> := <B> [B] | _;");
            checkParser(parser, "ac",
                "─<Program>",
                "  ├─[A:(Unnamed:1, 1, 1):\"a\"]",
                "  ├─<B>",
                "  └─[C:(Unnamed:1, 2, 2):\"c\"]");
            checkParser(parser, "abc",
                "─<Program>",
                "  ├─[A:(Unnamed:1, 1, 1):\"a\"]",
                "  ├─<B>",
                "  │  ├─<B>",
                "  │  └─[B:(Unnamed:1, 2, 2):\"b\"]",
                "  └─[C:(Unnamed:1, 3, 3):\"c\"]");
            checkParser(parser, "abbbbc",
                "─<Program>",
                "  ├─[A:(Unnamed:1, 1, 1):\"a\"]",
                "  ├─<B>",
                "  │  ├─<B>",
                "  │  │  ├─<B>",
                "  │  │  │  ├─<B>",
                "  │  │  │  │  ├─<B>",
                "  │  │  │  │  └─[B:(Unnamed:1, 2, 2):\"b\"]",
                "  │  │  │  └─[B:(Unnamed:1, 3, 3):\"b\"]",
                "  │  │  └─[B:(Unnamed:1, 4, 4):\"b\"]",
                "  │  └─[B:(Unnamed:1, 5, 5):\"b\"]",
                "  └─[C:(Unnamed:1, 6, 6):\"c\"]");
        }

        [TestMethod]
        public void ParserLoader05() {
            Parser parser = Loader.LoadParser(
                "> (Start): 'a' => [A];",
                "(Start): 'b' => [B];",
                "(Start): 'c' => [C];",
                "# A different way to match the same as test 04 but without the lambda.",
                "> <Program>;",
                "<Program> := [A] <B>;" +
                "<B> := [B] <B> | [C];");
            checkParser(parser, "ac",
                "─<Program>",
                "  ├─[A:(Unnamed:1, 1, 1):\"a\"]",
                "  └─<B>",
                "     └─[C:(Unnamed:1, 2, 2):\"c\"]");
            checkParser(parser, "abc",
                "─<Program>",
                "  ├─[A:(Unnamed:1, 1, 1):\"a\"]",
                "  └─<B>",
                "     ├─[B:(Unnamed:1, 2, 2):\"b\"]",
                "     └─<B>",
                "        └─[C:(Unnamed:1, 3, 3):\"c\"]");
            checkParser(parser, "abbbbc",
                "─<Program>",
                "  ├─[A:(Unnamed:1, 1, 1):\"a\"]",
                "  └─<B>",
                "     ├─[B:(Unnamed:1, 2, 2):\"b\"]",
                "     └─<B>",
                "        ├─[B:(Unnamed:1, 3, 3):\"b\"]",
                "        └─<B>",
                "           ├─[B:(Unnamed:1, 4, 4):\"b\"]",
                "           └─<B>",
                "              ├─[B:(Unnamed:1, 5, 5):\"b\"]",
                "              └─<B>",
                "                 └─[C:(Unnamed:1, 6, 6):\"c\"]");
        }

        [TestMethod]
        public void ParserLoader06() {
            Parser parser = Loader.LoadParser(
                "> (Start);",
                "(Start): '+' => [Pos];",
                "(Start): '-' => [Neg];",
                "(Start): '*' => [Mul];",
                "(Start): '/' => [Div];",
                "(Start): '(' => [Open];",
                "(Start): ')' => [Close];",
                "(Start): '0'..'9' => (Int): '0'..'9' => [Int];",
                "(Int): '.' => (Float-Start): '0'..'9' => (Float): '0'..'9' => [Float];",
                "(Start): 'a'..'z', 'A'..'Z', '_' => (Id): 'a'..'z', 'A'..'Z', '_', '0'..'9' => [Id];",
                "(Start): ' ' => (Space): ' ' => ^[Space];",
                "",
                "> <Expression>;",
                "<Expression> := <Term>",
                "    | <Expression> [Pos] <Term> {Add}",
                "    | <Expression> [Neg] <Term> {Subtract};",
                "<Term> := <Factor>",
                "    | <Term> [Mul] <Factor> {Multiply}",
                "    | <Term> [Div] <Factor> {Divide};",
                "<Factor> := <Value>",
                "    | [Open] <Expression> [Close]",
                "    | [Neg] <Factor> {Negate}",
                "    | [Pos] <Factor>;",
                "<Value> := [Id] {PushId}",
                "    | [Int] {PushInt}",
                "    | [Float] {PushFloat};");
            checkParser(parser, "5 + -2",
                "─<Expression>",
                "  ├─<Expression>",
                "  │  └─<Term>",
                "  │     └─<Factor>",
                "  │        └─<Value>",
                "  │           ├─[Int:(Unnamed:1, 1, 1):\"5\"]",
                "  │           └─{PushInt}",
                "  ├─[Pos:(Unnamed:1, 3, 3):\"+\"]",
                "  ├─<Term>",
                "  │  └─<Factor>",
                "  │     ├─[Neg:(Unnamed:1, 5, 5):\"-\"]",
                "  │     ├─<Factor>",
                "  │     │  └─<Value>",
                "  │     │     ├─[Int:(Unnamed:1, 6, 6):\"2\"]",
                "  │     │     └─{PushInt}",
                "  │     └─{Negate}",
                "  └─{Add}");
            checkParser(parser, "5 * 2 + 3",
                "─<Expression>",
                "  ├─<Expression>",
                "  │  └─<Term>",
                "  │     ├─<Term>",
                "  │     │  └─<Factor>",
                "  │     │     └─<Value>",
                "  │     │        ├─[Int:(Unnamed:1, 1, 1):\"5\"]",
                "  │     │        └─{PushInt}",
                "  │     ├─[Mul:(Unnamed:1, 3, 3):\"*\"]",
                "  │     ├─<Factor>",
                "  │     │  └─<Value>",
                "  │     │     ├─[Int:(Unnamed:1, 5, 5):\"2\"]",
                "  │     │     └─{PushInt}",
                "  │     └─{Multiply}",
                "  ├─[Pos:(Unnamed:1, 7, 7):\"+\"]",
                "  ├─<Term>",
                "  │  └─<Factor>",
                "  │     └─<Value>",
                "  │        ├─[Int:(Unnamed:1, 9, 9):\"3\"]",
                "  │        └─{PushInt}",
                "  └─{Add}");
            checkParser(parser, "5 * (2 + 3)",
                "─<Expression>",
                "  └─<Term>",
                "     ├─<Term>",
                "     │  └─<Factor>",
                "     │     └─<Value>",
                "     │        ├─[Int:(Unnamed:1, 1, 1):\"5\"]",
                "     │        └─{PushInt}",
                "     ├─[Mul:(Unnamed:1, 3, 3):\"*\"]",
                "     ├─<Factor>",
                "     │  ├─[Open:(Unnamed:1, 5, 5):\"(\"]",
                "     │  ├─<Expression>",
                "     │  │  ├─<Expression>",
                "     │  │  │  └─<Term>",
                "     │  │  │     └─<Factor>",
                "     │  │  │        └─<Value>",
                "     │  │  │           ├─[Int:(Unnamed:1, 6, 6):\"2\"]",
                "     │  │  │           └─{PushInt}",
                "     │  │  ├─[Pos:(Unnamed:1, 8, 8):\"+\"]",
                "     │  │  ├─<Term>",
                "     │  │  │  └─<Factor>",
                "     │  │  │     └─<Value>",
                "     │  │  │        ├─[Int:(Unnamed:1, 10, 10):\"3\"]",
                "     │  │  │        └─{PushInt}",
                "     │  │  └─{Add}",
                "     │  └─[Close:(Unnamed:1, 11, 11):\")\"]",
                "     └─{Multiply}");
        }
    }
}
