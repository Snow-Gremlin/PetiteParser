using Microsoft.VisualStudio.TestTools.UnitTesting;
using PetiteParser.Diff;
using PetiteParser.Grammar;
using PetiteParser.Misc;
using PetiteParser.Parser;
using PetiteParser.Tokenizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TestPetiteParser.Tools {
    static internal class Extensions {

        /// <summary>Checks the parser will parse the given input.</summary>
        static public void Check(this Parser parser, string input, params string[] expected) =>
            TestTools.AreEqual(expected.JoinLines(), parser.Parse(input).ToString());

        /// <summary>Checks the tokenizer will tokenize the given input.</summary>
        static public void Check(this Tokenizer tok, string input, params string[] expected) =>
            tok.Tokenize(input).CheckTokens(expected);

        /// <summary>Checks the tokens match the given input.</summary>
        static public void CheckTokens(this IEnumerable<Token> tokens, params string[] expected) =>
            Assert.AreEqual(expected.JoinLines(), tokens.JoinLines().Trim());

        /// <summary>Checks the tokenizer will fail with the given input.</summary>
        static public void CheckError(this Tokenizer tok, string input, params string[] expected) {
            StringBuilder resultBuf = new();
            try {
                foreach (Token token in tok.Tokenize(Watcher.Console, input))
                    resultBuf.AppendLine(token.ToString());
                Assert.Fail("Expected an exception but didn't get one.");
            } catch (Exception ex) {
                resultBuf.AppendLine(ex.Message);
            }
            TestTools.AreEqual(expected.JoinLines(), resultBuf.ToString().Trim());
        }

        /// <summary>Checks the grammar's string.</summary>
        static public void Check(this Grammar grammar, params string[] expected) =>
            TestTools.AreEqual(expected.JoinLines(), grammar.ToString().Trim());

        /// <summary>Checks the grammar term's first tokens results.</summary>
        static public void CheckFirstSets(this Grammar grammar, params string[] expected) =>
            TestTools.AreEqual(expected.JoinLines(), new Analyzer(grammar).ToString().Trim());

        /// <summary>Checks the grammar's first left recursion is as expected.</summary>
        static public void CheckFindFirstLeftRecursion(this Grammar grammar, params string[] expected) {
            Analyzer sets = new(grammar);
            Console.WriteLine(sets.ToString(true));
            TestTools.AreEqual(sets.FindFirstLeftRecursion().ToNames().JoinLines(), expected.JoinLines());
        }

        /// <summary>Checks if the given rule's string method.</summary>
        static public void CheckString(this Rule rule, int index, string exp) =>
            TestTools.AreEqual(exp, rule.ToString(index));

        /// <summary>Checks if the given lines diff with PlusMinus as expected.</summary>
        /// <param name="a">The list of strings for the first (added) source.</param>
        /// <param name="b">The list of strings for the second (removed) source.</param>
        /// <param name="exp">The expected result of the diff.</param>
        static public void CheckPlusMinus(this Diff diff, string[] a, string[] b, string[] exp) =>
            checkDiffs(a, b, exp, diff.PlusMinus(a, b).ToArray());

        /// <summary>Checks if the given lines diff with Merge as expected.</summary>
        /// <param name="a">The list of strings for the first (added) source.</param>
        /// <param name="b">The list of strings for the second (removed) source.</param>
        /// <param name="exp">The expected result of the diff.</param>
        static public void CheckMerge(this Diff diff, string[] a, string[] b, string[] exp) =>
            checkDiffs(a, b, exp, diff.Merge(a, b).ToArray());

        /// <summary>Checks if the given lines diff are as expected.</summary>
        /// <param name="a">The list of strings for the first (added) source.</param>
        /// <param name="b">The list of strings for the second (removed) source.</param>
        /// <param name="exp">The expected result of the diff.</param>
        /// <param name="result">The actual result of the diff.</param>
        static private void checkDiffs(string[] a, string[] b, string[] exp, string[] result) {
            string resultStr = result.Join("|");
            string expStr = exp.Join("|");
            Console.WriteLine("A Input:\n   "  + a.JoinLines("   ") + "\n");
            Console.WriteLine("B Input:\n   "  + b.JoinLines("   ") + "\n");
            Console.WriteLine("Expected:\n   " + exp.JoinLines("   ") + "\n");
            Console.WriteLine("Results:\n   "  + result.JoinLines("   ") + "\n");
            Assert.AreEqual(expStr, resultStr);
        }
    }
}
