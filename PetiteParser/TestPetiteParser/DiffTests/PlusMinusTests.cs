using Microsoft.VisualStudio.TestTools.UnitTesting;
using PetiteParser.Diff;

namespace TestPetiteParser.DiffTests;

[TestClass]
sealed public class PlusMinusTests {

    [TestMethod]
    public void Default01() => Diff.Default().CheckPlusMinus(
        new string[] { "cat" },
        new string[] { "cat" },
        new string[] { " cat" });

    [TestMethod]
    public void Default02() => Diff.Default().CheckPlusMinus(
        new string[] { "cat" },
        new string[] { "dog" },
        new string[] { "-cat", "+dog" });

    [TestMethod]
    public void Default03() => Diff.Default().CheckPlusMinus(
        new string[] { "A", "G", "T", "A", "C", "G", "C", "A" },
        new string[] { "T", "A", "T", "G", "C" },
        new string[] { "+T", " A", "-G", " T", "-A", "-C", " G", " C", "-A" });

    [TestMethod]
    public void Default04() => Diff.Default().CheckPlusMinus(
        new string[] { "cat", "dog" },
        new string[] { "cat", "horse" },
        new string[] { " cat", "-dog", "+horse" });

    [TestMethod]
    public void Default05() => Diff.Default().CheckPlusMinus(
        new string[] { "cat", "dog" },
        new string[] { "cat", "horse", "dog" },
        new string[] { " cat", "+horse", " dog" });

    [TestMethod]
    public void Default06() => Diff.Default().CheckPlusMinus(
        new string[] { "cat", "dog", "pig" },
        new string[] { "cat", "horse", "dog" },
        new string[] { " cat", "+horse", " dog", "-pig" });

    [TestMethod]
    public void Default07() => Diff.Default().CheckPlusMinus(
        new string[] { "Mike", "Ted", "Mark", "Jim" },
        new string[] { "Ted", "Mark", "Bob", "Bill" },
        new string[] { "-Mike", " Ted", " Mark", "-Jim", "+Bob", "+Bill" });

    [TestMethod]
    public void Default08() => Diff.Default().CheckPlusMinus(
        new string[] { "k", "i", "t", "t", "e", "n" },
        new string[] { "s", "i", "t", "t", "i", "n", "g" },
        new string[] { "-k", "+s", " i", " t", " t", "-e", "+i", " n", "+g" });

    [TestMethod]
    public void Default09() => Diff.Default().CheckPlusMinus(
        new string[] { "s", "a", "t", "u", "r", "d", "a", "y" },
        new string[] { "s", "u", "n", "d", "a", "y" },
        new string[] { " s", "-a", "-t", " u", "-r", "+n", " d", " a", " y" });

    [TestMethod]
    public void Default10() => Diff.Default().CheckPlusMinus(
        new string[] { "s", "a", "t", "x", "r", "d", "a", "y" },
        new string[] { "s", "u", "n", "d", "a", "y" },
        new string[] { " s", "-a", "-t", "-x", "-r", "+u", "+n", " d", " a", " y" });

    [TestMethod]
    public void Default11() => Diff.Default().CheckPlusMinus(
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
}
