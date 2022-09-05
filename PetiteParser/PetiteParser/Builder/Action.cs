using PetiteParser.Grammar;

namespace PetiteParser.Builder {

    /// <summary>
    /// The action pair of an item and state.
    /// When the item is reached this indicates which state to action.
    /// </summary>
    /// <param name="Item">This is the item which connect two states together.</param>
    /// <param name="State">This indicates which state to go to for the item.</param>
    public readonly record struct Action(Item Item, State State) {

        /// <summary>Indicates if this action is a goto (true) or a shift (false).s</summary>
        public bool IsGoto => this.Item is Term;

        /// <summary>The string for the action.</summary>
        /// <returns>The string of this action item.</returns>
        public override string ToString() =>
            this.Item + ": " + (this.IsGoto ? "goto" : "shift") + " state "+this.State.Number;
    }
}
