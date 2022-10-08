using PetiteParser.Misc;
using System.Collections.Generic;

namespace PetiteParser.Parser.Table;

/// <summary>
/// Conflict represents several actions which are in
/// conflict and written to the same table entry.
/// </summary>
/// <param name="Actions">The list of conflicting actions.</param>
internal readonly record struct Conflict(IAction[] Actions) : IAction {

    /// <summary>Join combines several conflicting actions.</summary>
    /// <param name="actions">The conflicting actions.</param>
    /// <returns>The joined actions into one action.</returns>
    static public IAction Join(params IAction[] actions) {
        List<IAction> list = new();
        foreach (IAction action in actions) addAll(list, action);
        return list.Count == 1 ? list[0] : new Conflict(list.ToArray());
    }

    /// <summary>
    /// Adds all the given actions to the given list if it is not null and unique,
    /// while expanding any other conflict actions.
    /// </summary>
    /// <param name="actions">The list to add actions to.</param>
    /// <param name="action">The action to add or expand if a conflict.</param>
    static private void addAll(List<IAction> actions, IAction action) {
        if (action is null || actions.Contains(action)) return;
        if (action is Conflict c) {
            foreach (IAction a in c.Actions) addAll(actions, a);
        } else actions.Add(action);
    }

    /// <summary>Gets the debug string for this action.</summary>
    /// <returns>The string for this action.</returns>
    public override string ToString() => "conflict("+this.Actions.Join(", ")+")";
}
