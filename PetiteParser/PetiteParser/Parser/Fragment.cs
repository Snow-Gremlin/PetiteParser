using PetiteParser.Grammar;
using PetiteParser.Misc;
using System.Collections.Generic;
using System.Linq;

namespace PetiteParser.Parser {

    /// <summary>A single rule, index, and lookahead for a state.</summary>
    internal class Fragment {

        /// <summary>The rule for this state with the given index.</summary>
        public readonly Rule Rule;

        /// <summary>The index to indicated the offset into the rule.</summary>
        public readonly int Index;

        /// <summary>The lookahead tokens for this rule at the index in the state.</summary>
        public readonly TokenItem[] Lookaheads;

        /// <summary>Creates a new state fragment.</summary>
        /// <param name="rule">The rule for the fragment.</param>
        /// <param name="index">The index into the given rule.</param>
        /// <param name="lookaheads">The lookahead tokens for this fragment.</param>
        public Fragment(Rule rule, int index, params TokenItem[] lookaheads) {
            this.Rule = rule;
            this.Index = index;
            this.Lookaheads = lookaheads;
        }

        /// <summary>
        /// Determines the closure look ahead for this fragment
        /// using the firsts and look ahead tokens.
        /// </summary>
        /// <see cref="https://en.wikipedia.org/wiki/LR_parser#Closure_of_item_sets"/>
        /// <param name="tokenSets">The set of tokens used to determine the closure.</param>
        /// <returns>The closure look ahead token items.</returns>
        public TokenItem[] ClosureLookAheads(Analyzer tokenSets) {
            HashSet<TokenItem> tokens = new();
            List<Item> items = this.Rule.BasicItems.ToList();
            for (int i = this.Index+1; i < items.Count; ++i) {
                if (!tokenSets.Firsts(items[i], tokens))
                    return tokens.ToArray();
            }
            this.Lookaheads.Foreach(tokens.Add);
            return tokens.ToArray();
        }

        /// <summary>Checks if the given object is equal to this fragment.</summary>
        /// <param name="obj">The object to compare against.</param>
        /// <returns>True if they are equal, false otherwise.</returns>
        public override bool Equals(object obj) {
            if (obj is not Fragment other) return false;
            if (this.Index != other.Index) return false;
            if (this.Rule != other.Rule) return false;
            if (other.Lookaheads.Length != this.Lookaheads.Length) return false;
            for (int i = this.Lookaheads.Length-1; i >= 0; --i) {
                if (other.Lookaheads[i] != this.Lookaheads[i]) return false;
            }
            return true;
        }

        /// <summary>Gets the objects hash code.</summary>
        /// <returns>The objects hash code.</returns>
        public override int GetHashCode() => base.GetHashCode();

        /// <summary>The string for this fragment.</summary>
        /// <returns>The fragments string.</returns>
        public override string ToString() =>
            this.Rule.ToString(this.Index) + " @ " + this.Lookaheads.Join(" ");
    }
}
