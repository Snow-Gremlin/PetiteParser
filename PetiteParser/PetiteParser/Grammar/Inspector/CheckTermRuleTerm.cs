namespace PetiteParser.Grammar.Inspector;

/// <summary>
/// An inspector to check if a rule's term was set correctly
/// and the rule was added to the correct term.
/// </remarks>
sealed internal class CheckTermRuleTerm : IInspector {
    
    /// <summary>The identifier name CheckStartTerm this inspector.</summary>
    public string Name => nameof(CheckTermRuleTerm);

    /// <summary>Performs this inspection on the given grammar.</summary>
    /// <param name="grammar">The grammar being validated.</param>
    /// <param name="log">The log to write errors and warnings out to.</param>
    public void Inspect(Grammar grammar, Logger.ILogger log) {
        foreach (Term term in grammar.Terms)
            foreach (Rule rule in term.Rules)
                inspect(term, rule, log);
    }

    /// <summary>Check that the term is set correctly in a rule for that term.</summary>
    /// <param name="term">The term containing the rule which the rule should also have.</param>
    /// <param name="rule">The rule from the given term to check.</param>
    /// <param name="log">The log to write errors and warnings out to.</param>
    static private void inspect(Term term, Rule rule, Logger.ILogger log) {
        if (rule.Term is null)
            log.AddErrorF("The rule for {0} has a nil term.", term);
        else if (rule.Term != term)
            log.AddErrorF("The rule for {0} says it is for {1}.", term, rule.Term);
    }
}
