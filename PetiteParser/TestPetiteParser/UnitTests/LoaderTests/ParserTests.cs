﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using PetiteParser.Loader;
using PetiteParser.Parser;
using System;
using TestPetiteParser.Tools;

namespace TestPetiteParser.UnitTests.LoaderTests {

    [TestClass]
    public class ParserTests {

        [TestMethod]
        public void ParserLoader01() {
            Parser parser = Loader.LoadParser(
                "> (Start): 'a' => [A];",
                "(Start): 'b' => [B];",
                "(Start): 'c' => [C];",
                "# Sets the start of the program. Accepts three consecutive tokens only.",
                "> <Program>;",
                "<Program> := [A] [B] [C];");

            parser.Check("abc",
                "─<Program>",
                "  ├─[A:(Unnamed:1, 1, 1):\"a\"]",
                "  ├─[B:(Unnamed:1, 2, 2):\"b\"]",
                "  └─[C:(Unnamed:1, 3, 3):\"c\"]");

            parser.Check("cba",
                "Unexpected item, [C:(Unnamed:1, 1, 1):\"c\"], in state 0. Expected: A.",
                "Unexpected item, [B:(Unnamed:1, 2, 2):\"b\"], in state 0. Expected: A.",
                "Unexpected item, [$EOFToken:(-):\"$EOFToken\"], in state 2. Expected: B.",
                "Unexpected end of input.");
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

            parser.Check("a",
                "─<Program>",
                "  └─[A:(Unnamed:1, 1, 1):\"a\"]");

            parser.Check("b",
                "─<Program>",
                "  └─[B:(Unnamed:1, 1, 1):\"b\"]");

            parser.Check("ab",
                "Unexpected item, [B:(Unnamed:1, 2, 2):\"b\"], in state 2. Expected: $EOFToken.",
                "─<Program>",
                "  └─[A:(Unnamed:1, 1, 1):\"a\"]");
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

            parser.Check("ab",
                "─<Program>",
                "  ├─<Value>",
                "  │  └─[A:(Unnamed:1, 1, 1):\"a\"]",
                "  └─<Value>",
                "     └─[B:(Unnamed:1, 2, 2):\"b\"]");

            parser.Check("ba",
                "─<Program>",
                "  ├─<Value>",
                "  │  └─[B:(Unnamed:1, 1, 1):\"b\"]",
                "  └─<Value>",
                "     └─[A:(Unnamed:1, 2, 2):\"a\"]");

            parser.Check("cc",
                "─<Program>",
                "  ├─<Value>",
                "  │  └─[C:(Unnamed:1, 1, 1):\"c\"]",
                "  └─<Value>",
                "     └─[C:(Unnamed:1, 2, 2):\"c\"]");

            parser.Check("a",
                "Unexpected item, [$EOFToken:(-):\"$EOFToken\"], in state 3. Expected: A, B, C.",
                "Unexpected end of input.");

            parser.Check("abc",
                "Unexpected item, [C:(Unnamed:1, 3, 3):\"c\"], in state 8. Expected: $EOFToken.",
                "─<Program>",
                "  ├─<Value>",
                "  │  └─[A:(Unnamed:1, 1, 1):\"a\"]",
                "  └─<Value>",
                "     └─[B:(Unnamed:1, 2, 2):\"b\"]");
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

            parser.Grammar.Check(
                "> <$StartTerm>",
                "<Program> → [A] <B> [C]",
                "<B> → <B'0>",
                "<B'0> → ",
                "<B'0> → [B] <B'0>",
                "<$StartTerm> → <Program> [$EOFToken]");

            parser.Check("ac",
                "─<Program>",
                "  ├─[A:(Unnamed:1, 1, 1):\"a\"]",
                "  ├─<B>",
                "  │  └─<B'0>",
                "  └─[C:(Unnamed:1, 2, 2):\"c\"]");

            parser.Check("abc",
                "─<Program>",
                "  ├─[A:(Unnamed:1, 1, 1):\"a\"]",
                "  ├─<B>",
                "  │  └─<B'0>",
                "  │     ├─[B:(Unnamed:1, 2, 2):\"b\"]",
                "  │     └─<B'0>",
                "  └─[C:(Unnamed:1, 3, 3):\"c\"]");

            parser.Check("abbbbc",
                "─<Program>",
                "  ├─[A:(Unnamed:1, 1, 1):\"a\"]",
                "  ├─<B>",
                "  │  └─<B'0>",
                "  │     ├─[B:(Unnamed:1, 2, 2):\"b\"]",
                "  │     └─<B'0>",
                "  │        ├─[B:(Unnamed:1, 3, 3):\"b\"]",
                "  │        └─<B'0>",
                "  │           ├─[B:(Unnamed:1, 4, 4):\"b\"]",
                "  │           └─<B'0>",
                "  │              ├─[B:(Unnamed:1, 5, 5):\"b\"]",
                "  │              └─<B'0>",
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

            parser.Check("ac",
                "─<Program>",
                "  ├─[A:(Unnamed:1, 1, 1):\"a\"]",
                "  └─<B>",
                "     └─[C:(Unnamed:1, 2, 2):\"c\"]");

            parser.Check("abc",
                "─<Program>",
                "  ├─[A:(Unnamed:1, 1, 1):\"a\"]",
                "  └─<B>",
                "     ├─[B:(Unnamed:1, 2, 2):\"b\"]",
                "     └─<B>",
                "        └─[C:(Unnamed:1, 3, 3):\"c\"]");

            parser.Check("abbbbc",
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
            parser.Grammar.Check(
                "> <$StartTerm>",
                "<Expression> → <Term> <Expression'0>",
                "<Term> → <Factor> <Term'0>",
                "<Factor> → <Value>",
                "<Factor> → [Neg] <Factor> {Negate}",
                "<Factor> → [Open] <Expression> [Close]",
                "<Factor> → [Pos] <Factor>",
                "<Value> → [Float] {PushFloat}",
                "<Value> → [Id] {PushId}",
                "<Value> → [Int] {PushInt}",
                "<Expression'0> → ",
                "<Expression'0> → [Neg] <Term> {Subtract} <Expression'0>",
                "<Expression'0> → [Pos] <Term> {Add} <Expression'0>",
                "<Term'0> → ",
                "<Term'0> → [Div] <Factor> {Divide} <Term'0>",
                "<Term'0> → [Mul] <Factor> {Multiply} <Term'0>",
                "<$StartTerm> → <Expression> [$EOFToken]");

            parser.Check("5 + -2",
                "─<Expression>",
                "  ├─<Term>",
                "  │  ├─<Factor>",
                "  │  │  └─<Value>",
                "  │  │     ├─[Int:(Unnamed:1, 1, 1):\"5\"]",
                "  │  │     └─{PushInt}",
                "  │  └─<Term'0>",
                "  └─<Expression'0>",
                "     ├─[Pos:(Unnamed:1, 3, 3):\"+\"]",
                "     ├─<Term>",
                "     │  ├─<Factor>",
                "     │  │  ├─[Neg:(Unnamed:1, 5, 5):\"-\"]",
                "     │  │  ├─<Factor>",
                "     │  │  │  └─<Value>",
                "     │  │  │     ├─[Int:(Unnamed:1, 6, 6):\"2\"]",
                "     │  │  │     └─{PushInt}",
                "     │  │  └─{Negate}",
                "     │  └─<Term'0>",
                "     ├─{Add}",
                "     └─<Expression'0>");

            parser.Check("5 * 2 + 3",
                "─<Expression>",
                "  ├─<Term>",
                "  │  ├─<Factor>",
                "  │  │  └─<Value>",
                "  │  │     ├─[Int:(Unnamed:1, 1, 1):\"5\"]",
                "  │  │     └─{PushInt}",
                "  │  └─<Term'0>",
                "  │     ├─[Mul:(Unnamed:1, 3, 3):\"*\"]",
                "  │     ├─<Factor>",
                "  │     │  └─<Value>",
                "  │     │     ├─[Int:(Unnamed:1, 5, 5):\"2\"]",
                "  │     │     └─{PushInt}",
                "  │     ├─{Multiply}",
                "  │     └─<Term'0>",
                "  └─<Expression'0>",
                "     ├─[Pos:(Unnamed:1, 7, 7):\"+\"]",
                "     ├─<Term>",
                "     │  ├─<Factor>",
                "     │  │  └─<Value>",
                "     │  │     ├─[Int:(Unnamed:1, 9, 9):\"3\"]",
                "     │  │     └─{PushInt}",
                "     │  └─<Term'0>",
                "     ├─{Add}",
                "     └─<Expression'0>");

            parser.Check("5 * (2 + 3)",
                "─<Expression>",
                "  ├─<Term>",
                "  │  ├─<Factor>",
                "  │  │  └─<Value>",
                "  │  │     ├─[Int:(Unnamed:1, 1, 1):\"5\"]",
                "  │  │     └─{PushInt}",
                "  │  └─<Term'0>",
                "  │     ├─[Mul:(Unnamed:1, 3, 3):\"*\"]",
                "  │     ├─<Factor>",
                "  │     │  ├─[Open:(Unnamed:1, 5, 5):\"(\"]",
                "  │     │  ├─<Expression>",
                "  │     │  │  ├─<Term>",
                "  │     │  │  │  ├─<Factor>",
                "  │     │  │  │  │  └─<Value>",
                "  │     │  │  │  │     ├─[Int:(Unnamed:1, 6, 6):\"2\"]",
                "  │     │  │  │  │     └─{PushInt}",
                "  │     │  │  │  └─<Term'0>",
                "  │     │  │  └─<Expression'0>",
                "  │     │  │     ├─[Pos:(Unnamed:1, 8, 8):\"+\"]",
                "  │     │  │     ├─<Term>",
                "  │     │  │     │  ├─<Factor>",
                "  │     │  │     │  │  └─<Value>",
                "  │     │  │     │  │     ├─[Int:(Unnamed:1, 10, 10):\"3\"]",
                "  │     │  │     │  │     └─{PushInt}",
                "  │     │  │     │  └─<Term'0>",
                "  │     │  │     ├─{Add}",
                "  │     │  │     └─<Expression'0>",
                "  │     │  └─[Close:(Unnamed:1, 11, 11):\")\"]",
                "  │     ├─{Multiply}",
                "  │     └─<Term'0>",
                "  └─<Expression'0>");
        }

        [TestMethod]
        public void ParserLoader07() {
            Parser parser = Loader.LoadParser(
                "> (Start): 'a' => [A];",
                "(Start): 'b' => [B];",
                "(Start): 'c' => [C];",
                "> <Program>;",
                "<Program> := [A] <B>;" +
                "<B> := [B] <B> | [C];" +
                "* => [E];");

            parser.Check("aGc",
                "received an error token: E:(Unnamed:1, 2, 2):\"G\"",
                "─<Program>",
                "  ├─[A:(Unnamed:1, 1, 1):\"a\"]",
                "  └─<B>",
                "     └─[C:(Unnamed:1, 3, 3):\"c\"]");

            parser.Check("aGbGaGc",
                "received an error token: E:(Unnamed:1, 2, 2):\"G\"",
                "received an error token: E:(Unnamed:1, 4, 4):\"G\"",
                "Unexpected item, [A:(Unnamed:1, 5, 5):\"a\"], in state 4. Expected: B, C.",
                "received an error token: E:(Unnamed:1, 6, 6):\"G\"",
                "─<Program>",
                "  ├─[A:(Unnamed:1, 1, 1):\"a\"]",
                "  └─<B>",
                "     ├─[B:(Unnamed:1, 3, 3):\"b\"]",
                "     └─<B>",
                "        └─[C:(Unnamed:1, 7, 7):\"c\"]");
        }

        [TestMethod]
        public void ParserLoader08() {
            Parser parser = Loader.LoadParser(
                    "> (Start): 'a' => [A];",
                    "(Start): 'b' => [B];",
                    "> <Start> := _ | <Part> <Start>;",
                    "<Part> := [A] | [B];");

            parser.Check("",
                "─<Start>");

            parser.Check("aaba",
                "─<Start>",
                "  ├─<Part>",
                "  │  └─[A:(Unnamed:1, 1, 1):\"a\"]",
                "  └─<Start>",
                "     ├─<Part>",
                "     │  └─[A:(Unnamed:1, 2, 2):\"a\"]",
                "     └─<Start>",
                "        ├─<Part>",
                "        │  └─[B:(Unnamed:1, 3, 3):\"b\"]",
                "        └─<Start>",
                "           ├─<Part>",
                "           │  └─[A:(Unnamed:1, 4, 4):\"a\"]",
                "           └─<Start>");
        }

        [TestMethod]
        public void ParserLoader09() {
            Parser parser = Loader.LoadParser(
                 "> (Start): 'a' => [A];",
                 "(Start): 'b' => [B];",
                 "> <Start> := _ | <Start> <Part>;",
                 "<Part> := [A] | [B];");

            parser.Grammar.Check(
                "> <$StartTerm>",
                "<Start> → <Start'0>",
                "<Part> → [A]",
                "<Part> → [B]",
                "<Start'0> → ",
                "<Start'0> → <Part> <Start'0>",
                "<$StartTerm> → <Start> [$EOFToken]");

            parser.Check("",
                "─<Start>",
                "  └─<Start'0>");

            parser.Check("aaba",
                "─<Start>",
                "  └─<Start'0>",
                "     ├─<Part>",
                "     │  └─[A:(Unnamed:1, 1, 1):\"a\"]",
                "     └─<Start'0>",
                "        ├─<Part>",
                "        │  └─[A:(Unnamed:1, 2, 2):\"a\"]",
                "        └─<Start'0>",
                "           ├─<Part>",
                "           │  └─[B:(Unnamed:1, 3, 3):\"b\"]",
                "           └─<Start'0>",
                "              ├─<Part>",
                "              │  └─[A:(Unnamed:1, 4, 4):\"a\"]",
                "              └─<Start'0>");
        }
    }
}
