using Microsoft.VisualStudio.TestTools.UnitTesting;
using PetiteParser.Formatting;
using System;
using System.Collections.Generic;

namespace TestPetiteParser.PetiteParserTests.FormattingTests;

[TestClass]
sealed public class StringExtTests {

    [TestMethod]
    public void IndentLinesTests() {
        Assert.AreEqual(
            ">>Hello",
            "Hello".IndentLines(">>"));
        Assert.AreEqual(
            ">>Hello"+Environment.NewLine+">>Mad"+Environment.NewLine+">>World",
            "Hello\nMad\nWorld".IndentLines(">>"));
    }

    [TestMethod]
    public void JoinTests() {
        List<int> lines = new() { 1, 2, 3, 4 };
        Assert.AreEqual("1234", lines.Join());
        Assert.AreEqual("1, 2, 3, 4", lines.Join(", "));
        Assert.AreEqual("1:2:3:4", lines.Join(":"));
    }

    [TestMethod]
    public void JoinLinesTests() {
        List<int> lines = new() { 1, 2, 3 };
        Assert.AreEqual(
            "1"+Environment.NewLine+"2"+Environment.NewLine+"3",
            lines.JoinLines());
        Assert.AreEqual(
            "1"+Environment.NewLine+">>2"+Environment.NewLine+">>3",
            lines.JoinLines(">>"));
    }

    [TestMethod]
    public void SplitLinesTests() {
        string[] exp = new string[] { "Hello", "Mad", "World" };
        CollectionAssert.AreEqual(exp, "Hello\nMad\nWorld".SplitLines());
        CollectionAssert.AreEqual(exp, "Hello\r\nMad\rWorld".SplitLines());
    }
}
