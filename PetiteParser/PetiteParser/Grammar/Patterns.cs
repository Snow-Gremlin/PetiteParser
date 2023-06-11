using System.Text.RegularExpressions;

namespace PetiteParser.Grammar;

/// <summary>The set of regular expression patterns.</summary>
internal partial class Patterns {

    /// <summary>The regular expression for checking for valid items.</summary>
    [GeneratedRegex(@"^\s* (?: (?: < [^>\]}]+ > | \[ [^>\]}]+ \] | { [^>\]}]+ } ) \s* )*$",
        RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace)]
    internal static partial Regex AllItemsMatcher();
    
    /// <summary>The regular expression for breaking up items.</summary>
    [GeneratedRegex(@"^[<\[{] [^>\]}]+ [>\]}]$",
        RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace)]
    internal static partial Regex ItemMatcher();

    /// <summary>The regular expression for breaking up items.</summary>
    [GeneratedRegex(@"[<\[{] [^>\]}]+ [>\]}]",
        RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace)]
    internal static partial Regex ItemCapture();

    /// <summary>The regular expression that item names must match.</summary>
    [GeneratedRegex(@"^[^\s <> \[\] {}]+$",
        RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace)]
    internal static partial Regex ItemNameMatcher();
}
