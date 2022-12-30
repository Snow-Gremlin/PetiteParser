using PetiteParser.Formatting;
using PetiteParser.Grammar;
using PetiteParser.Grammar.Analyzer;
using System.Linq;

namespace PetiteParser.Parser.States;

/// <summary>A single rule, index, and lookahead for a state.</summary>
sealed internal class Fragment {
        
    /// <summary>The initial lookaheads for the state 0 fragments.</summary>
    private static readonly TokenItem[] initLookaheads =
        new TokenItem[]{ new TokenItem(ParserStates.EofTokenName) };

    /// <summary>Creates a new state fragment.</summary>
    /// <param name="rule">The rule for the fragment.</param>
    /// <param name="index">The index into the given rule.</param>
    /// <param name="parent">
    /// The parent fragment to this fragment or
    /// null if there is no parent such as those fragments in state 0.
    /// </param>
    /// <param name="analyzer">The set of tokens used to determine the closure.</param>
    public Fragment(Rule rule, int index, Fragment? parent, Analyzer analyzer) {
        this.Rule       = rule;
        this.index      = index;
        this.Lookaheads =
            parent is null ? initLookaheads :
            this.index == 0 ? parent.closureLookaheads :
            parent.Lookaheads;
        this.closureLookaheads = analyzer.Follows(this.Rule, this.index, this.Lookaheads);
    }
    
    /// <summary>The rule for this state with the given index.</summary>
    public Rule Rule { get; }

    /// <summary>The index to indicated the offset into the rule.</summary>
    private int index { get; }

    /// <summary>The lookahead tokens for this rule at the index in the state.</summary>
    public TokenItem[] Lookaheads { get; }

    /// <summary>The closure lookahead for any next fragments of this fragment.</summary>
    private TokenItem[] closureLookaheads { get; }

    /// <summary>Indicates if the fragment is at the end of the rule.</summary>
    public bool AtEnd => this.Rule.BasicItems.Count() <= this.index;

    /// <summary>The next item in the rule after this fragment's index or null if at the end.</summary>
    public Item? NextItem => this.Rule.BasicItems.ElementAtOrDefault(this.index);

    /// <summary>Creates the next fragment after this fragment.</summary>
    /// <remarks>Will throw exception if this fragment is at the end.</remarks>
    /// <param name="analyzer">The set of tokens used to determine the closure.</param>
    /// <returns>The new next fragment after this fragment.</returns>
    public Fragment CreateNextFragment(Analyzer analyzer) =>
        this.AtEnd ? throw new ParserException("May not get the next fragment for " + this + ", it is at the end.") :
        new(this.Rule, this.index + 1, this, analyzer);

    /// <summary>Checks if the given object is equal to this fragment.</summary>
    /// <param name="obj">The object to compare against.</param>
    /// <returns>True if they are equal, false otherwise.</returns>
    public override bool Equals(object? obj) {
        if (obj is not Fragment other ||
            this.index != other.index ||
            !this.Rule.Equals(other.Rule) ||
            other.Lookaheads.Length != this.Lookaheads.Length) return false;

        for (int i = this.Lookaheads.Length-1; i >= 0; --i) {
            if (!other.Lookaheads[i].Equals(this.Lookaheads[i])) return false;
        }

        return true;
    }

    /// <summary>Gets the objects hash code.</summary>
    /// <returns>The objects hash code.</returns>
    public override int GetHashCode() => base.GetHashCode();

    /// <summary>The string for this fragment.</summary>
    /// <returns>The fragments string.</returns>
    public override string ToString() =>
        this.Rule.ToString(this.index) + " @ " + this.Lookaheads.Join(" ");
}
