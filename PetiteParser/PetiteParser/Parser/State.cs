using PetiteParser.Grammar;
using System.Collections.Generic;
using System.Text;

namespace PetiteParser.Parser {

    /// <summary>
    /// This is a state in the parser builder.
    /// The state is a collection of rules with offset indices.
    /// These states are used for generating the parser table.
    /// </summary>
    internal class State {

        /// <summary>This is the index of the state in the builder.</summary>
        public readonly int Number;

        /// <summary>Creates a new state for the parser builder.</summary>
        /// <param name="number">The index of the state.</param>
        public State(int number) {
            this.Number    = number;
            this.Fragments = new List<Fragment>();
            this.Actions     = new List<Action>();
            this.HasAccept = false;
        }

        /// <summary>The state rule fragments for this state.</summary>
        public List<Fragment> Fragments { get; }

        /// <summary>This is the list of actions which indicates which state to go to for an item.</summary>
        public List<Action> Actions { get; }

        /// <summary>Indicates if this state can acceptance for this grammar.</summary>
        public bool HasAccept { get; private set; }

        /// <summary>Sets this state as an accept state for the grammar.</summary>
        public void SetAccept() => this.HasAccept = true;

        /// <summary>Checks if the given index and rule exist in this state.</summary>
        /// <param name="fragment">The state rule fragment to check for.</param>
        /// <returns>True if the index and rule exists false otherwise.</returns>
        public bool HasRule(Fragment fragment) {
            foreach (Fragment other in this.Fragments) {
                if (other == fragment) return true;
            }
            return false;
        }

        /// <summary>Adds the given index and rule to this state.</summary>
        /// <param name="fragment">The state rule fragment to add.</param>
        /// <returns>False if it already exists, true if added.</returns>
        public bool AddRule(Fragment fragment) {
            if (this.HasRule(fragment)) return false;
            this.Fragments.Add(fragment);

            // Compute closure for the new rule.
            List<Item> items = fragment.Rule.BasicItems;
            if (fragment.Index < items.Count) {
                Item item = items[fragment.Index];
                if (item is Term) {
                    List<Rule> rules = (item as Term).Rules;

                    //
                    // TODO: Update, finish, and add correct lookahead set.
                    //
                    foreach (Rule otherRule in rules) {
                        this.AddRule(new Fragment(otherRule, 0, fragment.Lookaheads));
                    }
                }
            }
            return true;
        }

        /// <summary>Finds the action state from the given item.</summary>
        /// <param name="item">The item to find.</param>
        /// <returns>The state found or null if not found.</returns>
        public State FindActionTarget(Item item) {
            foreach (Action action in this.Actions) {
                if (action.Item == item) return action.State;
            }
            return null;
        }

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
            this.Actions.Add(action);
            return true;
        }

        /// <summary>Determines if this state is equal to the given state.</summary>
        /// <param name="obj">The object to compare against.</param>
        /// <returns>True if they are equal, false otherwise.</returns>
        public override bool Equals(object obj) {
            if (!(obj is State)) return false;
            State other = obj as State;
            if (other.Number != this.Number) return false;
            foreach (Fragment fragment in other.Fragments) {
                if (!this.HasRule(fragment)) return false;
            }
            foreach (Action action in other.Actions) {
                if (this.HasAction(action)) return false;
            }
            return true;
        }

        /// <summary>Gets the hash code for this state.</summary>
        /// <returns>The hash code for this state.</returns>
        public override int GetHashCode() => base.GetHashCode();

        /// <summary>Gets a string for this state for debugging the builder.</summary>
        /// <returns>The string for the state.</returns>
        public override string ToString() => this.ToString("");

        /// <summary>Gets a string for this state for debugging the builder.</summary>
        /// <returns>The string for the state.</returns>
        public string ToString(string indent) {
            StringBuilder buf = new();
            buf.AppendLine("state "+this.Number+":");
            foreach (Fragment fragment in this.Fragments)
                buf.AppendLine(indent+"  "+fragment);
            foreach (Action action in this.Actions)
                buf.AppendLine(indent+"  "+action);
            return buf.ToString();
        }
    }
}
