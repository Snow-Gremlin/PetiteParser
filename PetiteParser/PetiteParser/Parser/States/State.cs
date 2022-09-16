using PetiteParser.Grammar;
using PetiteParser.Misc;
using System.Collections.Generic;
using System.Linq;

namespace PetiteParser.Parser.States {

    /// <summary>
    /// This is a state in the parser builder.
    /// The state is a collection of rules with offset indices.
    /// These states are used for generating the parser table.
    /// </summary>
    internal class State {

        /// <summary>This is the index of the state in the builder.</summary>
        public readonly int Number;

        private readonly List<Fragment> fragments;
        private readonly List<Action> actions;

        /// <summary>Creates a new state for the parser builder.</summary>
        /// <param name="number">The index of the state.</param>
        public State(int number) {
            this.Number    = number;
            this.fragments = new();
            this.actions   = new();
            this.HasAccept = false;
        }

        /// <summary>The state rule fragments for this state.</summary>
        public IReadOnlyList<Fragment> Fragments => this.fragments;

        /// <summary>This is the list of actions which indicates which state to go to for an item.</summary>
        public IReadOnlyList<Action> Actions => this.actions;

        /// <summary>Indicates if this state can acceptance for this grammar.</summary>
        public bool HasAccept { get; private set; }

        /// <summary>Sets this state as an accept state for the grammar.</summary>
        public void SetAccept() => this.HasAccept = true;

        /// <summary>Checks if the given fragment exist in this state.</summary>
        /// <param name="fragment">The state rule fragment to check for.</param>
        /// <returns>True if the fragment exists false otherwise.</returns>
        public bool HasFragment(Fragment fragment) => this.fragments.Any(fragment.Equals);

        /// <summary>Adds the given fragment to this state.</summary>
        /// <param name="fragment">The state rule fragment to add.</param>
        /// <param name="analyzer">The analyzer to get the token sets with.</param>
        /// <returns>False if it already exists, true if added.</returns>
        public bool AddFragment(Fragment fragment, Analyzer.Analyzer analyzer) {
            if (HasFragment(fragment)) return false;
            this.fragments.Add(fragment);

            // Compute closure for the new rule.
            List<Item> items = fragment.Rule.BasicItems.ToList();
            if (fragment.Index < items.Count) {
                Item item = items[fragment.Index];
                if (item is Term term) {
                    TokenItem[] lookahead = fragment.ClosureLookAheads(analyzer);
                    foreach (Rule otherRule in term.Rules)
                        this.AddFragment(new(otherRule, 0, lookahead), analyzer);
                }
            }
            this.fragments.Sort();
            return true;
        }

        /// <summary>Finds the action state from the given item.</summary>
        /// <param name="item">The item to find.</param>
        /// <returns>The state found or null if not found.</returns>
        public State FindActionTarget(Item item) =>
            this.actions.FirstOrDefault(a => a.Item == item)?.State;

        /// <summary>Determines if the given action exists in this state.</summary>
        /// <param name="action">The action to check for.</param>
        /// <returns>True if the action exists, false otherwise.</returns>
        public bool HasAction(Action action) =>
            this.FindActionTarget(action.Item) == action.State;

        /// <summary>Adds a action connection between an item and the given state.</summary>
        /// <param name="action">The action state and item to add.</param>
        /// <returns>True if added, false otherwise.</returns>
        public bool AddAction(Action action) {
            if (this.HasAction(action)) return false;
            this.actions.Add(action);
            this.actions.Sort();
            return true;
        }

        /// <summary>Determines if this state is equal to the given state.</summary>
        /// <param name="obj">The object to compare against.</param>
        /// <returns>True if they are equal, false otherwise.</returns>
        public override bool Equals(object obj) =>
            obj is State other &&
            other.Number == this.Number &&
            other.Fragments.All(this.HasFragment) &&
            other.Actions.All(this.HasAction);

        /// <summary>Gets the hash code for this state.</summary>
        /// <returns>The hash code for this state.</returns>
        public override int GetHashCode() => base.GetHashCode();

        /// <summary>Gets a string for this state for debugging the builder.</summary>
        /// <returns>The string for the state.</returns>
        public override string ToString() => ToString("");

        /// <summary>Gets a string for this state for debugging the builder.</summary>
        /// <param name="indent">The indent to add to all the lines of the output except for the first.</param>
        /// <returns>The string for the state.</returns>
        public string ToString(string indent) {
            int count = this.fragments.Count + this.actions.Count;
            List<object> parts = new(count + 1) { "State " + this.Number + ":" };
            parts.AddRange(this.fragments);
            parts.AddRange(this.actions);
            return parts.JoinLines(indent + "  ");
        }
    }
}
