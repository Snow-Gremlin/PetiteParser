using PetiteParser.Grammar;

namespace PetiteParser.Parser.States {

    /// <summary>
    /// The action pair of an item and state.
    /// When the item is reached this indicates which state to action.
    /// </summary>
    internal class Action {

        /// <summary>This is the item which connect two states together.</summary>
        public readonly Item Item;

        /// <summary>This indicates which state to go to for the item.</summary>
        public readonly State State;

        /// <summary>Creates a new action.</summary>
        /// <param name="item">The item that indicates this action should be taken.</param>
        /// <param name="state">The state to go to when the item is reached.</param>
        public Action(Item item, State state) {
            this.Item  = item;
            this.State = state;
        }

        /// <summary>Indicates if this action is a goto (true) or a shift (false).s</summary>
        public bool IsGoto => this.Item is Term;

        /// <summary>The string for the action.</summary>
        /// <returns>The string of this action item.</returns>
        public override string ToString() =>
            this.Item + ": " + (this.IsGoto ? "goto" : "shift") + " state "+this.State.Number;
    }
}
