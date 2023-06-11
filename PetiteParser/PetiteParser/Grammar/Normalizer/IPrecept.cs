namespace PetiteParser.Grammar.Normalizer;

/// <summary>A precept performed on a grammar during normalization.</summary>
internal interface IPrecept {

    /// <summary>The name of the precept.</summary>
    public string Name { get; }

    /// <summary>Performs this precept on the given grammar.</summary>
    /// <param name="analyzer">The analyzer to perform this precept on.</param>
    /// <param name="log">The log to write notices, warnings, and errors.</param>
    /// <returns>True if the grammar was changed.</returns>
    public bool Perform(Analyzer.Analyzer analyzer, Logger.ILogger? log);
}
