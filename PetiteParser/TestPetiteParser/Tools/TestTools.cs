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
            StringBuilder buffer = new();
            buffer.AppendLine();
            buffer.AppendLine("Diff:");
            buffer.AppendLine(Diff.Default().PlusMinus(exp, result).IndentLines(" "));

            buffer.AppendLine("Expected:");
            buffer.AppendLine(exp.IndentLines("  "));

            buffer.AppendLine("Actual:");
            buffer.AppendLine(result.IndentLines("  "));

            buffer.AppendLine("Escaped:");
            buffer.AppendLine("  Expected: " + exp.Escape());
            buffer.Append("  Actual:   " + result.Escape());
            Assert.Fail(buffer.ToString());
        }
    }

    /// <summary>Checks if the given string is matched by the given regular expression pattern.</summary>
    /// <param name="pattern">The pattern to match the given string against.</param>
    /// <param name="result">The given string to match with the regular expression.</param>
    static public void RegexMatch(string pattern, string result) {
        Regex r = new(pattern);
        if (!r.IsMatch(result)) {
            StringBuilder buffer = new();
            buffer.AppendLine();
            buffer.AppendLine("Result failed to match regular expression:");
            buffer.AppendLine("  Pattern:");
            buffer.AppendLine(pattern.IndentLines("    "));
            buffer.AppendLine("  Actual:");
            buffer.Append(result.IndentLines("    "));
            Assert.Fail(buffer.ToString());
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
