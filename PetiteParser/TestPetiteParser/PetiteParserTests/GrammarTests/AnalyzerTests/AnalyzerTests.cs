using Microsoft.VisualStudio.TestTools.UnitTesting;
using PetiteParser.Grammar;

namespace TestPetiteParser.PetiteParserTests.GrammarTests.AnalyzerTests;

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
        ana.CheckFollows(r5, 0, true, "", "<T'0> → • λ");
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
        gram.NewRule("A", "<A> [a]");
        gram.CheckFindFirstLeftRecursion("A");
    }

    [TestMethod]
    public void FindLeftRecursion02() {
        Grammar gram = new();
        gram.NewRule("A", "<B> [b]");
        gram.NewRule("B", "<A> [a]");
        gram.CheckFindFirstLeftRecursion("A", "B");
    }

    [TestMethod]
    public void FindLeftRecursion03() {
        Grammar gram = new();
        gram.NewRule("A", "<B> <A>");
        gram.NewRule("B", "[a]");
        gram.CheckFindFirstLeftRecursion();

        // By adding a lambda B, then A can be reached and be recursive.
        gram.NewRule("B");
        gram.CheckFindFirstLeftRecursion("A");
    }

    [TestMethod]
    public void FindLeftRecursion04() {
        Grammar gram = new();
        gram.NewRule("A", "<B> [a]");
        gram.NewRule("A", "<E> [a]");
        gram.NewRule("B", "<C> [b]");
        gram.NewRule("C", "<D> [c]");
        gram.NewRule("D", "<A> [d]");
        gram.NewRule("E", "[e]");
        gram.CheckFindFirstLeftRecursion("A", "B", "C", "D");
    }

    [TestMethod]
    public void FindConflictPoint01() {
        Grammar gram = new();
        gram.NewRule("X", "[a] [b] <Y> [c]");
        gram.NewRule("Y", "[c]");
        gram.CheckFindConflictPoint();

        gram.NewRule("Y");
        gram.CheckFindConflictPoint("<X> → [a] [b] • <Y> [c]");
    }

    [TestMethod]
    public void FindConflictPoint02() {
        Grammar gram = new();
        gram.NewRule("X", "[a] <Y> [c] <Y> [c]");
        gram.NewRule("Y", "[c]");
        gram.CheckFindConflictPoint();

        gram.NewRule("Y");
        gram.CheckFindConflictPoint(
            "<X> → [a] • <Y> [c] <Y> [c]",
            "<X> → [a] <Y> [c] • <Y> [c]");
    }

    [TestMethod]
    public void FindConflictPoint03() {
        Grammar gram = new();
        gram.NewRule("X", "<Y> [a]");
        gram.NewRule("X", "[b] [d] <Y> [a]");
        gram.NewRule("Y");
        gram.NewRule("Y", "[a] [d] <Y>");
        gram.NewRule("Y", "[c] <Y>");

        gram.CheckFindConflictPoint(
            "<X> → • <Y> [a]",
            "<X> → [b] [d] • <Y> [a]");
    }

    [TestMethod]
    public void FindConflictPoint04() {
        Grammar gram = new();
        gram.NewRule("X", "<Y> [a]");
        gram.NewRule("Y", "<Z> [a]");
        gram.NewRule("Z");

        gram.CheckFindConflictPoint(
            "--");
    }

    [TestMethod]
    public void FindConflictPoint05() {
        Grammar gram = new();
        gram.NewRule("X", "<Y> [a]");
        gram.NewRule("Y", "<Z>");
        gram.NewRule("Z");
        gram.NewRule("Z", "[a]");

        gram.CheckFindConflictPoint(
            "--");
    }
}
