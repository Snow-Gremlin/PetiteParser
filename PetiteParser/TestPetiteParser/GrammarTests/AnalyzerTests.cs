﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using PetiteParser.Grammar;
using PetiteParser.Grammar.Analyzer;
using TestPetiteParser.Tools;

namespace TestPetiteParser.GrammarTests;

[TestClass]
sealed public class AnalyzerTests {

    [TestMethod]
    public void Analyzer01Firsts() {
        Grammar gram = new();
        gram.NewRule("E", "<T>");
        gram.NewRule("E", "[(] <E> [)]");
        gram.NewRule("T", "[+] <T> <T'0>");
        gram.NewRule("T", "[n] <T'0>");
        gram.NewRule("T'0");
        gram.NewRule("T'0", "[+] [n] <T'0>");
        gram.NewRule("$StartTerm", "<E> [$EOFToken]");
        gram.Start("$StartTerm");

        gram.CheckFirstSets(
            "┌────────────┬─────────┬───┐",
            "│ Term       │ Firsts  │ λ │",
            "├────────────┼─────────┼───┤",
            "│ $StartTerm │ (, +, n │   │",
            "│ E          │ (, +, n │   │",
            "│ T          │ +, n    │   │",
            "│ T'0        │ +       │ x │",
            "└────────────┴─────────┴───┘");
    }

    /*
    // TODO: Update test to use fragments.
    [TestMethod]
    public void Analyzer02Follows() {
        Grammar gram = new();
        Rule r1 = gram.NewRule("E", "<T>");
        Rule r2 = gram.NewRule("E", "[(] <E> [)]");
        Rule r3 = gram.NewRule("T", "[+] <T> <T'0>");
        Rule r4 = gram.NewRule("T", "[n] <T'0>");
        Rule r5 = gram.NewRule("T'0");
        Rule r6 = gram.NewRule("T'0", "[+] [n] <T'0>");
        Rule r7 = gram.NewRule("$StartTerm", "<E> [$EOFToken]");
        gram.Start("$StartTerm");
        gram.Check(
            "> <$StartTerm>",
            "<E> → <T>",
            "   | [(] <E> [)]",
            "<T> → [+] <T> <T'0>",
            "   | [n] <T'0>",
            "<T'0> → λ",
            "   | [+] [n] <T'0>",
            "<$StartTerm> → <E> [$EOFToken]");
        Analyzer ana = new(gram);
        ana.CheckFollows(r1, 0, true, "", "<E> → • <T>");
        ana.CheckFollows(r1, 1, true, "", "<E> → <T> •");
        ana.CheckFollows(r2, 0, false, "[(] [+] [n]", "<E> → • [(] <E> [)]");
        ana.CheckFollows(r2, 1, false, "[)]",         "<E> → [(] • <E> [)]");
        ana.CheckFollows(r2, 2, true,  "",            "<E> → [(] <E> • [)]");
        ana.CheckFollows(r2, 3, true,  "",            "<E> → [(] <E> [)] •");
        ana.CheckFollows(r3, 0, false, "[+] [n]", "<T> → • [+] <T> <T'0>");
        ana.CheckFollows(r3, 1, true,  "[+]",     "<T> → [+] • <T> <T'0>");
        ana.CheckFollows(r3, 2, true,  "",        "<T> → [+] <T> • <T'0>");
        ana.CheckFollows(r3, 3, true,  "",        "<T> → [+] <T> <T'0> •");
        ana.CheckFollows(r4, 0, true, "[+]", "<T> → • [n] <T'0>");
        ana.CheckFollows(r4, 1, true, "",    "<T> → [n] • <T'0>");
        ana.CheckFollows(r4, 2, true, "",    "<T> → [n] <T'0> •");
        ana.CheckFollows(r5, 0, true, "", "<T'0> → λ •");
        ana.CheckFollows(r6, 0, false, "[n]", "<T'0> → • [+] [n] <T'0>");
        ana.CheckFollows(r6, 1, true,  "[+]", "<T'0> → [+] • [n] <T'0>");
        ana.CheckFollows(r6, 2, true,  "",    "<T'0> → [+] [n] • <T'0>");
        ana.CheckFollows(r6, 3, true,  "",    "<T'0> → [+] [n] <T'0> •");
        ana.CheckFollows(r7, 0, false, "[$EOFToken]", "<$StartTerm> → • <E> [$EOFToken]");
        ana.CheckFollows(r7, 1, true,  "",            "<$StartTerm> → <E> • [$EOFToken]");
        ana.CheckFollows(r7, 2, true,  "",            "<$StartTerm> → <E> [$EOFToken] •");
    }
    */

    [TestMethod]
    public void FindLeftRecursion01() {
        Grammar gram = new();
        gram.NewRule("A").AddTerm("A").AddToken("a");
        gram.CheckFindFirstLeftRecursion("A");
    }

    [TestMethod]
    public void FindLeftRecursion02() {
        Grammar gram = new();
        gram.NewRule("A").AddTerm("B").AddToken("b");
        gram.NewRule("B").AddTerm("A").AddToken("a");
        gram.CheckFindFirstLeftRecursion("A", "B");
    }

    [TestMethod]
    public void FindLeftRecursion03() {
        Grammar gram = new();
        gram.NewRule("A").AddTerm("B").AddTerm("A");
        gram.NewRule("B").AddToken("a");
        gram.CheckFindFirstLeftRecursion();

        // By adding a lambda B, then A can be reached and be recursive.
        gram.NewRule("B");
        gram.CheckFindFirstLeftRecursion("A");
    }

    [TestMethod]
    public void FindLeftRecursion04() {
        Grammar gram = new();
        gram.NewRule("A").AddTerm("B").AddToken("a");
        gram.NewRule("A").AddTerm("E").AddToken("a");
        gram.NewRule("B").AddTerm("C").AddToken("b");
        gram.NewRule("C").AddTerm("D").AddToken("c");
        gram.NewRule("D").AddTerm("A").AddToken("d");
        gram.NewRule("E").AddToken("e");
        gram.CheckFindFirstLeftRecursion("A", "B", "C", "D");
    }
}
