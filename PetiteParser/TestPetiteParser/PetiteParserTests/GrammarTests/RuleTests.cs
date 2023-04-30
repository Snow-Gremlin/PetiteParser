using Microsoft.VisualStudio.TestTools.UnitTesting;
using PetiteParser.Formatting;
using PetiteParser.Grammar;
using System.Linq;

namespace TestPetiteParser.PetiteParserTests.GrammarTests;

[TestClass]
sealed public class RuleTests {

    [TestMethod]
    public void RulesAdd() {
        Grammar g = new();

        Rule r1 = g.NewRule("T").AddTerm("A").AddToken("B").AddPrompt("C");
        Assert.AreEqual("<T> → <A> [B] {C}", r1.ToString());

        Rule r2 = g.NewRule("T").AddItems("[D] {E} <F>");
        Assert.AreEqual("<T> → [D] {E} <F>", r2.ToString());
        
        Rule r3 = g.NewRule("T", "{G} <H> [I]");
        Assert.AreEqual("<T> → {G} <H> [I]", r3.ToString());
    }

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

    [TestMethod]
    public void BaseItems() {
        Grammar g = new();
        void check(string items, string expItems, string expBaseItems, bool expLambda) {
            Rule r = g.NewRule("T", items);
            Assert.AreEqual(expItems,     r.Items.Join(", "));
            Assert.AreEqual(expBaseItems, r.BasicItems.Join(", "));
            Assert.AreEqual(expLambda,    r.IsLambda);
        }

        check("",           "",              "",         true);
        check("<C>",        "<C>",           "<C>",      false);
        check("[D]",        "[D]",           "[D]",      false);
        check("[D]<C>",     "[D], <C>",      "[D], <C>", false);
        check("<C>[D]",     "<C>, [D]",      "<C>, [D]", false);
        check("{A}[D]{B}",  "{A}, [D], {B}", "[D]",      false);
        check("{A}",        "{A}",           "",         true);
        check("{A}{B}",     "{A}, {B}",      "",         true);
        check("{A}<C>{B}",  "{A}, <C>, {B}", "<C>",      false);
        check("[D]{A}<C>",  "[D], {A}, <C>", "[D], <C>", false);
    }

    [TestMethod]
    public void RulesEqualCompareTo() {
        Grammar g = new();
        void check(string leftTerm, string leftItems, string rightTerm, string rightItems, int expCmp) {
            Rule r1 = g.NewRule(leftTerm, leftItems);
            Rule r2 = g.NewRule(rightTerm, rightItems);
            Assert.AreEqual(expCmp == 0, r1.Equals(r2));
            Assert.AreEqual(expCmp == 0, r2.Equals(r1));
            Assert.AreEqual(expCmp, r1.CompareTo(r2));
            Assert.AreEqual(-expCmp, r2.CompareTo(r1));
        }
        check("A", "", "A", "", 0);
        check("A", "", "B", "", -1);
        check("A", "<A>", "A", "<A>", 0);
        check("A", "<A>", "A", "<B>", -1);
        check("A", "[A]", "A", "[A]", 0);
        check("A", "[A]", "A", "[B]", -1);
        check("A", "{A}", "A", "{A}", 0);
        check("A", "{A}", "A", "{B}", -1);
        check("A", "<A>", "A", "[A]", -1);
        check("A", "<A>", "A", "{A}", -2);
        check("A", "[A]", "A", "{A}", -1);
        check("A", "<A>", "A", "<A><A>", -1);
        check("A", "<A><A>", "A", "<B>", -1);
        check("A", "<A> [B] {C}", "A", "<A> [B] {C}", 0);
    }

    [TestMethod]
    public void RulesSame() {
        Grammar g = new();
        void check(string leftTerm, string leftItems, string rightTerm, string rightItems, string target, string alias, bool expSame) {
            Rule r1 = g.NewRule(leftTerm, leftItems);
            Rule r2 = g.NewRule(rightTerm, rightItems);
            Term t1 = g.Term(target);
            Term t2 = g.Term(alias);
            Assert.AreEqual(expSame, r1.Same(r2, t1, t2));
        }
        check("A", "<A><B>[C]", "B", "<A><B>", "A", "B", false);
        check("A", "<A><B>", "B", "<A><B>[C]", "A", "B", false);
        check("A", "<A><B>[C]", "B", "<A><B>[D]", "C", "D", false);
        check("A", "<A><B><C>", "B", "<A><B><D>", "C", "D", true);
        check("A", "<A><B><D>", "B", "<A><B><C>", "C", "D", true);
        check("A", "<A><B><C>", "B", "<A><B><D>", "D", "C", true);
        check("A", "<A><B><D>", "B", "<A><B><C>", "D", "C", true);
        check("A", "<A><B><C>", "B", "<A><B><D>", "D", "A", false);
    }
}
