﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using PetiteParser.Analyzer;
using PetiteParser.Grammar;
using PetiteParser.Logger;
using TestPetiteParser.Tools;

namespace TestPetiteParser.UnitTests;

[TestClass]
sealed public class GrammarTests {

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

        gram.Check(
            "> <defSet>",
            "<defSet> → <defSet> <def>",
            "   | λ",
            "<def> → <stateDef> <defBody>",
            "<stateDef> → [closeAngle]",
            "   | λ",
            "<defBody> → <stateOrTokenID>",
            "   | <defBody> [colon] [arrow] <stateOrTokenID>",
            "<stateOrTokenID> → <stateID>",
            "   | <tokenID>",
            "<stateID> → [openParen] [id] [closeParen]",
            "<tokenID> → [openBracket] [id] [closeBracket]");

        gram.CheckFirstSets(
            "def            → [closeAngle, openBracket, openParen]",
            "defBody        → [openBracket, openParen]",
            "defSet         → [closeAngle, openBracket, openParen] λ",
            "stateDef       → [closeAngle] λ",
            "stateID        → [openParen]",
            "stateOrTokenID → [openBracket, openParen]",
            "tokenID        → [openBracket]");
    }

    [TestMethod]
    public void Grammar2() {
        Grammar gram = new();
        Rule rule0 = gram.NewRule("E");
        Rule rule1 = gram.NewRule("E").AddTerm("E").AddToken("+").AddTerm("E");
        Rule rule2 = gram.NewRule("E").AddTerm("E").AddToken("+").AddTerm("E").AddPrompt("add");
        Rule rule3 = gram.NewRule("E").AddTerm("E").AddToken("+").AddPrompt("add").AddTerm("E");
        Rule rule4 = gram.NewRule("E").AddPrompt("add").AddTerm("E").AddToken("+").AddTerm("E");

        rule0.CheckString(-1, "<E> → λ");
        rule0.CheckString( 0, "<E> → λ •");
        rule0.CheckString( 1, "<E> → λ");

        rule1.CheckString(-1, "<E> → <E> [+] <E>");
        rule1.CheckString( 0, "<E> → • <E> [+] <E>");
        rule1.CheckString( 1, "<E> → <E> • [+] <E>");
        rule1.CheckString( 2, "<E> → <E> [+] • <E>");
        rule1.CheckString( 3, "<E> → <E> [+] <E> •");
        rule1.CheckString( 4, "<E> → <E> [+] <E>");

        rule2.CheckString(-1, "<E> → <E> [+] <E> {add}");
        rule2.CheckString( 0, "<E> → • <E> [+] <E> {add}");
        rule2.CheckString( 1, "<E> → <E> • [+] <E> {add}");
        rule2.CheckString( 2, "<E> → <E> [+] • <E> {add}");
        rule2.CheckString( 3, "<E> → <E> [+] <E> • {add}");
        rule2.CheckString( 4, "<E> → <E> [+] <E> {add}");

        rule3.CheckString(-1, "<E> → <E> [+] {add} <E>");
        rule3.CheckString( 0, "<E> → • <E> [+] {add} <E>");
        rule3.CheckString( 1, "<E> → <E> • [+] {add} <E>");
        rule3.CheckString( 2, "<E> → <E> [+] • {add} <E>");
        rule3.CheckString( 3, "<E> → <E> [+] {add} <E> •");
        rule3.CheckString( 4, "<E> → <E> [+] {add} <E>");

        rule4.CheckString(-1, "<E> → {add} <E> [+] <E>");
        rule4.CheckString( 0, "<E> → • {add} <E> [+] <E>");
        rule4.CheckString( 1, "<E> → {add} <E> • [+] <E>");
        rule4.CheckString( 2, "<E> → {add} <E> [+] • <E>");
        rule4.CheckString( 3, "<E> → {add} <E> [+] <E> •");
        rule4.CheckString( 4, "<E> → {add} <E> [+] <E>");
    }

    [TestMethod]
    public void Grammar3() {
        Grammar gram = new();
        gram.NewRule("C");
        gram.NewRule("C").AddTerm("X").AddTerm("C");
        gram.NewRule("X").AddToken("A");
        gram.NewRule("X").AddToken("B");

        gram.Check(
            "> <C>",
            "<C> → λ",
            "   | <X> <C>",
            "<X> → [A]",
            "   | [B]");

        gram.CheckFirstSets(
            "C → [A, B] λ",
            "X → [A, B]");
    }

    [TestMethod]
    public void Normalize1_RemoveDirectLeftRecursion() {
        Grammar gram = new();
        gram.Start("E");
        gram.NewRule("E").AddToken("n");
        gram.NewRule("E").AddTerm("E").AddToken("+").AddTerm("E");
        gram.Check(
            "> <E>",
            "<E> → [n]",
            "   | <E> [+] <E>");

        Buffered log = new();
        Grammar normal = Analyzer.Normalize(gram, log);
        log.Check(
            "Notice: Sorted the rules for <E>.",
            "Notice: Found first left recursion in [<E>].");
        normal.Check(
            "> <E>",
            "<E> → [n] <E'0>",
            "<E'0> → λ",
            "   | [+] <E> <E'0>");
    }

    [TestMethod]
    public void Normalize2_RemoveDirectLeftRecursion() {
        Grammar gram = new();
        gram.Start("E");
        gram.NewRule("E").AddTerm("T");
        gram.NewRule("E").AddToken("(").AddTerm("E").AddToken(")");
        gram.NewRule("T").AddToken("n");
        gram.NewRule("T").AddToken("+").AddTerm("T");
        gram.NewRule("T").AddTerm("T").AddToken("+").AddToken("n");
        gram.Check(
            "> <E>",
            "<E> → <T>",
            "   | [(] <E> [)]",
            "<T> → [n]",
            "   | [+] <T>",
            "   | <T> [+] [n]");

        Buffered log = new();
        Grammar normal = Analyzer.Normalize(gram, log);
        log.Check(
            "Notice: Sorted the rules for <T>.",
            "Notice: Found first left recursion in [<T>].");
        normal.Check( 
            "> <E>",
            "<E> → <T>",
            "   | [(] <E> [)]",
            "<T> → [+] <T> <T'0>",
            "   | [n] <T'0>",
            "<T'0> → λ",
            "   | [+] [n] <T'0>");
    }

    [TestMethod]
    public void Normalize3_RemoveProductionlessRule() {
        Grammar gram = new();
        gram.Start("E");
        gram.NewRule("E").AddTerm("E");
        gram.NewRule("E").AddToken("(").AddTerm("E").AddToken(")");

        Buffered log = new();
        Grammar normal = Analyzer.Normalize(gram, log);
        log.Check(
            "Notice: Removed 1 unproductive rules from <E>.");
        normal.Check(
            "> <E>",
            "<E> → [(] <E> [)]");
    }

    [TestMethod]
    public void Normalize4_RemoveDuplecateRule() {
        Grammar gram = new();
        gram.Start("E");
        gram.NewRule("E").AddTerm("E");
        gram.NewRule("E").AddToken("(").AddTerm("E").AddToken(")");
        gram.NewRule("E").AddToken("(").AddTerm("E").AddToken(")");
        gram.NewRule("E").AddToken("(").AddTerm("E").AddToken(")");
        Grammar normal = Analyzer.Normalize(gram);
        normal.Check(
            "> <E>",
            "<E> → [(] <E> [)]");
    }

    [TestMethod]
    public void LeftRecursion1() {
        Grammar gram = new();
        gram.NewRule("A").AddTerm("A").AddToken("a");
        gram.CheckFindFirstLeftRecursion("A");
    }

    [TestMethod]
    public void LeftRecursion2() {
        Grammar gram = new();
        gram.NewRule("A").AddTerm("B").AddToken("b");
        gram.NewRule("B").AddTerm("A").AddToken("a");
        gram.CheckFindFirstLeftRecursion("A", "B");
    }

    [TestMethod]
    public void LeftRecursion3() {
        Grammar gram = new();
        gram.NewRule("A").AddTerm("B").AddTerm("A");
        gram.NewRule("B").AddToken("a");
        gram.CheckFindFirstLeftRecursion();

        // By adding a lambda B, then A can be reached and be recursive.
        gram.NewRule("B");
        gram.CheckFindFirstLeftRecursion("A");
    }

    [TestMethod]
    public void LeftRecursion4() {
        Grammar gram = new();
        gram.NewRule("A").AddTerm("B").AddToken("a");
        gram.NewRule("A").AddTerm("E").AddToken("a");
        gram.NewRule("B").AddTerm("C").AddToken("b");
        gram.NewRule("C").AddTerm("D").AddToken("c");
        gram.NewRule("D").AddTerm("A").AddToken("d");
        gram.NewRule("E").AddToken("e");
        gram.CheckFindFirstLeftRecursion("A", "D", "C", "B");
    }
}
