using PetiteParser.Formatting;
using PetiteParser.Grammar;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PetiteParser.Parser.Table;

/// <summary>
/// Conflict represents several actions which are in
/// conflict and written to the same table entry.
/// </summary>
sealed internal class Conflict : IAction {

    /// <summary>Creates a new conflict action.</summary>
    public Conflict() => this.Actions = new();

    /// <summary>The actions which are in conflict keyed by the lookahead token.</summary>
    public List<IAction> Actions { get; }

    /// <summary>The action for the given lookahead token.</summary>
    /// <param name="token">The lookahead token to look for an action for.</param>
    /// <returns>The action for the token or null.</returns>
    public IAction? ActionFor(TokenItem token) =>
        this.Actions.FirstOrDefault(action => actionHasToken(action, token));

    /// <summary>Checks if the given action has the given token as a lookahead.</summary>
    /// <param name="action">The action to check.</param>
    /// <param name="token">The lookahead token to look for.</param>
    /// <returns>True if it is contained or if the action is an accept or error.</returns>
    static private bool actionHasToken(IAction action, TokenItem token) =>
        action switch {
            Shift  shift  => shift.Lookaheads.Contains(token),
            Reduce reduce => reduce.Lookaheads.Contains(token),
            _             => true
        };

    /// <summary>Gets the debug string for this action.</summary>
    /// <returns>The string for this action.</returns>
    public override string ToString() {
        StringBuilder result = new();
        result.Append("conflict:");
        foreach (IAction action in this.Actions) {
            result.AppendLine();
            result.Append(action?.ToString()?.IndentLines("  ") ?? "null");
        }
        return result.ToString();
    }
}
