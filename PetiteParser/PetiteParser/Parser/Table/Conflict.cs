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
    public Dictionary<TokenItem, IAction> Actions { get; }

    /// <summary>The action for the given lookahead token.</summary>
    /// <param name="token">The lookahead token to look for an action for.</param>
    /// <returns>The action for the token or null.</returns>
    public IAction? ActionFor(TokenItem token) => this.Actions.GetValueOrDefault(token);

    /// <summary>Gets the debug string for this action.</summary>
    /// <returns>The string for this action.</returns>
    public override string ToString() {
        Dictionary<IAction, List<TokenItem>> dic = this.Actions.GroupBy(p => p.Value).
            ToDictionary(g => g.Key, g => g.Select(p => p.Key).ToList());

        StringBuilder result = new();
        result.Append("conflict: ");
        foreach (KeyValuePair<IAction, List<TokenItem>> pair in dic) {
            result.AppendLine();
            result.Append(pair.Key + " @ "+pair.Value.Join(" "));
        }
        return result.ToString();
    }
}
