using Microsoft.VisualStudio.TestTools.UnitTesting;
using PetiteParser.Grammar;

namespace TestPetiteParser.PetiteParserTests.GrammarTests;

[TestClass]
sealed public class ItemTests {

    static private void checkItemComp(string left, string right, int expComp) {
        Grammar g = new();
        Item? item1 = string.IsNullOrEmpty(left)  ? null : g.Item(left);
        Item? item2 = string.IsNullOrEmpty(right) ? null : g.Item(right);

        if (item1 is not null) {
            Assert.AreEqual(expComp, item1.CompareTo(item2), left + " =?= " + right + " => " + expComp);
            Assert.AreEqual(expComp == 0, item1 == item2, left + " == " + right);
            Assert.AreEqual(expComp != 0, item1 != item2, left + " != " + right);
            Assert.AreEqual(expComp >  0, item1 >  item2, left + " > "  + right);
            Assert.AreEqual(expComp >= 0, item1 >= item2, left + " >= " + right);
            Assert.AreEqual(expComp <  0, item1 <  item2, left + " < "  + right);
            Assert.AreEqual(expComp <= 0, item1 <= item2, left + " <= " + right);
        }

        if (item2 is not null) {
            Assert.AreEqual(-expComp, item2.CompareTo(item1), right + " =?= " + left + " => " + -expComp);
            Assert.AreEqual(-expComp == 0, item2 == item1, right + " == " + left);
            Assert.AreEqual(-expComp != 0, item2 != item1, right + " != " + left);
            Assert.AreEqual(-expComp >  0, item2 >  item1, right + " > "  + left);
            Assert.AreEqual(-expComp >= 0, item2 >= item1, right + " >= " + left);
            Assert.AreEqual(-expComp <  0, item2 <  item1, right + " < "  + left);
            Assert.AreEqual(-expComp <= 0, item2 <= item1, right + " <= " + left);
        }
    }

    [TestMethod]
    public void ItemComparison() {
        checkItemComp("<A>", "", 1);
        checkItemComp("[A]", "", 1);
        checkItemComp("{A}", "", 1);
        
        checkItemComp("<A>", "<A>", 0);
        checkItemComp("[A]", "<A>", 1);
        checkItemComp("{A}", "<A>", 2);
        checkItemComp("<A>", "<B>", -1);
        
        checkItemComp("<A>", "[A]", -1);
        checkItemComp("[A]", "[A]", 0);
        checkItemComp("{A}", "[A]", 1);
        checkItemComp("[A]", "[B]", -1);

        checkItemComp("<A>", "{A}", -2);
        checkItemComp("[A]", "{A}", -1);
        checkItemComp("{A}", "{A}", 0);
        checkItemComp("{A}", "{B}", -1);
    }
}
