using Microsoft.VisualStudio.TestTools.UnitTesting;
using PetiteParser.Diff;
using PetiteParser.Misc;
using System.Linq;
using S = System;

namespace TestPetiteParser {

    [TestClass]
    public class DiffTests {

        /// <summary>Checks if the given lines diff with PlusMinus as expected.</summary>
        /// <param name="a">The list of strings for the first (added) source.</param>
        /// <param name="b">The list of strings for the second (removed) source.</param>
        /// <param name="exp">The expected result of the diff.</param>
        static private void checkPlusMinus(Diff diff, string[] a, string[] b, string[] exp) =>
            check(a, b, exp, diff.PlusMinus(a, b).ToArray());

        /// <summary>Checks if the given lines diff with Merge as expected.</summary>
        /// <param name="a">The list of strings for the first (added) source.</param>
        /// <param name="b">The list of strings for the second (removed) source.</param>
        /// <param name="exp">The expected result of the diff.</param>
        static private void checkMerge(Diff diff, string[] a, string[] b, string[] exp) =>
            check(a, b, exp, diff.Merge(a, b).ToArray());

        /// <summary>Checks if the given lines diff are as expected.</summary>
        /// <param name="a">The list of strings for the first (added) source.</param>
        /// <param name="b">The list of strings for the second (removed) source.</param>
        /// <param name="exp">The expected result of the diff.</param>
        /// <param name="result">The actual result of the diff.</param>
        static private void check(string[] a, string[] b, string[] exp, string[] result) {
            string resultStr = result.Join("|");
            string expStr = exp.Join("|");
            S.Console.WriteLine("A Input: " + S.Environment.NewLine + "   " + a     .JoinLines("   ") + S.Environment.NewLine);
            S.Console.WriteLine("B Input: " + S.Environment.NewLine + "   " + b     .JoinLines("   ") + S.Environment.NewLine);
            S.Console.WriteLine("Expected:" + S.Environment.NewLine + "   " + exp   .JoinLines("   ") + S.Environment.NewLine);
            S.Console.WriteLine("Results: " + S.Environment.NewLine + "   " + result.JoinLines("   ") + S.Environment.NewLine);
            Assert.AreEqual(expStr, resultStr);
        }

        [TestMethod]
        public void DefaultDiff1() => checkPlusMinus(Diff.Default(),
            new string[] { "cat" },
            new string[] { "cat" },
            new string[] { " cat" });


        [TestMethod]
        public void DefaultDiff2() => checkPlusMinus(Diff.Default(),
            new string[] { "cat" },
            new string[] { "dog" },
            new string[] { "-cat", "+dog" });

        [TestMethod]
        public void DefaultDiff3() => checkPlusMinus(Diff.Default(),
            new string[] { "A", "G", "T", "A", "C", "G", "C", "A" },
            new string[] { "T", "A", "T", "G", "C" },
            new string[] { "+T", " A", "-G", " T", "-A", "-C", " G", " C", "-A" });

        [TestMethod]
        public void DefaultDiff4() => checkPlusMinus(Diff.Default(),
            new string[] { "cat", "dog" },
            new string[] { "cat", "horse" },
            new string[] { " cat", "-dog", "+horse" });

        [TestMethod]
        public void DefaultDiff5() => checkPlusMinus(Diff.Default(),
            new string[] { "cat", "dog" },
            new string[] { "cat", "horse", "dog" },
            new string[] { " cat", "+horse", " dog" });

        [TestMethod]
        public void DefaultDiff6() => checkPlusMinus(Diff.Default(),
            new string[] { "cat", "dog", "pig" },
            new string[] { "cat", "horse", "dog" },
            new string[] { " cat", "+horse", " dog", "-pig" });

        [TestMethod]
        public void DefaultDiff7() => checkPlusMinus(Diff.Default(),
            new string[] { "Mike", "Ted", "Mark", "Jim" },
            new string[] { "Ted", "Mark", "Bob", "Bill" },
            new string[] { "-Mike", " Ted", " Mark", "-Jim", "+Bob", "+Bill" });

        [TestMethod]
        public void DefaultDiff8() => checkPlusMinus(Diff.Default(),
            new string[] { "k", "i", "t", "t", "e", "n" },
            new string[] { "s", "i", "t", "t", "i", "n", "g" },
            new string[] { "-k", "+s", " i", " t", " t", "-e", "+i", " n", "+g" });

        [TestMethod]
        public void DefaultDiff9() => checkPlusMinus(Diff.Default(),
            new string[] { "s", "a", "t", "u", "r", "d", "a", "y" },
            new string[] { "s", "u", "n", "d", "a", "y" },
            new string[] { " s", "-a", "-t", " u", "-r", "+n", " d", " a", " y" });

        [TestMethod]
        public void DefaultDiff10() => checkPlusMinus(Diff.Default(),
            new string[] { "s", "a", "t", "x", "r", "d", "a", "y" },
            new string[] { "s", "u", "n", "d", "a", "y" },
            new string[] { " s", "-a", "-t", "-x", "-r", "+u", "+n", " d", " a", " y" });

        [TestMethod]
        public void DefaultDiff11() => checkPlusMinus(Diff.Default(),
            new string[] {
                 "function A() int {",
                 "  return 10",
                 "}",
                 "",
                 "function C() int {",
                 "  a := 12",
                 "  return a",
                 "}" },
            new string[] {
                 "function A() int {",
                 "  return 10",
                 "}",
                 "",
                 "function B() int {",
                 "  return 11",
                 "}",
                 "",
                 "function C() int {",
                 "  return 12",
                 "}" },
            new string[] {
                 " function A() int {",
                 "   return 10",
                 " }",
                 " ",
                 "+function B() int {",
                 "+  return 11",
                 "+}",
                 "+",
                 " function C() int {",
                 "-  a := 12",
                 "-  return a",
                 "+  return 12",
                 " }" });

        [TestMethod]
        public void DefaultDiff12() => checkMerge(Diff.Default(),
            new string[] {
                 "function A() int {",
                 "  return 10",
                 "}",
                 "",
                 "function C() int {",
                 "  a := 12",
                 "  return a",
                 "}" },
            new string[] {
                 "function A() int {",
                 "  return 10",
                 "}",
                 "",
                 "function B() int {",
                 "  return 11",
                 "}",
                 "",
                 "function C() int {",
                 "  return 12",
                 "}" },
            new string[] {
                 "function A() int {",
                 "  return 10",
                 "}",
                 "",
                 "<<<<<<<<",
                 "========",
                 "function B() int {",
                 "  return 11",
                 "}",
                 "",
                 ">>>>>>>>",
                 "function C() int {",
                 "<<<<<<<<",
                 "  a := 12",
                 "  return a",
                 "========",
                 "  return 12",
                 ">>>>>>>>",
                 "}"});
    }
}
