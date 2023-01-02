using PetiteParser.Formatting;
using PetiteParser.Grammar;
using PetiteParser.Logger;
using PetiteParser.Parser.Table;
using System;
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
    private readonly Dictionary<Item, int> gotoStates;
    private readonly Dictionary<Item, State> nextStates;

    /// <summary>Creates a new state for the parser builder.</summary>
    /// <param name="number">The index of the state.</param>
    public State(int number) {
        this.Number     = number;
        this.fragments  = new();
        this.actions    = new();
        this.gotoStates = new();
        this.nextStates = new();
    }

    /// <summary>All the fragments in this state.</summary>
    public IReadOnlyList<Fragment> Fragments => this.fragments;

    /// <summary>All the actions in this state.</summary>
    public IReadOnlyDictionary<Item, IAction> Actions => this.actions;

    /// <summary>All the goto states in this state.</summary>
    public IReadOnlyDictionary<Item, int> GotoStates => this.gotoStates;

    /// <summary>All the next states in this state.</summary>
    public IReadOnlyDictionary<Item, State> NextStates => this.nextStates;

    /// <summary>Checks if the given fragment exist in this state.</summary>
    /// <param name="fragment">The state rule fragment to check for.</param>
    /// <returns>True if the fragment exists false otherwise.</returns>
    public bool HasFragment(Fragment fragment) =>
        this.fragments.Any(fragment.Equals);

    /// <summary>Adds the given fragment to this state.</summary>
    /// <param name="fragment">The state rule fragment to add.</param>
    /// <param name="analyzer">The analyzer to get the token sets with.</param>
    /// <param name="log">The logger to log information about creating the state to.</param>
    /// <returns>False if it already exists, true if added.</returns>
    public bool AddFragment(Fragment fragment, Grammar.Analyzer.Analyzer analyzer, ILogger? log) {
        if (HasFragment(fragment)) return false;
        this.fragments.Add(fragment);
        log?.AddInfoF("Adding fragment #{0} to state {1}: {2}", this.fragments.Count-1, this.Number, fragment);

        // Compute closure for the new rule.
        Item? item = fragment.NextItem;
        if (item is not null and Term term) {
            ILogger? log2 = log?.Indent();
            log2?.AddInfoF("Adding fragments for item {0}", item);
            foreach (Rule otherRule in term.Rules) {
                Fragment frag = Fragment.NewRule(otherRule, fragment);
                this.AddFragment(frag, analyzer, log2);
            }
        }
        return true;
    }

    /// <summary>Finds the next state for the given item.</summary>
    /// <param name="item">The item to find the next state from.</param>
    /// <returns>The state found or null if not found.</returns>
    public State? NextState(Item item) => this.nextStates.GetValueOrDefault(item);

    /// <summary>Adds a generalize connection between an item and the given state.</summary>
    /// <param name="item">The item to set this action for.</param>
    /// <param name="nextState">The next state that this action will shift or go to.</param>
    public void ConnectToState(Item item, State nextState) {
        if (this.nextStates.TryGetValue(item, out State? prior)) {
            if (prior == nextState) return;
            throw new ParserException("A connection from state " + this.Number + " with " + item +
                " exists to " + prior.Number + ", so a connection can not be set to " + nextState.Number + ".");
        }
        this.nextStates.Add(item, nextState);
    }

    /// <summary>Adds a goto connection between an item and the given state.</summary>
    /// <param name="item">The item to set this action for.</param>
    /// <param name="gotoState">The optional goto state that this action will shift or go to.</param>
    public void AddGotoState(Item item, int gotoState) =>
        this.gotoStates.Add(item, gotoState);

    /// <summary>Adds a action connection between an item and the given state.</summary>
    /// <param name="item">The item to set this action for.</param>
    /// <param name="action">The action to add to this state at the given item.</param>
    public void AddAction(Item item, IAction action) =>
        this.actions[item] = this.actions.TryGetValue(item, out IAction? prior) ?
           throw new ParserException("Grammar conflict at state " + this.Number +
                " and " + item + ": prior = " + prior + ", next = " + action + ":\n" + this.ToString()) :
            action;

    /// <summary>Writes this state to the table.</summary>
    /// <param name="table">The table to write to.</param>
    public void WriteToTable(Table.Table table) {
        foreach (KeyValuePair<Item, IAction> pair in this.actions)
            table.WriteShift(this.Number, pair.Key.Name, pair.Value);
        foreach (KeyValuePair<Item, int> pair in this.gotoStates)
            table.WriteGoto(this.Number, pair.Key.Name, pair.Value);
    }

    /// <summary>Determines if this state is equal to the given state.</summary>
    /// <param name="obj">The object to compare against.</param>
    /// <returns>True if they are equal, false otherwise.</returns>
    public override bool Equals(object? obj) {
        if (obj is not State other ||
            other.Number != this.Number ||
            other.actions.Count != this.actions.Count ||
            !other.fragments.All(this.HasFragment))
            return false;

        foreach (KeyValuePair<Item, IAction> pair in this.actions) {
            if (!other.actions.TryGetValue(pair.Key, out IAction? action) || pair.Value == action)
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
            result.Append("  " + fragment);
        }

        List<string> actions = new();
        actions.AddRange(this.actions.Select(p => p.Key + ": " + p.Value));
        actions.AddRange(this.gotoStates.Select(p => p.Key + ": goto " + p.Value));
        actions.Sort();
        foreach (string action in actions) {
            result.AppendLine();
            result.Append(action.IndentLines("  "));
        }
        return result.ToString();
    }
}
