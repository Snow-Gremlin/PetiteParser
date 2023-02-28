namespace PetiteParser.Table;

/// A goto indicates that the current token will be
/// handled by another action and simply move to the next state.
internal class Goto: IAction {

    /// <summary>The state number to goto.</summary>
    public readonly int State;

    /// <summary>Creates a new goto action.</summary>
    /// <param name="state">The state number to goto.<</param>
    internal Goto(int state) => this.State = state;

    /// <summary>Gets the debug string for this action.</summary>
    /// <returns>The string for this action.</returns>
    public override string ToString() => "goto "+this.State;
}
