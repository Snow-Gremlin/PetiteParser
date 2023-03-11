using Microsoft.VisualStudio.TestTools.UnitTesting;
using PetiteParser.Diff;

namespace TestPetiteParser.DiffTests;

[TestClass]
sealed public class MergeTests {

    [TestMethod]
    public void Default01() => Diff.Default().CheckMerge(
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
