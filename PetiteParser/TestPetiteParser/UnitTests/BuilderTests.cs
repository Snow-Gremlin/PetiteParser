using Microsoft.VisualStudio.TestTools.UnitTesting;
using PetiteParser.Parser.States;
using PetiteParser.Grammar;
using PetiteParser.Loader;
using PetiteParser.Logger;
using PetiteParser.Parser;
using System;
using TestPetiteParser.Tools;
using PetiteParser.Parser.Table;
using PetiteParser.Tokenizer;
using PetiteParser.Analyzer;
using PetiteParser.Matcher;

namespace TestPetiteParser.UnitTests {

    [TestClass]
    public class BuilderTests {

        [TestMethod]
        public void Builder01() {
            Grammar grammar = Loader.LoadGrammar(
                "> <Program>;",
                "<Program> := <OptionalA> <OptionalB> <OptionalC>;",
                "<OptionalA> := _ | [A];",
                "<OptionalB> := _ | [B];",
                "<OptionalC> := _ | [C];");

            Buffered log = new();
            _ = new ParserStates(grammar.Copy(), log);
            Console.WriteLine(log.ToString());
            
            grammar.CheckStates(
                "State 0:",
                "  <$StartTerm> → • <Program> [$EOFToken] @ [$EOFToken]",
                "  <OptionalA> → λ • @ [$EOFToken] [B] [C]",
                "  <OptionalA> → • [A] @ [$EOFToken] [B] [C]",
                "  <Program> → • <OptionalA> <OptionalB> <OptionalC> @ [$EOFToken]",
                "  <OptionalA>: goto state 3",
                "  <Program>: goto state 1",
                "  [A]: shift state 2",
                "State 1:",
                "  <$StartTerm> → <Program> • [$EOFToken] @ [$EOFToken]",
                "State 2:",
                "  <OptionalA> → [A] • @ [$EOFToken] [B] [C]",
                "State 3:",
                "  <OptionalB> → λ • @ [$EOFToken] [C]",
                "  <OptionalB> → • [B] @ [$EOFToken] [C]",
                "  <Program> → <OptionalA> • <OptionalB> <OptionalC> @ [$EOFToken]",
                "  <OptionalB>: goto state 5",
                "  [B]: shift state 4",
                "State 4:",
                "  <OptionalB> → [B] • @ [$EOFToken] [C]",
                "State 5:",
                "  <OptionalC> → λ • @ [$EOFToken]",
                "  <OptionalC> → • [C] @ [$EOFToken]",
                "  <Program> → <OptionalA> <OptionalB> • <OptionalC> @ [$EOFToken]",
                "  <OptionalC>: goto state 7",
                "  [C]: shift state 6",
                "State 6:",
                "  <OptionalC> → [C] • @ [$EOFToken]",
                "State 7:",
                "  <Program> → <OptionalA> <OptionalB> <OptionalC> • @ [$EOFToken]");
        }

        [TestMethod]
        public void Builder02() {
            Parser parser = Loader.LoadParser(
                "> (S);",
                "(S): '>' => [Start];",
                "(S): ',' => [Comma];",
                "(S): ';' => [End];",
                "(S): 'a' => [A];",
                "(S): 'b' => [B];",
                "(S): 'c' => [C];",
                "(S): 'd' => [D];",
                "(S): ' ' => ^[Space];",
                "",
                "> <Program>;",
                "<OptionalStart> := _ | [Start];",
                "<Program> := <OptionalStart> [B] <BTail> [End];",
                "<BTail>   := _ | [Comma] [B] <BTail>;",
                "<Program> := <OptionalStart> [C] <CTail> [End];",
                "<CTail>   := _ | [Comma] [C] <CTail>;",
                "<Program> := [D] [End];");

            parser.Grammar.Check(
                "> <$StartTerm>",
                "<Program> → <OptionalStart> [B] <BTail> [End]",
                "   | <OptionalStart> [C] <CTail> [End]",
                "   | [D] [End]",
                "<OptionalStart> → λ",
                "   | [Start]",
                "<BTail> → λ",
                "   | [Comma] [B] <BTail>",
                "<CTail> → λ",
                "   | [Comma] [C] <CTail>",
                "<$StartTerm> → <Program> [$EOFToken]");

            parser.Grammar.CheckStates(
                "State 0:",
                "  <$StartTerm> → • <Program> [$EOFToken] @ [$EOFToken]",
                "  <OptionalStart> → λ • @ [B]",
                "  <OptionalStart> → λ • @ [C]",
                "  <OptionalStart> → • [Start] @ [B]",
                "  <OptionalStart> → • [Start] @ [C]",
                "  <Program> → • <OptionalStart> [B] <BTail> [End] @ [$EOFToken]", // Why doesn't this have the next over the lambda.
                "  <Program> → • <OptionalStart> [C] <CTail> [End] @ [$EOFToken]",
                "  <Program> → • [D] [End] @ [$EOFToken]",
                "  <$StartTerm>: goto state 1",
                "  <OptionalStart>: goto state 4",
                "  <Program>: goto state 2",
                "  [D]: shift state 5",
                "  [Start]: shift state 3",
                "State 1:",
                "  <$StartTerm> → <$StartTerm> • [$EOFToken] @ [$EOFToken]",
                "State 2:",
                "  <$StartTerm> → <Program> • [$EOFToken] @ [$EOFToken]",
                "State 3:",
                "  <OptionalStart> → [Start] • @ [B]",
                "  <OptionalStart> → [Start] • @ [C]",
                "State 4:",
                "  <Program> → <OptionalStart> • [B] <BTail> [End] @ [$EOFToken]",
                "  <Program> → <OptionalStart> • [C] <CTail> [End] @ [$EOFToken]",
                "  [B]: shift state 6",
                "  [C]: shift state 7",
                "State 5:",
                "  <Program> → [D] • [End] @ [$EOFToken]",
                "  [End]: shift state 18",
                "State 6:",
                "  <BTail> → λ • @ [End]",
                "  <BTail> → • [Comma] [B] <BTail> @ [End]",
                "  <Program> → <OptionalStart> [B] • <BTail> [End] @ [$EOFToken]",
                "  <BTail>: goto state 14",
                "  [Comma]: shift state 13",
                "State 7:",
                "  <CTail> → λ • @ [End]",
                "  <CTail> → • [Comma] [C] <CTail> @ [End]",
                "  <Program> → <OptionalStart> [C] • <CTail> [End] @ [$EOFToken]",
                "  <CTail>: goto state 9",
                "  [Comma]: shift state 8",
                "State 8:",
                "  <CTail> → [Comma] • [C] <CTail> @ [End]",
                "  [C]: shift state 11",
                "State 9:",
                "  <Program> → <OptionalStart> [C] <CTail> • [End] @ [$EOFToken]",
                "  [End]: shift state 10",
                "State 10:",
                "  <Program> → <OptionalStart> [C] <CTail> [End] • @ [$EOFToken]",
                "State 11:",
                "  <CTail> → λ • @ [End]",
                "  <CTail> → • [Comma] [C] <CTail> @ [End]",
                "  <CTail> → [Comma] [C] • <CTail> @ [End]",
                "  <CTail>: goto state 12",
                "  [Comma]: shift state 8",
                "State 12:",
                "  <CTail> → [Comma] [C] <CTail> • @ [End]",
                "State 13:",
                "  <BTail> → [Comma] • [B] <BTail> @ [End]",
                "  [B]: shift state 16",
                "State 14:",
                "  <Program> → <OptionalStart> [B] <BTail> • [End] @ [$EOFToken]",
                "  [End]: shift state 15",
                "State 15:",
                "  <Program> → <OptionalStart> [B] <BTail> [End] • @ [$EOFToken]",
                "State 16:",
                "  <BTail> → λ • @ [End]",
                "  <BTail> → • [Comma] [B] <BTail> @ [End]",
                "  <BTail> → [Comma] [B] • <BTail> @ [End]",
                "  <BTail>: goto state 17",
                "  [Comma]: shift state 13",
                "State 17:",
                "  <BTail> → [Comma] [B] <BTail> • @ [End]",
                "State 18:",
                "  <Program> → [D] [End] • @ [$EOFToken]");

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

            Console.WriteLine(Parser.GetDebugStateString(parser.Grammar));

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
            Global.Log = new Writer(); // TODO: REMOVE

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
            Analyzer.Normalize(grammar);

            grammar.Check(
                 "> <Start>",
                 "<Start> → <OptionalVar> [Id] [Assign] [Id]",
                 "   | [Id]",
                 "<OptionalVar> → λ",
                 "   | [Var]");

            grammar.CheckStates(
                "State 0:",
                "  <$StartTerm> → • <Start> [$EOFToken] @ [$EOFToken]",
                "  <OptionalVar> → λ • @ [Id]",
                "  <OptionalVar> → • [Var] @ [Id]",
                "  <Start> → • <OptionalVar> [Id] [Assign] [Id] @ [$EOFToken]",
                "  <Start> → <OptionalVar> • [Id] [Assign] [Id] @ [Id]",
                "  <Start> → • [Id] @ [$EOFToken]",
                "  <OptionalVar>: goto state 3",
                "  <Start>: goto state 1",
                "  [Id]: shift state 4",
                "  [Var]: shift state 2",
                "State 1:",
                "  <$StartTerm> → <Start> • [$EOFToken] @ [$EOFToken]",
                "State 2:",
                "  <OptionalVar> → [Var] • @ [Id]",
                "State 3:",
                "  <Start> → <OptionalVar> • [Id] [Assign] [Id] @ [$EOFToken]",
                "  [Id]: shift state 5",
                "State 4:",
                "  <Start> → <OptionalVar> [Id] • [Assign] [Id] @ [Id]",
                "  <Start> → [Id] • @ [$EOFToken]",
                "  [Assign]: shift state 8",
                "State 5:",
                "  <Start> → <OptionalVar> [Id] • [Assign] [Id] @ [$EOFToken]",
                "  [Assign]: shift state 6",
                "State 6:",
                "  <Start> → <OptionalVar> [Id] [Assign] • [Id] @ [$EOFToken]",
                "  [Id]: shift state 7",
                "State 7:",
                "  <Start> → <OptionalVar> [Id] [Assign] [Id] • @ [$EOFToken]",
                "State 8:",
                "  <Start> → <OptionalVar> [Id] [Assign] • [Id] @ [Id]",
                "  [Id]: shift state 9",
                "State 9:",
                "  <Start> → <OptionalVar> [Id] [Assign] [Id] • @ [Id]");

            Parser parser = new(grammar, tok, new Writer());
            parser.CheckTable(
                "state | [$EOFToken]                                       | [Assign] | [Id]                         | [Var]   | <OptionalVar> | <Start>",
                "0     | -                                                 | -        | shift 4                      | shift 2 | goto 3        | goto 1 ",
                "1     | accept                                            | -        | -                            | -       | -             | -      ",
                "2     | -                                                 | -        | reduce <OptionalVar> → [Var] | -       | -             | -      ",
                "3     | -                                                 | -        | shift 5                      | -       | -             | -      ",
                "4     | reduce <Start> → [Id]                             | shift 8  | -                            | -       | -             | -      ",
                "5     | -                                                 | shift 6  | -                            | -       | -             | -      ",
                "6     | -                                                 | -        | shift 7                      | -       | -             | -      ",
                "7     | reduce <Start> → <OptionalVar> [Id] [Assign] [Id] | -        | -                            | -       | -             | -      ",
                "8     | -                                                 | -        | shift 9                      | -       | -             | -");

            parser.Check("a = b",
                "─<Start>");
            parser.Check("$a = b",
                "─<Start>");
            parser.Check("a;",
                "─<Start>");
        }
    }
}
