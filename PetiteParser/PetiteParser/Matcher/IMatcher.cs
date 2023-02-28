using System.Text;

namespace PetiteParser;

/// <summary>
/// The interface used by transitions to determine is a character
/// will transition the tokenizer from one state to another.
/// </summary>
public interface IMatcher {

    /// <summary>Determines if this matcher matches the given character.</summary>
    /// <param name="c">The character to match.</param>
    /// <returns>True if it is a match, false otherwise.</returns>
    bool Match(Rune c);
}
