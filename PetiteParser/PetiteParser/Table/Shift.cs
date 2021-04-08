namespace PetiteParser.Table {

    /// <summary>A shift indicates to put the token into the parse set and move to the next state.</summary>
    internal class Shift: IAction {

        /// <summary>The state number to move to.</summary>
        public readonly int State;

        /// <summary>Creates a new shift action.</summary>
        /// <param name="state">The state number to move to.</param>
        internal Shift(int state) {
            this.State = state;
        }

        /// <summary>Gets the debug string for this action.</summary>
        /// <returns>The string for this action.</returns>
        public override string ToString() => "shift "+this.State;
    }
}
