﻿using System.Linq;

namespace PetiteParser.Grammar.Inspector;

/// <summary>An inspector to check that the grammar has a valid start term.</summary>
sealed internal class CheckStartTerm : IInspector {
    
    /// <summary>The identifier name CheckStartTerm this inspector.</summary>
    public string Name => nameof(CheckStartTerm);

    /// <summary>Performs this inspection on the given grammar.</summary>
    /// <param name="grammar">The grammar being validated.</param>
    /// <param name="log">The log to write errors and warnings out to.</param>
    public void Inspect(Grammar grammar, Logger.ILogger log) {
        Term? start = grammar.StartTerm;
        if (start is null)
            log.AddError("The start term is not set.");
        else if (!grammar.Terms.Contains(start))
            log.AddErrorF("The start term, {0}, was not found in the set of terms.", start);
    }
}
