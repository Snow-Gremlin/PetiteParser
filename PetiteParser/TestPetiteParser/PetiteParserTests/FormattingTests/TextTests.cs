using Microsoft.VisualStudio.TestTools.UnitTesting;
using PetiteParser.Formatting;
using System;
using System.Numerics;
using System.Text;

namespace TestPetiteParser.PetiteParserTests.FormattingTests;

[TestClass]
sealed public class TextTests {

    static private void assertEscape(string input, string expected) =>
        Assert.AreEqual(expected, Text.Escape(input));

    static private void assertUnescape(string input, string expected) =>
        Assert.AreEqual(expected, Text.Unescape(input));

    static private void assertValueToString(object? input, string expected) =>
        Assert.AreEqual(expected, Text.ValueToString(input));

    sealed private class TestException : Exception {
        public TestException() : base() { }
        public TestException(string message) : base(message) { }
    }
    
    sealed private class TestObject {
        public TestObject(string? msg) => this.msg = msg;
        private string? msg { get; }
        public override string? ToString() => this.msg;
    }

    [TestMethod]
    public void Escape() {
        assertEscape("Hello World", "Hello World");
        assertEscape("\n\r\0\t\b\v\f", "\\n\\r\\0\\t\\b\\v\\f");
        assertEscape("\"'\\", "\\\"\\'\\\\");
        assertEscape("ç👽\uFEED", "\\xE7\\U0001F47D\\uFEED");
    }

    [TestMethod]
    public void Unescape() {
        assertUnescape("Hello World", "Hello World");
        assertUnescape("\\n\\r\\0\\t\\b\\v\\f", "\n\r\0\t\b\v\f");
        assertUnescape("\\\"\\'\\\\", "\"'\\");
        assertUnescape("\\xE7\\U0001F47D\\uFEED", "ç👽\uFEED");
    }

    [TestMethod]
    public void ValueToStringNull() =>
        assertValueToString(null, "null");

    [TestMethod]
    public void ValueToStringBool() {
        assertValueToString(true, "true");
        assertValueToString(false, "false");
    }

    [TestMethod]
    public void ValueToStringException() {
        assertValueToString(new TestException(), "Exception of type '"+typeof(TestException)+"' was thrown.");
        assertValueToString(new TestException("oops"), "oops");
    }

    [TestMethod]
    public void ValueToStringDouble() {
        assertValueToString(0.0, "0.0");
        assertValueToString(0.01, "0.01");
        assertValueToString(-0.01, "-0.01");
        assertValueToString(1.0, "1.0");
        assertValueToString(-1.0, "-1.0");

        assertValueToString(double.Pi, "3.141592653589793");
        assertValueToString(-double.Pi, "-3.141592653589793");
        assertValueToString(1.2e12, "1200000000000.0");
        assertValueToString(1.2e23, "1.2e+23");
        assertValueToString(1.2e-12, "1.2e-12");

        assertValueToString(double.MaxValue, "1.7976931348623157e+308");
        assertValueToString(double.MinValue, "-1.7976931348623157e+308");
        assertValueToString(double.Epsilon, "5e-324");
        assertValueToString(double.PositiveInfinity, "infinity");
        assertValueToString(double.NegativeInfinity, "-infinity");
        assertValueToString(double.NaN, "nan");
    }

    [TestMethod]
    public void ValueToStringFloat() {
        assertValueToString(0.0f, "0.0");
        assertValueToString(0.01f, "0.01");
        assertValueToString(-0.01f, "-0.01");
        assertValueToString(1.0f, "1.0");
        assertValueToString(-1.0f, "-1.0");

        assertValueToString(float.Pi, "3.1415927");
        assertValueToString(-float.Pi, "-3.1415927");
        assertValueToString(1.2e12f, "1.2e+12");
        assertValueToString(1.2e23f, "1.2e+23");
        assertValueToString(1.2e-12f, "1.2e-12");

        assertValueToString(float.MaxValue, "3.4028235e+38");
        assertValueToString(float.MinValue, "-3.4028235e+38");
        assertValueToString(float.Epsilon, "1e-45");
        assertValueToString(float.PositiveInfinity, "infinity");
        assertValueToString(float.NegativeInfinity, "-infinity");
        assertValueToString(float.NaN, "nan");
    }

    [TestMethod]
    public void ValueToStringChar() {
        assertValueToString('a', "a");
        assertValueToString('\n', "\\n");
        assertValueToString('\t', "\\t");
        assertValueToString(' ', " ");
        assertValueToString('ç', "\\xE7");
    }

    [TestMethod]
    public void ValueToStringRune() {
        assertValueToString(new Rune('a'), "a");
        assertValueToString(new Rune('\n'), "\\n");
        assertValueToString(new Rune(0xE7), "\\xE7");
        assertValueToString(new Rune(0xFEED), "\\uFEED");
        assertValueToString(new Rune(0x1F47D), "\\U0001F47D");
    }

    [TestMethod]
    public void ValueToStringString() {
        assertValueToString("Hello World", "Hello World");
        assertValueToString("\n\r\0\t\b\v\f", "\\n\\r\\0\\t\\b\\v\\f");
        assertValueToString("\"'\\", "\\\"\\'\\\\");
        assertValueToString("ç👽\uFEED", "\\xE7\\U0001F47D\\uFEED");
    }

    [TestMethod]
    public void ValueToStringOther() {
        assertValueToString(new TestObject(null), "null");
        assertValueToString(new TestObject("Hello"), "Hello");
        assertValueToString(new TestObject("👽"), "\\U0001F47D");
        assertValueToString(new Complex(1.0, 0.3), "<1; 0.3>");
    }
}
