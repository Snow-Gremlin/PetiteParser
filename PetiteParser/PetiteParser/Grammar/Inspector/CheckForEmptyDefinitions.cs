using System.Linq;

namespace PetiteParser.Grammar.Inspector;

/// <summary>An inspector to check for grammars which have nothing defined.</summary>
sealed internal class CheckForEmptyDefinitions : IInspector {

    /// <summary>The identifier name of this inspector.</summary>
    public string Name => nameof(CheckForEmptyDefinitions);

    /// <summary>Performs this inspection on the given grammar.</summary>
    /// <param name="grammar">The grammar being validated.</param>
    /// <param name="log">The log to write errors and warnings out to.</param>
    public void Inspect(Grammar grammar, Logger.ILogger log) {
        if (!grammar.Terms.Any())  log.AddError("No terms are defined.");
        if (!grammar.Tokens.Any()) log.AddError("No tokens are defined.");
    }
}
