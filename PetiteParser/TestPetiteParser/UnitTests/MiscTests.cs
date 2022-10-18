using Microsoft.VisualStudio.TestTools.UnitTesting;
using PetiteParser.Formatting;
using PetiteParser.Misc;
using System;

namespace TestPetiteParser.UnitTests;

[TestClass]
sealed public class MiscTests {

    [TestMethod]
    public void IsSorted() {
        checkIsSorted(true);
        checkIsSorted(true, 1);
        checkIsSorted(true, 1, 2, 3, 4, 5);
        checkIsSorted(true, 1, 1, 1, 1, 1);
        checkIsSorted(true, 1, 1, 6, 9, 9);
        checkIsSorted(false, 2, 1, 3, 4, 5);
    }

    static private void checkIsSorted(bool exp, params int[] input) =>
        Assert.AreEqual(exp, input.IsSorted(), "[{0}]", input.Join(", "));
}
