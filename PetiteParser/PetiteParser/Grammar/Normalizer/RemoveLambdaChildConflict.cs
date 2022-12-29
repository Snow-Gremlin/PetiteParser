using System.Linq;

namespace PetiteParser.Grammar.Normalizer;

/// <summary>
/// Removes conflict where a reduction and a shift occur from a term with a lambda is
/// followed by a token which is the first of another rule in the term.
/// </summary>
/// <example>
/// Terms `A` and `B`, token `a`, and the rules:
/// `A → ... B a ...; B → λ; B → a ...;`
/// The conflict will be from `a` reduce of the lambda rule of `B`
/// and the shift of `a` in the `A` rule.
/// The result would be to create a new rule for `B` and remove `a` from
/// the rule `A`. The new rule would be `B → a;`.
/// If `B` is used in more locations than in `A` in that specific part,
/// then `B` must be copied, `A` updated, and then the changes made to the copy.
/// Any prompts between `B` and `a` in rule `A` are moved with `a` to the new rule.
/// </example>
sealed internal class RemoveLambdaChildConflict : IPrecept {

    /// <summary>Performs this precept on the given grammar.</summary>
    /// <param name="analyzer">The analyzer to perform this precept on.</param>
    /// <param name="log">The log to write notices, warnings, and errors.</param>
    /// <returns>True if the grammar was changed.</returns>
    public bool Perform(Analyzer.Analyzer analyzer, Logger.ILogger? log) {
        foreach (Term term in analyzer.Grammar.Terms) {
            foreach (Rule rule in term.Rules) {
                if (checkRule(analyzer, rule, log)) return true;
            }
        }
        return false;
    }

    /// <summary>Checks for a rule which matches the lambda child condition.</summary>
    /// <param name="analyzer">The analyzer to read additional grammar information from.</param>
    /// <param name="rule">The rule to check and possibly update.</param>
    /// <param name="log">The log to write notices, warnings, and errors.</param>
    /// <returns>True if the grammar was changed.</returns>
    static private bool checkRule(Analyzer.Analyzer analyzer, Rule rule, Logger.ILogger? log) {
        Term? prev = null;
        int prevIndex = -1;
        for (int i = 0; i < rule.Items.Count; ++i) {
            Item item = rule.Items[i];
            if (item is Term term) {
                prev = term;
                prevIndex = i;
            } else if (item is TokenItem token && prev is not null) {
                if (analyzer.HasLambda(prev) && analyzer.HasFirst(prev, token)) {
                    performChange(analyzer, rule, prev, prevIndex, i, log);
                    return true;
                }
                prev = null;
            }
        }
        return false;
    }

    /// <summary>Performs a removal of the lambda conflict for the given rule and target.</summary>
    /// <param name="analyzer">The analyzer to read additional grammar information from.</param>
    /// <param name="rule">The rule containing the target that causes a conflict. This rule contributes the shift.</param>
    /// <param name="target">The target which needs to be updated. This term contributes the reduce.</param>
    /// <param name="targetIndex">The index of the target in the given rule which is causing the conflict.</param>
    /// <param name="tokenIndex">The index of the conflicting token in the given rule.</param>
    /// <param name="log">The log to write notices, warnings, and errors.</param>
    static private void performChange(Analyzer.Analyzer analyzer, Rule rule, Term target, int targetIndex, int tokenIndex, Logger.ILogger? log) {
        log?.AddInfoF("Removing lambda conflict for {0} at {1} in `{2}`", target, targetIndex, rule);

        // Copy target term, if needed.
        bool needsToCopy = checkIfTargetNeedsACopy(analyzer, rule, target, targetIndex, tokenIndex, log, out Rule? matchingRule);
        if (needsToCopy) target = createCopy(analyzer, rule, matchingRule, target, targetIndex, log);

        // Move token (and preceding prompts) to new rule.
        int targetCount = tokenIndex-targetIndex;
        target.NewRule().Items.AddRange(rule.Items.GetRange(targetIndex+1, targetCount));
        if (matchingRule is not null) rule.Term.Rules.Remove(rule);
        else rule.Items.RemoveRange(targetIndex+1, targetCount);
        analyzer.NeedsToRefresh();
    }

    /// <summary>Checks if the given target term needs to be copied to prevent other usages of the target term from being effected.</summary>
    /// <param name="analyzer">The analyzer to read additional grammar information from.</param>
    /// <param name="rule">The rule containing the target that causes a conflict. This rule contributes the shift.</param>
    /// <param name="target">The target which needs to be updated. This term contributes the reduce.</param>
    /// <param name="targetIndex">The index of the target in the given rule which is causing the conflict.</param>
    /// <param name="tokenIndex">The index of the conflicting token in the given rule.</param>
    /// <param name="log">The log to write notices, warnings, and errors.</param>
    /// <param name="matchingRule">Returns null or a matching rule, a rule which matches the given rule after the trailing token range has been removed, if one was found.</param>
    /// <returns>True if the target needs to be copied, false otherwise.</returns>
    static private bool checkIfTargetNeedsACopy(Analyzer.Analyzer analyzer, Rule rule, Term target, int targetIndex, int tokenIndex, Logger.ILogger? log, out Rule? matchingRule) {
        matchingRule = null;
        int usageCount = analyzer.UsageCount(target);
        if (usageCount <= 1) return false;

        // If there are only two usages, check if the other rule matches the given rule when the change has removed the target range from the given rule.
        // e.g. `A → B | B a;` when the target is `B` and `a` is removed then the rules `B` and `B a` will match.
        if (usageCount == 2) return true;

        int remainingCount = rule.Items.Count - (tokenIndex-targetIndex);
        foreach (Rule otherRule in rule.Term.Rules.
            Where(otherRule => rule != otherRule).
            Where(otherRule => otherRule.Items.Count == remainingCount)) {

            // Check if this rule is a match to the given rule.
            bool isMatch = true;
            for (int i = 0, j = 0; i < remainingCount; ++i, ++j) {
                if (otherRule.Items[i] != rule.Items[j]) {
                    isMatch = false;
                    break;
                }
                // When we reach the part that will be removed from the given rule then skip removed part.
                if (j == targetIndex) j = tokenIndex;
            }
            if (isMatch) {
                log?.AddInfoF("Found matching rule for `{0}` when removing lambda conflict for {1} at {2}.", rule, target, targetIndex);
                matchingRule = otherRule;
                return false;
            }
        }
        return true;
    }

    /// <summary>Creates a copy of the target term so that the change does not cause problems in other usages of the target term.</summary>
    /// <param name="analyzer">The analyzer to read additional grammar information from.</param>
    /// <param name="rule">The rule containing the target that causes a conflict. This rule contributes the shift.</param>
    /// <param name="target">The target which needs to be updated. This term contributes the reduce.</param>
    /// <param name="targetIndex">The index of the target in the given rule which is causing the conflict.</param>
    /// <param name="log">The log to write notices, warnings, and errors.</param>
    /// <returns>The copied target term to use instead of the given target term.</returns>
    static private Term createCopy(Analyzer.Analyzer analyzer, Rule rule, Rule? matchingRule, Term target, int targetIndex, Logger.ILogger? log) {
        Term copyTarget = analyzer.Grammar.AddGeneratedTerm(target.Name);
        foreach (Rule originalRule in target.Rules) {
            Rule copyRule = copyTarget.NewRule();
            foreach (Item item in originalRule.Items)
                copyRule.Items.Add(item == target ? copyTarget : item);
            if (originalRule == rule) rule = originalRule;
        }

        // Only replace the term in the rule being processed or the matching rule.
        if (matchingRule is null) rule.Items[targetIndex] = copyTarget;
        else matchingRule.Items[targetIndex] = copyTarget;
        log?.AddInfoF("Created copy of {0} called {1} before removing lambda conflict.", target, copyTarget);
        return copyTarget;
    }
}
