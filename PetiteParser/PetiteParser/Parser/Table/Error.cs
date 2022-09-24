namespace PetiteParser.Parser.Table;

/// <summary>
/// An error indicates that the given token can not
/// be processed from the current state.
/// A null action is a generic error, this one gives specific information.
/// </summary>
/// <param name="Message">The error message for this action.</param>
internal readonly record struct Error(string Message): IAction {

    /// <summary>Gets the debug string for this action.</summary>
    /// <returns>The string for this action.</returns>
    public override string ToString() => "error "+this.Message;
}
