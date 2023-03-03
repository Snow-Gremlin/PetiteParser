namespace PetiteParser.Parser.Table;

/// <summary>
/// An accept indicates that the full input has been
/// checked by the grammar and fits to the grammar.
/// </summary>
internal readonly record struct Accept() : IAction {

    /// <summary>Gets the debug string for this action.</summary>
    /// <returns>The string for this action.</returns>
    public override string ToString() => "accept";
}
