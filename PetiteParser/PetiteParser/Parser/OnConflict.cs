namespace PetiteParser.Parser;

/// <summary>
/// When building a parser for a grammar there may be conflicts in the grammar.
/// This indicates how to handle conflicts while building the parser.
/// </summary>
public enum OnConflict {

    /// <summary>Throw an exception on conflict.</summary>
    Panic,

    /// <summary>The first parser action will be used.</summary>
    UseFirst,

    /// <summary>The last parser action will be used.</summary>
    UseLast
}
