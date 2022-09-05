using Microsoft.VisualStudio.TestTools.UnitTesting;
using PetiteParser.Builder;
using PetiteParser.Grammar;
using PetiteParser.Loader;
using PetiteParser.Logger;
using PetiteParser.Parser;
using PetiteParser.Tokenizer;
using System;
using TestPetiteParser.Tools;

namespace TestPetiteParser.UnitTests
{

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

            Log log = new();
            Builder builder = new(grammar.Copy(), log);
            builder.DetermineStates();
            Console.WriteLine(log.ToString());
            
            grammar.CheckStates(
                "State 0:",
                "  <$StartTerm> → • <Program> [$EOFToken] @ [$EOFToken]",
                "  <Program> → • <OptionalA> <OptionalB> <OptionalC> @ [$EOFToken]",
                "  <OptionalA> → λ • @ [B] [C] [$EOFToken]",
                "  <OptionalA> → • [A] @ [B] [C] [$EOFToken]",
                "  <Program>: goto state 1",
                "  <OptionalA>: goto state 2",
                "  [A]: shift state 3",
                "State 1:",
                "  <$StartTerm> → <Program> • [$EOFToken] @ [$EOFToken]",
                "State 2:",
                "  <Program> → <OptionalA> • <OptionalB> <OptionalC> @ [$EOFToken]",
                "  <OptionalB> → λ • @ [C] [$EOFToken]",
                "  <OptionalB> → • [B] @ [C] [$EOFToken]",
                "  <OptionalB>: goto state 4",
                "  [B]: shift state 5",
                "State 3:",
                "  <OptionalA> → [A] • @ [B] [C] [$EOFToken]",
                "State 4:",
                "  <Program> → <OptionalA> <OptionalB> • <OptionalC> @ [$EOFToken]",
                "  <OptionalC> → λ • @ [$EOFToken]",
                "  <OptionalC> → • [C] @ [$EOFToken]",
                "  <OptionalC>: goto state 6",
                "  [C]: shift state 7",
                "State 5:",
                "  <OptionalB> → [B] • @ [C] [$EOFToken]",
                "State 6:",
                "  <Program> → <OptionalA> <OptionalB> <OptionalC> • @ [$EOFToken]",
                "State 7:",
                "  <OptionalC> → [C] • @ [$EOFToken]");
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

            Log log = new();
            Builder builder = new(parser.Grammar.Copy(), log);
            builder.DetermineStates();
            Console.WriteLine(log.ToString());

            parser.Grammar.CheckStates(
                "State 0:",
                "  <$StartTerm> → • <Program> [$EOFToken] @ [$EOFToken]",
                "  <Program> → • <OptionalStart> [B] <BTail> [End] @ [$EOFToken]",
                "  <OptionalStart> → λ • @ [B]",
                "  <OptionalStart> → • [Start] @ [B]",
                "  <Program> → • <OptionalStart> [C] <CTail> [End] @ [$EOFToken]",
                "  <OptionalStart> → λ • @ [C]",
                "  <OptionalStart> → • [Start] @ [C]",
                "  <Program> → • [D] [End] @ [$EOFToken]",
                "  <$StartTerm> → • <$StartTerm> [$EOFToken] @ [$EOFToken]",
                "  <Program>: goto state 1",
                "  <OptionalStart>: goto state 2",
                "  [Start]: shift state 3",
                "  [D]: shift state 4",
                "  <$StartTerm>: goto state 5",
                "State 1:",
                "  <$StartTerm> → <Program> • [$EOFToken] @ [$EOFToken]",
                "State 2:",
                "  <Program> → <OptionalStart> • [B] <BTail> [End] @ [$EOFToken]",
                "  <Program> → <OptionalStart> • [C] <CTail> [End] @ [$EOFToken]",
                "  [B]: shift state 6",
                "  [C]: shift state 7",
                "State 3:",
                "  <OptionalStart> → [Start] • @ [B]",
                "  <OptionalStart> → [Start] • @ [C]",
                "State 4:",
                "  <Program> → [D] • [End] @ [$EOFToken]",
                "  [End]: shift state 16",
                "State 5:",
                "  <$StartTerm> → <$StartTerm> • [$EOFToken] @ [$EOFToken]",
                "State 6:",
                "  <Program> → <OptionalStart> [B] • <BTail> [End] @ [$EOFToken]",
                "  <BTail> → λ • @ [End]",
                "  <BTail> → • [Comma] [B] <BTail> @ [End]",
                "  <BTail>: goto state 11",
                "  [Comma]: shift state 12",
                "State 7:",
                "  <Program> → <OptionalStart> [C] • <CTail> [End] @ [$EOFToken]",
                "  <CTail> → λ • @ [End]",
                "  <CTail> → • [Comma] [C] <CTail> @ [End]",
                "  <CTail>: goto state 8",
                "  [Comma]: shift state 9",
                "State 8:",
                "  <Program> → <OptionalStart> [C] <CTail> • [End] @ [$EOFToken]",
                "  [End]: shift state 10",
                "State 9:",
                "  <CTail> → [Comma] • [C] <CTail> @ [End]",
                "  [C]: shift state 17",
                "State 10:",
                "  <Program> → <OptionalStart> [C] <CTail> [End] • @ [$EOFToken]",
                "State 11:",
                "  <Program> → <OptionalStart> [B] <BTail> • [End] @ [$EOFToken]",
                "  [End]: shift state 15",
                "State 12:",
                "  <BTail> → [Comma] • [B] <BTail> @ [End]",
                "  [B]: shift state 13",
                "State 13:",
                "  <BTail> → [Comma] [B] • <BTail> @ [End]",
                "  <BTail> → λ • @ [End]",
                "  <BTail> → • [Comma] [B] <BTail> @ [End]",
                "  <BTail>: goto state 14",
                "  [Comma]: shift state 12",
                "State 14:",
                "  <BTail> → [Comma] [B] <BTail> • @ [End]",
                "State 15:",
                "  <Program> → <OptionalStart> [B] <BTail> [End] • @ [$EOFToken]",
                "State 16:",
                "  <Program> → [D] [End] • @ [$EOFToken]",
                "State 17:",
                "  <CTail> → [Comma] [C] • <CTail> @ [End]",
                "  <CTail> → λ • @ [End]",
                "  <CTail> → • [Comma] [C] <CTail> @ [End]",
                "  <CTail>: goto state 18",
                "  [Comma]: shift state 9",
                "State 18:",
                "  <CTail> → [Comma] [C] <CTail> • @ [End]");

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
                "<Start> := <DefineDefine> [End]",
                "    | [DeclareDefine] <DefineAssign> [End]",
                "    | [DeclareDefine] [CurlOpen] <DefineGroup> [CurlClose];",
                "<DefineDefine> := <RootType> <DefineDefinePart> {typeDefine}",
                "    | <OptionalVar> <DefineDefinePart> {varDefine};",
                "<DefineDefinePart> := [Id] {defineId} [Define] <Equation>;",
                "<DefineAssign> := <RootType> <DefineAssignPart> {typeDefine}",
                "    | <OptionalVar> <DefineAssignPart> {varDefine};",
                "<DefineAssignPart> := [Id] {defineId} [Assign] <Equation>;",
                "<DefineGroup> := _ | <DefineAssign> [End] <DefineGroup>;",
                "<Equation> := [Number] <EquationTail>;",
                "<EquationTail> := _ | [Add] [Number] <EquationTail>;");

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
    }
}
