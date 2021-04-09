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
            this.Indices   = new List<int>();
            this.Rules     = new List<Rule>();
            this.OnItems   = new List<Item>();
            this.Gotos     = new List<State>();
            this.HasAccept = false;
        }

        /// <summary>The indices which indicated the offset into the matching rule.</summary>
        public List<int> Indices { get; }

        /// <summary>The rules for this state which match up with the indices.</summary>
        public List<Rule> Rules { get; }

        /// <summary>
        /// This is the items which connect two states together.
        /// This matches with the goto values to create a connection.
        /// </summary>
        public List<Item> OnItems { get; }

        /// <summary>
        /// This is the goto which indicates which state to go to for the matched items.
        /// This matches with the `onItems` to create a connection.
        /// </summary>
        public List<State> Gotos { get; }

        /// <summary>Indicates if this state can acceptance for this grammar.</summary>
        public bool HasAccept { get; private set; }

        /// <summary>Sets this state as an accept state for the grammar.</summary>
        public void SetAccept() => this.HasAccept = true;

        /// <summary>Checks if the given index and rule exist in this state.</summary>
        /// <param name="index">This index to check for.</param>
        /// <param name="rule">The given rule to check for.</param>
        /// <returns>True if the index and rule exists false otherwise.</returns>
        public bool HasRule(int index, Rule rule) {
            for (int i = this.Indices.Count - 1; i >= 0; --i) {
                if ((this.Indices[i] == index) && (this.Rules[i] == rule))
                    return true;
            }
            return false;
        }

        /// <summary>Adds the given index and rule to this state.</summary>
        /// <param name="index">This index to add.</param>
        /// <param name="rule">This rule to add.</param>
        /// <returns>False if it already exists, true if added.</returns>
        public bool AddRule(int index, Rule rule) {
            if (this.HasRule(index, rule)) return false;
            this.Indices.Add(index);
            this.Rules.Add(rule);

            List<Item> items = rule.BasicItems;
            if (index < items.Count) {
                Item item = items[index];
                if (item is Term) {
                    foreach (Rule otherRule in (item as Term).Rules)
                        this.AddRule(0, otherRule);
                }
            }
            return true;
        }

        /// <summary>Finds the go to state from the given item, null is returned if none is found.</summary>
        /// <param name="item">The item to find.</param>
        /// <returns>The state found or null.</returns>
        public State FindGoto(Item item) {
            for (int i = this.OnItems.Count - 1; i >= 0; --i) {
                if (this.OnItems[i] == item) return this.Gotos[i];
            }
            return null;
        }

        /// <summary>Adds a goto connection on the given item to the given state.</summary>
        /// <param name="item">The item to add.</param>
        /// <param name="state">The state to add.</param>
        /// <returns>True if added, false otherwise.</returns>
        public bool AddGoto(Item item, State state) {
            if (this.FindGoto(item) == state) return false;
            this.OnItems.Add(item);
            this.Gotos.Add(state);
            return true;
        }

        /// <summary>Determines if this state is equal to the given state.</summary>
        /// <param name="obj">The object to compare against.</param>
        /// <returns>True if they are equal, false otherwise.</returns>
        public override bool Equals(object obj) {
            if (!(obj is State)) return false;
            State other = obj as State;
            if (other.Number != this.Number) return false;
            if (other.Indices.Count != this.Indices.Count) return false;
            if (other.OnItems.Count != this.OnItems.Count) return false;
            for (int i = this.Indices.Count - 1; i >= 0; --i) {
                if (!this.HasRule(other.Indices[i], other.Rules[i])) return false;
            }
            for (int i = this.OnItems.Count - 1; i >= 0; --i) {
                if (this.FindGoto(other.OnItems[i]) != other.Gotos[i]) return false;
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
            for (int i = 0; i < this.Rules.Count; ++i)
                buf.AppendLine(indent+"  "+this.Rules[i].ToString(this.Indices[i]));
            for (int i = 0; i < this.OnItems.Count; ++i)
                buf.AppendLine(indent+"  "+this.OnItems[i]+": goto state "+this.Gotos[i].Number);
            return buf.ToString();
        }
    }
}
