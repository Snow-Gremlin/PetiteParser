﻿using PetiteParser.Formatting;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PetiteParser.Tokenizer.Matcher;

/// <summary>A matcher which matches a set of characters.</summary>
sealed public class CharSet : IMatcher {

    /// <summary>Creates a set matcher for all the characters in the given string.</summary>
    /// <param name="set">The string containing all the runes to match.</param>
    public CharSet(string set) :
        this(set.EnumerateRunes()) { }

    /// <summary>Creates a set matcher for all the characters in the given characters.</summary>
    /// <param name="set">The set of characters to match.</param>
    public CharSet(params char[] set) :
        this(set as IEnumerable<char>) { }

    /// <summary>Creates a set matcher for all the characters in the given characters.</summary>
    /// <param name="set">The set of characters to match.</param>
    public CharSet(IEnumerable<char> set) :
        this(set.Select((c) => new Rune(c))) { }

    /// <summary>Creates a set matcher for all the characters in the given runes.</summary>
    /// <param name="set">The set of runes to match.</param>
    public CharSet(params Rune[] set) :
        this(set as IEnumerable<Rune>) { }

    /// <summary>Creates a set matcher for all the characters in the given runes.</summary>
    /// <param name="set">The set of runes to match.</param>
    public CharSet(IEnumerable<Rune> set) =>
        this.Runes = new SortedSet<Rune>(set);

    /// <summary>
    /// The set of all the runes to match.
    /// The set must contain at least one character to function.
    /// </summary>
    public SortedSet<Rune> Runes { get; }

    /// <summary>Determines if this matcher matches the given character.</summary>
    /// <param name="c">The character to match.</param>
    /// <returns>True if the given character is in the set, false otherwise.</returns>
    public bool Match(Rune c) => this.Runes.Contains(c);

    /// <summary>Returns the string for this matcher.</summary>
    /// <returns>The string for this matcher.</returns>
    public override string ToString() => this.Runes.Join();
}
