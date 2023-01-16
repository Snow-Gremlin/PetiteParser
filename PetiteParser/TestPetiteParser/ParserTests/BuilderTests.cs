using Microsoft.VisualStudio.CodeCoverage;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PetiteParser.Grammar;
using PetiteParser.Grammar.Normalizer;
using PetiteParser.Loader;
using PetiteParser.Logger;
using PetiteParser.Parser;
using PetiteParser.Parser.States;
using PetiteParser.Parser.Table;
using PetiteParser.Tokenizer;
using System.Security.Claims;
using TestPetiteParser.GrammarTests;
using TestPetiteParser.Tools;

namespace TestPetiteParser.ParserTests;

[TestClass]
public class BuilderTests {

    [TestMethod]
    public void Builder01() {
        Tokenizer tok = new();
        tok.Start("S");
        tok.JoinToToken("S", "A").AddSingle('A');
        tok.JoinToToken("S", "B").AddSingle('B');
        tok.JoinToToken("S", "C").AddSingle('C');

        Grammar grammar = new();
        grammar.Start("Program");
        grammar.NewRule("Program", "<OptionalA> <OptionalB> <OptionalC>");
        grammar.NewRule("OptionalA");
        grammar.NewRule("OptionalA", "[A]");
        grammar.NewRule("OptionalB");
        grammar.NewRule("OptionalB", "[B]");
        grammar.NewRule("OptionalC");
        grammar.NewRule("OptionalC", "[C]");
        // Language Accepts: "A?B?C?"

        ParserStates states = new();
        states.DetermineStates(grammar.Copy(), new Writer());

        states.Check(
            "State 0:",
            "  <$StartTerm> → • <Program> [$EOFToken] @ [$EOFToken]",
            "  <Program> → • <OptionalA> <OptionalB> <OptionalC> @ [$EOFToken]",
            "  <OptionalA> → λ • @ [$EOFToken] [B] [C]",
            "  <OptionalA> → • [A] @ [$EOFToken] [B] [C]",
            "  [$EOFToken]: reduce <OptionalA> → λ",
            "  [A]: shift 3",
            "  [B]: reduce <OptionalA> → λ",
            "  [C]: reduce <OptionalA> → λ",
            "  <OptionalA>: goto 2",
            "  <Program>: goto 1",
            "State 1:",
            "  <$StartTerm> → <Program> • [$EOFToken] @ [$EOFToken]",
            "  [$EOFToken]: accept",
            "State 2:",
            "  <Program> → <OptionalA> • <OptionalB> <OptionalC> @ [$EOFToken]",
            "  <OptionalB> → λ • @ [$EOFToken] [C]",
            "  <OptionalB> → • [B] @ [$EOFToken] [C]",
            "  [$EOFToken]: reduce <OptionalB> → λ",
            "  [B]: shift 5",
            "  [C]: reduce <OptionalB> → λ",
            "  <OptionalB>: goto 4",
            "State 3:",
            "  <OptionalA> → [A] • @ [$EOFToken] [B] [C]",
            "  [$EOFToken]: reduce <OptionalA> → [A]",
            "  [B]: reduce <OptionalA> → [A]",
            "  [C]: reduce <OptionalA> → [A]",
            "State 4:",
            "  <Program> → <OptionalA> <OptionalB> • <OptionalC> @ [$EOFToken]",
            "  <OptionalC> → λ • @ [$EOFToken]",
            "  <OptionalC> → • [C] @ [$EOFToken]",
            "  [$EOFToken]: reduce <OptionalC> → λ",
            "  [C]: shift 7",
            "  <OptionalC>: goto 6",
            "State 5:",
            "  <OptionalB> → [B] • @ [$EOFToken] [C]",
            "  [$EOFToken]: reduce <OptionalB> → [B]",
            "  [C]: reduce <OptionalB> → [B]",
            "State 6:",
            "  <Program> → <OptionalA> <OptionalB> <OptionalC> • @ [$EOFToken]",
            "  [$EOFToken]: reduce <Program> → <OptionalA> <OptionalB> <OptionalC>",
            "State 7:",
            "  <OptionalC> → [C] • @ [$EOFToken]",
            "  [$EOFToken]: reduce <OptionalC> → [C]");

        Table table = states.CreateTable();
        table.Check(
            "state ║ [$EOFToken]                                            │ [A]     │ [B]                      │ [C]                      ║ <OptionalA> │ <OptionalB> │ <OptionalC> │ <Program>",
            "──────╫────────────────────────────────────────────────────────┼─────────┼──────────────────────────┼──────────────────────────╫─────────────┼─────────────┼─────────────┼──────────",
            "0     ║ reduce <OptionalA> → λ                                 │ shift 3 │ reduce <OptionalA> → λ   │ reduce <OptionalA> → λ   ║ 2           │             │             │ 1        ",
            "1     ║ accept                                                 │         │                          │                          ║             │             │             │          ",
            "2     ║ reduce <OptionalB> → λ                                 │         │ shift 5                  │ reduce <OptionalB> → λ   ║             │ 4           │             │          ",
            "3     ║ reduce <OptionalA> → [A]                               │         │ reduce <OptionalA> → [A] │ reduce <OptionalA> → [A] ║             │             │             │          ",
            "4     ║ reduce <OptionalC> → λ                                 │         │                          │ shift 7                  ║             │             │ 6           │          ",
            "5     ║ reduce <OptionalB> → [B]                               │         │                          │ reduce <OptionalB> → [B] ║             │             │             │          ",
            "6     ║ reduce <Program> → <OptionalA> <OptionalB> <OptionalC> │         │                          │                          ║             │             │             │          ",
            "7     ║ reduce <OptionalC> → [C]                               │         │                          │                          ║             │             │             │");

        Parser parser = new(table, grammar, tok);
        parser.Check("",
            "─<Program>",
            "  ├─<OptionalA>",
            "  ├─<OptionalB>",
            "  └─<OptionalC>");
        parser.Check("A",
            "─<Program>",
            "  ├─<OptionalA>",
            "  │  └─[A:(Unnamed:1, 1, 1):\"A\"]",
            "  ├─<OptionalB>",
            "  └─<OptionalC>");
        parser.Check("B",
            "─<Program>",
            "  ├─<OptionalA>",
            "  ├─<OptionalB>",
            "  │  └─[B:(Unnamed:1, 1, 1):\"B\"]",
            "  └─<OptionalC>");
        parser.Check("C",
            "─<Program>",
            "  ├─<OptionalA>",
            "  ├─<OptionalB>",
            "  └─<OptionalC>",
            "     └─[C:(Unnamed:1, 1, 1):\"C\"]");
        parser.Check("AB",
            "─<Program>",
            "  ├─<OptionalA>",
            "  │  └─[A:(Unnamed:1, 1, 1):\"A\"]",
            "  ├─<OptionalB>",
            "  │  └─[B:(Unnamed:1, 2, 2):\"B\"]",
            "  └─<OptionalC>");
        parser.Check("AC",
            "─<Program>",
            "  ├─<OptionalA>",
            "  │  └─[A:(Unnamed:1, 1, 1):\"A\"]",
            "  ├─<OptionalB>",
            "  └─<OptionalC>",
            "     └─[C:(Unnamed:1, 2, 2):\"C\"]");
        parser.Check("BC",
            "─<Program>",
            "  ├─<OptionalA>",
            "  ├─<OptionalB>",
            "  │  └─[B:(Unnamed:1, 1, 1):\"B\"]",
            "  └─<OptionalC>",
            "     └─[C:(Unnamed:1, 2, 2):\"C\"]");
        parser.Check("ABC",
            "─<Program>",
            "  ├─<OptionalA>",
            "  │  └─[A:(Unnamed:1, 1, 1):\"A\"]",
            "  ├─<OptionalB>",
            "  │  └─[B:(Unnamed:1, 2, 2):\"B\"]",
            "  └─<OptionalC>",
            "     └─[C:(Unnamed:1, 3, 3):\"C\"]");

        parser.Check("AA",
            "Unexpected item, [A:(Unnamed:1, 2, 2):\"A\"], in state 3. Expected: $EOFToken, B, C.",
            "─<Program>",
            "  ├─<OptionalA>",
            "  │  └─[A:(Unnamed:1, 1, 1):\"A\"]",
            "  ├─<OptionalB>",
            "  └─<OptionalC>");
        parser.Check("CA",
            "Unexpected item, [A:(Unnamed:1, 2, 2):\"A\"], in state 7. Expected: $EOFToken.",
            "─<Program>",
            "  ├─<OptionalA>",
            "  ├─<OptionalB>",
            "  └─<OptionalC>",
            "     └─[C:(Unnamed:1, 1, 1):\"C\"]");
    }

    [TestMethod]
    public void Builder02() {
        Tokenizer tok = Loader.LoadTokenizer(
            "> (S);",
            "(S): '>' => [Start];",
            "(S): ',' => [Comma];",
            "(S): ';' => [End];",
            "(S): 'a' => [A];",
            "(S): 'b' => [B];",
            "(S): 'c' => [C];",
            "(S): 'd' => [D];",
            "(S): ' ' => ^[Space];");

        Grammar grammar = Loader.LoadGrammar(
            "> <Program>;",
            "<OptionalStart> := _ | [Start];",
            "<Program> := <OptionalStart> [B] <BTail> [End];",
            "<BTail>   := _ | [Comma] [B] <BTail>;",
            "<Program> := <OptionalStart> [C] <CTail> [End];",
            "<CTail>   := _ | [Comma] [C] <CTail>;",
            "<Program> := [D] [End];");

        grammar.Check(
             "> <Program>",
             "<Program> → <OptionalStart> [B] <BTail> [End]",
             "   | <OptionalStart> [C] <CTail> [End]",
             "   | [D] [End]",
             "<OptionalStart> → λ",
             "   | [Start]",
             "<BTail> → λ",
             "   | [Comma] [B] <BTail>",
             "<CTail> → λ",
             "   | [Comma] [C] <CTail>");

        ParserStates states = new();
        states.DetermineStates(grammar.Copy());
        states.Check(
            "State 0:",
            "  <$StartTerm> → • <Program> [$EOFToken] @ [$EOFToken]",
            "  <Program> → • <OptionalStart> [B] <BTail> [End] @ [$EOFToken]",
            "  <OptionalStart> → λ • @ [B]",
            "  <OptionalStart> → • [Start] @ [B]",
            "  <Program> → • <OptionalStart> [C] <CTail> [End] @ [$EOFToken]",
            "  <OptionalStart> → λ • @ [C]",
            "  <OptionalStart> → • [Start] @ [C]",
            "  <Program> → • [D] [End] @ [$EOFToken]",
            "  [B]: reduce <OptionalStart> → λ",
            "  [C]: reduce <OptionalStart> → λ",
            "  [D]: shift 4",
            "  [Start]: shift 3",
            "  <OptionalStart>: goto 2",
            "  <Program>: goto 1",
            "State 1:",
            "  <$StartTerm> → <Program> • [$EOFToken] @ [$EOFToken]",
            "  [$EOFToken]: accept",
            "State 2:",
            "  <Program> → <OptionalStart> • [B] <BTail> [End] @ [$EOFToken]",
            "  <Program> → <OptionalStart> • [C] <CTail> [End] @ [$EOFToken]",
            "  [B]: shift 5",
            "  [C]: shift 6",
            "State 3:",
            "  <OptionalStart> → [Start] • @ [B]",
            "  <OptionalStart> → [Start] • @ [C]",
            "  [B]: reduce <OptionalStart> → [Start]",
            "  [C]: reduce <OptionalStart> → [Start]",
            "State 4:",
            "  <Program> → [D] • [End] @ [$EOFToken]",
            "  [End]: shift 15",
            "State 5:",
            "  <Program> → <OptionalStart> [B] • <BTail> [End] @ [$EOFToken]",
            "  <BTail> → λ • @ [End]",
            "  <BTail> → • [Comma] [B] <BTail> @ [End]",
            "  [Comma]: shift 11",
            "  [End]: reduce <BTail> → λ",
            "  <BTail>: goto 10",
            "State 6:",
            "  <Program> → <OptionalStart> [C] • <CTail> [End] @ [$EOFToken]",
            "  <CTail> → λ • @ [End]",
            "  <CTail> → • [Comma] [C] <CTail> @ [End]",
            "  [Comma]: shift 8",
            "  [End]: reduce <CTail> → λ",
            "  <CTail>: goto 7",
            "State 7:",
            "  <Program> → <OptionalStart> [C] <CTail> • [End] @ [$EOFToken]",
            "  [End]: shift 9",
            "State 8:",
            "  <CTail> → [Comma] • [C] <CTail> @ [End]",
            "  [C]: shift 16",
            "State 9:",
            "  <Program> → <OptionalStart> [C] <CTail> [End] • @ [$EOFToken]",
            "  [$EOFToken]: reduce <Program> → <OptionalStart> [C] <CTail> [End]",
            "State 10:",
            "  <Program> → <OptionalStart> [B] <BTail> • [End] @ [$EOFToken]",
            "  [End]: shift 14",
            "State 11:",
            "  <BTail> → [Comma] • [B] <BTail> @ [End]",
            "  [B]: shift 12",
            "State 12:",
            "  <BTail> → [Comma] [B] • <BTail> @ [End]",
            "  <BTail> → λ • @ [End]",
            "  <BTail> → • [Comma] [B] <BTail> @ [End]",
            "  [Comma]: shift 11",
            "  [End]: reduce <BTail> → λ",
            "  <BTail>: goto 13",
            "State 13:",
            "  <BTail> → [Comma] [B] <BTail> • @ [End]",
            "  [End]: reduce <BTail> → [Comma] [B] <BTail>",
            "State 14:",
            "  <Program> → <OptionalStart> [B] <BTail> [End] • @ [$EOFToken]",
            "  [$EOFToken]: reduce <Program> → <OptionalStart> [B] <BTail> [End]",
            "State 15:",
            "  <Program> → [D] [End] • @ [$EOFToken]",
            "  [$EOFToken]: reduce <Program> → [D] [End]",
            "State 16:",
            "  <CTail> → [Comma] [C] • <CTail> @ [End]",
            "  <CTail> → λ • @ [End]",
            "  <CTail> → • [Comma] [C] <CTail> @ [End]",
            "  [Comma]: shift 8",
            "  [End]: reduce <CTail> → λ",
            "  <CTail>: goto 17",
            "State 17:",
            "  <CTail> → [Comma] [C] <CTail> • @ [End]",
            "  [End]: reduce <CTail> → [Comma] [C] <CTail>");

        Table table = states.CreateTable();
        Parser parser = new(table, grammar, tok);

        parser.Check("b;",
            "─<Program>",
            "  ├─<OptionalStart>",
            "  ├─[B:(Unnamed:1, 1, 1):\"b\"]",
            "  ├─<BTail>",
            "  └─[End:(Unnamed:1, 2, 2):\";\"]");
        parser.Check("b, b, b;",
            "─<Program>",
            "  ├─<OptionalStart>",
            "  ├─[B:(Unnamed:1, 1, 1):\"b\"]",
            "  ├─<BTail>",
            "  │  ├─[Comma:(Unnamed:1, 2, 2):\",\"]",
            "  │  ├─[B:(Unnamed:1, 4, 4):\"b\"]",
            "  │  └─<BTail>",
            "  │     ├─[Comma:(Unnamed:1, 5, 5):\",\"]",
            "  │     ├─[B:(Unnamed:1, 7, 7):\"b\"]",
            "  │     └─<BTail>",
            "  └─[End:(Unnamed:1, 8, 8):\";\"]");

        parser.Check("> b;",
            "─<Program>",
            "  ├─<OptionalStart>",
            "  │  └─[Start:(Unnamed:1, 1, 1):\">\"]",
            "  ├─[B:(Unnamed:1, 3, 3):\"b\"]",
            "  ├─<BTail>",
            "  └─[End:(Unnamed:1, 4, 4):\";\"]");
        parser.Check("> b, b, b;",
            "─<Program>",
            "  ├─<OptionalStart>",
            "  │  └─[Start:(Unnamed:1, 1, 1):\">\"]",
            "  ├─[B:(Unnamed:1, 3, 3):\"b\"]",
            "  ├─<BTail>",
            "  │  ├─[Comma:(Unnamed:1, 4, 4):\",\"]",
            "  │  ├─[B:(Unnamed:1, 6, 6):\"b\"]",
            "  │  └─<BTail>",
            "  │     ├─[Comma:(Unnamed:1, 7, 7):\",\"]",
            "  │     ├─[B:(Unnamed:1, 9, 9):\"b\"]",
            "  │     └─<BTail>",
            "  └─[End:(Unnamed:1, 10, 10):\";\"]");

        parser.Check("c;",
            "─<Program>",
            "  ├─<OptionalStart>",
            "  ├─[C:(Unnamed:1, 1, 1):\"c\"]",
            "  ├─<CTail>",
            "  └─[End:(Unnamed:1, 2, 2):\";\"]");
        parser.Check("c, c, c;",
            "─<Program>",
            "  ├─<OptionalStart>",
            "  ├─[C:(Unnamed:1, 1, 1):\"c\"]",
            "  ├─<CTail>",
            "  │  ├─[Comma:(Unnamed:1, 2, 2):\",\"]",
            "  │  ├─[C:(Unnamed:1, 4, 4):\"c\"]",
            "  │  └─<CTail>",
            "  │     ├─[Comma:(Unnamed:1, 5, 5):\",\"]",
            "  │     ├─[C:(Unnamed:1, 7, 7):\"c\"]",
            "  │     └─<CTail>",
            "  └─[End:(Unnamed:1, 8, 8):\";\"]");

        parser.Check("> c;",
            "─<Program>",
            "  ├─<OptionalStart>",
            "  │  └─[Start:(Unnamed:1, 1, 1):\">\"]",
            "  ├─[C:(Unnamed:1, 3, 3):\"c\"]",
            "  ├─<CTail>",
            "  └─[End:(Unnamed:1, 4, 4):\";\"]");
        parser.Check("> c, c, c;",
            "─<Program>",
            "  ├─<OptionalStart>",
            "  │  └─[Start:(Unnamed:1, 1, 1):\">\"]",
            "  ├─[C:(Unnamed:1, 3, 3):\"c\"]",
            "  ├─<CTail>",
            "  │  ├─[Comma:(Unnamed:1, 4, 4):\",\"]",
            "  │  ├─[C:(Unnamed:1, 6, 6):\"c\"]",
            "  │  └─<CTail>",
            "  │     ├─[Comma:(Unnamed:1, 7, 7):\",\"]",
            "  │     ├─[C:(Unnamed:1, 9, 9):\"c\"]",
            "  │     └─<CTail>",
            "  └─[End:(Unnamed:1, 10, 10):\";\"]");

        parser.Check("d;",
            "─<Program>",
            "  ├─[D:(Unnamed:1, 1, 1):\"d\"]",
            "  └─[End:(Unnamed:1, 2, 2):\";\"]");
    }

    [TestMethod]
    public void Builder03() {
        Parser parser = Loader.LoadParser(
            "> (S);",
            "(S): 'a'..'z' => (Id): 'a'..'z' => [Id];",
            "(S): '=' => [Assign];",
            "(S): ':' => (T): '=' => [Define];",
            "(S): ';' => [End];",
            "(S): '0'..'9' => (Number): '0'..'9' => [Number];",
            "(S): '+' => [Add];",
            "(S): '{' => [CurlOpen];",
            "(S): '}' => [CurlClose];",
            "(S): ' ' => ^[Space];",
            "",
            "[Id] = 'var'    => [Var];",
            "[Id] = 'bool'   => [Bool];",
            "[Id] = 'int'    => [Int];",
            "[Id] = 'double' => [Double];",
            "[Id] = 'define' => [DeclareDefine];",
            "",
            "> <Start>;",
            "<OptionalVar> := _ | [Var];",
            "<RootType>    := [Bool] | [Int] | [Double];",
            "<Value>       := [Number] | [Id];",
            "<Start> := <DefineDefine> [End]",
            "    | [DeclareDefine] <DefineAssign> [End]",
            "    | [DeclareDefine] [CurlOpen] <DefineGroup> [CurlClose]",
            "    | <Equation> [End];",
            "<DefineDefine> := <RootType> <DefineDefinePart> {typeDefine}",
            "    | <OptionalVar> <DefineDefinePart> {varDefine};",
            "<DefineDefinePart> := [Id] {defineId} [Define] <Equation>;",
            "<DefineAssign> := <RootType> <DefineAssignPart> {typeDefine}",
            "    | <OptionalVar> <DefineAssignPart> {varDefine};",
            "<DefineAssignPart> := [Id] {defineId} [Assign] <Equation>;",
            "<DefineGroup> := _ | <DefineAssign> [End] <DefineGroup>;",
            "<Equation> := <Value> <EquationTail>;",
            "<EquationTail> := _ | [Add] <Value> <EquationTail>;");
        
        ParserStates states = new();
        states.DetermineStates(parser.Grammar.Copy());
        states.Check();

        // TODO: WHY? :...(



        parser.Check("a := 0;",
            "─<Start>",
            "  ├─<DefineDefine>",
            "  │  ├─<OptionalVar>",
            "  │  ├─<DefineDefinePart>",
            "  │  │  ├─[Id:(Unnamed:1, 1, 1):\"a\"]",
            "  │  │  ├─{defineId}",
            "  │  │  ├─[Define:(Unnamed:1, 3, 3):\":=\"]",
            "  │  │  └─<Equation>",
            "  │  │     ├─[Number:(Unnamed:1, 6, 6):\"0\"]",
            "  │  │     └─<EquationTail>",
            "  │  └─{varDefine}",
            "  └─[End:(Unnamed:1, 7, 7):\";\"]");
        parser.Check("var a := 0;",
            "─<Start>",
            "  ├─<DefineDefine>",
            "  │  ├─<OptionalVar>",
            "  │  │  └─[Var:(Unnamed:1, 1, 1):\"var\"]",
            "  │  ├─<DefineDefinePart>",
            "  │  │  ├─[Id:(Unnamed:1, 5, 5):\"a\"]",
            "  │  │  ├─{defineId}",
            "  │  │  ├─[Define:(Unnamed:1, 7, 7):\":=\"]",
            "  │  │  └─<Equation>",
            "  │  │     ├─[Number:(Unnamed:1, 10, 10):\"0\"]",
            "  │  │     └─<EquationTail>",
            "  │  └─{varDefine}",
            "  └─[End:(Unnamed:1, 11, 11):\";\"]");
    }

    [TestMethod]
    public void Builder04() {
        Tokenizer tok = new();
        tok.Start("S");
        tok.JoinToToken("S", "Space").AddPredef("WhiteSpace");
        tok.Consume("Space");
        tok.JoinToToken("S", "Id").AddRange('a', 'z');
        tok.JoinToToken("S", "Assign").AddSingle('=');
        tok.JoinToToken("S", "Var").AddSingle('$');

        Grammar grammar = new();
        grammar.Start("Start");
        grammar.NewRule("Start").AddItems("<OptionalVar> [Id] [Assign] [Id]");
        grammar.NewRule("Start").AddItems("[Id]");
        grammar.NewRule("OptionalVar");
        grammar.NewRule("OptionalVar").AddItems("[Var]");
        grammar = Normalizer.GetNormal(grammar);

        grammar.Check(
             "> <Start>",
             "<Start> → <OptionalVar> [Id] [Assign] [Id]",
             "   | [Id]",
             "<OptionalVar> → λ",
             "   | [Var]");

        ParserStates states = new();
        states.DetermineStates(grammar, new Writer());
        states.Check(
            "State 0:",
            "  <$StartTerm> → • <Start> [$EOFToken] @ [$EOFToken]",
            "  <OptionalVar> → λ • @ [Id]",
            "  <OptionalVar> → • [Var] @ [Id]",
            "  <Start> → • <OptionalVar> [Id] [Assign] [Id] @ [$EOFToken]",
            "  <Start> → • [Id] @ [$EOFToken]",
            "  [Id]: conflict:",
            "    shift 4 @ [$EOFToken]",
            "    reduce <OptionalVar> → λ @ [Assign]",
            "  [Var]: shift 2 @ [Id]",
            "  <OptionalVar>: goto 3",
            "  <Start>: goto 1",
            "State 1:",
            "  <$StartTerm> → <Start> • [$EOFToken] @ [$EOFToken]",
            "  [$EOFToken]: accept",
            "State 2:",
            "  <OptionalVar> → [Var] • @ [Id]",
            "  [Id]: reduce <OptionalVar> → [Var] @ [Assign]",
            "State 3:",
            "  <Start> → <OptionalVar> • [Id] [Assign] [Id] @ [$EOFToken]",
            "  [Id]: shift 5 @ [Assign]",
            "State 4:",
            "  <Start> → [Id] • @ [$EOFToken]",
            "  [$EOFToken]: reduce <Start> → [Id]",
            "State 5:",
            "  <Start> → <OptionalVar> [Id] • [Assign] [Id] @ [$EOFToken]",
            "  [Assign]: shift 6 @ [Id]",
            "State 6:",
            "  <Start> → <OptionalVar> [Id] [Assign] • [Id] @ [$EOFToken]",
            "  [Id]: shift 7 @ [$EOFToken]",
            "State 7:",
            "  <Start> → <OptionalVar> [Id] [Assign] [Id] • @ [$EOFToken]",
            "  [$EOFToken]: reduce <Start> → <OptionalVar> [Id] [Assign] [Id]");

        Table table = states.CreateTable();
        Parser parser = new(table, grammar, tok);
        table.Check(
            "state | [$EOFToken]           | [Assign] | [Id]                                        | [Var]   | <OptionalVar> | <Start>",
            "0     | -                     | -        | conflict(shift 4, reduce <OptionalVar> → λ) | shift 2 | goto 3        | goto 1 ",
            "1     | accept                | -        | -                                           | -       | -             | -      ",
            "2     | -                     | -        | reduce <OptionalVar> → [Var]                | -       | -             | -      ",
            "3     | -                     | -        | shift 5                                     | -       | -             | -      ",
            "4     | reduce <Start> → [Id] | -        | -                                           | -       | -             | -      ",
            "5     | -                     | shift 6  | -                                           | -       | -             | -      ",
            "6     | -                     | -        | shift 7                                     | -       | -             | -");

        parser.Check("$a = b",
            "─<Start>",
            "  ├─<OptionalVar>",
            "  │  └─[Var:(Unnamed:1, 1, 1):\"$\"]",
            "  ├─[Id:(Unnamed:1, 2, 2):\"a\"]",
            "  ├─[Assign:(Unnamed:1, 4, 4):\"=\"]",
            "  └─[Id:(Unnamed:1, 6, 6):\"b\"]");
        parser.Check("a",
            "─<Start>",
            "  └─[Id:(Unnamed:1, 1, 1):\"a\"]");

        // TODO: NEED TO FIX
        parser.Check("a = b",
            "─<Start>",
            "  ├─<OptionalVar>",
            "  ├─[Id:(Unnamed:1, 2, 2):\"a\"]",
            "  ├─[Assign:(Unnamed:1, 4, 4):\"=\"]",
            "  └─[Id:(Unnamed:1, 6, 6):\"b\"]");
    }

    [TestMethod]
    public void Builder05() {
        // From https://www.geeksforgeeks.org/slr-clr-and-lalr-parsers-set-3/
        Grammar grammar = new();
        grammar.Start("S");
        grammar.NewRule("S").AddItems("<A> [a] <A> [b]");
        grammar.NewRule("S").AddItems("<B> [b] <B> [a]");
        grammar.NewRule("A");
        grammar.NewRule("B");
        grammar.Check(
             "> <S>",
             "<S> → <A> [a] <A> [b]",
             "   | <B> [b] <B> [a]",
             "<A> → λ",
             "<B> → λ");

        ParserStates states = new();
        states.DetermineStates(grammar);
        states.Check(
            "State 0:",
            "  <$StartTerm> → • <S> [$EOFToken] @ [$EOFToken]",
            "  <A> → λ • @ [a]",
            "  <B> → λ • @ [b]",
            "  <S> → • <A> [a] <A> [b] @ [$EOFToken]",
            "  <S> → • <B> [b] <B> [a] @ [$EOFToken]",
            "  <A>: goto state 2",
            "  <B>: goto state 3",
            "  <S>: goto state 1",
            "State 1:",
            "  <$StartTerm> → <S> • [$EOFToken] @ [$EOFToken]",
            "State 2:",
            "  <S> → <A> • [a] <A> [b] @ [$EOFToken]",
            "  [a]: shift state 4",
            "State 3:",
            "  <S> → <B> • [b] <B> [a] @ [$EOFToken]",
            "  [b]: shift state 7",
            "State 4:",
            "  <A> → λ • @ [b]",
            "  <S> → <A> [a] • <A> [b] @ [$EOFToken]",
            "  <A>: goto state 5",
            "State 5:",
            "  <S> → <A> [a] <A> • [b] @ [$EOFToken]",
            "  [b]: shift state 6",
            "State 6:",
            "  <S> → <A> [a] <A> [b] • @ [$EOFToken]",
            "State 7:",
            "  <B> → λ • @ [a]",
            "  <S> → <B> [b] • <B> [a] @ [$EOFToken]",
            "  <B>: goto state 8",
            "State 8:",
            "  <S> → <B> [b] <B> • [a] @ [$EOFToken]",
            "  [a]: shift state 9",
            "State 9:",
            "  <S> → <B> [b] <B> [a] • @ [$EOFToken]");

        Table table = states.CreateTable();
        table.Check(
            "state | [$EOFToken]                  | [a]            | [b]            | <A>    | <B>    | <S>   ",
            "0     | -                            | reduce <A> → λ | reduce <B> → λ | goto 2 | goto 3 | goto 1",
            "1     | accept                       | -              | -              | -      | -      | -     ",
            "2     | -                            | shift 4        | -              | -      | -      | -     ",
            "3     | -                            | -              | shift 7        | -      | -      | -     ",
            "4     | -                            | -              | reduce <A> → λ | goto 5 | -      | -     ",
            "5     | -                            | -              | shift 6        | -      | -      | -     ",
            "6     | reduce <S> → <A> [a] <A> [b] | -              | -              | -      | -      | -     ",
            "7     | -                            | reduce <B> → λ | -              | -      | goto 8 | -     ",
            "8     | -                            | shift 9        | -              | -      | -      | -");

        // With my normalization applied
        grammar = Normalizer.GetNormal(grammar);
        grammar.Check(
             "> <S>",
             "<S> → <A> [a] <A> [b]",
             "   | <A> [b] <A> [a]",
             "<A> → λ");

        states = new();
        states.DetermineStates(grammar);
        states.Check(
            "State 0:",
            "  <$StartTerm> → • <S> [$EOFToken] @ [$EOFToken]",
            "  <A> → λ • @ [a]",
            "  <A> → λ • @ [b]",
            "  <S> → • <A> [a] <A> [b] @ [$EOFToken]",
            "  <S> → • <A> [b] <A> [a] @ [$EOFToken]",
            "  <A>: goto state 2",
            "  <S>: goto state 1",
            "State 1:",
            "  <$StartTerm> → <S> • [$EOFToken] @ [$EOFToken]",
            "State 2:",
            "  <S> → <A> • [a] <A> [b] @ [$EOFToken]",
            "  <S> → <A> • [b] <A> [a] @ [$EOFToken]",
            "  [a]: shift state 3",
            "  [b]: shift state 4",
            "State 3:",
            "  <A> → λ • @ [b]",
            "  <S> → <A> [a] • <A> [b] @ [$EOFToken]",
            "  <A>: goto state 7",
            "State 4:",
            "  <A> → λ • @ [a]",
            "  <S> → <A> [b] • <A> [a] @ [$EOFToken]",
            "  <A>: goto state 5",
            "State 5:",
            "  <S> → <A> [b] <A> • [a] @ [$EOFToken]",
            "  [a]: shift state 6",
            "State 6:",
            "  <S> → <A> [b] <A> [a] • @ [$EOFToken]",
            "State 7:",
            "  <S> → <A> [a] <A> • [b] @ [$EOFToken]",
            "  [b]: shift state 8",
            "State 8:",
            "  <S> → <A> [a] <A> [b] • @ [$EOFToken]");

        table = states.CreateTable();
        table.Check(
            "state | [$EOFToken]                  | [a]            | [b]            | <A>    | <S>   ",
            "0     | -                            | reduce <A> → λ | reduce <A> → λ | goto 2 | goto 1",
            "1     | accept                       | -              | -              | -      | -     ",
            "2     | -                            | shift 3        | shift 4        | -      | -     ",
            "3     | -                            | -              | reduce <A> → λ | goto 7 | -     ",
            "4     | -                            | reduce <A> → λ | -              | goto 5 | -     ",
            "5     | -                            | shift 6        | -              | -      | -     ",
            "6     | reduce <S> → <A> [b] <A> [a] | -              | -              | -      | -     ",
            "7     | -                            | -              | shift 8        | -      | -");
    }

    [TestMethod]
    public void Builder06() {
        // From https://www.geeksforgeeks.org/slr-clr-and-lalr-parsers-set-3/
        Grammar grammar = new();
        grammar.Start("S");
        grammar.NewRule("S").AddItems("<A> <A>");
        grammar.NewRule("A").AddItems("[a] <A>");
        grammar.NewRule("A").AddItems("[b]");

        grammar.Check(
             "> <S>",
             "<S> → <A> <A>",
             "<A> → [a] <A>",
             "   | [b]");

        ParserStates states = new();
        states.DetermineStates(grammar);
        states.Check(
            "State 0:",
            "  <$StartTerm> → • <S> [$EOFToken] @ [$EOFToken]",
            "  <A> → • [a] <A> @ [a] [b]",
            "  <A> → • [b] @ [a] [b]",
            "  <S> → • <A> <A> @ [$EOFToken]",
            "  <A>: goto state 4",
            "  <S>: goto state 1",
            "  [a]: shift state 2",
            "  [b]: shift state 3",
            "State 1:",
            "  <$StartTerm> → <S> • [$EOFToken] @ [$EOFToken]",
            "State 2:",
            "  <A> → • [a] <A> @ [a] [b]",
            "  <A> → [a] • <A> @ [a] [b]",
            "  <A> → • [b] @ [a] [b]",
            "  <A>: goto state 5",
            "  [a]: shift state 2",
            "  [b]: shift state 3",
            "State 3:",
            "  <A> → [b] • @ [a] [b]",
            "State 4:",
            "  <A> → • [a] <A> @ [$EOFToken]",
            "  <A> → • [b] @ [$EOFToken]",
            "  <S> → <A> • <A> @ [$EOFToken]",
            "  <A>: goto state 8",
            "  [a]: shift state 6",
            "  [b]: shift state 7",
            "State 5:",
            "  <A> → [a] <A> • @ [a] [b]",
            "State 6:",
            "  <A> → • [a] <A> @ [$EOFToken]",
            "  <A> → [a] • <A> @ [$EOFToken]",
            "  <A> → • [b] @ [$EOFToken]",
            "  <A>: goto state 9",
            "  [a]: shift state 6",
            "  [b]: shift state 7",
            "State 7:",
            "  <A> → [b] • @ [$EOFToken]",
            "State 8:",
            "  <S> → <A> <A> • @ [$EOFToken]",
            "State 9:",
            "  <A> → [a] <A> • @ [$EOFToken]");

        Table table = states.CreateTable();
        table.Check(
            "state | [$EOFToken]          | [a]                  | [b]                  | <A>    | <S>   ",
            "0     | -                    | shift 2              | shift 3              | goto 4 | goto 1",
            "1     | accept               | -                    | -                    | -      | -     ",
            "2     | -                    | shift 2              | shift 3              | goto 5 | -     ",
            "3     | -                    | reduce <A> → [b]     | reduce <A> → [b]     | -      | -     ",
            "4     | -                    | shift 6              | shift 7              | goto 8 | -     ",
            "5     | -                    | reduce <A> → [a] <A> | reduce <A> → [a] <A> | -      | -     ",
            "6     | -                    | shift 6              | shift 7              | goto 9 | -     ",
            "7     | reduce <A> → [b]     | -                    | -                    | -      | -     ",
            "8     | reduce <S> → <A> <A> | -                    | -                    | -      | -");
    }

    [TestMethod]
    public void Builder07() {
        // From https://en.wikipedia.org/wiki/Canonical_LR_parser#Constructing_LR(1)_parsing_tables
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
        grammar.NewRule("T").AddToken("+").AddTerm("T"); // Unary positive
        grammar.NewRule("T").AddTerm("T").AddToken("+").AddToken("n"); // Binary plus with left recursion

        // Removing left recursion
        grammar = Normalizer.GetNormal(grammar);
        grammar.Check(
             "> <E>",
             "<E> → <T>",
             "   | [(] <E> [)]",
             "<T> → [+] <T> <T'0>",
             "   | [n] <T'0>",
             "<T'0> → λ",
             "   | [+] [n] <T'0>");

        ParserStates states = new();
        states.DetermineStates(grammar, new Writer());
        states.Check(
            "State 0:",
            "  <$StartTerm> → • <E> [$EOFToken] @ [$EOFToken]",
            "  <E> → • <T> @ [$EOFToken]",
            "  <E> → • [(] <E> [)] @ [$EOFToken]",
            "  <T> → • [+] <T> <T'0> @ [$EOFToken]",
            "  <T> → • [n] <T'0> @ [$EOFToken]",
            "  <E>: goto state 1",
            "  <T>: goto state 2",
            "  [(]: shift state 3",
            "  [+]: shift state 4",
            "  [n]: shift state 5",
            "State 1:",
            "  <$StartTerm> → <E> • [$EOFToken] @ [$EOFToken]",
            "State 2:",
            "  <E> → <T> • @ [$EOFToken]",
            "State 3:",
            "  <E> → • <T> @ [)]",
            "  <E> → • [(] <E> [)] @ [)]",
            "  <E> → [(] • <E> [)] @ [$EOFToken]",
            "  <T> → • [+] <T> <T'0> @ [)]",
            "  <T> → • [n] <T'0> @ [)]",
            "  <E>: goto state 8",
            "  <T>: goto state 6",
            "  [(]: shift state 7",
            "  [+]: shift state 9",
            "  [n]: shift state 10",
            "State 4:",
            "  <T> → • [+] <T> <T'0> @ [+] [$EOFToken]",
            "  <T> → [+] • <T> <T'0> @ [$EOFToken]",
            "  <T> → • [n] <T'0> @ [+] [$EOFToken]",
            "  <T>: goto state 15",
            "  [+]: shift state 14",
            "  [n]: shift state 16",
            "State 5:",
            "  <T> → [n] • <T'0> @ [$EOFToken]",
            "  <T'0> → λ • @ [$EOFToken]",
            "  <T'0> → • [+] [n] <T'0> @ [$EOFToken]",
            "  <T'0>: goto state 27",
            "  [+]: shift state 22",
            "State 6:",
            "  <E> → <T> • @ [)]",
            "State 7:",
            "  <E> → • <T> @ [)]",
            "  <E> → • [(] <E> [)] @ [)]",
            "  <E> → [(] • <E> [)] @ [)]",
            "  <T> → • [+] <T> <T'0> @ [)]",
            "  <T> → • [n] <T'0> @ [)]",
            "  <E>: goto state 12",
            "  <T>: goto state 6",
            "  [(]: shift state 7",
            "  [+]: shift state 9",
            "  [n]: shift state 10",
            "State 8:",
            "  <E> → [(] <E> • [)] @ [$EOFToken]",
            "  [)]: shift state 11",
            "State 9:",
            "  <T> → • [+] <T> <T'0> @ [)] [+]",
            "  <T> → [+] • <T> <T'0> @ [)]",
            "  <T> → • [n] <T'0> @ [)] [+]",
            "  <T>: goto state 29",
            "  [+]: shift state 28",
            "  [n]: shift state 30",
            "State 10:",
            "  <T> → [n] • <T'0> @ [)]",
            "  <T'0> → λ • @ [)]",
            "  <T'0> → • [+] [n] <T'0> @ [)]",
            "  <T'0>: goto state 41",
            "  [+]: shift state 36",
            "State 11:",
            "  <E> → [(] <E> [)] • @ [$EOFToken]",
            "State 12:",
            "  <E> → [(] <E> • [)] @ [)]",
            "  [)]: shift state 13",
            "State 13:",
            "  <E> → [(] <E> [)] • @ [)]",
            "State 14:",
            "  <T> → • [+] <T> <T'0> @ [+] [$EOFToken]",
            "  <T> → [+] • <T> <T'0> @ [+] [$EOFToken]",
            "  <T> → • [n] <T'0> @ [+] [$EOFToken]",
            "  <T>: goto state 25",
            "  [+]: shift state 14",
            "  [n]: shift state 16",
            "State 15:",
            "  <T> → [+] <T> • <T'0> @ [$EOFToken]",
            "  <T'0> → λ • @ [$EOFToken]",
            "  <T'0> → • [+] [n] <T'0> @ [$EOFToken]",
            "  <T'0>: goto state 21",
            "  [+]: shift state 22",
            "State 16:",
            "  <T> → [n] • <T'0> @ [+] [$EOFToken]", // Shift 18
            "  <T'0> → λ • @ [+] [$EOFToken]", // Reduce
            "  <T'0> → • [+] [n] <T'0> @ [+] [$EOFToken]",
            "  <T'0>: goto state 17",
            "  [+]: shift state 18",
            "State 17:",
            "  <T> → [n] <T'0> • @ [+] [$EOFToken]",
            "State 18:",
            "  <T'0> → [+] • [n] <T'0> @ [+] [$EOFToken]",
            "  [n]: shift state 19",
            "State 19:",
            "  <T'0> → λ • @ [+] [$EOFToken]",
            "  <T'0> → • [+] [n] <T'0> @ [+] [$EOFToken]",
            "  <T'0> → [+] [n] • <T'0> @ [+] [$EOFToken]",
            "  <T'0>: goto state 20",
            "  [+]: shift state 18",
            "State 20:",
            "  <T'0> → [+] [n] <T'0> • @ [+] [$EOFToken]",
            "State 21:",
            "  <T> → [+] <T> <T'0> • @ [$EOFToken]",
            "State 22:",
            "  <T'0> → [+] • [n] <T'0> @ [$EOFToken]",
            "  [n]: shift state 23",
            "State 23:",
            "  <T'0> → λ • @ [$EOFToken]",
            "  <T'0> → • [+] [n] <T'0> @ [$EOFToken]",
            "  <T'0> → [+] [n] • <T'0> @ [$EOFToken]",
            "  <T'0>: goto state 24",
            "  [+]: shift state 22",
            "State 24:",
            "  <T'0> → [+] [n] <T'0> • @ [$EOFToken]",
            "State 25:",
            "  <T> → [+] <T> • <T'0> @ [+] [$EOFToken]",
            "  <T'0> → λ • @ [+] [$EOFToken]",
            "  <T'0> → • [+] [n] <T'0> @ [+] [$EOFToken]",
            "  <T'0>: goto state 26",
            "  [+]: shift state 18",
            "State 26:",
            "  <T> → [+] <T> <T'0> • @ [+] [$EOFToken]",
            "State 27:",
            "  <T> → [n] <T'0> • @ [$EOFToken]",
            "State 28:",
            "  <T> → • [+] <T> <T'0> @ [)] [+]",
            "  <T> → [+] • <T> <T'0> @ [)] [+]",
            "  <T> → • [n] <T'0> @ [)] [+]",
            "  <T>: goto state 39",
            "  [+]: shift state 28",
            "  [n]: shift state 30",
            "State 29:",
            "  <T> → [+] <T> • <T'0> @ [)]",
            "  <T'0> → λ • @ [)]",
            "  <T'0> → • [+] [n] <T'0> @ [)]",
            "  <T'0>: goto state 35",
            "  [+]: shift state 36",
            "State 30:",
            "  <T> → [n] • <T'0> @ [)] [+]",
            "  <T'0> → λ • @ [)] [+]",
            "  <T'0> → • [+] [n] <T'0> @ [)] [+]",
            "  <T'0>: goto state 31",
            "  [+]: shift state 32",
            "State 31:",
            "  <T> → [n] <T'0> • @ [)] [+]",
            "State 32:",
            "  <T'0> → [+] • [n] <T'0> @ [)] [+]",
            "  [n]: shift state 33",
            "State 33:",
            "  <T'0> → λ • @ [)] [+]",
            "  <T'0> → • [+] [n] <T'0> @ [)] [+]",
            "  <T'0> → [+] [n] • <T'0> @ [)] [+]",
            "  <T'0>: goto state 34",
            "  [+]: shift state 32",
            "State 34:",
            "  <T'0> → [+] [n] <T'0> • @ [)] [+]",
            "State 35:",
            "  <T> → [+] <T> <T'0> • @ [)]",
            "State 36:",
            "  <T'0> → [+] • [n] <T'0> @ [)]",
            "  [n]: shift state 37",
            "State 37:",
            "  <T'0> → λ • @ [)]",
            "  <T'0> → • [+] [n] <T'0> @ [)]",
            "  <T'0> → [+] [n] • <T'0> @ [)]",
            "  <T'0>: goto state 38",
            "  [+]: shift state 36",
            "State 38:",
            "  <T'0> → [+] [n] <T'0> • @ [)]",
            "State 39:",
            "  <T> → [+] <T> • <T'0> @ [)] [+]",
            "  <T'0> → λ • @ [)] [+]",
            "  <T'0> → • [+] [n] <T'0> @ [)] [+]",
            "  <T'0>: goto state 40",
            "  [+]: shift state 32",
            "State 40:",
            "  <T> → [+] <T> <T'0> • @ [)] [+]",
            "State 41:",
            "  <T> → [n] <T'0> • @ [)]");

        Table table = states.CreateTable();
        table.Check(
            "");
    }
}
