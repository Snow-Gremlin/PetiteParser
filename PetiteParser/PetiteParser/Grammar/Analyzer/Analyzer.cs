using PetiteParser.Formatting;
using PetiteParser.Misc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PetiteParser.Grammar.Analyzer;

/// <summary>This is a tool for calculating the firsts tokens and term sets for a grammar.</summary>
/// <remarks>
/// This performs analysis of the given grammar as it is, any changes to the grammar
/// will require reanalysis. This analysis required propagation through the rules of the grammar
/// meaning that this may be slow for large complex grammars.
/// </remarks>
sealed public partial class Analyzer {

    /// <summary>Indicates if the grammar has changed and needs refreshed.</summary>
    private bool needsToRefresh;

    /// <summary>The set of groups for all terms in the grammar.</summary>
    private readonly Dictionary<Term, TermData> terms;

    /// <summary>Create a new analyzer which will read from the given grammar.</summary>
    /// <param name="grammar">The grammar to analyze.</param>
    public Analyzer(Grammar grammar) {
        this.Grammar        = grammar;
        this.needsToRefresh = true;
        this.terms          = new();
    }

    /// <summary>The grammar being analyzed.</summary>
    public Grammar Grammar { get; }

    /// <summary>This indicates that the grammar has changed and needs refreshed.</summary>
    /// <remarks>
    /// The next time that the analyzer is used a refresh will occur.
    /// This should be called anytime the grammar has been changed so that the data it up-to-date.
    /// </remarks>
    public void NeedsToRefresh() => this.needsToRefresh = true;

    /// <summary>Updates the analyzed information for the grammar.</summary>
    /// <remarks>
    /// This may be called anytime the grammar has been changed so that the data it up-to-date,
    /// or the analyzer can be set to refresh automatically with NeedsToRefresh the next time it is used.
    /// </remarks>
    public void Refresh() {
        // Initialize the term list
        this.terms.Clear();
        this.Grammar.Terms.ToDictionary(term => term, term => new TermData(t => this.terms[t], term)).
            Foreach(pair => terms.Add(pair.Key, pair.Value));

        // Propagate the terms' data until there is nothing left to propagate.
        while (this.terms.Values.ForeachAny(group => group.Propagate())) ;
        this.needsToRefresh = false;
    }

    /// <summary>Determines if the given term has the given token as a first.</summary>
    /// <param name="term">The term to check the first from.</param>
    /// <param name="token">The token to check for in the firsts for the term.</param>
    /// <returns>True if the term has a first, false otherwise.</returns>
    public bool HasFirst(Term term, TokenItem token) {
        if (this.needsToRefresh) this.Refresh();
        return this.terms[term].HasFirst(token);
    }

    /// <summary>Gets the determined first token sets for the grammar item.</summary>
    /// <param name="item">This is the item to get the token set for.</param>
    /// <param name="tokens">The handle to a set to add the found tokens to.</param>
    /// <returns>True if the item has a lambda, false otherwise.</returns>
    private bool firsts(Item item, HashSet<TokenItem> tokens) {
        if (item is TokenItem token) {
            tokens.Add(token);
            return false;
        }

        if (item is Term term) {
            TermData group = this.terms[term];
            group.Firsts.Foreach(tokens.Add);
            return group.HasLambda;
        }

        return false; // Prompt
    }

    /// <summary>
    /// Determines the closure lookahead for a fragment, the follows,
    /// using the firsts and look ahead tokens from the parent fragment.
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/LR_parser#Closure_of_item_sets"/>
    /// <param name="rule">The rule for the fragment.</param>
    /// <param name="index">The index into the rule for the fragment offset.</param>
    /// <param name="parentFollows">The follow tokens from the parent fragment.</param>
    /// <returns>The closure look ahead token items, the follows.</returns>
    public TokenItem[] Follows(Rule rule, int index, TokenItem[] parentFollows) {
        if (this.needsToRefresh) this.Refresh();

        bool reachedEnd = true;
        HashSet<TokenItem> tokens = new();
        List<Item> items = rule.BasicItems.ToList();
        for (int i = index + 1; i < items.Count; ++i) {
            bool hasLambda = this.firsts(items[i], tokens);
            if (!hasLambda) {
                reachedEnd = false;
                break;
            }
        }

        if (reachedEnd) parentFollows.Foreach(tokens.Add);

        TokenItem[] lookahead = tokens.ToArray();
        Array.Sort(lookahead);
        return lookahead;
    }

    /// <summary>Indicates if the given term has a lambda rule in it.</summary>
    /// <param name="term">The term to determine if it has a lambda rule.</param>
    /// <returns>True if there is a lambda rule, false otherwise.</returns>
    public bool HasLambda(Term term) {
        if (this.needsToRefresh) this.Refresh();
        return this.terms[term].HasLambda;
    }

    /// <summary>Tries to find the first direct or indirect left recursion.</summary>
    /// <returns>The tokens in the loop for the left recursion or null if none.</returns>
    /// <see cref="https://handwiki.org/wiki/Left_recursion"/>
    public List<Term> FindFirstLeftRecursion() {
        if (this.needsToRefresh) this.Refresh();

        TermData? target = this.terms.Values.FirstOrDefault(g => g.LeftRecursive());
        if (target is null) return new List<Term>();

        List<Term> path = new() { target.Term };
        TermData group = target;
        while (true) {
            TermData? next = group.ChildInPath(target);

            // If the data propagation worked correctly, then the following exception should never be seen.
            if (next is null)
                throw new AnalyzerException("No children found in path from " + group.Term +
                    " to " + target.Term + " when left recursive found.");

            if (next == target) return path;
            path.Add(next.Term);
            group = next;
        }
    }

    /// <summary>This counts the number of times the given item is used all the rules.</summary>
    /// <param name="item">The item to count.</param>
    /// <returns>The number of times the given item was used.</returns>
    public int UsageCount(Item item) =>
        this.Grammar.Terms.SelectMany(t => t.Rules).SelectMany(r => r.Items).Count(i => i == item);

    /// <summary>Gets a string for debugging the grammar's first tokens.</summary>
    /// <param name="verbose">Shows the children and parent terms.</param>
    /// <returns>The string with the first tokens.</returns>
    public string ToString(bool verbose = false) {
        if (this.needsToRefresh) this.Refresh();
        int maxWidth = this.terms.Keys.Select(term => term.Name.Length).Aggregate(Math.Max);
        string[] parts = this.terms.Values.Select(g => g.ToString(maxWidth, verbose)).ToArray();
        Array.Sort(parts);
        return parts.JoinLines();
    }
}
