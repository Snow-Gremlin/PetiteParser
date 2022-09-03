using Microsoft.VisualStudio.TestTools.UnitTesting;
using PetiteParser.Grammar;
using PetiteParser.Loader;
using PetiteParser.Logger;
using PetiteParser.Parser;
using System;
using TestPetiteParser.Tools;

namespace TestPetiteParser.UnitTests {

    [TestClass]
    public class BuilderTests {

        [TestMethod]
        public void Builder01() {
            Grammar grammar = Loader.LoadGrammar(
                "> <Program>;",
                "<Program> := <OptionalA> <OptionalB> <OptionalC>;" +
                "<OptionalA> := _ | [A];" +
                "<OptionalB> := _ | [B];" +
                "<OptionalC> := _ | [C];");

            Log log = new();
            Builder builder = new(grammar.Copy(), log);
            builder.DetermineStates();

            Console.WriteLine(log.ToString());
            
            Console.WriteLine();
            foreach (State state in builder.States)
                Console.WriteLine(state.ToString());
            
            /*
            grammar.CheckStates(
                "state 0:",
                "  <$StartTerm> → • <Program> [$EOFToken] @ [$EOFToken]",
                "  <Program> → • <OptionalA> <OptionalB> <OptionalC> @ [$EOFToken]",
                "  <OptionalA> → λ • @ [B] [C] [$EOFToken]",
                "  <OptionalA> → • [A] @ [B] [C] [$EOFToken]",
                "  <$StartTerm> → • <$StartTerm> [$EOFToken] @ [$EOFToken]",
                "  <$StartTerm> → • <Program> [$EOFToken] @ [$EOFToken]",
                "  <$StartTerm> → • <$StartTerm> [$EOFToken] @ [$EOFToken]",
                "  <Program>: goto state 1",
                "  <OptionalA>: goto state 2",
                "  [A]: shift state 3",
                "  <$StartTerm>: goto state 4",
                "state 1:",
                "  <$StartTerm> → <Program> • [$EOFToken] @ [$EOFToken]",
                "state 2:",
                "  <Program> → <OptionalA> • <OptionalB> <OptionalC> @ [$EOFToken]",
                "  <OptionalB> → λ • @ [C] [$EOFToken]",
                "  <OptionalB> → • [B] @ [C] [$EOFToken]",
                "  <OptionalB>: goto state 5",
                "  [B]: shift state 6",
                "state 3:",
                "  <OptionalA> → [A] • @ [B] [C] [$EOFToken]",
                "state 4:",
                "  <$StartTerm> → <$StartTerm> • [$EOFToken] @ [$EOFToken]",
                "state 5:",
                "  <Program> → <OptionalA> <OptionalB> • <OptionalC> @ [$EOFToken]",
                "  <OptionalC> → λ • @ [$EOFToken]",
                "  <OptionalC> → • [C] @ [$EOFToken]",
                "  <OptionalC>: goto state 7",
                "  [C]: shift state 8",
                "state 6:",
                "  <OptionalB> → [B] • @ [C] [$EOFToken]",
                "state 7:",
                "  <Program> → <OptionalA> <OptionalB> <OptionalC> • @ [$EOFToken]",
                "state 8:",
                "  <OptionalC> → [C] • @ [$EOFToken]");
            */

            Assert.Fail("TODO: REMOVE ME!");
        }
    }
}
