using System.Text;

namespace PetiteParser.Tokenizer.Matcher;

/// <summary>
/// A matcher which matches all characters.
/// Since transitions are called in the order they are added
/// this matcher can be used as an "else" matcher.
/// </summary>
sealed public class All : IMatcher {

    /// <summary>Determines if this matcher matches the given character.</summary>
    /// <param name="c">The character to match.</param>
    /// <returns>In this case it always returns true.</returns>
    public bool Match(Rune c) => true;

    /// <summary>Returns the string for this matcher.</summary>
    /// <returns>The string for this matcher.</returns>
    public override string ToString() => "all";
}
