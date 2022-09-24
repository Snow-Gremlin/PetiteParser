using PetiteParser.Misc;
using System;

namespace PetiteParser.Loader;

/// <summary>The collection of features and the state that they are in.</summary>
public class Features {

    /// <summary>Indicates that double quote strings in matchers are used as regular expressions.</summary>
    [Name("use_regex_matchers")]
    public bool UseRegexMatchers {
        get => false;
        set {
            if (value) throw new Exception("The use_regex_matchers feature is not implemented yet.");
        }
    }

    /// <summary>Indicates that whitespace in regular expressions should be ignored.</summary>
    /// <remarks>This has no effect unless use_regex_matchers is set to true.</remarks>
    [Name("ignore_whitespace_in_regex")]
    public bool IgnoreWhitespaceInRegex = false;
}
