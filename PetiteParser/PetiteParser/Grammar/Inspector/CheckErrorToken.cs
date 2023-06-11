using System.Linq;

namespace PetiteParser.Grammar.Inspector;

/// <summary>An inspector to check that, if an error token is set, then it is valid.</summary>
sealed internal class CheckErrorToken : IInspector {
    
    /// <summary>The identifier name of this inspector.</summary>
    public string Name => nameof(CheckErrorToken);

    /// <summary>Performs this inspection on the given grammar.</summary>
    /// <param name="grammar">The grammar being validated.</param>
    /// <param name="log">The log to write errors and warnings out to.</param>
    public void Inspect(Grammar grammar, Logger.ILogger log) {
        TokenItem? errorToken = grammar.ErrorToken;
        if (errorToken is not null && !grammar.Tokens.Contains(errorToken))
            log.AddErrorF("The error term, {0}, was not found in the set of tokens.", errorToken);
    }
}
