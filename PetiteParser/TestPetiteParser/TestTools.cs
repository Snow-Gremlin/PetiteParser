using Microsoft.VisualStudio.TestTools.UnitTesting;
using PetiteParser.Diff;
using PetiteParser.Misc;
using S = System;

namespace TestPetiteParser {

    /// <summary>This is a set of tools uses for testing.</summary>
    static public class TestTools {

        /// <summary>Checks the equality of the given strings and displays a diff if not equal.</summary>
        /// <param name="exp">The expected value.</param>
        /// <param name="result">The resulting value.</param>
        static public void AreEqual(string exp , string result) {
            if (exp != result) {
                S.Console.WriteLine("Diff:");
                S.Console.WriteLine(Diff.Default().PlusMinus(exp, result));
                S.Console.WriteLine("Expected:");
                S.Console.WriteLine(exp);
                S.Console.WriteLine("Actual:");
                S.Console.WriteLine(result);
                S.Console.WriteLine("Escaped:");
                S.Console.WriteLine("  Expected: " + exp.Escape());
                S.Console.WriteLine("  Actual:   " + result.Escape());
                Assert.Fail();
            }
        }
    }
}
