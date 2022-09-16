using PetiteParser.Misc;
using System.Collections.Generic;

namespace PetiteParser.Parser.Table {

    /// <summary>
    /// Conflict represents several actions which are in
    /// conflict and written to the same table entry.
    /// </summary>
    sealed internal class Conflict: IAction {
        
        /// <summary>The list of conflicting actions.</summary>
        public readonly IAction[] Actions;

        /// <summary>Creates a new shift action.</summary>
        /// <param name="state">The state number to move to.</param>
        internal Conflict(params IAction[] actions) {
            List<IAction> list = new();
            foreach (IAction action in actions) {
                if (action is Conflict c) list.AddRange(c.Actions);
                else list.Add(action);
            }
            this.Actions = list.ToArray();
        }

        /// <summary>Gets the debug string for this action.</summary>
        /// <returns>The string for this action.</returns>
        public override string ToString() => "conflict("+this.Actions.Join(", ")+")";
    }
}
