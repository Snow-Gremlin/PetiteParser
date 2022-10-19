using PetiteParser.Formatting;
using PetiteParser.Grammar;
using System.Collections.Generic;

namespace PetiteParser.Parser.Table;

/// <summary>
/// Conflict represents several actions which are in
/// conflict and written to the same table entry.
/// </summary>
sealed internal class Conflict : IAction {

    /// <summary>The actions which are in conflict keyed by the lookahead token.</summary>
    public Dictionary<TokenItem, Conflict> Actions { get; }

    /// <summary>Creates a new conflict action.</summary>
    public Conflict() => this.Actions = new();

    /// <summary>Gets the debug string for this action.</summary>
    /// <returns>The string for this action.</returns>
    public override string ToString() {
        //TODO: Organize pairs
        return "conflict(" + this.Actions.Join(", ") + ")";
    }
}
