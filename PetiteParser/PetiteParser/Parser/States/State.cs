using PetiteParser.Formatting;
using PetiteParser.Grammar;
using PetiteParser.Parser.Table;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PetiteParser.Parser.States;

/// <summary>
/// This is a state in the parser builder.
/// The state is a collection of rules with offset indices.
/// These states are used for generating the parser table.
/// </summary>
sealed internal class State {

    /// <summary>This is the index of the state in the builder.</summary>
    public readonly int Number;

    private readonly List<Fragment> fragments;
    private readonly Dictionary<Item, IAction> actions;
    private readonly Dictionary<Item, State> nextStates;

    /// <summary>Creates a new state for the parser builder.</summary>
    /// <param name="number">The index of the state.</param>
    public State(int number) {
        this.Number     = number;
        this.fragments  = new();
        this.actions    = new();
        this.nextStates = new();
    }

    /// <summary>All the fragments in this state.</summary>
    public IReadOnlyList<Fragment> Fragments => this.fragments;

    /// <summary>Checks if the given fragment exist in this state.</summary>
    /// <param name="fragment">The state rule fragment to check for.</param>
    /// <returns>True if the fragment exists false otherwise.</returns>
    public bool HasFragment(Fragment fragment) =>
        this.fragments.Any(fragment.Equals);

    /// <summary>Adds the given fragment to this state.</summary>
    /// <param name="fragment">The state rule fragment to add.</param>
    /// <param name="analyzer">The analyzer to get the token sets with.</param>
    /// <returns>False if it already exists, true if added.</returns>
    public bool AddFragment(Fragment fragment, Analyzer.Analyzer analyzer) {
        if (HasFragment(fragment)) return false;
        this.fragments.Add(fragment);

        // Compute closure for the new rule.
        List<Item> items = fragment.Rule.BasicItems.ToList();
        if (fragment.Index < items.Count) {
            Item item = items[fragment.Index];
            if (item is Term term) {
                TokenItem[] lookahead = fragment.ClosureLookAheads(analyzer);
                foreach (Rule otherRule in term.Rules)
                    this.AddFragment(new(otherRule, 0, lookahead), analyzer);
            }
        }
        this.fragments.Sort();
        return true;
    }

    /// <summary>Finds the action state from the given item.</summary>
    /// <param name="item">The item to find.</param>
    /// <returns>The state found or null if not found.</returns>
    public State? NextState(Item item) =>
        this.nextStates.GetValueOrDefault(item);

    /// <summary>Adds a action connection between an item and the given state.</summary>
    /// <param name="item">The item to set this action for.</param>
    /// <param name="action">The action to add to this state at the given item..</param>
    /// <param name="nextState">The optional next state that this action will shift or go to.</param>
    /// <returns>True if added, false otherwise.</returns>
    public bool AddAction(Item item, IAction action, State? nextState = null) {
        if (this.HasAction(action)) return false;
        this.actions.Add(action);
        return true;
    }




    /// <summary>Writes this state to the table.</summary>
    /// <param name="table">The table to write to.</param>
    public void WriteToTable(Table.Table table) {
        foreach (KeyValuePair<Item, IAction> pair in this.actions)
            table.Write(this.Number, pair.Key.Name, pair.Value);
    }

    /// <summary>Determines if this state is equal to the given state.</summary>
    /// <param name="obj">The object to compare against.</param>
    /// <returns>True if they are equal, false otherwise.</returns>
    public override bool Equals(object? obj) {
        if (obj is not State other ||
            other.Number != this.Number ||
            other.actions.Count != this.actions.Count ||
            other.nextStates.Count != this.nextStates.Count ||
            !other.fragments.All(this.HasFragment))
            return false;

        foreach (KeyValuePair<Item, IAction> pair in this.actions) {
            if (!other.actions.TryGetValue(pair.Key, out IAction? action) ||
                pair.Value == action)
                return false;
        }

        foreach (KeyValuePair<Item, State> pair in this.nextStates) {
            if (!other.nextStates.TryGetValue(pair.Key, out State? nextState) ||
                pair.Value == nextState)
                return false;
        }

        return true;
    }

    /// <summary>Gets the hash code for this state.</summary>
    /// <returns>The hash code for this state.</returns>
    public override int GetHashCode() => base.GetHashCode();

    /// <summary>Gets a string for this state for debugging the builder.</summary>
    /// <returns>The string for the state.</returns>
    public override string ToString() {
        StringBuilder result = new();
        result.Append("State " + this.Number + ":");

        foreach (Fragment fragment in this.fragments) {
            result.AppendLine();
            result.Append("  ");
            result.Append(fragment.ToString());
        }

        List<Item> items = this.actions.Keys.Concat(this.nextStates.Keys).Distinct().ToList();
        items.Sort();
        foreach (Item item in items) {
            result.AppendLine();
            result.Append("  ");
            result.Append(item.ToString());

            if (this.actions.TryGetValue(item, out IAction? action)) {
                result.Append(": ");
                result.Append(action?.ToString() ?? "null");
            }

            if (this.nextStates.TryGetValue(item, out State? nextState)) {
                result.Append(" -> ");
                result.Append(nextState?.ToString() ?? "null");
            }
        }
        return result.ToString();
    }
}
