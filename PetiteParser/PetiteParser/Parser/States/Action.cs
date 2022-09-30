using PetiteParser.Grammar;
using PetiteParser.Misc;

namespace PetiteParser.Parser.States;

/// <summary>
/// The action pair of an item and state.
/// When the item is reached this indicates which state to action.
/// </summary>
sealed internal class Action : System.IComparable<Action> {

    /// <summary>This is the item which connects two states together.</summary>
    public readonly Item Item;

    /// <summary>This indicates which state to go to for the item.</summary>
    public readonly State State;

    /// <summary>The lookahead tokens for this rule at the index in the state.</summary>
    public readonly TokenItem[] Lookaheads;

    /// <summary>Creates a new action.</summary>
    /// <param name="item">The item that indicates this action should be taken.</param>
    /// <param name="state">The state to go to when the item is reached.</param>
    /// <param name="lookaheads">The lookahead tokens for this action.</param>
    public Action(Item item, State state, params TokenItem[] lookaheads) {
        this.Item  = item;
        this.State = state;
        this.Lookaheads = lookaheads;
    }

    /// <summary>Indicates if this action is a goto (true) or a shift (false).s</summary>
    public bool IsGoto => this.Item is Term;

    /// <summary>Compares this action to the other action.</summary>
    /// <param name="other">The other action to compare against.</param>
    /// <returns>This is the comparison result.</returns>
    public int CompareTo(Action? other) {
        if (other is null) return 1;
        int cmp = this.Item.CompareTo(other.Item);
        return cmp != 0 ? cmp :
            this.State.Number.CompareTo(other.State.Number);
    }

    /// <summary>The string for the action.</summary>
    /// <returns>The string of this action item.</returns>
    public override string ToString() =>
        this.Item + ": " + (this.IsGoto ? "goto" : "shift") + " state " +
        this.State.Number + " @ " + this.Lookaheads.Join(" ");
}
