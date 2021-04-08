using PetiteParser.Grammar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetiteParser.Parser {
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

    }
}
