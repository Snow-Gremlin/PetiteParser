using System.Text;

namespace PetiteParser.Tokenizer.Matcher;

/// <summary>A matcher which matches a single character.</summary>
sealed public class SingleChar : IMatcher {

    /// <summary>Creates a single matcher for the given character.</summary>
    /// <param name="singleChar">The character to match.</param>
    public SingleChar(char singleChar) => this.Rune = new(singleChar);

    /// <summary>Creates a single matcher for the given rune.</summary>
    /// <param name="singleRune">The rune to match.</param>
    public SingleChar(Rune singleRune) => this.Rune = singleRune;

    /// <summary>The rune to match against.</summary>
    public Rune Rune { get; }

    /// <summary>Determines if this matcher matches the given character.</summary>
    /// <param name="c">The character to match.</param>
    /// <returns>True if the given character is in the set, false otherwise.</returns>
    public bool Match(Rune c) => this.Rune == c;

    /// <summary>Returns the string for this matcher.</summary>
    /// <returns>The string for this matcher.</returns>
    public override string ToString() => this.Rune.ToString();
}
