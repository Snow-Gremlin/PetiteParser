using Microsoft.VisualStudio.TestTools.UnitTesting;
using PetiteParser.Diff;
using PetiteParser.Formatting;
using System;
using System.Text;
using System.Text.RegularExpressions;

namespace TestPetiteParser.Tools;

/// <summary>This is a set of tools uses for testing.</summary>
static internal class TestTools {

    /// <summary>Checks the equality of the given strings and displays a diff if not equal.</summary>
    /// <param name="exp">The expected value.</param>
    /// <param name="result">The resulting value.</param>
    static public void AreEqual(string exp, string result) {
        if (exp != result) {
            StringBuilder buf = new();
            buf.AppendLine();
            buf.AppendLine("Diff:");
            buf.AppendLine(Diff.Default().PlusMinus(exp, result).IndentLines(" "));

            buf.AppendLine("Expected:");
            buf.AppendLine(exp.IndentLines("  "));

            buf.AppendLine("Actual:");
            buf.AppendLine(result.IndentLines("  "));

            buf.AppendLine("Escaped:");
            buf.AppendLine("  Expected: " + exp.Escape());
            buf.Append("  Actual:   " + result.Escape());
            Assert.Fail(buf.ToString());
        }
    }

    /// <summary>Checks if the given string is matched by the given regular expression pattern.</summary>
    /// <param name="pattern">The pattern to match the given string against.</param>
    /// <param name="result">The given string to match with the regular expression.</param>
    static public void RegexMatch(string pattern, string result) {
        Regex r = new(pattern);
        if (!r.IsMatch(result)) {
            StringBuilder buf = new();
            buf.AppendLine();
            buf.AppendLine("Result failed to match regular expression:");
            buf.AppendLine("  Pattern:");
            buf.AppendLine(pattern.IndentLines("    "));
            buf.AppendLine("  Actual:");
            buf.Append(result.IndentLines("    "));
            Assert.Fail(buf.ToString());
        }
    }

    /// <summary>Checks that an expected error is thrown from the given action.</summary>
    static public void ThrowsException(Action handle, params string[] expected) {
        try {
            handle();
        } catch (Exception err) {
            TestTools.AreEqual(expected.JoinLines(), err.Message.TrimEnd());
            return;
        }
        Assert.Fail("Expected an exception none. Expected:" +
            Environment.NewLine + "  " + expected.JoinLines().IndentLines("  "));
    }
}
