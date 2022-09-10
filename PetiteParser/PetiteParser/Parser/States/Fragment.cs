using PetiteParser.Grammar;
using PetiteParser.Misc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PetiteParser.Parser.States {

    /// <summary>A single rule, index, and lookahead for a state.</summary>
    internal class Fragment : IComparable<Fragment> {

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

            Array.Sort(lookaheads);
            this.Lookaheads = lookaheads;
        }

        /// <summary>
        /// Determines the closure look ahead for this fragment
        /// using the firsts and look ahead tokens.
        /// </summary>
        /// <see cref="https://en.wikipedia.org/wiki/LR_parser#Closure_of_item_sets"/>
        /// <param name="analyzer">The set of tokens used to determine the closure.</param>
        /// <returns>The closure look ahead token items.</returns>
        public TokenItem[] ClosureLookAheads(Analyzer.Analyzer analyzer) {
            HashSet<TokenItem> tokens = new();
            List<Item> items = this.Rule.BasicItems.ToList();
            for (int i = this.Index+1; i < items.Count; ++i) {
                if (!analyzer.Firsts(items[i], tokens))
                    return tokens.ToArray();
            }
            this.Lookaheads.Foreach(tokens.Add);
            return tokens.ToArray();
        }

        /// <summary>Compares this fragment to the other fragment.</summary>
        /// <param name="other">The other fragment to compare against.</param>
        /// <returns>This is the comparison result.</returns>
        public int CompareTo(Fragment other) {
            int cmp = this.Rule.CompareTo(other.Rule);
            if (cmp != 0) return cmp;
            cmp = this.Index.CompareTo(other.Index);
            if (cmp != 0) return cmp;

            int min = Math.Min(this.Lookaheads.Length, other.Lookaheads.Length);
            for (int i = 0; i < min; ++i) {
                cmp = this.Lookaheads[i].CompareTo(other.Lookaheads[i]);
                if (cmp != 0) return cmp;
            }
            return this.Lookaheads.Length.CompareTo(other.Lookaheads.Length);
        }

        /// <summary>Checks if the given object is equal to this fragment.</summary>
        /// <param name="obj">The object to compare against.</param>
        /// <returns>True if they are equal, false otherwise.</returns>
        public override bool Equals(object obj) {
            if (obj is not Fragment other ||
                this.Index != other.Index ||
                !this.Rule.Equals(other.Rule) ||
                other.Lookaheads.Length != this.Lookaheads.Length) return false;
            for (int i = this.Lookaheads.Length-1; i >= 0; --i) {
                if (!other.Lookaheads[i].Equals(this.Lookaheads[i])) return false;
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
