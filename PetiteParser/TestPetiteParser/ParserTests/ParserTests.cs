using Microsoft.VisualStudio.TestTools.UnitTesting;
using PetiteParser.Grammar;
using PetiteParser.Grammar.Inspector;
using PetiteParser.Grammar.Normalizer;
using PetiteParser.Logger;
using PetiteParser.Parser;
using PetiteParser.Parser.States;
using PetiteParser.Parser.Table;
using PetiteParser.Tokenizer;
using System;
using TestPetiteParser.GrammarTests;
using TestPetiteParser.Tools;

namespace TestPetiteParser.ParserTests;

[TestClass]
sealed public class ParserTests {

    [TestMethod]
    public void Parser01() {
        Tokenizer tok = new();
        tok.Start("start");
        tok.Join("start", "(").AddSet("(");
        tok.Join("start", ")").AddSet(")");
        tok.Join("start", "+").AddSet("+");
        tok.Join("start", "number").AddRange("0", "9");
        tok.Join("number", "number").AddRange("0", "9");
        tok.SetToken("(", "(");
        tok.SetToken(")", ")");
        tok.SetToken("+", "+");
        tok.SetToken("number", "n");
        // 1. E → T
        // 2. E → ( E )
        // 3. T → n
        // 4. T → + T
        // 5. T → T + n
        Grammar grammar = new();
        grammar.Start("E");
        grammar.NewRule("E").AddTerm("T");
        grammar.NewRule("E").AddToken("(").AddTerm("E").AddToken(")");
        grammar.NewRule("T").AddToken("n");
        grammar.NewRule("T").AddToken("+").AddTerm("T");
        grammar.NewRule("T").AddTerm("T").AddToken("+").AddToken("n");

        Writer log = new();
        Inspector.Validate(grammar, log);
        grammar = Normalizer.GetNormal(grammar, log);

        ParserStates states = new();
        try {
            states.DetermineStates(grammar, OnConflict.Panic, log);
        } finally {
            Console.WriteLine(states.ToString());
        }

        if (log.Failed)
            throw new ParserException("Errors while building parser:" + Environment.NewLine + log.ToString());
        Table table = states.CreateTable();
        Parser parser = new(table, grammar, tok);
        parser.Grammar.Check(
            "> <$StartTerm>",
            "<E> → <T>",
            "   | [(] <E> [)]",
            "<T> → [+] <T> <T'0>",
            "   | [n] <T'0>",
            "<T'0> → λ",
            "   | [+] [n] <T'0>",
            "<$StartTerm> → <E> [$EOFToken]");

        states.Check();
        //table.Check();

        parser.Check("103",
            "─<E>",
            "  └─<T>",
            "     ├─[n:(Unnamed:1, 1, 1):\"103\"]",
            "     └─<T'0>");

        parser.Check("+2",
            "─<E>",
            "  └─<T>",
            "     ├─[+:(Unnamed:1, 1, 1):\"+\"]",
            "     ├─<T>",
            "     │  ├─[n:(Unnamed:1, 2, 2):\"2\"]",
            "     │  └─<T'0>",
            "     └─<T'0>");

        parser.Check("3+4",
            "─<E>",
            "  └─<T>",
            "     ├─[n:(Unnamed:1, 1, 1):\"3\"]",
            "     └─<T'0>",
            "        ├─[+:(Unnamed:1, 2, 2):\"+\"]",
            "        ├─[n:(Unnamed:1, 3, 3):\"4\"]",
            "        └─<T'0>");

        parser.Check("((42+6))",
            "─<E>",
            "  ├─[(:(Unnamed:1, 1, 1):\"(\"]",
            "  ├─<E>",
            "  │  ├─[(:(Unnamed:1, 2, 2):\"(\"]",
            "  │  ├─<E>",
            "  │  │  └─<T>",
            "  │  │     ├─[n:(Unnamed:1, 3, 3):\"42\"]",
            "  │  │     └─<T'0>",
            "  │  │        ├─[+:(Unnamed:1, 5, 5):\"+\"]",
            "  │  │        ├─[n:(Unnamed:1, 6, 6):\"6\"]",
            "  │  │        └─<T'0>",
            "  │  └─[):(Unnamed:1, 7, 7):\")\"]",
            "  └─[):(Unnamed:1, 8, 8):\")\"]");
    }

    [TestMethod]
    public void Parser01_Smaller() {
        Tokenizer tok = new();
        tok.Start("start");
        tok.Join("start", "+").AddSet("+");
        tok.Join("start", "number").AddRange("0", "9");
        tok.Join("number", "number").AddRange("0", "9");
        tok.SetToken("+", "+");
        tok.SetToken("number", "n");
        // 1. T → n
        // 2. T → + T
        // 3. T → T + n
        Grammar grammar = new();
        grammar.Start("T");
        grammar.NewRule("T").AddToken("n");
        grammar.NewRule("T").AddToken("+").AddTerm("T");
        grammar.NewRule("T").AddTerm("T").AddToken("+").AddToken("n");

        Writer log = new();
        Inspector.Validate(grammar, log);
        grammar = Normalizer.GetNormal(grammar, log);

        Console.WriteLine(grammar.ToString()); // TODO: REMOVE

        ParserStates states = new();
        try {
            states.DetermineStates(grammar, OnConflict.Panic, log);
        } finally {
            Console.WriteLine(states.ToString());
        }

        if (log.Failed)
            throw new ParserException("Errors while building parser:" + Environment.NewLine + log.ToString());
        Table table = states.CreateTable();
        Parser parser = new(table, grammar, tok);
        parser.Grammar.Check(
            "> <$StartTerm>",
            "<T> → [+] <T> <T'0>",
            "   | [n] <T'0>",
            "<T'0> → λ",
            "   | [+] [n] <T'0>",
            "<$StartTerm> → <T> [$EOFToken]");

        states.Check();
        //table.Check();

        parser.Check("103",
            "─<E>",
            "  └─<T>",
            "     ├─[n:(Unnamed:1, 1, 1):\"103\"]",
            "     └─<T'0>");

        parser.Check("+2",
            "─<E>",
            "  └─<T>",
            "     ├─[+:(Unnamed:1, 1, 1):\"+\"]",
            "     ├─<T>",
            "     │  ├─[n:(Unnamed:1, 2, 2):\"2\"]",
            "     │  └─<T'0>",
            "     └─<T'0>");

        parser.Check("3+4",
            "─<E>",
            "  └─<T>",
            "     ├─[n:(Unnamed:1, 1, 1):\"3\"]",
            "     └─<T'0>",
            "        ├─[+:(Unnamed:1, 2, 2):\"+\"]",
            "        ├─[n:(Unnamed:1, 3, 3):\"4\"]",
            "        └─<T'0>");

        parser.Check("((42+6))",
            "─<E>",
            "  ├─[(:(Unnamed:1, 1, 1):\"(\"]",
            "  ├─<E>",
            "  │  ├─[(:(Unnamed:1, 2, 2):\"(\"]",
            "  │  ├─<E>",
            "  │  │  └─<T>",
            "  │  │     ├─[n:(Unnamed:1, 3, 3):\"42\"]",
            "  │  │     └─<T'0>",
            "  │  │        ├─[+:(Unnamed:1, 5, 5):\"+\"]",
            "  │  │        ├─[n:(Unnamed:1, 6, 6):\"6\"]",
            "  │  │        └─<T'0>",
            "  │  └─[):(Unnamed:1, 7, 7):\")\"]",
            "  └─[):(Unnamed:1, 8, 8):\")\"]");
    }

    [TestMethod]
    public void Parser02() {
        Tokenizer tok = new();
        tok.Start("start");
        tok.Join("start", "(").AddSet("(");
        tok.Join("start", ")").AddSet(")");
        tok.SetToken("(", "(");
        tok.SetToken(")", ")");
        // 1. X → ( X )
        // 2. X → ( )
        Grammar grammar = new();
        grammar.Start("X");
        grammar.NewRule("X").AddToken("(").AddTerm("X").AddToken(")");
        grammar.NewRule("X").AddToken("(").AddToken(")");
        Parser parser = new(grammar, tok);

        parser.Check("()",
            "─<X>",
            "  ├─[(:(Unnamed:1, 1, 1):\"(\"]",
            "  └─[):(Unnamed:1, 2, 2):\")\"]");

        parser.Check("((()))",
            "─<X>",
            "  ├─[(:(Unnamed:1, 1, 1):\"(\"]",
            "  ├─<X>",
            "  │  ├─[(:(Unnamed:1, 2, 2):\"(\"]",
            "  │  ├─<X>",
            "  │  │  ├─[(:(Unnamed:1, 3, 3):\"(\"]",
            "  │  │  └─[):(Unnamed:1, 4, 4):\")\"]",
            "  │  └─[):(Unnamed:1, 5, 5):\")\"]",
            "  └─[):(Unnamed:1, 6, 6):\")\"]");
    }

    [TestMethod]
    public void Parser03() {
        Tokenizer tok = new();
        tok.Start("start");
        tok.Join("start", "(").AddSet("(");
        tok.Join("start", ")").AddSet(")");
        tok.SetToken("(", "(");
        tok.SetToken(")", ")");
        // 1. X → ( X )
        // 2. X → λ
        Grammar grammar = new();
        grammar.Start("X");
        grammar.NewRule("X").AddToken("(").AddTerm("X").AddToken(")");
        grammar.NewRule("X");
        Parser parser = new(grammar, tok);

        parser.Check("",
            "─<X>");

        parser.Check("()",
            "─<X>",
            "  ├─[(:(Unnamed:1, 1, 1):\"(\"]",
            "  ├─<X>",
            "  └─[):(Unnamed:1, 2, 2):\")\"]");

        parser.Check("((()))",
            "─<X>",
            "  ├─[(:(Unnamed:1, 1, 1):\"(\"]",
            "  ├─<X>",
            "  │  ├─[(:(Unnamed:1, 2, 2):\"(\"]",
            "  │  ├─<X>",
            "  │  │  ├─[(:(Unnamed:1, 3, 3):\"(\"]",
            "  │  │  ├─<X>",
            "  │  │  └─[):(Unnamed:1, 4, 4):\")\"]",
            "  │  └─[):(Unnamed:1, 5, 5):\")\"]",
            "  └─[):(Unnamed:1, 6, 6):\")\"]");
    }

    [TestMethod]
    public void Parser04() {
        Tokenizer tok = new();
        tok.Start("start");
        tok.Join("start", "a").AddSet("a");
        tok.Join("start", "b").AddSet("b");
        tok.Join("start", "d").AddSet("d");
        tok.SetToken("a", "a");
        tok.SetToken("b", "b");
        tok.SetToken("d", "d");
        // 1. S → b A d S
        // 2. S → λ
        // 3. A → a A
        // 4. A → λ
        Grammar grammar = new();
        grammar.Start("S");
        grammar.NewRule("S").AddToken("b").AddTerm("A").AddToken("d").AddTerm("S");
        grammar.NewRule("S");
        grammar.NewRule("A").AddToken("a").AddTerm("A");
        grammar.NewRule("A");
        Parser parser = new(grammar, tok);

        parser.Check("bd",
            "─<S>",
            "  ├─[b:(Unnamed:1, 1, 1):\"b\"]",
            "  ├─<A>",
            "  ├─[d:(Unnamed:1, 2, 2):\"d\"]",
            "  └─<S>");

        parser.Check("bad",
            "─<S>",
            "  ├─[b:(Unnamed:1, 1, 1):\"b\"]",
            "  ├─<A>",
            "  │  ├─[a:(Unnamed:1, 2, 2):\"a\"]",
            "  │  └─<A>",
            "  ├─[d:(Unnamed:1, 3, 3):\"d\"]",
            "  └─<S>");

        parser.Check("bdbadbd",
            "─<S>",
            "  ├─[b:(Unnamed:1, 1, 1):\"b\"]",
            "  ├─<A>",
            "  ├─[d:(Unnamed:1, 2, 2):\"d\"]",
            "  └─<S>",
            "     ├─[b:(Unnamed:1, 3, 3):\"b\"]",
            "     ├─<A>",
            "     │  ├─[a:(Unnamed:1, 4, 4):\"a\"]",
            "     │  └─<A>",
            "     ├─[d:(Unnamed:1, 5, 5):\"d\"]",
            "     └─<S>",
            "        ├─[b:(Unnamed:1, 6, 6):\"b\"]",
            "        ├─<A>",
            "        ├─[d:(Unnamed:1, 7, 7):\"d\"]",
            "        └─<S>");
    }

    [TestMethod]
    public void Parser05() {
        Tokenizer tok = new();
        tok.Start("start");
        tok.Join("start", "id").AddRange("a", "z");
        tok.Join("id", "id").AddRange("a", "z");
        tok.Join("start", "add").AddSet("+");
        tok.Join("start", "multiply").AddSet("*");
        tok.Join("start", "open").AddSet("(");
        tok.Join("start", "close").AddSet(")");
        tok.Join("start", "start").SetConsume(true).AddSet(" ");
        tok.SetToken("add", "+");
        tok.SetToken("multiply", "*");
        tok.SetToken("open", "(");
        tok.SetToken("close", ")");
        tok.SetToken("id", "id");
        // 1. E → E + E
        // 2. E → E * E
        // 3. E → ( E )
        // 4. E → id
        Grammar grammar = new();
        grammar.Start("E");
        grammar.NewRule("E").AddTerm("E").AddToken("+").AddTerm("E");
        grammar.NewRule("E").AddTerm("E").AddToken("*").AddTerm("E");
        grammar.NewRule("E").AddToken("(").AddTerm("E").AddToken(")");
        grammar.NewRule("E").AddToken("id");
        Parser parser = new(grammar, tok);
        parser.Grammar.Check(
            "> <$StartTerm>",
            "<E> → [(] <E> [)] <E'0>",
            "   | [id] <E'0>",
            "<E'0> → λ",
            "   | [*] <E> <E'0>",
            "   | [+] <E> <E'0>",
            "<$StartTerm> → <E> [$EOFToken]");

        parser.Check("a",
            "─<E>",
            "  ├─[id:(Unnamed:1, 1, 1):\"a\"]",
            "  └─<E'0>");

        parser.Check("(a + b)",
            "─<E>",
            "  ├─[(:(Unnamed:1, 1, 1):\"(\"]",
            "  ├─<E>",
            "  │  ├─[id:(Unnamed:1, 2, 2):\"a\"]",
            "  │  └─<E'0>",
            "  │     ├─[+:(Unnamed:1, 3, 3):\"+\"]",
            "  │     ├─<E>",
            "  │     │  ├─[id:(Unnamed:1, 5, 5):\"b\"]",
            "  │     │  └─<E'0>",
            "  │     └─<E'0>",
            "  ├─[):(Unnamed:1, 7, 7):\")\"]",
            "  └─<E'0>");

        parser.Check("a + b * c",
            "─<E>",
            "  ├─[id:(Unnamed:1, 1, 1):\"a\"]",
            "  └─<E'0>",
            "     ├─[+:(Unnamed:1, 2, 2):\"+\"]",
            "     ├─<E>",
            "     │  ├─[id:(Unnamed:1, 4, 4):\"b\"]",
            "     │  └─<E'0>",
            "     │     ├─[*:(Unnamed:1, 6, 6):\"*\"]",
            "     │     ├─<E>",
            "     │     │  ├─[id:(Unnamed:1, 8, 8):\"c\"]",
            "     │     │  └─<E'0>",
            "     │     └─<E'0>",
            "     └─<E'0>");

        parser.Check("a + (b * c) + d",
            "─<E>",
            "  ├─[id:(Unnamed:1, 1, 1):\"a\"]",
            "  └─<E'0>",
            "     ├─[+:(Unnamed:1, 2, 2):\"+\"]",
            "     ├─<E>",
            "     │  ├─[(:(Unnamed:1, 4, 4):\"(\"]",
            "     │  ├─<E>",
            "     │  │  ├─[id:(Unnamed:1, 6, 6):\"b\"]",
            "     │  │  └─<E'0>",
            "     │  │     ├─[*:(Unnamed:1, 7, 7):\"*\"]",
            "     │  │     ├─<E>",
            "     │  │     │  ├─[id:(Unnamed:1, 9, 9):\"c\"]",
            "     │  │     │  └─<E'0>",
            "     │  │     └─<E'0>",
            "     │  ├─[):(Unnamed:1, 11, 11):\")\"]",
            "     │  └─<E'0>",
            "     │     ├─[+:(Unnamed:1, 12, 12):\"+\"]",
            "     │     ├─<E>",
            "     │     │  ├─[id:(Unnamed:1, 14, 14):\"d\"]",
            "     │     │  └─<E'0>",
            "     │     └─<E'0>",
            "     └─<E'0>");
    }

    [TestMethod]
    public void Parser06() {
        Tokenizer tok = new();
        tok.Start("start");
        tok.Join("start", "a").AddSet("a");
        tok.SetToken("a", "a");

        Grammar grammar = new();
        grammar.Start("E");
        grammar.NewRule("E");
        grammar.NewRule("E").AddTerm("E").AddTerm("T");
        grammar.NewRule("T").AddToken("a");
        Parser parser = new(grammar, tok);
        parser.Grammar.Check(
            "> <$StartTerm>",
            "<E> → <E'0>",
            "<T> → [a]",
            "<E'0> → λ",
            "   | <T> <E'0>",
            "<$StartTerm> → <E> [$EOFToken]");

        parser.Check("aaa",
            "─<E>",
            "  └─<E'0>",
            "     ├─<T>",
            "     │  └─[a:(Unnamed:1, 1, 1):\"a\"]",
            "     └─<E'0>",
            "        ├─<T>",
            "        │  └─[a:(Unnamed:1, 2, 2):\"a\"]",
            "        └─<E'0>",
            "           ├─<T>",
            "           │  └─[a:(Unnamed:1, 3, 3):\"a\"]",
            "           └─<E'0>");
    }

    [TestMethod]
    public void Parser07() {
        Tokenizer tok = new();
        tok.Start("start");
        tok.JoinToToken("start", "*").AddSet("*");

        Grammar grammar = new();
        grammar.Start("E");
        grammar.NewRule("E");
        grammar.NewRule("E").AddTerm("T").AddTerm("E");
        grammar.NewRule("T").AddToken("*");

        Parser parser = new(grammar, tok);
        parser.Check("",
            "─<E>");
        parser.Check("*",
            "─<E>",
            "  ├─<T>",
            "  │  └─[*:(Unnamed:1, 1, 1):\"*\"]",
            "  └─<E>");
        parser.Check("***",
            "─<E>",
            "  ├─<T>",
            "  │  └─[*:(Unnamed:1, 1, 1):\"*\"]",
            "  └─<E>",
            "     ├─<T>",
            "     │  └─[*:(Unnamed:1, 2, 2):\"*\"]",
            "     └─<E>",
            "        ├─<T>",
            "        │  └─[*:(Unnamed:1, 3, 3):\"*\"]",
            "        └─<E>");
    }

    [TestMethod]
    public void Parser08() {
        Tokenizer tok = new();
        tok.Start("start");
        tok.Join("start", "a").AddSet("a");
        tok.SetToken("a", "a");

        Grammar grammar = new();
        grammar.Start("S");
        grammar.NewRule("S").AddTerm("E").AddTerm("E");
        grammar.NewRule("E").AddToken("a");
        Parser parser = new(grammar, tok);

        parser.Check("aa",
            "─<S>",
            "  ├─<E>",
            "  │  └─[a:(Unnamed:1, 1, 1):\"a\"]",
            "  └─<E>",
            "     └─[a:(Unnamed:1, 2, 2):\"a\"]");
    }

    [TestMethod]
    public void Parser09() {
        // See: http://www.cs.ecu.edu/karl/5220/spr16/Notes/Bottom-up/lr1.html

        Tokenizer tok = new();
        tok.Start("start");
        tok.Join("start", "c").AddSet("c");
        tok.SetToken("c", "c");
        tok.Join("start", "d").AddSet("d");
        tok.SetToken("d", "d");

        Grammar grammar = new();
        grammar.Start("S");
        grammar.NewRule("S").AddTerm("C").AddTerm("C");
        grammar.NewRule("C").AddToken("c").AddTerm("C");
        grammar.NewRule("C").AddToken("d");
        Parser parser = new(grammar, tok);

        parser.Check("dd",
            "─<S>",
            "  ├─<C>",
            "  │  └─[d:(Unnamed:1, 1, 1):\"d\"]",
            "  └─<C>",
            "     └─[d:(Unnamed:1, 2, 2):\"d\"]");

        parser.Check("cdd",
            "─<S>",
            "  ├─<C>",
            "  │  ├─[c:(Unnamed:1, 1, 1):\"c\"]",
            "  │  └─<C>",
            "  │     └─[d:(Unnamed:1, 2, 2):\"d\"]",
            "  └─<C>",
            "     └─[d:(Unnamed:1, 3, 3):\"d\"]");

        parser.Check("dcd",
            "─<S>",
            "  ├─<C>",
            "  │  └─[d:(Unnamed:1, 1, 1):\"d\"]",
            "  └─<C>",
            "     ├─[c:(Unnamed:1, 2, 2):\"c\"]",
            "     └─<C>",
            "        └─[d:(Unnamed:1, 3, 3):\"d\"]");

        parser.Check("cdcd",
            "─<S>",
            "  ├─<C>",
            "  │  ├─[c:(Unnamed:1, 1, 1):\"c\"]",
            "  │  └─<C>",
            "  │     └─[d:(Unnamed:1, 2, 2):\"d\"]",
            "  └─<C>",
            "     ├─[c:(Unnamed:1, 3, 3):\"c\"]",
            "     └─<C>",
            "        └─[d:(Unnamed:1, 4, 4):\"d\"]");
    }
}
