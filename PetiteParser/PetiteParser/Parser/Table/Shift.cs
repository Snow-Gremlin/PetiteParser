using PetiteParser.Grammar;

namespace PetiteParser.Parser.Table;

/// <summary>
/// A shift indicates to put the token into the parse set and move to the next state.
/// </summary>
/// <param name="State">The state number to move to.</param>
/// <param name="lookaheads">The lookaheads for this shift.</param>
internal readonly record struct Shift(int State, TokenItem[] Lookaheads) : IAction {

    /// <summary>Gets the debug string for this action.</summary>
    /// <returns>The string for this action.</returns>
    public override string ToString() => "shift " + this.State;
}
