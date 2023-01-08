using PetiteParser.Formatting;
using PetiteParser.Misc;
using PetiteParser.Parser.States;
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

    /// <summary>The maximum number of propagation loops are allowed before failing.</summary>
    private const int loopLimit = 10000;

    /// <summary>Indicates if the grammar has changed and needs refreshed.</summary>
    private bool needsToRefresh;

    /// <summary>The set of data for all terms in the grammar.</summary>
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
        this.Grammar.Terms.Foreach(term => terms.Add(term, new TermData(this.lookupData, term)));

        // Propagate the terms' data until there is nothing left to propagate.
        for (int i = 0; i < loopLimit; ++i) {
            if (!this.terms.Values.ForeachAny(data => data.Propagate())) {
                this.needsToRefresh = false;
                return;
            };
        }
        throw new AnalyzerException("Grammar analyzer got stuck in a loop.");
    }

    /// <summary>Looks up term data for a given term.</summary>
    /// <param name="term">The term to get the data for.</param>
    /// <returns>The data for the term or an exception is thrown if term doesn't exist.</returns>
    private TermData lookupData(Term term) => this.terms[term];

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
    /// <param name="tokens">The set to add the found tokens to.</param>
    /// <returns>True if the item has a lambda, false otherwise.</returns>
    public bool Firsts(Item item, HashSet<TokenItem> tokens) {
        if (item is TokenItem token) {
            tokens.Add(token);
            return false;
        }

        if (item is Term term) {
            if (this.needsToRefresh) this.Refresh();
            return this.terms[term].Firsts(tokens);
        }

        return false; // Prompt
    }

    /// <summary>Determines the follow closure (lookaheads) for the new rule fragments of the a parent fragment.</summary>
    /// <see cref="https://en.wikipedia.org/wiki/LR_parser#Closure_of_item_sets"/>
    /// <remarks>
    /// This is only done for new rule fragments (the fragment index is zero) because we are switching
    /// from one rule to another, where as next fragments (with indices above zero) are using the same
    /// rule as the parent and therefore the same set of follows as the parent.
    /// </remarks>
    /// <param name="term">The term the follows are being determined for.</param>
    /// <param name="parent">
    /// The parent fragment that is calling the given term and
    /// will be used for the fragments made from the rules in the given term.
    /// </param>
    /// <returns></returns>
    internal TokenItem[] Follows(Term term, Fragment parent) {
        HashSet<TokenItem> tokens = new();

        bool needsParentsFollows = true;
        foreach (Item item in parent.FollowingItems) {
            if (!this.Firsts(item, tokens)) {
                needsParentsFollows = false;
                break;
            }
        }

        if (needsParentsFollows)
            parent.Follows.Foreach(tokens.Add);

        TokenItem[] follows = tokens.ToArray();
        Array.Sort(follows);
        return follows;
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

        TermData? target = this.terms.Values.FirstOrDefault(data => data.LeftRecursive());
        if (target is null) return new List<Term>();
        List<Term> path = new() { target.Term };

        // Check for self-looping term, i.e. the term has itself as a direct child.
        if (target.HasChild(target)) return path;

        // Find path which will walk towards the target without repeating terms.
        HashSet<TermData> touched = new();
        TermData current = target;
        const int loopLimit = 10000;
        for (int i = 0; i < loopLimit; ++i) {
            TermData? next = current.ChildInPath(target, touched);

            // If the data propagation worked correctly, then the following exception should never be seen.
            if (next is null)
                throw new AnalyzerException("No children found in path from " + current.Term +
                    " to " + target.Term + " when left recursive found.");

            if (next.Equals(target)) return path;
            touched.Add(next);
            path.Add(next.Term);
            current = next;
        }
        throw new AnalyzerException("Too many attempts to find path from " + current.Term +
            " to " + target.Term + " when removing found left recursive.");
    }

    /// <summary>This counts the number of times the given item is used all the rules.</summary>
    /// <param name="item">The item to count.</param>
    /// <returns>The number of times the given item was used.</returns>
    public int UsageCount(Item item) =>
        this.Grammar.Terms.SelectMany(t => t.Rules).SelectMany(r => r.Items).Count(i => i == item);

    /// <summary>Gets a string for debugging the grammar's first tokens.</summary>
    /// <returns>The string with the first tokens.</returns>
    public override string ToString() => this.ToString(false);

    /// <summary>Gets a string for debugging the grammar's first tokens.</summary>
    /// <param name="verbose">
    /// Indicates that the parents and children sets should be added,
    /// otherwise only firsts and lambdas are outputted.
    /// </param>
    /// <returns>The string with the first tokens.</returns>
    public string ToString(bool verbose) {
        if (this.needsToRefresh) this.Refresh();

        StringTable st = new(this.terms.Count+1, verbose?7:3);
        st.Data[0, 0] = "Term";
        st.Data[0, 1] = "Firsts";
        st.Data[0, 2] = "λ";
        if (verbose) {
            st.Data[0, 3] = "Children";
            st.Data[0, 4] = "Parents";
            st.Data[0, 5] = "Descendants";
            st.Data[0, 6] = "Ancestors";
        }
        st.SetRowHeaderDefaultEdges();

        Term[] terms = this.terms.Keys.ToArray();
        Array.Sort(terms);
        for (int i = 0; i < terms.Length; ++i)
            this.terms[terms[i]].AddRow(st, i+1, verbose);

        return st.ToString();
    }
}
