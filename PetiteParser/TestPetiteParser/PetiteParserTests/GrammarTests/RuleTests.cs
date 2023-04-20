using Microsoft.VisualStudio.TestTools.UnitTesting;
using PetiteParser.Grammar;
using PetiteParser.Formatting;
using System.Linq;

namespace TestPetiteParser.PetiteParserTests.GrammarTests;

[TestClass]
sealed public class RuleTests {

    [TestMethod]
    public void Rule01String() {
        Grammar gram = new();
        Rule rule0 = gram.NewRule("E");
        Rule rule1 = gram.NewRule("E", "<E> [+] <E>");
        Rule rule2 = gram.NewRule("E", "<E> [+] <E> {add}");
        Rule rule3 = gram.NewRule("E", "<E> [+] {add} <E>");
        Rule rule4 = gram.NewRule("E", "{add} <E> [+] <E>");
        Rule rule5 = gram.NewRule("E", "{nope}");

        rule0.CheckString(-1, "<E> → λ");
        rule0.CheckString(0, "<E> → • λ");
        rule0.CheckString(1, "<E> → λ");

        rule1.CheckString(-1, "<E> → <E> [+] <E>");
        rule1.CheckString(0, "<E> → • <E> [+] <E>");
        rule1.CheckString(1, "<E> → <E> • [+] <E>");
        rule1.CheckString(2, "<E> → <E> [+] • <E>");
        rule1.CheckString(3, "<E> → <E> [+] <E> •");
        rule1.CheckString(4, "<E> → <E> [+] <E>");

        rule2.CheckString(-1, "<E> → <E> [+] <E> {add}");
        rule2.CheckString(0, "<E> → • <E> [+] <E> {add}");
        rule2.CheckString(1, "<E> → <E> • [+] <E> {add}");
        rule2.CheckString(2, "<E> → <E> [+] • <E> {add}");
        rule2.CheckString(3, "<E> → <E> [+] <E> • {add}");
        rule2.CheckString(4, "<E> → <E> [+] <E> {add}");

        rule3.CheckString(-1, "<E> → <E> [+] {add} <E>");
        rule3.CheckString(0, "<E> → • <E> [+] {add} <E>");
        rule3.CheckString(1, "<E> → <E> • [+] {add} <E>");
        rule3.CheckString(2, "<E> → <E> [+] • {add} <E>");
        rule3.CheckString(3, "<E> → <E> [+] {add} <E> •");
        rule3.CheckString(4, "<E> → <E> [+] {add} <E>");

        rule4.CheckString(-1, "<E> → {add} <E> [+] <E>");
        rule4.CheckString(0, "<E> → • {add} <E> [+] <E>");
        rule4.CheckString(1, "<E> → {add} <E> • [+] <E>");
        rule4.CheckString(2, "<E> → {add} <E> [+] • <E>");
        rule4.CheckString(3, "<E> → {add} <E> [+] <E> •");
        rule4.CheckString(4, "<E> → {add} <E> [+] <E>");

        rule5.CheckString(-1, "<E> → {nope} λ");
        rule5.CheckString(0, "<E> → • {nope} λ");
        rule5.CheckString(1, "<E> → {nope} λ");
    }

    static private void checkRule(Grammar g, string items, string expItems, string expBaseItems, bool expLambda) {
        Rule r = g.NewRule("T", items);
        Assert.AreEqual(expItems,     r.Items.Join(", "));
        Assert.AreEqual(expBaseItems, r.BasicItems.Join(", "));
        Assert.AreEqual(expLambda,    r.IsLambda);
    }
    
    [TestMethod]
    public void BaseItems() {
        Grammar g = new();
        checkRule(g, "",           "",              "",         true);
        checkRule(g, "<C>",        "<C>",           "<C>",      false);
        checkRule(g, "[D]",        "[D]",           "[D]",      false);
        checkRule(g, "[D]<C>",     "[D], <C>",      "[D], <C>", false);
        checkRule(g, "<C>[D]",     "<C>, [D]",      "<C>, [D]", false);
        checkRule(g, "{A}[D]{B}",  "{A}, [D], {B}", "[D]",      false);
        checkRule(g, "{A}",        "{A}",           "",         true);
        checkRule(g, "{A}{B}",     "{A}, {B}",      "",         true);
        checkRule(g, "{A}<C>{B}",  "{A}, <C>, {B}", "<C>",      false);
        checkRule(g, "[D]{A}<C>",  "[D], {A}, <C>", "[D], <C>", false);
    }

    // TODO: Check Equal
    // TODO: Check Same
    // TODO: Check CompareTo
}
