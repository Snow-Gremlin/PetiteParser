using PetiteParser.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PetiteParser.Analyzer;

/// <summary>This stores the token sets and other analytic data for a term in the grammar.</summary>
sealed internal class TermData {

    /// <summary>This is a lookup function used for this data to find other data for the given term.</summary>
    private readonly Func<Grammar.Term, TermData> lookup;

    /// <summary>The term this data if for.</summary>
    public readonly Grammar.Term Term;

    /// <summary>Indicates this data needs to be updated.</summary>
    private bool update;

    /// <summary>
    /// Indicates if this term has rules such that it can
    /// pass over this term without consuming any tokens.
    /// </summary>
    public bool HasLambda;

    /// <summary>The set of first tokens for this term.</summary>
    public readonly HashSet<Grammar.TokenItem> Tokens;

    /// <summary>The other terms which depends directly on this term.</summary>
    public readonly HashSet<TermData> Children;

    /// <summary>The other terms which depends in at least one rule on this term.</summary>
    public readonly HashSet<TermData> Dependents;

    /// <summary>The other terms which this term depends upon in at least one rule.</summary>
    public readonly HashSet<TermData> Ancestors;

    /// <summary>Creates a new term data of first tokens.</summary>
    /// <param name="lookup">This a method for looking up other term's data.</param>
    /// <param name="term">The term this data belongs to.</param>
    public TermData(Func<Grammar.Term, TermData> lookup, Grammar.Term term) {
        this.lookup     = lookup;
        this.Term       = term;
        this.update     = true;
        this.HasLambda  = false;
        this.Tokens     = new HashSet<Grammar.TokenItem>();
        this.Children   = new HashSet<TermData>();
        this.Dependents = new HashSet<TermData>();
        this.Ancestors  = new HashSet<TermData>();
    }

    /// <summary>Joins two terms as parent and dependent.</summary>
    /// <param name="dep">The dependent to join to this parent.</param>
    private bool joinTo(TermData dep) {
        bool changed =
            this.Children.Add(dep) |
            this.Dependents.Add(dep) |
            dep.Ancestors.Add(this);

        // Propagate the join up to the grandparents.
        foreach (TermData grandparent in this.Ancestors) {
            changed =
                grandparent.Dependents.Add(dep) |
                dep.Ancestors.Add(grandparent) |
                changed;
        }

        // Add the tokens forward to the new dependent.
        return this.Tokens.ForeachAny(dep.Tokens.Add) || changed;
    }

    /// <summary>Propagates the rule information into the given data.</summary>
    /// <param name="rule">The rule to add token firsts into the data.</param>
    /// <returns>True if the data has been changed, false otherwise.</returns>
    private bool propageteRule(Grammar.Rule rule) {
        bool updated = false;
        foreach (Grammar.Item item in rule.BasicItems) {

            // Check if token, if so skip the lambda check and just leave.
            if (item is Grammar.TokenItem tItem)
                return this.Tokens.Add(tItem);

            // If term, then join to the parents.
            if (item is Grammar.Term term) {
                TermData parent = this.lookup(term);
                updated = parent.joinTo(this) || updated;
                if (!parent.HasLambda) return updated;
            }
        }

        // If the end has been reached with out stopping then set this as having a lambda.
        if (!this.HasLambda) {
            this.HasLambda = true;
            updated = true;
        }
        return updated;
    }

    /// <summary>Propagate all the rules for this term.</summary>
    /// <returns>True if the data has been changed, false otherwise.</returns>
    public bool Propagate() {
        if (!this.update) return false;
        this.update = false;

        // Run through all rules and update them.
        bool updated = this.Term.Rules.ForeachAny(this.propageteRule);

        // Mark all dependents as needing updates.
        if (updated) this.Dependents.Foreach(d => d.update = true);
        return updated;
    }

    /// <summary>Indicates if this term is left recursive.</summary>
    /// <returns>True if this term is left recursive.</returns>
    public bool LeftRecursive() => this.Dependents.Contains(this);

    /// <summary>Determine if a child is the next part in the path to the target.</summary>
    /// <param name="target">The target to try to find.</param>
    /// <returns>The child in the path to the target or null if none found.</returns>
    public TermData ChildInPath(TermData target) =>
        this.Children.FirstOrDefault(child => child.Dependents.Contains(target));

    /// <summary>Gets the sorted term names from this data.</summary>
    /// <param name="terms">The terms to get the names from.</param>
    /// <returns>The sorted names from the given terms.</returns>
    static private string[] termSetNames(HashSet<TermData> terms) {
        string[] results = terms.Select(g =>  g.Term.Name).ToArray();
        Array.Sort(results);
        return results;
    }

    /// <summary>Gets a string for this data.</summary>
    /// <param name="namePadding">The padding for the name to align the data's strings.</param>
    /// <param name="verbose">Shows the children and parent terms.</param>
    /// <returns>The string for this data.</returns>
    public string ToString(int namePadding = 0, bool verbose = false) {
        StringBuilder result = new();
        result.Append(this.Term.Name.PadRight(namePadding)).Append(" →");

        if (this.Tokens.Any()) {
            string[] tokens = this.Tokens.ToNames().ToArray();
            Array.Sort(tokens);
            result.Append(verbose ? " Tokens[" : " [").AppendJoin(", ", tokens).Append(']');
        }

        if (verbose) {
            if (this.Children.Any())
                result.Append(" Children[").AppendJoin(", ", termSetNames(this.Children)).Append(']');

            if (this.Dependents.Any())
                result.Append(" Dependents[").AppendJoin(", ", termSetNames(this.Dependents)).Append(']');

            if (this.Ancestors.Any())
                result.Append(" Ancestors[").AppendJoin(", ", termSetNames(this.Ancestors)).Append(']');
        }

        if (this.HasLambda) result.Append(" λ");
        return result.ToString();
    }
}
