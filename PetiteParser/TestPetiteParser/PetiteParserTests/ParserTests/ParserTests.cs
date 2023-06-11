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
using TestPetiteParser.PetiteParserTests.GrammarTests;
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
            states.DetermineStates(grammar, log);
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
            "<E'0> → λ",
            "   | [a] <E'0>",
            "<$StartTerm> → <E> [$EOFToken]");

        parser.Check("aaa",
            "─<E>",
            "  └─<E'0>",
            "     ├─[a:(Unnamed:1, 1, 1):\"a\"]",
            "     └─<E'0>",
            "        ├─[a:(Unnamed:1, 2, 2):\"a\"]",
            "        └─<E'0>",
            "           ├─[a:(Unnamed:1, 3, 3):\"a\"]",
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
            "  ├─[*:(Unnamed:1, 1, 1):\"*\"]",
            "  └─<E>");
        parser.Check("***",
            "─<E>",
            "  ├─[*:(Unnamed:1, 1, 1):\"*\"]",
            "  └─<E>",
            "     ├─[*:(Unnamed:1, 2, 2):\"*\"]",
            "     └─<E>",
            "        ├─[*:(Unnamed:1, 3, 3):\"*\"]",
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
            "  ├─[a:(Unnamed:1, 1, 1):\"a\"]",
            "  └─[a:(Unnamed:1, 2, 2):\"a\"]");
    }

    [TestMethod]
    public void Parser09() {
        // See: http://www.cs.ecu.edu/karl/5220/spr16/Notes/Bottom-up/lr1.html
        // From: Page 262, Dragon Book
        // 0. S′ → S
        // 1. S  → C C
        // 2. C  → c C
        // 3. C  → d
        // Matches: "( c* d ){2}"

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
        parser.Grammar.Check(
            "> <$StartTerm>",
            "<S> → <C> <C>",
            "<C> → [c] <C>",
            "   | [d]",
            "<$StartTerm> → <S> [$EOFToken]");

        ParserStates states = new();
        states.DetermineStates(parser.Grammar, log: new Writer());
        states.Check(
            "State 0:",
            "  <$StartTerm> → • <S> [$EOFToken] @ [$EOFToken]",
            "  <S> → • <C> <C> @ [$EOFToken]",
            "  <C> → • [c] <C> @ [c] [d]",
            "  <C> → • [d] @ [c] [d]",
            "  [c]: shift 3",
            "  [d]: shift 4",
            "  <C>: goto 2",
            "  <S>: goto 1",
            "State 1:",
            "  <$StartTerm> → <S> • [$EOFToken] @ [$EOFToken]",
            "  [$EOFToken]: accept",
            "State 2:",
            "  <S> → <C> • <C> @ [$EOFToken]",
            "  <C> → • [c] <C> @ [$EOFToken]",
            "  <C> → • [d] @ [$EOFToken]",
            "  [c]: shift 6",
            "  [d]: shift 7",
            "  <C>: goto 5",
            "State 3:",
            "  <C> → [c] • <C> @ [c] [d]",
            "  <C> → • [c] <C> @ [c] [d]",
            "  <C> → • [d] @ [c] [d]",
            "  [c]: shift 3",
            "  [d]: shift 4",
            "  <C>: goto 9",
            "State 4:",
            "  <C> → [d] • @ [c] [d]",
            "  [c]: reduce <C> → [d]",
            "  [d]: reduce <C> → [d]",
            "State 5:",
            "  <S> → <C> <C> • @ [$EOFToken]",
            "  [$EOFToken]: reduce <S> → <C> <C>",
            "State 6:",
            "  <C> → [c] • <C> @ [$EOFToken]",
            "  <C> → • [c] <C> @ [$EOFToken]",
            "  <C> → • [d] @ [$EOFToken]",
            "  [c]: shift 6",
            "  [d]: shift 7",
            "  <C>: goto 8",
            "State 7:",
            "  <C> → [d] • @ [$EOFToken]",
            "  [$EOFToken]: reduce <C> → [d]",
            "State 8:",
            "  <C> → [c] <C> • @ [$EOFToken]",
            "  [$EOFToken]: reduce <C> → [c] <C>",
            "State 9:",
            "  <C> → [c] <C> • @ [c] [d]",
            "  [c]: reduce <C> → [c] <C>",
            "  [d]: reduce <C> → [c] <C>");

        parser.table.Check(
            "state ║ [$EOFToken]          │ [c]                  │ [d]                  ║ <C> │ <S>",
            "──────╫──────────────────────┼──────────────────────┼──────────────────────╫─────┼────",
            "0     ║                      │ shift 3              │ shift 4              ║ 2   │ 1  ",
            "1     ║ accept               │                      │                      ║     │    ",
            "2     ║                      │ shift 6              │ shift 7              ║ 5   │    ",
            "3     ║                      │ shift 3              │ shift 4              ║ 9   │    ",
            "4     ║                      │ reduce <C> → [d]     │ reduce <C> → [d]     ║     │    ",
            "5     ║ reduce <S> → <C> <C> │                      │                      ║     │    ",
            "6     ║                      │ shift 6              │ shift 7              ║ 8   │    ",
            "7     ║ reduce <C> → [d]     │                      │                      ║     │    ",
            "8     ║ reduce <C> → [c] <C> │                      │                      ║     │    ",
            "9     ║                      │ reduce <C> → [c] <C> │ reduce <C> → [c] <C> ║     │");

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

    [TestMethod]
    public void Parser10() {
        Tokenizer tok = new();
        tok.Start("start");
        tok.Join("start", "X").AddSet("X");
        tok.Join("start", "(").AddSet("(");
        tok.Join("start", ")").AddSet(")");
        tok.SetToken("X", "X");
        tok.SetToken("(", "(");
        tok.SetToken(")", ")");
        // 1. X → ( X )
        // 2. X → ( )
        Grammar grammar = new();
        grammar.Start("X");
        grammar.NewRule("X", "[(] <X> [)]");
        grammar.NewRule("X", "[(] [)]");

        ParserStates states = new();
        states.DetermineStates(grammar);
        states.Check(
            "State 0:",
            "  <$StartTerm> → • <X> [$EOFToken] @ [$EOFToken]",
            "  <X> → • [(] <X> [)] @ [$EOFToken]",
            "  <X> → • [(] [)] @ [$EOFToken]",
            "  [(]: shift 2",
            "  <X>: goto 1",
            "State 1:",
            "  <$StartTerm> → <X> • [$EOFToken] @ [$EOFToken]",
            "  [$EOFToken]: accept",
            "State 2:",
            "  <X> → [(] • <X> [)] @ [$EOFToken]",
            "  <X> → • [(] <X> [)] @ [)]",
            "  <X> → • [(] [)] @ [)]",
            "  <X> → [(] • [)] @ [$EOFToken]",
            "  [(]: shift 4",
            "  [)]: shift 5",
            "  <X>: goto 3",
            "State 3:",
            "  <X> → [(] <X> • [)] @ [$EOFToken]",
            "  [)]: shift 9",
            "State 4:",
            "  <X> → [(] • <X> [)] @ [)]",
            "  <X> → • [(] <X> [)] @ [)]",
            "  <X> → • [(] [)] @ [)]",
            "  <X> → [(] • [)] @ [)]",
            "  [(]: shift 4",
            "  [)]: shift 7",
            "  <X>: goto 6",
            "State 5:",
            "  <X> → [(] [)] • @ [$EOFToken]",
            "  [$EOFToken]: reduce <X> → [(] [)]",
            "State 6:",
            "  <X> → [(] <X> • [)] @ [)]",
            "  [)]: shift 8",
            "State 7:",
            "  <X> → [(] [)] • @ [)]",
            "  [)]: reduce <X> → [(] [)]",
            "State 8:",
            "  <X> → [(] <X> [)] • @ [)]",
            "  [)]: reduce <X> → [(] <X> [)]",
            "State 9:",
            "  <X> → [(] <X> [)] • @ [$EOFToken]",
            "  [$EOFToken]: reduce <X> → [(] <X> [)]");

        Parser parser = new(grammar, tok);
        parser.table.Check(
            "state ║ [(]     │ [)]                      │ [$EOFToken]              ║ <X>",
            "──────╫─────────┼──────────────────────────┼──────────────────────────╫────",
            "0     ║ shift 2 │                          │                          ║ 1  ",
            "1     ║         │                          │ accept                   ║    ",
            "2     ║ shift 4 │ shift 5                  │                          ║ 3  ",
            "3     ║         │ shift 9                  │                          ║    ",
            "4     ║ shift 4 │ shift 7                  │                          ║ 6  ",
            "5     ║         │                          │ reduce <X> → [(] [)]     ║    ",
            "6     ║         │ shift 8                  │                          ║    ",
            "7     ║         │ reduce <X> → [(] [)]     │                          ║    ",
            "8     ║         │ reduce <X> → [(] <X> [)] │                          ║    ",
            "9     ║         │                          │ reduce <X> → [(] <X> [)] ║");

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
}
