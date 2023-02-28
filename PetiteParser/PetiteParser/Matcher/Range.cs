using System.Linq;
using System.Text;

namespace PetiteParser.Matcher;

/// <summary>A matcher to match a range of characters.</summary>
public class Range: IMatcher {

    /// <summary>The lowest character value included in this range.</summary>
    public readonly Rune Low;

    /// <summary>The highest character value included in this range.</summary>
    public readonly Rune High;

    /// <summary>Creates a new range matcher.</summary>
    /// <param name="low">The lower rune inclusively in the range.</param>
    /// <param name="high">The higher rune inclusively in the range.</param>
    public Range(string low, string high) :
        this(low.EnumerateRunes().First(), high.EnumerateRunes().First()) { }

    /// <summary>Creates a new range matcher.</summary>
    /// <param name="low">The lower rune inclusively in the range.</param>
    /// <param name="high">The higher rune inclusively in the range.</param>
    public Range(char low, char high) :
        this(new Rune(low), new Rune(high)) { }

    /// <summary>Creates a new range matcher.</summary>
    /// <param name="low">The lower rune inclusively in the range.</param>
    /// <param name="high">The higher rune inclusively in the range.</param>
    public Range(Rune low, Rune high) {
        if (low < high) {
            this.Low = low;
            this.High = high;
        } else {
            this.Low = high;
            this.High = low;
        }
    }

    /// <summary>Determines if this matcher matches the given character.</summary>
    /// <param name="c">The character to match.</param>
    /// <returns>True if the character is inclusively in the given range, false otherwise.</returns>
    public bool Match(Rune c) => (this.Low <= c) && (this.High >= c);

    /// <summary>Returns the string for this matcher.</summary>
    /// <returns>The string for this matcher.</returns>
    public override string ToString() => this.Low + ".." + this.High;
}
