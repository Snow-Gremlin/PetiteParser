using Microsoft.VisualStudio.TestTools.UnitTesting;
using PetiteParser.Diff;
using PetiteParser.Formatting;
using System;
using System.Linq;

namespace TestPetiteParser.Tools;

static public class DiffExt {
    
    /// <summary>Checks if the given lines diff with PlusMinus as expected.</summary>
    /// <param name="a">The list of strings for the first (added) source.</param>
    /// <param name="b">The list of strings for the second (removed) source.</param>
    /// <param name="exp">The expected result of the diff.</param>
    static public void CheckPlusMinus(this Diff diff, string[] a, string[] b, string[] exp) =>
        checkDiffs(a, b, exp, diff.PlusMinus(a, b).ToArray());

    /// <summary>Checks if the given lines diff with Merge as expected.</summary>
    /// <param name="a">The list of strings for the first (added) source.</param>
    /// <param name="b">The list of strings for the second (removed) source.</param>
    /// <param name="exp">The expected result of the diff.</param>
    static public void CheckMerge(this Diff diff, string[] a, string[] b, string[] exp) =>
        checkDiffs(a, b, exp, diff.Merge(a, b).ToArray());

    /// <summary>Checks if the given lines diff are as expected.</summary>
    /// <param name="a">The list of strings for the first (added) source.</param>
    /// <param name="b">The list of strings for the second (removed) source.</param>
    /// <param name="exp">The expected result of the diff.</param>
    /// <param name="result">The actual result of the diff.</param>
    static private void checkDiffs(string[] a, string[] b, string[] exp, string[] result) {
        string resultStr = result.Join("|");
        string expStr = exp.Join("|");
        Console.WriteLine("A Input:\n   "  + a.JoinLines("   ") + "\n");
        Console.WriteLine("B Input:\n   "  + b.JoinLines("   ") + "\n");
        Console.WriteLine("Expected:\n   " + exp.JoinLines("   ") + "\n");
        Console.WriteLine("Results:\n   "  + result.JoinLines("   ") + "\n");
        Assert.AreEqual(expStr, resultStr);
    }
}
