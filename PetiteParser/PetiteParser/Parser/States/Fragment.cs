using PetiteParser.Formatting;
using PetiteParser.Grammar;
using PetiteParser.Grammar.Analyzer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PetiteParser.Parser.States;

/// <summary>A single rule, index, and lookahead for a state.</summary>
sealed internal class Fragment {
        
    /// <summary>The initial lookahead for the state 0 fragments.</summary>
    private static readonly TokenItem initLookahead = new(ParserStates.EofTokenName);

    /// <summary>Creates a fragment for the start of the given rule.</summary>
    /// <remarks>
    /// The parent may only be null when creating state 0's
    /// initial fragment set with the rules from the stating term.
    /// </remarks>
    /// <param name="rule">The rule to start the fragment with.</param>
    /// <param name="parent">The parent that this rule is being called from.</param>
    /// <returns>The new fragment starting at the given rule.</returns>
    static public Fragment NewRule(Rule rule, Fragment? parent) => new(rule, 0, parent);
    
    /// <summary>Creates a fragment for the same rule as the given parent but stepped to the next item index.</summary>
    /// <remarks>Will throw exception if the given parent fragment is at the end.</remarks>
    /// <param name="parent">The parent fragment to get the next fragment after.</param>
    /// <returns>The new next fragment after the given parent fragment.</returns>
    static public Fragment NextFragment(Fragment parent) =>
        parent.AtEnd ? throw new ParserException("May not get the next fragment for " + parent + ", it is at the end.") :
        new(parent.Rule, parent.index + 1, parent);

    /// <summary>Creates a new state fragment.</summary>
    /// <param name="rule">The rule for the fragment.</param>
    /// <param name="index">The index into the given rule.</param>
    /// <param name="parent">The parent fragment to this fragment.</param>
    private Fragment(Rule rule, int index, Fragment? parent) {
        this.Rule   = rule;
        this.index  = index;
        this.parent = parent;
    }
    
    /// <summary>The rule for this state with the given index.</summary>
    public Rule Rule { get; }

    /// <summary>The index to indicated the offset into the rule.</summary>
    private int index { get; }

    /// <summary>The parent fragment to this fragment.</summary>
    /// <remarks>This will be null for any root fragments in state 0 with a rule from the starting term.</remarks>
    private Fragment? parent { get; }

    /// <summary>Determines the lookahead tokens for this rule at the index in the state.</summary>
    /// <param name="analyzer">The set of tokens used to determine the closure.</param>
    public TokenItem[] Lookaheads(Analyzer analyzer) {
        HashSet<TokenItem> tokens = new();
        this.addLookaheads(analyzer, tokens);

        TokenItem[] lookaheads = tokens.ToArray();
        Array.Sort(lookaheads);
        return lookaheads;
    }
    
    /// <summary>Determines the lookahead tokens for this rule at the index in the state and adds them to the given set.</summary>
    /// <param name="analyzer">The set of tokens used to determine the closure.</param>
    /// <param name="tokens">The set of tokens to add to.</param>
    private void addLookaheads(Analyzer analyzer, HashSet<TokenItem> tokens) {
        if (this.parent is null) tokens.Add(initLookahead);
        else if (this.index > 0 || analyzer.Follows(this.parent.Rule, this.parent.index, tokens))
            this.parent.addLookaheads(analyzer, tokens);
    }

    /// <summary>Indicates if the fragment is at the end of the rule.</summary>
    public bool AtEnd => this.Rule.BasicItems.Count() <= this.index;

    /// <summary>The next item in the rule after this fragment's index or null if at the end.</summary>
    public Item? NextItem => this.Rule.BasicItems.ElementAtOrDefault(this.index);

    /// <summary>Checks if the given object is equal to this fragment.</summary>
    /// <param name="obj">The object to compare against.</param>
    /// <returns>True if they are equal, false otherwise.</returns>
    public override bool Equals(object? obj) =>
        obj is Fragment other &&
        this.index == other.index &&
        this.Rule == other.Rule &&
        ReferenceEquals(this.parent, other.parent);

    /// <summary>Gets the objects hash code.</summary>
    /// <returns>The objects hash code.</returns>
    public override int GetHashCode() => base.GetHashCode();

    /// <summary>The string for this fragment.</summary>
    /// <param name="analyzer">The analyzer to get the follows with.</param>
    /// <returns>The fragments string.</returns>
    public string ToString(Analyzer analyzer) =>
        this.Rule.ToString(this.index) + " @ " + this.Lookaheads(analyzer).Join(" ");

    /// <summary>The string for this fragment.</summary>
    /// <returns>The fragments string.</returns>
    public override string ToString() => this.Rule.ToString(this.index);
}
