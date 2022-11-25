using PetiteParser.Misc;

namespace PetiteParser.Grammar.Normalizer;

/// <summary>Removes any rule in a term which is identical to another rule in the same term.</summary>
sealed internal class RemoveDuplicateRules : IPrecept {

    /// <summary>Performs this precept on the given grammar.</summary>
    /// <param name="analyzer">The analyzer to perform this precept on.</param>
    /// <param name="log">The log to write notices, warnings, and errors.</param>
    /// <returns>True if the grammar was changed.</returns>
    public bool Perform(Analyzer.Analyzer analyzer, Logger.ILogger? log) =>
        analyzer.Grammar.Terms.ForeachAny(t => removeDuplicatesInTerm(t, log));

    /// <summary>Remove duplicate rules in terms.</summary>
    /// <param name="term">The term to look for a duplicate withing</param>
    /// <param name="log">The log to write notices, warnings, and errors.</param>
    /// <returns>True if rules were removed or false if not.</returns>
    static private bool removeDuplicatesInTerm(Term term, Logger.ILogger? log) {
        bool changed = false;
        for (int i = term.Rules.Count - 1; i >= 1; i--) {
            if (term.Rules[i].Equals(term.Rules[i - 1])) {
                term.Rules.RemoveAt(i);
                log?.AddNoticeF("Removed duplicate rule ({0}): \"{1}\"", i, term.Rules[i - 1].ToString());
                changed = true;
            }
        }
        return changed;
    }
}
