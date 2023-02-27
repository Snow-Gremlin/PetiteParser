using PetiteParser.Misc;

namespace PetiteParser.Loader;

/// <summary>The collection of features and the state that they are in.</summary>
public class Features {

    public Features(bool useRegexMatchers = false) =>
        this.useRegexMatchers = useRegexMatchers;

    // Temporary to make the UseRegexMatcher happy.
    private readonly bool useRegexMatchers;

    /// <summary>Indicates that double quote strings in matchers are used as regular expressions.</summary>
    [Name("use_regex_matchers")]
    public bool UseRegexMatchers {
        get => this.useRegexMatchers;
        set {
            if (value) throw new System.Exception("The use_regex_matchers feature is not implemented yet.");
        }
    }

    /// <summary>Indicates that whitespace in regular expressions should be ignored.</summary>
    /// <remarks>This has no effect unless use_regex_matchers is set to true.</remarks>
    [Name("ignore_whitespace_in_regex")]
    public bool IgnoreWhitespaceInRegex { get; set; } = false;
}
