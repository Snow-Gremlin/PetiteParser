using Microsoft.VisualStudio.TestTools.UnitTesting;
using PetiteParser.Diff;
using PetiteParser.Misc;
using System.Text;

namespace TestPetiteParser {

    /// <summary>This is a set of tools uses for testing.</summary>
    static public class TestTools {

        /// <summary>Checks the equality of the given strings and displays a diff if not equal.</summary>
        /// <param name="exp">The expected value.</param>
        /// <param name="result">The resulting value.</param>
        static public void AreEqual(string exp , string result) {
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
    }
}
