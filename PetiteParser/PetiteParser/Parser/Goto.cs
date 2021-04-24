using PetiteParser.Grammar;

namespace PetiteParser.Parser {

    /// <summary>
    /// The goto pair of an item and state.
    /// When the item is reached this indicates which state to goto.
    /// </summary>
    internal class Goto {

        /// <summary>This is the item which connect two states together.</summary>
        public readonly Item Item;

        /// <summary>This is the goto which indicates which state to go to for the item.</summary>
        public readonly State State;

        /// <summary>Creates a new goto.</summary>
        /// <param name="item">The item that indicates this goto should be taken.</param>
        /// <param name="state">The state to goto when the item is reached.</param>
        public Goto(Item item, State state) {
            this.Item  = item;
            this.State = state;
        }

        /// <summary>The string for the goto.</summary>
        /// <returns>The string of this goto item.</returns>
        public override string ToString() =>
            this.Item+": goto state "+this.State.Number;
    }
}
