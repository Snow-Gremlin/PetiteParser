namespace PetiteParser.Grammar.Inspector;

/// <summary>An inspector to check for terms with no rules.</summary>
sealed internal class CheckForEmptyTerms : IInspector {

    /// <summary>Performs this inspection on the given grammar.</summary>
    /// <param name="grammar">The grammar being validated.</param>
    /// <param name="log">The log to write errors and warnings out to.</param>
    public void Inspect(Grammar grammar, Logger.ILogger log) {
        foreach (Term term in grammar.Terms) {
            if (term.Rules.Count <= 0)
                log.AddErrorF("The term, {0}, has no rules defined for it.", term);
        }
    }
}
