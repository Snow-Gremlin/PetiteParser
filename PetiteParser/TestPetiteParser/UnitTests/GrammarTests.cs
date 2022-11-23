using Microsoft.VisualStudio.TestTools.UnitTesting;
using PetiteParser.Analyzer;
using PetiteParser.Grammar;
using PetiteParser.Logger;
using PetiteParser.Normalizer;
using PetiteParser.Parser;
using PetiteParser.Parser.States;
using System;
using TestPetiteParser.Tools;

namespace TestPetiteParser.UnitTests;

[TestClass]
sealed public class GrammarTests {
    
    [TestMethod]
    public void Analyzer1() {
        Grammar gram = new();
        Rule r1 = gram.NewRule("E").AddTerm("T");
        Rule r2 = gram.NewRule("E").AddToken("(").AddTerm("E").AddToken(")");
        Rule r3 = gram.NewRule("T").AddToken("+").AddTerm("T").AddTerm("T'0");
        Rule r4 = gram.NewRule("T").AddToken("n").AddTerm("T'0");
        Rule r5 = gram.NewRule("T'0");
        Rule r6 = gram.NewRule("T'0").AddToken("+").AddToken("n").AddTerm("T'0");
        Rule r7 = gram.NewRule("$StartTerm").AddTerm("E").AddToken("$EOFToken");
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
        ana.CheckFirsts("<E>", false, "[(] [+] [n]");
        ana.CheckFirsts("<T>", false, "[+] [n]");
        ana.CheckFirsts("<T'0>", true, "[+]");

        ana.CheckFollows(r1, 0, "x", "[x]"); // <E> → • <T>
        ana.CheckFollows(r1, 1, "x", "[x]"); // <E> → <T> •
        
        ana.CheckFollows(r2, 0, "x", "[(] [+] [n]"); // <E> → • [(] <E> [)]
        ana.CheckFollows(r2, 1, "x", "[)]");         // <E> → [(] • <E> [)]
        ana.CheckFollows(r2, 2, "x", "[x]");         // <E> → [(] <E> • [)]
        ana.CheckFollows(r2, 3, "x", "[x]");         // <E> → [(] <E> [)] •

        ana.CheckFollows(r3, 0, "x", "[+] [n]"); // <T> → • [+] <T> <T'0>
        ana.CheckFollows(r3, 1, "x", "[+] [x]"); // <T> → [+] • <T> <T'0>
        ana.CheckFollows(r3, 2, "x", "[x]");     // <T> → [+] <T> • <T'0>
        ana.CheckFollows(r3, 3, "x", "[x]");     // <T> → [+] <T> <T'0> •
        
        ana.CheckFollows(r4, 0, "x", "[+] [x]"); // <T> → • [n] <T'0>
        ana.CheckFollows(r4, 1, "x", "[x]");     // <T> → [n] • <T'0>
        ana.CheckFollows(r4, 2, "x", "[x]");     // <T> → [n] <T'0> •

        ana.CheckFollows(r5, 0, "x", "[x]"); // <T'0> → λ •
        
        ana.CheckFollows(r6, 0, "x", "[n]");     // <T'0> → • [+] [n] <T'0>
        ana.CheckFollows(r6, 1, "x", "[+] [x]"); // <T'0> → [+] • [n] <T'0>
        ana.CheckFollows(r6, 2, "x", "[x]");     // <T'0> → [+] [n] • <T'0>
        ana.CheckFollows(r6, 3, "x", "[x]");     // <T'0> → [+] [n] <T'0> •

        ana.CheckFollows(r7, 0, "x", "[$EOFToken]"); // <$StartTerm> → • <E> [$EOFToken]
        ana.CheckFollows(r7, 1, "x", "[x]");         // <$StartTerm> → <E> • [$EOFToken]
        ana.CheckFollows(r7, 2, "x", "[x]");         // <$StartTerm> → <E> [$EOFToken] •
    }

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
    public void Grammar4LookAheads() {
        Grammar g1 = new();
        g1.Start("E");
        g1.NewRule("E").AddTerm("T");
        g1.NewRule("E").AddToken("(").AddTerm("E").AddToken(")");
        g1.NewRule("T").AddToken("n");
        g1.NewRule("T").AddToken("+").AddTerm("T");
        g1.NewRule("T").AddTerm("T").AddToken("+").AddToken("n");
        Grammar g2 = Normalizer.GetNormal(g1);
        g2.Check(
            "> <E>",
            "<E> → <T>",
            "   | [(] <E> [)]",
            "<T> → [+] <T> <T'0>",
            "   | [n] <T'0>",
            "<T'0> → λ",
            "   | [+] [n] <T'0>");

        Analyzer ana = new(g2);
        ana.CheckFirsts("<E>", false, "[(] [+] [n]");
        ana.CheckFirsts("<T>", false, "[+] [n]");
        ana.CheckFirsts("<T'0>", true, "[+]");

        //ana.CheckClosureLookAheads();
        // TODO: Check Lookaheads too
    }

    [TestMethod]
    public void Normalize1RemoveDirectLeftRecursion() {
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
    }

    [TestMethod]
    public void Normalize2RemoveDirectLeftRecursion() {
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
    }

    [TestMethod]
    public void Normalize3RemoveProductionlessRule() {
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
    }

    [TestMethod]
    public void Normalize4RemoveDuplecateRule() {
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
    }

    [TestMethod]
    public void Normilize5SortRules() {
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
    }

    [TestMethod]
    public void Normilize6RemoveSingleUseRule() {
        Grammar g1 = new();
        g1.NewRule("A").AddItems("<B> <C>");
        g1.NewRule("B").AddItems("[b]");
        g1.NewRule("C").AddItems("[c] [c]");

        Grammar g2 = Normalizer.GetNormal(g1);
        g2.Check(
            "> <A>",
            "<A> → [b] [c] [c]");
    }

    [TestMethod]
    public void Normilize7ConflictResolution() {
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
        
        Grammar g2 = Normalizer.GetNormal(g1);
        g2.Check(
            "> <E>",
            "<E> → <T>",
            "   | [(] <E> [)]",
            "<T> → [+] <T> <T'0>",
            "   | [n] <T'0>",
            "<T'0> → λ",
            "   | [+] [n] <T'0>");

        ParserStates states = new();
        states.DetermineStates(g2, OnConflict.Panic, new Writer());
        states.Check();
    }

    [TestMethod]
    public void LeftRecursion01() {
        Grammar gram = new();
        gram.NewRule("A").AddTerm("A").AddToken("a");
        gram.CheckFindFirstLeftRecursion("A");
    }

    [TestMethod]
    public void LeftRecursion02() {
        Grammar gram = new();
        gram.NewRule("A").AddTerm("B").AddToken("b");
        gram.NewRule("B").AddTerm("A").AddToken("a");
        gram.CheckFindFirstLeftRecursion("A", "B");
    }

    [TestMethod]
    public void LeftRecursion03() {
        Grammar gram = new();
        gram.NewRule("A").AddTerm("B").AddTerm("A");
        gram.NewRule("B").AddToken("a");
        gram.CheckFindFirstLeftRecursion();

        // By adding a lambda B, then A can be reached and be recursive.
        gram.NewRule("B");
        gram.CheckFindFirstLeftRecursion("A");
    }

    [TestMethod]
    public void LeftRecursion04() {
        Grammar gram = new();
        gram.NewRule("A").AddTerm("B").AddToken("a");
        gram.NewRule("A").AddTerm("E").AddToken("a");
        gram.NewRule("B").AddTerm("C").AddToken("b");
        gram.NewRule("C").AddTerm("D").AddToken("c");
        gram.NewRule("D").AddTerm("A").AddToken("d");
        gram.NewRule("E").AddToken("e");
        gram.CheckFindFirstLeftRecursion("A", "D", "C", "B");
    }

    [TestMethod]
    public void LeftRecursion05_Elimination() {
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

        // TODO: REMOVE
        ParserStates states = new();
        states.DetermineStates(g2, OnConflict.Panic, new Writer());
        states.Check();
    }

    [TestMethod]
    public void LeftRecursion06_Elimination() {
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
    }

    [TestMethod]
    public void LeftRecursion07_Elimination() {
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
            "<E> → <T> <E'0>",
            "<T> → [id] <T'0>",
            "<E'0> → λ",
            "   | [+] <T> <E'0>",
            "<T'0> → λ",
            "   | [x] [id] <T'0>");
    }
    
    [TestMethod]
    public void LeftRecursion08_Elimination() {
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
    }
    
    [TestMethod]
    public void LeftRecursion09_Elimination() {
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
    }

    [TestMethod]
    public void LeftRecursion10_Elimination() {
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
    }
    
    [TestMethod]
    public void LeftRecursion11_Elimination() {
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
    }
    
    [TestMethod]
    public void LeftRecursion12_Elimination() {
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
        Grammar g2 = g1.Copy();

        // Perform Step 1
        g2.CheckFindFirstLeftRecursion("A");
        g2.StepRemoveLeftRecursion();
        g2.Check(
            "> <A>",
            "<A> → <B> [a] <A'0>",
            "   | [c] <A'0>",
            "<B> → <B> [b]",
            "   | <A> [b]",
            "   | [d]",
            "<A'0> → λ",
            "   | [a] <A'0>");

        // Perform Step 2
        g2.CheckFindFirstLeftRecursion("A", "B");
        g2.StepRemoveLeftRecursion();
        g2.Check(
            "> <A>",
            "<A> → <B> [a] <A'0>",
            "   | [c] <A'0>",
            "<B> → <B> [b]",
            "   | <B> [a] <A'0> [b]",
            "   | [c] <A'0> [b]",
            "   | [d]",
            "<A'0> → λ",
            "   | [a] <A'>");

        Grammar g3 = Normalizer.GetNormal(g1, new Writer());
        g3.Check(
            ""); // TODO: FIX: I got a difference answer
    }
    
    [TestMethod]
    public void LeftRecursion13_Elimination() {
        // Problem-09 from https://www.gatevidyalay.com/left-recursion-left-recursion-elimination/
        // Problem: X → XSb / Sa / b
        //          S → Sb / Xa / a
        // Solution: X → SaX’ / bX’
        //           X’ → SbX’ / λ
        //           S → bX’aS’ / aS’
        //           S’ → bS’ / aX’aS’ / λ
        
        Grammar g1 = new();
        g1.NewRule("X").AddItems("<X><S>[b]");
        g1.NewRule("X").AddItems("<S>[a]");
        g1.NewRule("X").AddItems("[b]");
        g1.NewRule("S").AddItems("<S>[b]");
        g1.NewRule("S").AddItems("<X>[a]");
        g1.NewRule("S").AddItems("[a]");

        Grammar g2 = Normalizer.GetNormal(g1, new Writer());
        g2.Check(
            ""); // TODO: FIX: I got a difference answer
    }
    
    [TestMethod]
    public void LeftRecursion14_Elimination() {
        // Problem-10 from https://www.gatevidyalay.com/left-recursion-left-recursion-elimination/
        // Problem: S → Aa / b
        //          A → Ac / Sd / λ
        // Solution: S → Aa / b
        //           A → bdA’ / A’
        //           A’ → cA’ / adA’ / λ
        
        Grammar g1 = new();
        g1.NewRule("S").AddItems("<A>[a]");
        g1.NewRule("S").AddItems("[b]");
        g1.NewRule("A").AddItems("<A>[c]");
        g1.NewRule("A").AddItems("<S>[d]");
        g1.NewRule("A");

        Grammar g2 = Normalizer.GetNormal(g1, new Writer());
        g2.Check(
            ""); // TODO: FIX: I got a difference answer
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
}
