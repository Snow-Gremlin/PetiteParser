using PetiteParser.Formatting;
using PetiteParser.Logger;
using System.Collections.Generic;
using System.Linq;

namespace PetiteParser.Grammar.Normalizer;

/// <summary>A precept to remove any terms which are mono-productive.</summary>
/// <remarks>
/// Mono-productive terms are terms with only one rule which is a lambda
/// or has only one non-prompt item in it.
/// </remarks>
internal class RemoveMonoproductiveTerms : IPrecept {

    /// <summary>Performs this precept on the given grammar.</summary>
    /// <param name="analyzer">The analyzer to perform this precept on.</param>
    /// <param name="log">The log to write notices, warnings, and errors.</param>
    /// <returns>True if the grammar was changed.</returns>
    public bool Perform(Analyzer.Analyzer analyzer, ILogger? log) {
        Grammar grammar = analyzer.Grammar;
        List<Term> monoproductive = grammar.Terms.Where(t => monoproductiveTerm(grammar, t)).ToList();
        if (monoproductive.Count <= 0) return false;

        monoproductive.Sort();
        log?.AddNoticeF("Removing mono-productive terms: {0}.", monoproductive.Join(" "));
        monoproductive.ForEach(t => removeMonoproductive(grammar, t));
        return true;
    }

    /// <summary>Determines if the term is mono-productive.</summary>
    /// <remarks>This avoids nonproductive rules since those are handled by another precept and are recursive.</remarks>
    /// <param name="grammar">The grammar this term belongs to.</param>
    /// <param name="term">The term to check if mono-productive.</param>
    /// <returns>True if it is an mono-productive term.</returns>
    /// <example>Look for a term with only one rule like "T := A", "T := a", or "T := λ".</example>
    static private bool monoproductiveTerm(Grammar grammar, Term term) {
        if (ReferenceEquals(grammar.StartTerm, term)) return false;
        if (term.Rules.Count != 1) return false;
        Rule rule = term.Rules[0];
        int count = rule.BasicItems.Count();
        return count <= 0 || (count <= 1 && rule.BasicItems.First() != term);
    }

    /// <summary>Removes the mono-productive term from the grammar.</summary>
    /// <remarks>Any places in the grammar that uses the term is replaced by whatever is in the term.</remarks>
    /// <param name="grammar">The grammar to remove the term from.</param>
    /// <param name="target">The term to be removed.</param>
    static private void removeMonoproductive(Grammar grammar, Term target) {
        List<Item> remainder = new(target.Rules[0].Items);
        grammar.RemoveTerm(target);
        replaceAll(grammar, target, remainder);
    }

    /// <summary>Replaces the target term in all the rules in the given grammar.</summary>
    /// <param name="grammar">The grammar to replace the target term with.</param>
    /// <param name="target">The term to replace.</param>
    /// <param name="replacement">The items to replace the term with or empty to simply remove the term.</param>
    static private void replaceAll(Grammar grammar, Term target, List<Item> replacement) {
        foreach (Term term in grammar.Terms)
            foreach (Rule rule in term.Rules)
                replaceAll(rule, target, replacement);
    }

    /// <summary>Replaces the target term in the given rule anywhere it exists.</summary>
    /// <param name="rule">The rule to replace the term inside of.</param>
    /// <param name="target">The term to replace.</param>
    /// <param name="replacement">The items to replace the term with or empty to simply remove the term.</param>
    static private void replaceAll(Rule rule, Term target, List<Item> replacement) {
        for (int i = rule.Items.Count - 1; i >= 0; i--) {
            if (ReferenceEquals(rule.Items[i], target)) {
                rule.Items.RemoveAt(i);
                rule.Items.InsertRange(i, replacement);
            }
        }
    }
}
