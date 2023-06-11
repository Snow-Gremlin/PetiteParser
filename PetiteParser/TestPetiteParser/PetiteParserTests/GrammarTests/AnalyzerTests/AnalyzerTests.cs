using Microsoft.VisualStudio.TestTools.UnitTesting;
using PetiteParser.Grammar;
using PetiteParser.Grammar.Analyzer;
using PetiteParser.Parser.States;
using TestPetiteParser.Tools;

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

    [TestMethod]
    public void Analyzer02Follows() {
        Grammar gram = new();
        Rule r1 = gram.NewRule("$StartTerm", "<E> [$EOFToken]");
        Rule r2 = gram.NewRule("E", "<T>");
        Rule r3 = gram.NewRule("E", "[(] <E> [)]");
        Rule r4 = gram.NewRule("T", "[+] <T> <T'0>");
        Rule r5 = gram.NewRule("T", "[n] <T'0>");
        Rule r6 = gram.NewRule("T'0");
        Rule r7 = gram.NewRule("T'0", "[+] [n] <T'0>");
        gram.Start("$StartTerm");
        gram.Check(
            "> <$StartTerm>",
            "<$StartTerm> → <E> [$EOFToken]",
            "<E> → <T>",
            "   | [(] <E> [)]",
            "<T> → [+] <T> <T'0>",
            "   | [n] <T'0>",
            "<T'0> → λ",
            "   | [+] [n] <T'0>");
        Analyzer ana = new(gram);

        Fragment f1a = Fragment.NewRootRule(r1).
            Check("<$StartTerm> → • <E> [$EOFToken] @ [$EOFToken]", null, false).
            CheckNext("<E>", "[$EOFToken]").
            CheckFollows(ana, "[$EOFToken]");
        Fragment f1b = Fragment.NextFragment(f1a).
            Check("<$StartTerm> → <E> • [$EOFToken] @ [$EOFToken]", null, false).
            CheckNext("[$EOFToken]", "").
            CheckFollows(ana, "[$EOFToken]");
        Fragment f1c = Fragment.NextFragment(f1b).
            Check("<$StartTerm> → <E> [$EOFToken] • @ [$EOFToken]", null, true).
            CheckNext("null", "").
            CheckFollows(ana, "[$EOFToken]");
        TestTools.ThrowsException(() => Assert.IsNull(Fragment.NextFragment(f1c)),
            "May not get the next fragment for <$StartTerm> → <E> [$EOFToken] • @ [$EOFToken], it is at the end.");

        Fragment f2a = Fragment.NewRule(r2, f1a, ana.Follows(f1a)).
            Check("<E> → • <T> @ [$EOFToken]", f1a, false).
            CheckNext("<T>", "").
            CheckFollows(ana, "[$EOFToken]");
        Fragment f2b = Fragment.NextFragment(f2a).
            Check("<E> → <T> • @ [$EOFToken]", f1a, true).
            CheckNext("null", "").
            CheckFollows(ana, "[$EOFToken]");

        Fragment f3a = Fragment.NewRule(r3, f1a, ana.Follows(f1a)).
            Check("<E> → • [(] <E> [)] @ [$EOFToken]", f1a, false).
            CheckNext("[(]", "<E>, [)]").
            CheckFollows(ana, "[(], [+], [n]");
        Fragment f3b = Fragment.NextFragment(f3a).
            Check("<E> → [(] • <E> [)] @ [$EOFToken]", f1a, false).
            CheckNext("<E>", "[)]").
            CheckFollows(ana, "[)]");
        Fragment f3c = Fragment.NextFragment(f3b).
            Check("<E> → [(] <E> • [)] @ [$EOFToken]", f1a, false).
            CheckNext("[)]", "").
            CheckFollows(ana, "[$EOFToken]");
    }

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
        gram.NewRule("Y");
        gram.NewRule("Z");

        gram.CheckFindConflictPoint(
            "<X> → • <Y> [a]");
    }

    [TestMethod]
    public void FindConflictPoint05() {
        Grammar gram = new();
        gram.NewRule("X", "<Y> [a]");
        gram.NewRule("Y", "<Z>");
        gram.NewRule("Z");
        gram.NewRule("Z", "[a]");

        gram.CheckFindConflictPoint(
            "<X> → • <Y> [a]");
    }
}
