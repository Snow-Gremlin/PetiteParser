using PetiteParser.Formatting;
using PetiteParser.Grammar;
using PetiteParser.Grammar.Analyzer;
using System.Linq;

namespace PetiteParser.Parser.States;

/// <summary>A single rule, index, and lookahead for a state.</summary>
/// <param name="Rule">The rule for this state with the given index.</param>
/// <param name="Index">The index to indicated the offset into the rule.</param>
/// <param name="Lookaheads">The lookahead tokens for this rule at the index in the state.</param>
internal readonly record struct Fragment(Rule Rule, int Index, TokenItem[] Lookaheads) {
        
    /// <summary>The initial lookaheads for the state 0 fragments.</summary>
    private static readonly TokenItem[] initLookaheads =
        new TokenItem[]{ new TokenItem(ParserStates.EofTokenName) };

    /// <summary>Creates a root fragment for the start of the given rule from the starting term.</summary>
    /// <remarks>These are only used when creating state 0's initial fragment set.</remarks>
    /// <param name="rule">The rule to start the fragment with.</param>
    /// <returns>The new root fragment for a starting term rule.</returns>
    static public Fragment NewRoot(Rule rule) =>
        new(rule, 0, initLookaheads);

    /// <summary>Creates a fragment for the start of the given rule.</summary>
    /// <param name="rule">The rule to start the fragment with.</param>
    /// <param name="parent">The parent that this rule is being called from.</param>
    /// <param name="analyzer">The set of tokens used to determine the closure.</param>
    /// <returns>The new fragment starting at the given rule.</returns>
    static public Fragment NewRule(Rule rule, Fragment parent, Analyzer analyzer) =>
        new(rule, 0, analyzer.Follows(parent.Rule, parent.Index, parent.Lookaheads));
    
    /// <summary>Creates a fragment for the same rule as the given parent but stepped to the next item index.</summary>
    /// <remarks>Will throw exception if the given parent fragment is at the end.</remarks>
    /// <param name="parent">The parent fragment to get the next fragment after.</param>
    /// <returns>The new next fragment after the given parent fragment.</returns>
    static public Fragment NextFragment(Fragment parent) =>
        parent.AtEnd ? throw new ParserException("May not get the next fragment for " + parent + ", it is at the end.") :
        new(parent.Rule, parent.Index + 1, parent.Lookaheads);
    
    /// <summary>Indicates if the fragment is at the end of the rule.</summary>
    public bool AtEnd => this.Rule.BasicItems.Count() <= this.Index;

    /// <summary>The next item in the rule after this fragment's index or null if at the end.</summary>
    public Item? NextItem => this.Rule.BasicItems.ElementAtOrDefault(this.Index);

    /// <summary>The string for this fragment.</summary>
    /// <returns>The fragments string.</returns>
    public override string ToString() => this.Rule.ToString(this.Index) + " @ " + this.Lookaheads.Join(" ");
}
