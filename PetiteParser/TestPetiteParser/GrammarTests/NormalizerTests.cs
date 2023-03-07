using Microsoft.VisualStudio.TestTools.UnitTesting;
using PetiteParser.Grammar;
using PetiteParser.Grammar.Normalizer;
using PetiteParser.Logger;
using TestPetiteParser.Tools;

namespace TestPetiteParser.GrammarTests;

[TestClass]
sealed public class NormalizerTests {

    [TestMethod]
    public void DirectLeftRecursion01() {
        Grammar gram = new();
        gram.Start("E");
        gram.NewRule("E").AddToken("n");
        gram.NewRule("E").AddTerm("E").AddToken("+").AddTerm("E");
        gram.Check(
            "> <E>",
            "<E> → [n]",
            "   | <E> [+] <E>");

        Buffered log = new();
        Grammar normal = Normalizer.GetNormal(gram, log);
        log.Check(
            "Notice: Sorted the rules for <E>.",
            "Notice: Found first left recursion in [<E>].");
        normal.Check(
            "> <E>",
            "<E> → [n] <E'0>",
            "<E'0> → λ",
            "   | [+] <E> <E'0>");
        normal.CheckNoStateConflicts();
    }

    [TestMethod]
    public void DirectLeftRecursion02() {
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
        Grammar normal = Normalizer.GetNormal(gram, log);
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
        normal.CheckNoStateConflicts();
    }

    [TestMethod]
    public void RemoveProductionlessRule01() {
        Grammar gram = new();
        gram.Start("E");
        gram.NewRule("E").AddTerm("E");
        gram.NewRule("E").AddToken("(").AddTerm("E").AddToken(")");

        Buffered log = new();
        Grammar normal = Normalizer.GetNormal(gram, log);
        log.Check(
            "Notice: Removed 1 unproductive rules from <E>.");
        normal.Check(
            "> <E>",
            "<E> → [(] <E> [)]");
        normal.CheckNoStateConflicts();
    }

    [TestMethod]
    public void RemoveDuplecateRule01() {
        Grammar gram = new();
        gram.Start("E");
        gram.NewRule("E").AddTerm("E");
        gram.NewRule("E").AddToken("(").AddTerm("E").AddToken(")");
        gram.NewRule("E").AddToken("(").AddTerm("E").AddToken(")");
        gram.NewRule("E").AddToken("(").AddTerm("E").AddToken(")");
        Grammar normal = Normalizer.GetNormal(gram);
        normal.Check(
            "> <E>",
            "<E> → [(] <E> [)]");
        normal.CheckNoStateConflicts();
    }

    [TestMethod]
    public void SortRules01() {
        Grammar g1 = new();
        g1.NewRule("C").AddItems("[a] [dog]");
        g1.NewRule("C").AddItems("[a] [cat]");
        g1.NewRule("A").AddItems("[a]");
        g1.NewRule("A").AddItems("[a] [b]");
        g1.NewRule("A").AddItems("[a] [a]");
        g1.NewRule("A").AddItems("[a] <C>");
        g1.NewRule("A").AddItems("[a] [c]");
        g1.NewRule("A").AddItems("[apple]");
        g1.NewRule("A").AddItems("[at]");
        g1.NewRule("A").AddItems("[c]");
        g1.NewRule("A").AddItems("[c] <C>");

        Grammar g2 = Normalizer.GetNormal(g1);
        g2.Check(
            "> <C>",
            "<C> → [a] [cat]",
            "   | [a] [dog]",
            "<A> → [a]",
            "   | [a] <C>",
            "   | [a] [a]",
            "   | [a] [b]",
            "   | [a] [c]",
            "   | [apple]",
            "   | [at]",
            "   | [c]",
            "   | [c] <C>");
        g2.CheckNoStateConflicts();
    }

    [TestMethod]
    public void RemoveSingleUseRule01() {
        Grammar g1 = new();
        g1.NewRule("A").AddItems("<B> <C>");
        g1.NewRule("B").AddItems("[b]");
        g1.NewRule("C").AddItems("[c] [c]");

        Grammar g2 = Normalizer.GetNormal(g1);
        g2.Check(
            "> <A>",
            "<A> → [b] [c] [c]");
        g2.CheckNoStateConflicts();
    }

    [TestMethod]
    public void ConflictResolution01() {
        // 1. E → T
        // 2. E → ( E )
        // 3. T → n
        // 4. T → + T
        // 5. T → T + n
        Grammar g1 = new();
        g1.Start("E");
        g1.NewRule("E").AddTerm("T");
        g1.NewRule("E").AddToken("(").AddTerm("E").AddToken(")");
        g1.NewRule("T").AddToken("n");
        g1.NewRule("T").AddToken("+").AddTerm("T");
        g1.NewRule("T").AddTerm("T").AddToken("+").AddToken("n");

        Grammar g2 = Normalizer.GetNormal(g1, new Writer());
        g2.Check(
            "> <E>",
            "<E> → <T>",
            "   | [(] <E> [)]",
            "<T> → [+] <T> <T'0>",
            "   | [n] <T'0>",
            "<T'0> → λ",
            "   | [+] [n] <T'0>");

        // This caused a conflict in the following state and [+] with "reduce <T'0> → λ" and "shift":
        //   1. <T>   → [n] • <T'0>     @ [$EOFToken] [+]
        //   2. <T'0> → λ •             @ [$EOFToken] [+]
        //   3. <T'0> → • [+] [n] <T'0> @ [$EOFToken] [+]
        // The [+] seen for the reduction is caused by a possible <T'0> following the <T> in #1.
        // In this case we should take the "shift" in #3 because it is already in a <T'0>.
        // We should bias towards a shift over reduce in the cases where the follow
        // is from the same term as the shift is from.
        g2.CheckNoStateConflicts(true);
    }

    [TestMethod]
    public void EliminationLeftRecursion01() {
        // Problem-01 from https://www.gatevidyalay.com/left-recursion-left-recursion-elimination/
        // Problem: A → ABd / Aa / a
        //          B → Be / b
        // Solution: A → aA’
        //           A’ → BdA’ / aA’ / λ
        //           B → bB’
        //           B’ → eB’ / λ
        // Note: B gets removed as single use rule.

        Grammar g1 = new();
        g1.NewRule("A").AddItems("<A><B>[d]");
        g1.NewRule("A").AddItems("<A>[a]");
        g1.NewRule("A").AddItems("[a]");
        g1.NewRule("B").AddItems("<B>[e]");
        g1.NewRule("B").AddItems("[b]");

        Grammar g2 = Normalizer.GetNormal(g1, new Writer());
        g2.Check(
            "> <A>",
            "<A> → [a] <A'0>",
            "<A'0> → λ",
            "   | [a] <A'0>",
            "   | [b] <B'0> [d] <A'0>",
            "<B'0> → λ",
            "   | [e] <B'0>");
        g2.CheckNoStateConflicts();
    }

    [TestMethod]
    public void EliminationLeftRecursion02() {
        // Problem-02 from https://www.gatevidyalay.com/left-recursion-left-recursion-elimination/
        // Problem: E → E+E / ExE / a
        // Solution: E → aA
        //           A → +EA / xEA / λ

        Grammar g1 = new();
        g1.NewRule("E").AddItems("<E>[+]<E>");
        g1.NewRule("E").AddItems("<E>[x]<E>");
        g1.NewRule("E").AddItems("[a]");

        Grammar g2 = Normalizer.GetNormal(g1, new Writer());
        g2.Check(
            "> <E>",
            "<E> → [a] <E'0>",
            "<E'0> → λ",
            "   | [+] <E> <E'0>",
            "   | [x] <E> <E'0>");
        g2.CheckNoStateConflicts();
    }

    [TestMethod]
    public void EliminationLeftRecursion03() {
        // Problem-03 from https://www.gatevidyalay.com/left-recursion-left-recursion-elimination/
        // Problem: E → E + T / T
        //          T → T x F / F
        //          F → id
        // Solution: E → TE’
        //           E’ → +TE’ / λ
        //           T → FT’
        //           T’ → xFT’ / λ
        //           F → id
        // Note: Removed mono-productive term, <F>.

        Grammar g1 = new();
        g1.NewRule("E").AddItems("<E>[+]<T>");
        g1.NewRule("E").AddItems("<T>");
        g1.NewRule("T").AddItems("<T>[x]<F>");
        g1.NewRule("T").AddItems("<F>");
        g1.NewRule("F").AddItems("[id]");

        Grammar g2 = Normalizer.GetNormal(g1, new Writer());
        g2.Check(
            "> <E>",
            "<E> → [id] <T'0> <E'0>",
            "<E'0> → λ",
            "   | [+] [id] <T'0> <E'0>",
            "<T'0> → λ",
            "   | [x] [id] <T'0>");
        g2.CheckNoStateConflicts();
    }

    [TestMethod]
    public void EliminationLeftRecursion04() {
        // Problem-04 from https://www.gatevidyalay.com/left-recursion-left-recursion-elimination/
        // Problem: S → (L) / a
        //          L → L,S / S
        // Solution: S → (L) / a
        //           L → SL’
        //           L’ → ,SL’ / λ
        // Note: Removed single use rule for <L>.

        Grammar g1 = new();
        g1.NewRule("S").AddItems("[(]<L>[)]");
        g1.NewRule("S").AddItems("[a]");
        g1.NewRule("L").AddItems("<L>[,]<S>");
        g1.NewRule("L").AddItems("<S>");

        Grammar g2 = Normalizer.GetNormal(g1, new Writer());
        g2.Check(
            "> <S>",
            "<S> → [(] <S> <L'0> [)]",
            "   | [a]",
            "<L'0> → λ",
            "   | [,] <S> <L'0>");
        g2.CheckNoStateConflicts();
    }

    [TestMethod]
    public void EliminationLeftRecursion05() {
        // Problem-05 from https://www.gatevidyalay.com/left-recursion-left-recursion-elimination/
        // Problem: S → S 0 S 1 S / 0 1
        // Solution: S → 0 1 A
        //           A → 0 S 1 S A / λ

        Grammar g1 = new();
        g1.NewRule("S").AddItems("<S>[0]<S>[1]<S>");
        g1.NewRule("S").AddItems("[0][1]");

        Grammar g2 = Normalizer.GetNormal(g1, new Writer());
        g2.Check(
            "> <S>",
            "<S> → [0] [1] <S'0>",
            "<S'0> → λ",
            "   | [0] <S> [1] <S> <S'0>");
        g2.CheckNoStateConflicts();
    }

    [TestMethod]
    public void EliminationLeftRecursion06() {
        // Problem-06 from https://www.gatevidyalay.com/left-recursion-left-recursion-elimination/
        // Problem: S → A
        //          A → Ad / Ae / aB / ac
        //          B → bBc / f
        // Solution: S → A
        //           A → aBA’ / acA’
        //           A’ → dA’ / eA’ / λ
        //           B → bBc / f

        // TODO: <S> could be mono-productive where it could be replaced by <A>.

        Grammar g1 = new();
        g1.NewRule("S").AddItems("<A>");
        g1.NewRule("A").AddItems("<A>[d]");
        g1.NewRule("A").AddItems("<A>[e]");
        g1.NewRule("A").AddItems("[a]<B>");
        g1.NewRule("A").AddItems("[a][c]");
        g1.NewRule("B").AddItems("[b]<B>[c]");
        g1.NewRule("B").AddItems("[f]");

        Grammar g2 = Normalizer.GetNormal(g1, new Writer());
        g2.Check(
            "> <S>",
            "<S> → <A>",
            "<A> → [a] <B> <A'0>",
            "   | [a] [c] <A'0>",
            "<B> → [b] <B> [c]",
            "   | [f]",
            "<A'0> → λ",
            "   | [d] <A'0>",
            "   | [e] <A'0>");
        g2.CheckNoStateConflicts();
    }

    [TestMethod]
    public void EliminationLeftRecursion07() {
        // Problem-07 from https://www.gatevidyalay.com/left-recursion-left-recursion-elimination/
        // Problem: A → AAα / β
        // Solution: A → βA’
        //           A’ → AαA’ / λ

        Grammar g1 = new();
        g1.NewRule("A").AddItems("<A><A>[α]");
        g1.NewRule("A").AddItems("[β]");

        Grammar g2 = Normalizer.GetNormal(g1, new Writer());
        g2.Check(
            "> <A>",
            "<A> → [β] <A'0>",
            "<A'0> → λ",
            "   | <A> [α] <A'0>");
        g2.CheckNoStateConflicts();
    }

    [TestMethod]
    public void EliminationLeftRecursion08() {
        // Problem-08 from https://www.gatevidyalay.com/left-recursion-left-recursion-elimination/
        // Problem: A → Ba / Aa / c
        //          B → Bb / Ab / d
        // Step 1: Remove A → Aa
        //         A → BaA’ / cA’
        //         A’ → aA’ / λ
        //         B → Bb / Ab / d
        // Step 2: Substituting the productions of A in B → Ab
        //         A → BaA’ / cA’
        //         A’ → aA’ / λ
        //         B → Bb / BaA’b / cA’b / d
        // Step 3: Remove B → Ba / BaA’b
        // Solution: A → BaA’ / cA’
        //           A’ → aA’ / λ
        //           B → cA’bB’ / dB’
        //           B’ → bB’ / aA’bB’ / λ

        Grammar g1 = new();
        g1.NewRule("A").AddItems("<B>[a]");
        g1.NewRule("A").AddItems("<A>[a]");
        g1.NewRule("A").AddItems("[c]");
        g1.NewRule("B").AddItems("<B>[b]");
        g1.NewRule("B").AddItems("<A>[b]");
        g1.NewRule("B").AddItems("[d]");

        // This had a conflict the [a] in "<A> → • <B> [a] <A'0>" with "<B> → [c] <A'0> [b] • <B'0>"
        // since both "<B'0> → • [a] <A'0> [b] <B'0>" or "<B'0> → λ •" will work for this as a shift
        // or reduce respectfully.
        Grammar g2 = Normalizer.GetNormal(g1, new Writer());
        g2.Check(
            "> <A>",
            "<A> → <B> [a] <A'0>",
            "   | [c] <A'0>",
            "<B> → [c] <A'0> [b] <B'0>",
            "   | [d] <B'0>",
            "<A'0> → λ",
            "   | [a] <A'0>",
            "<B'0> → λ",
            "   | [a] <A'0> [b] <B'0>",
            "   | [b] <B'0>");
        g2.CheckNoStateConflicts();
    }

    [TestMethod]
    public void EliminationLeftRecursion09() {
        // Problem-09 from https://www.gatevidyalay.com/left-recursion-left-recursion-elimination/
        // Problem:
        //   X → XSb / Sa / b
        //   S → Sb / Xa / a
        // Solution:
        //   X → SaX’ / bX’
        //   X’ → SbX’ / λ
        //   S → bX’aS’ / aS’
        //   S’ → bS’ / aX’aS’ / λ
        // Then removing lambda conflict (not part of problem-09).
        // The conflict from being able to reduce with X’ → λ in S → bX’aS’
        // or for S → aS’ in X’ → SbX’ for the lookahead [a].

        Grammar g1 = new();
        g1.NewRule("X").AddItems("<X><S>[b]");
        g1.NewRule("X").AddItems("<S>[a]");
        g1.NewRule("X").AddItems("[b]");
        g1.NewRule("S").AddItems("<S>[b]");
        g1.NewRule("S").AddItems("<X>[a]");
        g1.NewRule("S").AddItems("[a]");

        Grammar g2 = Normalizer.GetNormal(g1, new Writer());
        g2.Check(
            "> <X>",
            "<X> → <S> [a] <X'0>",
            "   | [b] <X'0>",
            "<S> → [a] <S'0>",
            "   | [b] <X'1> <S'0>",
            "<X'0> → λ",
            "   | <S> [b] <X'0>",
            "<S'0> → λ",
            "   | [a] <X'1> <S'0>",
            "   | [b] <S'0>",
            "<X'1> → λ",
            "   | <S> [b] <X'1>",
            "   | [a]");
        g2.CheckNoStateConflicts();
    }

    [TestMethod]
    public void EliminationLeftRecursion10() {
        // Problem-10 from https://www.gatevidyalay.com/left-recursion-left-recursion-elimination/
        // Problem:
        //   S → Aa / b
        //   A → Ac / Sd / λ
        // Solution:
        //   S → Aa / b
        //   A → bdA’ / A’
        //   A’ → cA’ / adA’ / λ
        // Then removing lambda conflict (not part of problem-10).
        // The lambda conflict is for <A> at 0 in `<S> → <A> [a]`.

        Grammar g1 = new();
        g1.NewRule("S").AddItems("<A>[a]");
        g1.NewRule("S").AddItems("[b]");
        g1.NewRule("A").AddItems("<A>[c]");
        g1.NewRule("A").AddItems("<S>[d]");
        g1.NewRule("A");

        Grammar g2 = Normalizer.GetNormal(g1, new Writer());
        g2.Check(
            "> <S>",
            "<S> → <A>",
            "   | [b]",
            "<A> → <A'0>",
            "   | [a]",
            "   | [b] [d] <A'0>",
            "<A'0> → λ",
            "   | [a] [d] <A'0>",
            "   | [c] <A'0>");
        g2.CheckNoStateConflicts();
    }

    // TODO: Add this test
    // https://stackoverflow.com/questions/15999916/step-by-step-elimination-of-this-indirect-left-recursion
    // A -> Cd
    // B -> Ce
    // C -> A | B | f
    //
    // One possible response:
    // A -> Cd
    // B -> Ce
    // C -> fC'
    // C' -> dC' | eC' | eps

    [TestMethod]
    public void RemoveLambdaConflict01() {
        Grammar g1 = new();
        g1.NewRule("P").AddItems("<S>");
        g1.NewRule("P").AddItems("<S> [;]");
        g1.NewRule("S").AddItems("<B>");
        g1.NewRule("S").AddItems("<S> [;] <B>");
        g1.NewRule("B").AddItems("[a]");
        g1.NewRule("B").AddItems("[b]");
        // Grammar allows: ( (a|b) ; )* (a|b) ;?
        g1.Check(
            "> <P>",
            "<P> → <S>",
            "   | <S> [;]", // <-- Will cause a reduce for [;]
            "<S> → <B>",
            "   | <S> [;] <B>", // <-- Will cause a shift for [;]
            "<B> → [a]",
            "   | [b]");

        // Substituted <S> with a new rule otherwise the states
        // can't tell "<P> → <B> <S'0>"     with "<S'0> → [;] <B> <S'0>"
        // and        "<P> → <B> <S'0> [;]" with "λ",
        // without the lookahead after ";", i.e. LR(k) with k > 1.
        Grammar g2 = Normalizer.GetNormal(g1, new Writer());
        g2.Check(
            "> <P>",
            "<P> → <B> <S'0>",
            "<B> → [a]",
            "   | [b]",
            "<S'0> → λ",
            "   | [;]",
            "   | [;] <B> <S'0>");
        g2.CheckNoStateConflicts();
    }

    [TestMethod]
    public void InlineOneSingleUseTail01() {
        Grammar g1 = new();
        g1.NewRule("S").AddItems("[a] [b] [c] <P> [d] [e] [f]");
        g1.NewRule("P");
        g1.NewRule("P").AddItems("[d]");
        g1.Check(
             "> <S>",
             "<S> → [a] [b] [c] <P> [d] [e] [f]",
             "<P> → λ",
             "   | [d]");

        Grammar g2 = Normalizer.GetNormal(g1, new Writer());
        g2.Check(
            "> <S>",
            "<S> → [a] [b] [c] <P>",
            "<P> → [d] [d] [e] [f]",
            "   | [d] [e] [f]");
        g2.CheckNoStateConflicts();
    }

    [TestMethod]
    public void InlineOneSingleUseTail02() {
        Grammar g1 = new();
        g1.NewRule("S").AddItems("[a] [b] [c] <P> [d] [e] [f]");
        g1.NewRule("P").AddItems("<H>");
        g1.NewRule("P").AddItems("[g]");
        g1.NewRule("H");
        g1.NewRule("H").AddItems("[d]");
        g1.Check(
             "> <S>",
             "<S> → [a] [b] [c] <P> [d] [e] [f]",
             "<P> → <H>",
             "   | [g]",
             "<H> → λ",
             "   | [d]");

        Grammar g2 = Normalizer.GetNormal(g1, new Writer());
        g2.Check(
            "> <S>",
            "<S> → [a] [b] [c] <P>",
            "<P> → <H>",
            "   | [g] [d] [e] [f]",
            "<H> → [d] [d] [e] [f]",
            "   | [d] [e] [f]");
        g2.CheckNoStateConflicts();
    }

    [TestMethod]
    public void InlineOneSingleUseTail03() {
        Grammar g1 = new();
        g1.NewRule("S").AddItems("[a] [b] <P> [c] [d] <P> [c] [e] <P> [c] [f]");
        g1.NewRule("P");
        g1.NewRule("P").AddItems("[c]");
        g1.Check(
             "> <S>",
             "<S> → [a] [b] <P> [c] [d] <P> [c] [e] <P> [c] [f]",
             "<P> → λ",
             "   | [c]");

        Grammar g2 = Normalizer.GetNormal(g1, new Writer());
        g2.Check(
            "> <S>",
            "<S> → [a] [b] <P'0>",
            "<P> → [c] [c] [f]",
            "   | [c] [f]",
            "<P'0> → [c] [c] [d] <P'1>",
            "   | [c] [d] <P'1>",
            "<P'1> → [c] [c] [e] <P>",
            "   | [c] [e] <P>");
        g2.CheckNoStateConflicts();
    }
}
