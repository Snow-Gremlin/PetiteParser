using Microsoft.VisualStudio.TestTools.UnitTesting;
using PetiteParser.Grammar;

namespace TestPetiteParser.PetiteParserTests.GrammarTests;

[TestClass]
internal class ItemTests {

    [TestMethod]
    public void ItemComparison() {
        Grammar g = new();
        Item item1 = g.Term("A");
        Item item2 = g.Prompt("A");
        Item item3 = g.Token("A");
        
        Item item4 = g.Term("B");
        Item item5 = g.Prompt("B");
        Item item6 = g.Token("B");

        Item item7 = g.Term("C");
        Item item8 = g.Prompt("C");
        Item item9 = g.Token("C");

        // TODO: Finish testing comparisons




    }
}
