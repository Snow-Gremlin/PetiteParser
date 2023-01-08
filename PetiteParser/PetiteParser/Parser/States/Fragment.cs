using PetiteParser.Formatting;
using PetiteParser.Grammar;
using System.Collections.Generic;
using System.Linq;

namespace PetiteParser.Parser.States;

/// <summary>A single rule, index, and lookahead for a state.</summary>
sealed internal class Fragment {
        
    /// <summary>The initial lookaheads for the state 0 fragments.</summary>
    private static readonly TokenItem[] initLookahead = new TokenItem[] { new(ParserStates.EofTokenName) };

    /// <summary>
    /// Creates a root fragment for the start of the given rule when creating state 0's
    /// initial fragment set with the rules from the stating term.
    /// </summary>
    /// <param name="rule">The rule to start the fragment with.</param>
    /// <returns>The new fragment starting at the given rule.</returns>
    static public Fragment NewRootRule(Rule rule) =>
        new(rule, 0, null, initLookahead);

    /// <summary>Creates a fragment for the start of the given rule with a parent fragment.</summary>
    /// <param name="rule">The rule to start the fragment with.</param>
    /// <param name="parent">The parent that this rule is being called from.</param>
    /// <param name="follows">The predetermined follow set of tokens for the closure for this rule.</param>
    /// <returns>The new fragment starting at the given rule.</returns>
    static public Fragment NewRule(Rule rule, Fragment parent, TokenItem[] follows) =>
        new(rule, 0, parent, follows);
    
    /// <summary>Creates a fragment for the same rule as the given parent but stepped to the next item index.</summary>
    /// <remarks>Will throw exception if the given parent fragment is at the end.</remarks>
    /// <param name="parent">The parent fragment to get the next fragment after.</param>
    /// <returns>The new next fragment after the given parent fragment.</returns>
    static public Fragment NextFragment(Fragment parent) =>
        parent.AtEnd ? throw new ParserException("May not get the next fragment for " + parent + ", it is at the end.") :
        new(parent.Rule, parent.Index + 1, parent, parent.Follows);

    /// <summary>Creates a new state fragment.</summary>
    /// <param name="rule">The rule for the fragment.</param>
    /// <param name="index">The index into the given rule.</param>
    /// <param name="parent">The parent fragment to this fragment.</param>
    /// <param name="follows">The follows tokens for this fragment.</param>
    private Fragment(Rule rule, int index, Fragment? parent, TokenItem[] follows) {
        this.Rule    = rule;
        this.Index   = index;
        this.Parent  = parent;
        this.Follows = follows;
    }
    
    /// <summary>The rule for this state with the given index.</summary>
    public Rule Rule { get; }

    /// <summary>The index to indicated the offset into the rule.</summary>
    public int Index { get; }
    
    /// <summary>The parent fragment to this fragment.</summary>
    /// <remarks>This will be null for any root fragments in state 0 with a rule from the starting term.</remarks>
    public Fragment? Parent { get; }

    /// <summary>The follow lookahead tokens for this fragment.</summary>
    /// <remarks>
    /// If this is a root rule, then the follows are only the EOF token.
    /// If this is a start of a rule (index == 0), then the follows are the follow closure of the parent fragment.
    /// Otherwise (index > 0) the follows are the same as the parent fragment for the same rule with index == 0.
    /// </remarks>
    public TokenItem[] Follows { get; }

    /// <summary>Indicates if the fragment is at the end of the rule.</summary>
    public bool AtEnd => this.Rule.BasicItems.Count() <= this.Index;

    /// <summary>The next item in the rule after this fragment's index or null if at the end.</summary>
    public Item? NextItem => this.Rule.BasicItems.ElementAtOrDefault(this.Index);

    /// <summary>This enumerates all the base items (no prompts) in this fragment's rules after the fragment's index.</summary>
    public IEnumerable<Item> FollowingItems => this.Rule.BasicItems.Skip(this.Index+1);

    /// <summary>Checks if the given object is equal to this fragment.</summary>
    /// <param name="obj">The object to compare against.</param>
    /// <returns>True if they are equal, false otherwise.</returns>
    public override bool Equals(object? obj) =>
        obj is Fragment other &&
        this.Index == other.Index &&
        this.Rule == other.Rule &&
        Enumerable.SequenceEqual(this.Follows, other.Follows);

    /// <summary>Gets the objects hash code.</summary>
    /// <returns>The objects hash code.</returns>
    public override int GetHashCode() => base.GetHashCode();

    /// <summary>The string for this fragment.</summary>
    /// <returns>The fragments string.</returns>
    public override string ToString() => this.Rule.ToString(this.Index) + " @ " + this.Follows.Join(" ");
}
