﻿using System;
using System.Linq;

namespace PetiteParser.Grammar.Inspector;

/// <summary>This is an inspector to check an item from a term in a rule is valid.</summary>
sealed internal class CheckRuleItems : IInspector {
    
    /// <summary>The identifier name of this inspector.</summary>
    public string Name => nameof(CheckRuleItems);

    /// <summary>Performs this inspection on the given grammar.</summary>
    /// <param name="grammar">The grammar being validated.</param>
    /// <param name="log">The log to write errors and warnings out to.</param>
    public void Inspect(Grammar grammar, Logger.ILogger log) {
        foreach (Term term in grammar.Terms)
            foreach (Rule rule in term.Rules)
                foreach (Item item in rule.Items)
                    inspect(grammar, term, item, log);
    }

    /// <summary>Inspects a single item in a rule.</summary>
    /// <param name="grammar">The grammar being checked.</param>
    /// <param name="term">The term containing the rule being checked.</param>
    /// <param name="item">The item in the rule being checked.</param>
    /// <param name="log">The log to write errors and warnings out to.</param>
    static private void inspect(Grammar grammar, Term term, Item item, Logger.ILogger log) {
        if (item is Term) {
            if (!grammar.Terms.Contains(item))
                log.AddErrorF("The term, {0}, in a rule for {1}, was not found in the set of terms.", item, term);

        } else if (item is TokenItem) {
            if (!grammar.Tokens.Contains(item))
                log.AddErrorF("The token, {0}, in a rule for {1}, was not found in the set of tokens.", item, term);

        } else if (item is Prompt) {
            if (!grammar.Prompts.Contains(item))
                log.AddErrorF("The prompt, {0}, in a rule for {1}, was not found in the set of prompts.", item, term);

        } else log.AddErrorF("Unknown item, {0}, type in {1}.", item, term);
    }
}
