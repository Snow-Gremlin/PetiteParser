using PetiteParser.Grammar;
using PetiteParser.Logger;
using System.Collections.Generic;
using System.Linq;

namespace PetiteParser.Grammar.Normalizer;

/// <summary>
/// Removes any term which has only one rule which is used only once
/// by replacing the one usage with all the items for the rule.
/// </summary>
internal class RemoveSingleUseRules : IPrecept
{

    /// <summary>Performs this precept on the given grammar.</summary>
    /// <param name="analyzer">The analyzer to perform this precept on.</param>
    /// <param name="log">The log to write notices, warnings, and errors.</param>
    /// <returns>True if the grammar was changed.</returns>
    public bool Perform(Analyzer.Analyzer analyzer, ILogger? log)
    {
        Grammar.Grammar grammar = analyzer.Grammar;
        bool changed = false;
        List<Term> candidates = grammar.Terms.Where(t => isCandidate(grammar, t)).ToList();
        foreach (Term term in candidates)
        {
            Rule? rule = ruleHavingUsedTermOnlyOnce(grammar, term);
            if (rule is null) continue;

            log?.AddNoticeF("Removing single use rule for {0}.", term);
            int index = rule.Items.IndexOf(term);
            rule.Items.RemoveAt(index);
            rule.Items.InsertRange(index, term.Rules[0].Items);
            grammar.RemoveTerm(term);
            changed = true;
        }
        return changed;
    }

    /// <summary>Determines if the given term is a candidate to be replaced.</summary>
    /// <param name="grammar">The grammar this term belongs to.</param>
    /// <param name="term">The term to check if a candidate.</param>
    /// <returns>True if it is a candidate, false otherwise.</returns>
    static private bool isCandidate(Grammar.Grammar grammar, Term term)
    {
        if (ReferenceEquals(grammar.StartTerm, term)) return false;
        if (term.Rules.Count != 1) return false;

        foreach (Rule rule in term.Rules)
        {
            foreach (Item item in rule.Items)
            {
                if (ReferenceEquals(item, term)) return false;
            }
        }
        return true;
    }

    /// <summary>The rule containing the only usage of the term or null.</summary>
    /// <param name="grammar">The grammar to check in.</param>
    /// <param name="term">The term look for a single use.</param>
    /// <returns>The single use rule or null if the term is used more than once.</returns>
    static private Rule? ruleHavingUsedTermOnlyOnce(Grammar.Grammar grammar, Term term)
    {
        Rule? found = null;
        foreach (Term other in grammar.Terms)
        {
            if (ReferenceEquals(other, term)) continue;
            foreach (Rule rule in other.Rules)
            {
                foreach (Item item in rule.Items)
                {
                    if (ReferenceEquals(item, term))
                    {
                        if (found is not null) return null;
                        found = rule;
                    }
                }
            }
        }
        return found;
    }
}
