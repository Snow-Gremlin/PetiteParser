using Microsoft.VisualStudio.TestTools.UnitTesting;
using PetiteParser.Diff;
using TestPetiteParser.Tools;

namespace TestPetiteParser.UnitTests;

[TestClass]
sealed public class DiffTests {

    [TestMethod]
    public void DefaultDiff1() =>
        Diff.Default().CheckPlusMinus(
            new string[] { "cat" },
            new string[] { "cat" },
            new string[] { " cat" });

    [TestMethod]
    public void DefaultDiff2() =>
        Diff.Default().CheckPlusMinus(
            new string[] { "cat" },
            new string[] { "dog" },
            new string[] { "-cat", "+dog" });

    [TestMethod]
    public void DefaultDiff3() =>
        Diff.Default().CheckPlusMinus(
            new string[] { "A", "G", "T", "A", "C", "G", "C", "A" },
            new string[] { "T", "A", "T", "G", "C" },
            new string[] { "+T", " A", "-G", " T", "-A", "-C", " G", " C", "-A" });

    [TestMethod]
    public void DefaultDiff4() =>
        Diff.Default().CheckPlusMinus(
            new string[] { "cat", "dog" },
            new string[] { "cat", "horse" },
            new string[] { " cat", "-dog", "+horse" });

    [TestMethod]
    public void DefaultDiff5() =>
        Diff.Default().CheckPlusMinus(
            new string[] { "cat", "dog" },
            new string[] { "cat", "horse", "dog" },
            new string[] { " cat", "+horse", " dog" });

    [TestMethod]
    public void DefaultDiff6() =>
        Diff.Default().CheckPlusMinus(
            new string[] { "cat", "dog", "pig" },
            new string[] { "cat", "horse", "dog" },
            new string[] { " cat", "+horse", " dog", "-pig" });

    [TestMethod]
    public void DefaultDiff7() =>
        Diff.Default().CheckPlusMinus(
            new string[] { "Mike", "Ted", "Mark", "Jim" },
            new string[] { "Ted", "Mark", "Bob", "Bill" },
            new string[] { "-Mike", " Ted", " Mark", "-Jim", "+Bob", "+Bill" });

    [TestMethod]
    public void DefaultDiff8() =>
        Diff.Default().CheckPlusMinus(
            new string[] { "k", "i", "t", "t", "e", "n" },
            new string[] { "s", "i", "t", "t", "i", "n", "g" },
            new string[] { "-k", "+s", " i", " t", " t", "-e", "+i", " n", "+g" });

    [TestMethod]
    public void DefaultDiff9() =>
        Diff.Default().CheckPlusMinus(
            new string[] { "s", "a", "t", "u", "r", "d", "a", "y" },
            new string[] { "s", "u", "n", "d", "a", "y" },
            new string[] { " s", "-a", "-t", " u", "-r", "+n", " d", " a", " y" });

    [TestMethod]
    public void DefaultDiff10() =>
        Diff.Default().CheckPlusMinus(
            new string[] { "s", "a", "t", "x", "r", "d", "a", "y" },
            new string[] { "s", "u", "n", "d", "a", "y" },
            new string[] { " s", "-a", "-t", "-x", "-r", "+u", "+n", " d", " a", " y" });

    [TestMethod]
    public void DefaultDiff11() => 
        Diff.Default().CheckPlusMinus(
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
    public void DefaultDiff12() =>
        Diff.Default().CheckMerge(
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
