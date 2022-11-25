using System.Collections.Generic;
using System.Linq;

namespace PetiteParser.Grammar.Normalizer;

/// <summary>
/// Removes any term which has identical rules to another and
/// updates all rules using the repeat term with the term which is not removed.
/// </summary>
sealed internal class RemoveDuplicateTerms : IPrecept {

    /// <summary>Performs this precept on the given grammar.</summary>
    /// <param name="analyzer">The analyzer to perform this precept on.</param>
    /// <param name="log">The log to write notices, warnings, and errors.</param>
    /// <returns>True if the grammar was changed.</returns>
    public bool Perform(Analyzer.Analyzer analyzer, Logger.ILogger? log) {
        Grammar grammar = analyzer.Grammar;
        foreach (Term term1 in grammar.Terms)
            foreach (Term term2 in grammar.Terms) {
                if (term1 != term2 && termsSame(term1, term2)) {
                    log?.AddNoticeF("Removed term {0} which is a duplicate of term {1}.", term2, term1);
                    removeDuplicate(grammar, term1, term2);
                    return true;
                }
            }
        return false;
    }

    /// <summary>Determines if two terms and their rules are the same.</summary>
    /// <param name="term1">The first term to check the rules in.</param>
    /// <param name="term2">The second term to check the rules against.</param>
    /// <returns>True if the two terms and their rules are the same.</returns>
    static private bool termsSame(Term term1, Term term2) {
        if (term1.Rules.Count != term2.Rules.Count) return false;

        List<Rule> rules = term2.Rules.ToList();
        foreach (Rule rule1 in term1.Rules) {

            bool found = false;
            foreach (Rule rule2 in rules) {
                if (rule1.Same(rule2, term1, term2)) {
                    rules.Remove(rule2);
                    found = true;
                    break;
                }
            }

            if (!found) return false;
        }

        return true;
    }

    /// <summary>Remove duplicate term from the grammar.</summary>
    /// <param name="grammar">The grammar to remove the term from.</param>
    /// <param name="term1">The term to use in place of the other term.</param>
    /// <param name="term2">The term to remove.</param>
    static private void removeDuplicate(Grammar grammar, Term term1, Term term2) {
        grammar.RemoveTerm(term2);
        foreach (Term term in grammar.Terms) {
            foreach (Rule rule in term.Rules) {
                for (int i = rule.Items.Count - 1; i >= 0; --i) {
                    if (rule.Items[i] == term2) rule.Items[i] = term1;
                }
            }
        }
    }
}
