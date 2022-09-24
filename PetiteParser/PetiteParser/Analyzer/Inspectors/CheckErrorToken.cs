using PetiteParser.Grammar;
using System.Linq;

namespace PetiteParser.Analyzer.Inspectors;

/// <summary>An inspector to check that, if an error token is set, then it is valid.</summary>
sealed internal class CheckErrorToken : IInspector {

    /// <summary>Performs this inspection on the given grammar.</summary>
    /// <param name="grammar">The grammar being validated.</param>
    /// <param name="log">The log to write errors and warnings out to.</param>
    public void Inspect(Grammar.Grammar grammar, Logger.ILogger log) {
        TokenItem errorTok = grammar.ErrorToken;
        if (errorTok is not null && !grammar.Tokens.Contains(errorTok))
            log.AddErrorF("The error term, {0}, was not found in the set of tokens.", errorTok);
    }
}
