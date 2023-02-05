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
    private readonly Dictionary<Item, List<IAction>> actions;
    private readonly Dictionary<Item, List<int>> gotoStates;
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
            TokenItem[] follows = analyzer.Follows(fragment);
            log2?.AddInfoF("Adding fragments for item {0}. Follows = [{1}]", item, follows.Join(", "));
            foreach (Rule otherRule in term.Rules) {
                Fragment frag = Fragment.NewRule(otherRule, fragment, follows);
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
    public void AddGotoState(Item item, int gotoState) {
        List<int> gotos = this.gotoStates.GetValueOrDefault(item) ?? new();
        if (!gotos.Contains(gotoState)) {
            gotos.Add(gotoState);
            this.gotoStates[item] = gotos;
        }
    }

    /// <summary>Adds a action connection between an item and the given state.</summary>
    /// <param name="item">The item to set this action for.</param>
    /// <param name="action">The action to add to this state at the given item.</param>
    public void AddAction(Item item, IAction action) {
        List<IAction> actions = this.actions.GetValueOrDefault(item) ?? new();
        if (!actions.Contains(action)) {
            actions.Add(action);
            this.actions[item] = actions;
        }
    }

    /// <summary>Determines which action to use based on one or more actions for this state and given item.</summary>
    /// <param name="item">The item that indicates the given action(s) should be done.</param>
    /// <param name="actions">One or more actions to perform for the given item.</param>
    /// <param name="log">The logger to log information about creating the state to.</param>
    /// <returns>The selected action to use for this state and given action.</returns>
    private IAction actionSelector(Item item, List<IAction> actions, ILogger? log) {
        if (actions.Count == 1) return actions[0];
        
        // Error can be used over any other action since they have to be defined specifically by the language.
        if (actions.OfType<Error>().Count() > 1)
            throw new ParserException("Conflicting errors in state " + this.Number + " for " + item + ": " + actions.Join(", "));
        Error? error = actions.OfType<Error?>().FirstOrDefault();
        if (error is not null) {
            log?.AddNotice("Selecting " + error + " to resolve conflict in state " + this.Number + " for " + item + ": " + actions.Join(", "));
            return error;
        }

        // Accept can be used over any other action except for error, and all accepts are the same so duplicates can be ignored.
        Accept? accept = actions.OfType<Accept?>().FirstOrDefault();
        if (accept is not null) {
            log?.AddNotice("Selecting " + accept + " to resolve conflict in state " + this.Number + " for " + item + ": " + actions.Join(", "));
            return accept;
        }
        
        // Shift should be used over reduce. Expecting there to be only one.
        if (actions.OfType<Shift>().Count() > 1)
            throw new ParserException("Conflicting shifts in state " + this.Number + " for " + item + ": " + actions.Join(", "));
        Shift? shift = actions.OfType<Shift?>().FirstOrDefault();
        if (shift is not null) {
            log?.AddNotice("Selecting " + shift + " to resolve conflict in state " + this.Number + " for " + item + ": " + actions.Join(", "));
            return shift;
        }

        // Take the reduce if there is only one.
        if (actions.OfType<Reduce>().Count() > 1)
            throw new ParserException("Conflicting reduce in state " + this.Number + " for " + item + ": " + actions.Join(", "));
        Reduce? reduce = actions.OfType<Reduce?>().FirstOrDefault();
        if (reduce is not null) {
            log?.AddNotice("Selecting " + reduce + " to resolve conflict in state " + this.Number + " for " + item + ": " + actions.Join(", "));
            return reduce;
        }

        // Otherwise, unknown actions must have been in these actions.
        throw new ParserException("Unexpected actions in state " + this.Number + " for " + item + ": " + actions.Join(", "));
    }

    /// <summary>Performs any final steps for preparing the states, such as checking for conflicts.</summary>
    /// <param name="ignoreConflicts">
    /// This indicates that as many conflicts in state actions as possible should be ignored.
    /// Typically this is only when there is a reduce or shift, but multiple shifts or multiple reduce can't be ignored.
    /// </param>
    /// <param name="log">The logger to log information about creating the state to.</param>
    public void FinalizeState(bool ignoreConflicts, ILogger? log) {

        // Determine the shift or reduce to use for the table.
        foreach (KeyValuePair<Item, List<IAction>> pair in this.actions) {
            if (ignoreConflicts) {
                IAction action = this.actionSelector(pair.Key, pair.Value, log);
                pair.Value.Clear();
                pair.Value.Add(action);
            } else if (pair.Value.Count != 1)
                throw new ParserException("State "+ this.Number +
                    " had conflicting actions for " + pair.Key + ": " + pair.Value.Join(", "));
        }

        // Check that there is one and only one goto for each item in this set.
        foreach (KeyValuePair<Item, List<int>> pair in this.gotoStates) {
            if (pair.Value.Count != 1)
                throw new ParserException("State "+ this.Number +
                    " had conflicting goto for " + pair.Key + ": " + pair.Value.Join(", "));
        }
    }

    /// <summary>Writes this state to the table.</summary>
    /// <param name="table">The table to write to.</param>
    public void WriteToTable(Table.Table table) {
        foreach (KeyValuePair<Item, List<IAction>> pair in this.actions)
            table.WriteShift(this.Number, pair.Key.Name, pair.Value[0]);
        foreach (KeyValuePair<Item, List<int>> pair in this.gotoStates)
            table.WriteGoto(this.Number, pair.Key.Name, pair.Value[0]);
    }

    /// <summary>Determines if this state is equal to the given state.</summary>
    /// <param name="obj">The object to compare against.</param>
    /// <returns>True if they are equal, false otherwise.</returns>
    public override bool Equals(object? obj) =>
        obj is State other && other.Number == this.Number;

    /// <summary>Gets the hash code for this state.</summary>
    /// <returns>The hash code for this state.</returns>
    public override int GetHashCode() => base.GetHashCode();
    
    /// <summary>Gets a string for this state for debugging the builder.</summary>
    /// <param name="fragments">Indicates if the fragments should be outputted.</param>
    /// <param name="actions">Indicates if the actions should be outputted.</param>
    /// <param name="gotos">Indicates if the gotos should be outputted.</param>
    /// <returns>The string for the state.</returns>
    public string ToString(bool fragments = true, bool actions = true, bool gotos = true) {
        StringBuilder result = new();
        result.Append("State " + this.Number + ":");

        if (fragments) {
            foreach (Fragment fragment in this.fragments) {
                result.AppendLine();
                result.Append("  " + fragment);
            }
        }

        List<string> parts = new();
        if (actions) parts.AddRange(this.actions.Select(p => p.Key + ": " + p.Value.Join(", ")));
        if (gotos)   parts.AddRange(this.gotoStates.Select(p => p.Key + ": goto " + p.Value.Join(", ")));
        parts.Sort();
        foreach (string part in parts) {
            result.AppendLine();
            result.Append(part.IndentLines("  "));
        }
        return result.ToString();
    }

    /// <summary>Gets a string for this state for debugging the builder.</summary>
    /// <returns>The string for the state.</returns>
    public override string ToString() => this.ToString(true);
}
