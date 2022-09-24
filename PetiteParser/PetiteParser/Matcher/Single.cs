﻿using System.Text;

namespace PetiteParser.Matcher;

/// <summary>A matcher which matches a single character.</summary>
sealed public class Single : IMatcher {
    
    /// <summary>The rune to match against.</summary>
    public readonly Rune Rune;

    /// <summary>Creates a single matcher for the given character.</summary>
    /// <param name="single">The character to match.</param>
    public Single(char single) => this.Rune = new(single);

    /// <summary>Creates a single matcher for the given rune.</summary>
    /// <param name="single">The rune to match.</param>
    public Single(Rune single) => this.Rune = single;

    /// <summary>Determines if this matcher matches the given character.</summary>
    /// <param name="c">The character to match.</param>
    /// <returns>True if the given character is in the set, false otherwise.</returns>
    public bool Match(Rune c) => this.Rune == c;

    /// <summary>Returns the string for this matcher.</summary>
    /// <returns>The string for this matcher.</returns>
    public override string ToString() => this.Rune.ToString();
}
