using PetiteParser.Grammar;
using PetiteParser.Logger;
using System.Collections.Generic;
using System.Linq;

namespace PetiteParser.Normalizer;

/// <summary>A precept to remove any terms which are unproductive.</summary>
/// <remarks>
/// Unproductive terms are terms with only one rule which is a lambda
/// or has only one non-prompt item in it.
/// </remarks>
internal class RemoveUnproductiveTerms : IPrecept {
    
    /// <summary>Performs this precept on the given grammar.</summary>
    /// <param name="analyzer">The analyzer to perform this precept on.</param>
    /// <param name="log">The log to write notices, warnings, and errors.</param>
    /// <returns>True if the grammar was changed.</returns>
    public bool Perform(Analyzer.Analyzer analyzer, ILogger log) {
        Grammar.Grammar grammar = analyzer.Grammar;
        List<Term> unproductive = grammar.Terms.Where(unproductiveTerm).ToList();
        if (unproductive.Count <= 0) return false;

        foreach (Term target in unproductive)
            removeUnproductive(grammar, target, log);
        return true;
    }

    /// <summary>Determines if the term is unproductive.</summary>
    /// <remarks>This avoids unproductive rules since those are handled by another precept and are recursive.</remarks>
    /// <param name="term">The term to check if unproductive.</param>
    /// <returns>True if it is an unproductive term.</returns>
    /// <example>Look for a term with only one rule like "T := A", "T := a", or "T := λ".</example>
    static private bool unproductiveTerm(Term term) {
        if (term.Rules.Count != 1) return false;
        int count = term.Rules[0].BasicItems.Count();
        return count <= 1 && (count != 1 || term.Rules[0].BasicItems.First() != term);
    }

    /// <summary>Removes the unproductive term from the grammar.</summary>
    /// <remarks>Any places in the grammar that uses the term is replaced by whatever is in the term.</remarks>
    /// <param name="grammar">The grammar to remove the term from.</param>
    /// <param name="target">The term to be removed.</param>
    /// <param name="log">The log to write notices, warnings, and errors.</param>
    static private void removeUnproductive(Grammar.Grammar grammar, Term target, ILogger log) {
        log?.AddNoticeF("Removing unproductive term, {0}.", target);

        List<Item> remainder = new(target.Rules[0].Items);
        grammar.RemoveTerm(target);
        replaceAll(grammar, target, remainder);
    }

    /// <summary>Replaces the target term in all the rules in the given grammar.</summary>
    /// <param name="grammar">The grammar to replace the target term with.</param>
    /// <param name="target">The term to replace.</param>
    /// <param name="replacement">The items to replace the term with or empty to simply remove the term.</param>
    static private void replaceAll(Grammar.Grammar grammar, Term target, List<Item> replacement) {
        foreach (Term term in grammar.Terms)
            foreach (Rule rule in term.Rules)
                replaceAll(rule, target, replacement);
    }

    /// <summary>Replaces the target term in the given rule anywhere it exists.</summary>
    /// <param name="rule">The rule to replace the term inside of.</param>
    /// <param name="target">The term to replace.</param>
    /// <param name="replacement">The items to replace the term with or empty to simply remove the term.</param>
    static private void replaceAll(Rule rule, Term target, List<Item> replacement) {
        for (int i = rule.Items.Count-1; i >= 0; i--) {
            if (rule.Items[i] == target) {
                rule.Items.RemoveAt(i);
                rule.Items.InsertRange(i, replacement);
            }
        }
    }
}
