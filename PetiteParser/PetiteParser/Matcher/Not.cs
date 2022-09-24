using System.Collections.Generic;
using System.Text;

namespace PetiteParser.Matcher;

/// <summary>
/// A group of matchers which returns the opposite
/// of the contained group of matchers.
/// </summary>
sealed public class Not : Group {

    /// <summary>Creates a new not matcher.</summary>
    /// <param name="matchers">The initial matchers.</param>
    public Not(params IMatcher[] matchers) :
        base(matchers) { }

    /// <summary>Creates a new not matcher.</summary>
    /// <param name="matchers">The initial matchers.</param>
    public Not(IEnumerable<IMatcher> matchers) :
        base(matchers) { }

    /// <summary>Determines if this matcher matches the given character.</summary>
    /// <param name="c">The character to match.</param>
    /// <returns>The opposite of the matches in the group.</returns>
    public override bool Match(Rune c) => !base.Match(c);

    /// <summary>Returns the string for this matcher.</summary>
    /// <returns>The string for this matcher.</returns>
    public override string ToString() => "!["+base.ToString()+"]";
}
