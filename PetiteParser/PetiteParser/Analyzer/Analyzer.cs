﻿using PetiteParser.Formatting;
using PetiteParser.Grammar;
using PetiteParser.Misc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PetiteParser.Analyzer;

/// <summary>This is a tool for calculating the firsts tokens and term sets for a grammar.</summary>
/// <remarks>
/// This performs analysis of the given grammar as it is, any changes to the grammar
/// will require reanalysis. This analysis required propagation through the rules of the grammar
/// meaning that this may be slow for large complex grammars.
/// </remarks>
sealed public class Analyzer {
    
    /// <summary>Indicates if the grammar has changed and needs refreshed.</summary>
    private bool needsToRefresh;

    /// <summary>The set of groups for all terms in the grammar.</summary>
    private readonly Dictionary<Term, TermData> terms;

    /// <summary>Create a new analyzer which will read from the given grammar.</summary>
    /// <param name="grammar">The grammar to analyze.</param>
    public Analyzer(Grammar.Grammar grammar) {
        this.Grammar        = grammar;
        this.needsToRefresh = true;
        this.terms          = new();
    }

    /// <summary>The grammar being analyzed.</summary>
    public Grammar.Grammar Grammar { get; }
    
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
            Foreach(pair => this.terms.Add(pair.Key, pair.Value));
        
        // Propagate the terms' data until there is nothing left to propagate.
        while (this.terms.Values.ForeachAny(group => group.Propagate())) ;
        this.needsToRefresh = false;
    }

    /// <summary>Gets the determined first token sets for the grammar item.</summary>
    /// <param name="item">This is the item to get the token set for.</param>
    /// <param name="tokens">The set to add the found tokens to.</param>
    /// <returns>True if the item has a lambda, false otherwise.</returns>
    public bool Firsts(Item item, HashSet<TokenItem> tokens) {
        if (this.needsToRefresh) this.Refresh();

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
    /// Determines the closure look ahead for this fragment
    /// using the firsts and look ahead tokens.
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/LR_parser#Closure_of_item_sets"/>
    /// 
    /// 
    /// TODO: COMMENT
    /// <param name="analyzer">The set of tokens used to determine the closure.</param>
    /// 
    /// 
    /// <returns>The closure look ahead token items.</returns>
    public TokenItem[] ClosureLookAheads(Rule rule, int index, TokenItem[] parentLookaheads) {
        HashSet<TokenItem> tokens = new();
        List<Item> items = rule.BasicItems.ToList();
        for (int i = index+1; i < items.Count; ++i) {
            if (!this.Firsts(items[i], tokens))
                return tokens.ToArray();
        }

        parentLookaheads.Foreach(tokens.Add);
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

    /// <summary>Determines if the given rule reaches the target term first.</summary>
    /// <param name="rule">The rule to check if the target term is a first.</param>
    /// <param name="target">The target term to check for in the rule.</param>
    /// <returns>True if the target term is first, false otherwise.</returns>
    private bool ruleReaches(Rule rule, Term target) {
        foreach (Item item in rule.BasicItems) {
            if (item is not Term term) return false;
            if (term == target) return true;
            if (!this.terms[term]?.HasLambda ?? false) return false;
        }
        return false;
    }

    /// <summary>Determines the first rule which goes from the parent to the child.</summary>
    /// <param name="parent">The parent to find the rule within.</param>
    /// <param name="child">The child to find the rule to.</param>
    /// <returns>The first rule from the parent to the child or null if none is found.</returns>
    public Rule? FirstRuleBetween(Term parent, Term child) {
        if (this.needsToRefresh) this.Refresh();
        return this.terms[parent]?.Term.Rules.FirstOrDefault(r => this.ruleReaches(r, child));
    }

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
