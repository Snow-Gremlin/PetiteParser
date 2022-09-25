using PetiteParser.Logger;
using PetiteParser.Misc;
using System;
using System.Collections.Generic;

namespace PetiteParser.Inspector;

static public class Inspector {

    /// <summary>Performs an inspection of the grammar and throws an exception on error.</summary>
    /// <param name="grammar">The grammar to validate.</param>
    /// <param name="log">The optional log to collect warnings and errors with.</param>
    /// <exception cref="Exception">The validation results in an exception which is thrown on failure.</exception>
    static public void Validate(Grammar.Grammar grammar, ILogger? log = null) {
        Buffered bufLog = new(log);
        Inspect(grammar, bufLog);
        if (bufLog.Failed)
            throw new PetiteParserException("Grammar failed validation:"+Environment.NewLine+bufLog);
    }

    /// <summary>Inspect the grammar and log any warnings or errors to the given log.</summary>
    /// <param name="log">The log to output warnings and errors to.</param>
    /// <param name="grammar">The grammar to analyze.</param>
    /// <returns>The string of warnings and errors separated by new lines.</returns>
    static public string Inspect(Grammar.Grammar grammar, ILogger? log = null) {
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

        Buffered bufLog = new(log);
        inspectors.ForEach(i => i.Inspect(grammar, bufLog));
        return bufLog.ToString();
    }
}
