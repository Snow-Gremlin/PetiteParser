using PetiteParser.Grammar;

namespace PetiteParser.Table {

    /// <summary>
    /// A reduce indicates that the current token will be handled by another action
    /// and the current rule is used to reduce the parse set down to a term.
    /// </summary>
    internal class Reduce: IAction {

        /// <summary>The rule to reduce from the parse set.</summary>
        public readonly Rule Rule;

        /// <summary>Creates a new reduce action.</summary>
        /// <param name="rule">The rule for this action.</param>
        internal Reduce(Rule rule) {
            this.Rule = rule;
        }

        /// <summary>Gets the debug string for this action.</summary>
        /// <returns>The string for this action.</returns>
        public override string ToString() => "reduce "+this.Rule;
    }
}
