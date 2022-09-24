namespace PetiteParser.Parser.Table;

/// <summary>
/// A goto indicates that the current token will be
/// handled by another action and simply move to the next state.
/// </summary>
/// <param name="State">The state number to goto.<</param>
internal readonly record struct Goto(int State): IAction {

    /// <summary>Gets the debug string for this action.</summary>
    /// <returns>The string for this action.</returns>
    public override string ToString() => "goto "+this.State;
}
