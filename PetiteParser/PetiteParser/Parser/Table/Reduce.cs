using PetiteParser.Grammar;
using PetiteParser.Misc;

namespace PetiteParser.Parser.Table;

/// <summary>
/// A reduce indicates that the current token will be handled by another action
/// and the current rule is used to reduce the parse set down to a term.
/// </summary>
/// <param name="Rule">The rule for this action.</param>
/// <param name="Lookaheads">The lookaheads for this reduce.</param>
internal readonly record struct Reduce(Rule Rule, TokenItem[] Lookaheads) : IAction {

    /// <summary>Gets the debug string for this action.</summary>
    /// <returns>The string for this action.</returns>
    public override string ToString() => "reduce "+this.Rule + " @ " + this.Lookaheads.Join(" ");
}
