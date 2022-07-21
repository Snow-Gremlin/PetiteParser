using Microsoft.VisualStudio.TestTools.UnitTesting;
using PetiteParser.Grammar;
using PetiteParser.Parser;
using PetiteParser.Tokenizer;
using TestPetiteParser.Tools;

namespace TestPetiteParser.UnitTests {
    [TestClass]
    public class ParserTests {

        [TestMethod]
        public void Parser1() {
            Tokenizer tok = new();
            tok.Start("start");
            tok.Join("start", "(").AddSet("(");
            tok.Join("start", ")").AddSet(")");
            tok.Join("start", "+").AddSet("+");
            tok.Join("start", "number").AddRange("0", "9");
            tok.Join("number", "number").AddRange("0", "9");
            tok.SetToken("(", "(");
            tok.SetToken(")", ")");
            tok.SetToken("+", "+");
            tok.SetToken("number", "n");
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
            grammar.NewRule("T").AddToken("+").AddTerm("T");
            grammar.NewRule("T").AddTerm("T").AddToken("+").AddToken("n");
            Parser parser = new(grammar, tok);

            parser.Check("103",
               "─<E>",
               "  └─<T>",
               "     └─[n:(Unnamed:1, 1, 1):\"103\"]");

            parser.Check("+2",
               "─<E>",
               "  └─<T>",
               "     ├─[+:(Unnamed:1, 1, 1):\"+\"]",
               "     └─<T>",
               "        └─[n:(Unnamed:1, 2, 2):\"2\"]");

            parser.Check("3+4",
               "─<E>",
               "  └─<T>",
               "     ├─<T>",
               "     │  └─[n:(Unnamed:1, 1, 1):\"3\"]",
               "     ├─[+:(Unnamed:1, 2, 2):\"+\"]",
               "     └─[n:(Unnamed:1, 3, 3):\"4\"]");

            parser.Check("((42+6))",
               "─<E>",
               "  ├─[(:(Unnamed:1, 1, 1):\"(\"]",
               "  ├─<E>",
               "  │  ├─[(:(Unnamed:1, 2, 2):\"(\"]",
               "  │  ├─<E>",
               "  │  │  └─<T>",
               "  │  │     ├─<T>",
               "  │  │     │  └─[n:(Unnamed:1, 3, 3):\"42\"]",
               "  │  │     ├─[+:(Unnamed:1, 5, 5):\"+\"]",
               "  │  │     └─[n:(Unnamed:1, 6, 6):\"6\"]",
               "  │  └─[):(Unnamed:1, 7, 7):\")\"]",
               "  └─[):(Unnamed:1, 8, 8):\")\"]");
        }

        [TestMethod]
        public void Parser2() {
            Tokenizer tok = new();
            tok.Start("start");
            tok.Join("start", "(").AddSet("(");
            tok.Join("start", ")").AddSet(")");
            tok.SetToken("(", "(");
            tok.SetToken(")", ")");
            // 1. X → ( X )
            // 2. X → ( )
            Grammar grammar = new();
            grammar.Start("X");
            grammar.NewRule("X").AddToken("(").AddTerm("X").AddToken(")");
            grammar.NewRule("X").AddToken("(").AddToken(")");
            Parser parser = new(grammar, tok);

            parser.Check("()",
               "─<X>",
               "  ├─[(:(Unnamed:1, 1, 1):\"(\"]",
               "  └─[):(Unnamed:1, 2, 2):\")\"]");

            parser.Check("((()))",
               "─<X>",
               "  ├─[(:(Unnamed:1, 1, 1):\"(\"]",
               "  ├─<X>",
               "  │  ├─[(:(Unnamed:1, 2, 2):\"(\"]",
               "  │  ├─<X>",
               "  │  │  ├─[(:(Unnamed:1, 3, 3):\"(\"]",
               "  │  │  └─[):(Unnamed:1, 4, 4):\")\"]",
               "  │  └─[):(Unnamed:1, 5, 5):\")\"]",
               "  └─[):(Unnamed:1, 6, 6):\")\"]");
        }

        [TestMethod]
        public void Parser3() {
            Tokenizer tok = new();
            tok.Start("start");
            tok.Join("start", "(").AddSet("(");
            tok.Join("start", ")").AddSet(")");
            tok.SetToken("(", "(");
            tok.SetToken(")", ")");
            // 1. X → ( X )
            // 2. X → 𝜀
            Grammar grammar = new();
            grammar.Start("X");
            grammar.NewRule("X").AddToken("(").AddTerm("X").AddToken(")");
            grammar.NewRule("X");
            Parser parser = new(grammar, tok);

            parser.Check("",
               "─<X>");

            parser.Check("()",
               "─<X>",
               "  ├─[(:(Unnamed:1, 1, 1):\"(\"]",
               "  ├─<X>",
               "  └─[):(Unnamed:1, 2, 2):\")\"]");

            parser.Check("((()))",
               "─<X>",
               "  ├─[(:(Unnamed:1, 1, 1):\"(\"]",
               "  ├─<X>",
               "  │  ├─[(:(Unnamed:1, 2, 2):\"(\"]",
               "  │  ├─<X>",
               "  │  │  ├─[(:(Unnamed:1, 3, 3):\"(\"]",
               "  │  │  ├─<X>",
               "  │  │  └─[):(Unnamed:1, 4, 4):\")\"]",
               "  │  └─[):(Unnamed:1, 5, 5):\")\"]",
               "  └─[):(Unnamed:1, 6, 6):\")\"]");
        }

        [TestMethod]
        public void Parser4() {
            Tokenizer tok = new();
            tok.Start("start");
            tok.Join("start", "a").AddSet("a");
            tok.Join("start", "b").AddSet("b");
            tok.Join("start", "d").AddSet("d");
            tok.SetToken("a", "a");
            tok.SetToken("b", "b");
            tok.SetToken("d", "d");
            // 1. S → b A d S
            // 2. S → 𝜀
            // 3. A → a A
            // 4. A → 𝜀
            Grammar grammar = new();
            grammar.Start("S");
            grammar.NewRule("S").AddToken("b").AddTerm("A").AddToken("d").AddTerm("S");
            grammar.NewRule("S");
            grammar.NewRule("A").AddToken("a").AddTerm("A");
            grammar.NewRule("A");
            Parser parser = new(grammar, tok);

            parser.Check("bd",
               "─<S>",
               "  ├─[b:(Unnamed:1, 1, 1):\"b\"]",
               "  ├─<A>",
               "  ├─[d:(Unnamed:1, 2, 2):\"d\"]",
               "  └─<S>");

            parser.Check("bad",
               "─<S>",
               "  ├─[b:(Unnamed:1, 1, 1):\"b\"]",
               "  ├─<A>",
               "  │  ├─[a:(Unnamed:1, 2, 2):\"a\"]",
               "  │  └─<A>",
               "  ├─[d:(Unnamed:1, 3, 3):\"d\"]",
               "  └─<S>");

            parser.Check("bdbadbd",
               "─<S>",
               "  ├─[b:(Unnamed:1, 1, 1):\"b\"]",
               "  ├─<A>",
               "  ├─[d:(Unnamed:1, 2, 2):\"d\"]",
               "  └─<S>",
               "     ├─[b:(Unnamed:1, 3, 3):\"b\"]",
               "     ├─<A>",
               "     │  ├─[a:(Unnamed:1, 4, 4):\"a\"]",
               "     │  └─<A>",
               "     ├─[d:(Unnamed:1, 5, 5):\"d\"]",
               "     └─<S>",
               "        ├─[b:(Unnamed:1, 6, 6):\"b\"]",
               "        ├─<A>",
               "        ├─[d:(Unnamed:1, 7, 7):\"d\"]",
               "        └─<S>");
        }

        [TestMethod]
        public void Parser5() {
            Tokenizer tok = new();
            tok.Start("start");
            tok.Join("start", "id").AddRange("a", "z");
            tok.Join("id", "id").AddRange("a", "z");
            tok.Join("start", "add").AddSet("+");
            tok.Join("start", "multiply").AddSet("*");
            tok.Join("start", "open").AddSet("(");
            tok.Join("start", "close").AddSet(")");
            tok.Join("start", "start").SetConsume(true).AddSet(" ");
            tok.SetToken("add", "+");
            tok.SetToken("multiply", "*");
            tok.SetToken("open", "(");
            tok.SetToken("close", ")");
            tok.SetToken("id", "id");
            // 1. E → E + E
            // 2. E → E * E
            // 3. E → ( E )
            // 4. E → id
            Grammar grammar = new();
            grammar.Start("E");
            grammar.NewRule("E").AddTerm("E").AddToken("+").AddTerm("E");
            grammar.NewRule("E").AddTerm("E").AddToken("*").AddTerm("E");
            grammar.NewRule("E").AddToken("(").AddTerm("E").AddToken(")");
            grammar.NewRule("E").AddToken("id");
            Parser parser = new(grammar, tok);

            parser.Check("a",
               "─<E>",
               "  └─[id:(Unnamed:1, 1, 1):\"a\"]");

            parser.Check("(a + b)",
               "─<E>",
               "  ├─[(:(Unnamed:1, 1, 1):\"(\"]",
               "  ├─<E>",
               "  │  ├─<E>",
               "  │  │  └─[id:(Unnamed:1, 2, 2):\"a\"]",
               "  │  ├─[+:(Unnamed:1, 3, 3):\"+\"]",
               "  │  └─<E>",
               "  │     └─[id:(Unnamed:1, 5, 5):\"b\"]",
               "  └─[):(Unnamed:1, 7, 7):\")\"]");

            parser.Check("a + b * c",
               "─<E>",
               "  ├─<E>",
               "  │  └─[id:(Unnamed:1, 1, 1):\"a\"]",
               "  ├─[+:(Unnamed:1, 2, 2):\"+\"]",
               "  └─<E>",
               "     ├─<E>",
               "     │  └─[id:(Unnamed:1, 4, 4):\"b\"]",
               "     ├─[*:(Unnamed:1, 6, 6):\"*\"]",
               "     └─<E>",
               "        └─[id:(Unnamed:1, 8, 8):\"c\"]");

            parser.Check("a + (b * c) + d",
               "─<E>",
               "  ├─<E>",
               "  │  └─[id:(Unnamed:1, 1, 1):\"a\"]",
               "  ├─[+:(Unnamed:1, 2, 2):\"+\"]",
               "  └─<E>",
               "     ├─<E>",
               "     │  ├─[(:(Unnamed:1, 4, 4):\"(\"]",
               "     │  ├─<E>",
               "     │  │  ├─<E>",
               "     │  │  │  └─[id:(Unnamed:1, 6, 6):\"b\"]",
               "     │  │  ├─[*:(Unnamed:1, 7, 7):\"*\"]",
               "     │  │  └─<E>",
               "     │  │     └─[id:(Unnamed:1, 9, 9):\"c\"]",
               "     │  └─[):(Unnamed:1, 11, 11):\")\"]",
               "     ├─[+:(Unnamed:1, 12, 12):\"+\"]",
               "     └─<E>",
               "        └─[id:(Unnamed:1, 14, 14):\"d\"]");
        }

        [TestMethod]
        public void Parser6() {
            Tokenizer tok = new();
            tok.Start("start");
            tok.Join("start", "a").AddSet("a");
            tok.SetToken("a", "a");

            Grammar grammar = new();
            grammar.Start("E");
            grammar.NewRule("E");
            grammar.NewRule("E").AddTerm("E").AddTerm("T");
            grammar.NewRule("T").AddToken("a");
            Parser parser = new(grammar, tok);

            parser.Check("aaa",
               "─<E>",
               "  ├─<E>",
               "  │  ├─<E>",
               "  │  │  ├─<E>",
               "  │  │  └─<T>",
               "  │  │     └─[a:(Unnamed:1, 1, 1):\"a\"]",
               "  │  └─<T>",
               "  │     └─[a:(Unnamed:1, 2, 2):\"a\"]",
               "  └─<T>",
               "     └─[a:(Unnamed:1, 3, 3):\"a\"]");
        }

        [TestMethod]
        public void Parser7() {
            Tokenizer tok = new();
            tok.Start("start");
            tok.JoinToToken("start", "*").AddSet("*");

            Grammar grammar = new();
            grammar.Start("E");
            grammar.NewRule("E");
            grammar.NewRule("E").AddTerm("T").AddTerm("E");
            grammar.NewRule("T").AddToken("*");

            Parser parser = new(grammar, tok);
            parser.Check("",
                "─<E>");
            parser.Check("*",
                "─<E>",
                "  ├─<T>",
                "  │  └─[*:(Unnamed:1, 1, 1):\"*\"]",
                "  └─<E>");
            parser.Check("***",
                "─<E>",
                "  ├─<T>",
                "  │  └─[*:(Unnamed:1, 1, 1):\"*\"]",
                "  └─<E>",
                "     ├─<T>",
                "     │  └─[*:(Unnamed:1, 2, 2):\"*\"]",
                "     └─<E>",
                "        ├─<T>",
                "        │  └─[*:(Unnamed:1, 3, 3):\"*\"]",
                "        └─<E>");
        }

        [TestMethod]
        public void Parser8() {
            Tokenizer tok = new();
            tok.Start("start");
            tok.Join("start", "a").AddSet("a");
            tok.SetToken("a", "a");

            Grammar grammar = new();
            grammar.Start("S");
            grammar.NewRule("S").AddTerm("E").AddTerm("E");
            grammar.NewRule("E").AddToken("a");
            Parser parser = new(grammar, tok);

            parser.Check("aa",
               "─<S>",
               "  ├─<E>",
               "  │  └─[a:(Unnamed:1, 1, 1):\"a\"]",
               "  └─<E>",
               "     └─[a:(Unnamed:1, 2, 2):\"a\"]");
        }

        [TestMethod]
        public void Parser9() {
            // See: http://www.cs.ecu.edu/karl/5220/spr16/Notes/Bottom-up/lr1.html

            Tokenizer tok = new();
            tok.Start("start");
            tok.Join("start", "c").AddSet("c");
            tok.SetToken("c", "c");
            tok.Join("start", "d").AddSet("d");
            tok.SetToken("d", "d");

            Grammar grammar = new();
            grammar.Start("S");
            grammar.NewRule("S").AddTerm("C").AddTerm("C");
            grammar.NewRule("C").AddToken("c").AddTerm("C");
            grammar.NewRule("C").AddToken("d");
            Parser parser = new(grammar, tok);

            parser.Check("dd",
                "─<S>",
                "  ├─<C>",
                "  │  └─[d:(Unnamed:1, 1, 1):\"d\"]",
                "  └─<C>",
                "     └─[d:(Unnamed:1, 2, 2):\"d\"]");

            parser.Check("cdd",
                "─<S>",
                "  ├─<C>",
                "  │  ├─[c:(Unnamed:1, 1, 1):\"c\"]",
                "  │  └─<C>",
                "  │     └─[d:(Unnamed:1, 2, 2):\"d\"]",
                "  └─<C>",
                "     └─[d:(Unnamed:1, 3, 3):\"d\"]");

            parser.Check("dcd",
                "─<S>",
                "  ├─<C>",
                "  │  └─[d:(Unnamed:1, 1, 1):\"d\"]",
                "  └─<C>",
                "     ├─[c:(Unnamed:1, 2, 2):\"c\"]",
                "     └─<C>",
                "        └─[d:(Unnamed:1, 3, 3):\"d\"]");

            parser.Check("cdcd",
                "─<S>",
                "  ├─<C>",
                "  │  ├─[c:(Unnamed:1, 1, 1):\"c\"]",
                "  │  └─<C>",
                "  │     └─[d:(Unnamed:1, 2, 2):\"d\"]",
                "  └─<C>",
                "     ├─[c:(Unnamed:1, 3, 3):\"c\"]",
                "     └─<C>",
                "        └─[d:(Unnamed:1, 4, 4):\"d\"]");
        }

        [TestMethod]
        public void Parser10() {
            Tokenizer tok = new();
            tok.Start("start");
            tok.JoinToToken("start", "A").AddSet("a");
            tok.JoinToToken("start", "B").AddSet("b");

            Grammar grammar = new();
            grammar.Start("S");
            grammar.NewRule("S").AddTerm("S");
            grammar.NewRule("S").AddToken("A");
            grammar.NewRule("S").AddToken("B");

            grammar.CheckParserBuildError(tok,
                "Exception: Parser can not use invalid grammar: "+
                    "There exists a rule for <S> which is nonproductive, <S> → <S>.");
        }
    }
}
