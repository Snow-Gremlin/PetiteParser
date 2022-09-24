using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PetiteParser.Matcher;

/// <summary>A matcher for a predefined set of characters.</summary>
sealed public class Predef : IMatcher {

    /// <summary>Matches any rune that is categorized as a control character.</summary>
    static public Predef Control => new("Control", Rune.IsControl);

    /// <summary>Matches any rune that is categorized as a decimal digit.</summary>
    static public Predef Digit => new("Digit", Rune.IsDigit);

    /// <summary>Matches any rune that is categorized as a letter.</summary>
    static public Predef Letter => new("Letter", Rune.IsLetter);

    /// <summary>Matches any rune that is categorized as a letter or a decimal digit.</summary>
    static public Predef LetterOrDigit => new("LetterOrDigit", Rune.IsLetterOrDigit);

    /// <summary>Matches any rune that is categorized as a lowercase letter.</summary>
    static public Predef Lower => new("Lower", Rune.IsLower);

    /// <summary>Matches any rune that is categorized as a number.</summary>
    static public Predef Number => new("Number", Rune.IsNumber);

    /// <summary>Matches any rune that is categorized as a punctuation mark.</summary>
    static public Predef Punctuation => new("Punctuation", Rune.IsPunctuation);

    /// <summary>Matches any rune that is categorized as a separator character.</summary>
    static public Predef Separator => new("Separator", Rune.IsSeparator);

    /// <summary>Matches any rune that is categorized as a symbol character.</summary>
    static public Predef Symbol => new("Symbol", Rune.IsSymbol);

    /// <summary>Matches any rune that is categorized as a uppercase letter.</summary>
    static public Predef Upper => new("Upper", Rune.IsUpper);

    /// <summary>Matches any rune that is categorized as a white space character.</summary>
    static public Predef WhiteSpace => new("WhiteSpace", Rune.IsWhiteSpace);

    /// <summary>Enumerates all the predefined matchers.</summary>
    static public IEnumerable<Predef> All {
        get {
            yield return Control;
            yield return Digit;
            yield return Letter;
            yield return LetterOrDigit;
            yield return Lower;
            yield return Number;
            yield return Punctuation;
            yield return Separator;
            yield return Symbol;
            yield return Upper;
            yield return WhiteSpace;
        }
    }

    /// <summary>Finds a predefined matcher by its name.</summary>
    /// <param name="name">The name to look for. Case is ignored.</param>
    /// <returns>The found predefined matcher, otherwise null if not found.</returns>
    static public Predef FromName(string name) =>
        All.FirstOrDefault(matcher => string.Equals(matcher.name, name, StringComparison.OrdinalIgnoreCase));

    /// <summary>The name of the predefinition.</summary>
    private readonly string name;

    /// <summary>The handler for checking the rune.</summary>
    private readonly Func<Rune, bool> handler;

    /// <summary>Creates a new predefined matcher.</summary>
    /// <param name="name">The name of the matcher.</param>
    /// <param name="handler">The handler for checking the rune.</param>
    public Predef(string name, Func<Rune, bool> handler) {
        this.name = name;
        this.handler = handler;
    }

    /// <summary>Determines if this matcher matches the given character.</summary>
    /// <param name="c">The character to match.</param>
    /// <returns>True if the character is matched by the predefined set, false otherwise.</returns>
    public bool Match(Rune c) => this.handler(c);

    /// <summary>Returns the string for this matcher.</summary>
    /// <returns>The string for this matcher.</returns>
    public override string ToString() => this.name;
}
