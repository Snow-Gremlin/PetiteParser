using PetiteParser.Logger;

namespace PetiteParser.Normalizer;

internal class LeftFactor : IPrecept {
    
    /// <summary>Performs this action on the given grammar.</summary>
    /// <param name="analyzer">The analyzer to perform this action on.</param>
    /// <param name="log">The log to write notices, warnings, and errors.</param>
    /// <returns>True if the grammar was changed.</returns>
    public bool Perform(Analyzer.Analyzer analyzer, ILogger? log) {

        return false;
    }
}
