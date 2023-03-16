using Microsoft.VisualStudio.TestTools.UnitTesting;
using PetiteParser.Grammar;
using TestPetiteParser.PetiteParserTests.GrammarTests;

namespace TestPetiteParser.GrammarTests;

[TestClass]
sealed public class RuleTests {

    [TestMethod]
    public void Rule01String() {
        Grammar gram = new();
        Rule rule0 = gram.NewRule("E");
        Rule rule1 = gram.NewRule("E").AddTerm("E").AddToken("+").AddTerm("E");
        Rule rule2 = gram.NewRule("E").AddTerm("E").AddToken("+").AddTerm("E").AddPrompt("add");
        Rule rule3 = gram.NewRule("E").AddTerm("E").AddToken("+").AddPrompt("add").AddTerm("E");
        Rule rule4 = gram.NewRule("E").AddPrompt("add").AddTerm("E").AddToken("+").AddTerm("E");

        rule0.CheckString(-1, "<E> → λ");
        rule0.CheckString(0, "<E> → λ •");
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
    }
}
