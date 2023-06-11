using PetiteParser.Logger;
using System;
using System.Collections.Generic;

namespace PetiteParser.Grammar.Inspector;

static public class Inspector {

    /// <summary>Performs an inspection of the grammar and throws an exception on error.</summary>
    /// <param name="grammar">The grammar to validate.</param>
    /// <param name="log">The optional log to collect warnings and errors with.</param>
    /// <exception cref="Exception">The validation results in an exception which is thrown on failure.</exception>
    static public void Validate(Grammar grammar, ILogger? log = null) {
        Buffered bufferedLog = new(log);
        Inspect(grammar, bufferedLog);
        if (bufferedLog.Failed)
            throw new GrammarException("Grammar failed validation:" + Environment.NewLine + bufferedLog);
    }

    /// <summary>Inspect the grammar and log any warnings or errors to the given log.</summary>
    /// <param name="log">The log to output warnings and errors to.</param>
    /// <param name="grammar">The grammar to analyze.</param>
    /// <returns>The string of warnings and errors separated by new lines.</returns>
    static public string Inspect(Grammar grammar, ILogger? log = null) {
        List<IInspector> inspectors = new() {
            new CheckErrorToken(),
            new CheckForEmptyDefinitions(),
            new CheckForEmptyTerms(),
            new CheckNames(),
            new CheckReachability(),
            new CheckRuleItems(),
            new CheckStartTerm(),
            new CheckTermRuleTerm(),
        };

        Buffered bufferedLog = new(log);
        inspectors.ForEach(i => i.Inspect(grammar, bufferedLog));
        return bufferedLog.ToString();
    }
}
