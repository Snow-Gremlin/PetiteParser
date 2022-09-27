using PetiteParser.Grammar;
using PetiteParser.Logger;
using PetiteParser.Misc;
using PetiteParser.Parser.Table;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PetiteParser.Parser.States;

/// <summary>This is a builder used to generate the LR parser states for a giving a grammar.</summary>
internal class ParserStates {
    static public readonly string StartTerm    = "$StartTerm";
    static public readonly string EofTokenName = "$EOFToken";

    private readonly Grammar.Grammar grammar;
    private readonly Analyzer.Analyzer analyzer;
    private readonly Buffered log;

    /// <summary>Constructs of a new parser state collection.</summary>
    /// <param name="grammar">The grammar to build states for.</param>
    /// <param name="log">The optional logger to log the steps the builder has performed.</param>
    public ParserStates(Grammar.Grammar grammar, ILogger? log = null) {
        this.grammar = grammar;
        this.log = new Buffered(log);

        Term? startTerm = this.grammar.StartTerm;
        if (startTerm is null) throw new ParserException("Grammar did not have start term set.");

        // Check if the grammar has already been decorated with the StartTerm and EofTokenName,
        // if not then add them. Always ensure the StartTerm is set as the start term.
        if (this.grammar.Terms.FindItemByName(StartTerm) is null) {
            Term oldStart = startTerm;
            this.grammar.NewRule(StartTerm).AddTerm(oldStart.Name).AddToken(EofTokenName);
        }
        startTerm = this.grammar.Start(StartTerm);

        this.States   = new();
        this.analyzer = new(this.grammar);

        this.determineStates(startTerm);
    }

    /// <summary>The set of states for the parser.</summary>
    public readonly List<State> States;

    #region State Building...

    /// <summary>Finds a state with the given fragment.</summary>
    /// <param name="fragment">The fragment to find.</param>
    /// <returns>The found state or null.</returns>
    private State? findState(Fragment fragment) =>
        this.States.FirstOrDefault(state => state.HasFragment(fragment));

    /// <summary>Determines all the parser states for the grammar.</summary>
    /// <param name="startTerm">The start term of the grammar.</param>
    private void determineStates(Term startTerm) {
        // Create the first state, state 0.
        State startState = new(0);
        TokenItem eof = new(EofTokenName);
        foreach (Rule rule in startTerm.Rules)
            startState.AddFragment(new Fragment(rule, 0, eof), analyzer);
        this.States.Add(startState);
        this.log?.AddInfo("Created initial start state:",
            "  " + startState.ToString("  "));

        // Fill out all other states.
        HashSet<State> changed = new() { startState };
        while (changed.Count > 0) {
            State state = changed.First();
            changed.Remove(state);
            this.nextStates(state).Foreach(changed.Add);
        }
    }

    /// <summary>Determines the next state with the given fragment from the given state.</summary>
    /// <param name="state">The state that is being followed.</param>
    /// <param name="fragment">The fragment from the state to follow.</param>
    /// <param name="changed">The states which have been changed.</param>
    private void determineNextStateFragment(State state, Fragment fragment, HashSet<State> changed) {
        Rule rule = fragment.Rule;
        int index = fragment.Index;

        // If there are any items left in this fragment get it or leave.
        Item? item = rule.BasicItems.ElementAtOrDefault(index);
        if (item is null) return;

        // If this item is the EOF token then we have found the grammar accept.
        if (item is TokenItem && item.Name == EofTokenName) {
            state.SetAccept();
            return;
        }

        // Create a new fragment for the action.
        Fragment nextFrag = new(rule, index + 1, fragment.Lookaheads);
        this.log?.AddInfoF("  Created fragment: {0}", nextFrag);

        // Get or create a new state for the target of the action.
        State? next = state.FindActionTarget(item);
        if (next is null) {
            next = this.findState(nextFrag);
            if (next is null) {
                next = new State(States.Count);
                States.Add(next);
            }
            state.AddAction(new Action(item, next));
        }
        this.log?.AddInfoF("    Adding fragment to state {0}.", next.Number);

        // Try to add the fragment and indicate a change if it was changed.
        if (next.AddFragment(nextFrag, analyzer))
            changed.Add(next);
    }

    /// <summary>Determines the next states following the given state.</summary>
    /// <param name="state">The state to follow.</param>
    /// <returns>The next states.</returns>
    private HashSet<State> nextStates(State state) {
        this.log?.AddInfoF("Next States from state {0}.", state.Number);
        HashSet<State> changed = new();
        // Use fragment count instead of for-each because fragments will be added to the list,
        // this means we also need to increment and check count on each loop.
        for (int i = 0; i < state.Fragments.Count; ++i)
            this.determineNextStateFragment(state, state.Fragments[i], changed);
        return changed;
    }

    #endregion
    #region Table Building...

    /// <summary>Add a fragment to the table for the state with the given number.</summary>
    /// <param name="stateNumber">The state number for the state to add the fragments for.</param>
    /// <param name="frag">The fragment to add to the table.</param>
    private void addFragmentForStateToTable(Table.Table table, int stateNumber, Fragment frag) {
        List<Item> items = frag.Rule.BasicItems.ToList();
        if (items.Count > frag.Index) return;

        Reduce reduce = new(frag.Rule);
        foreach (TokenItem follow in frag.Lookaheads) {
            this.log?.AddInfo("  WriteReduce("+stateNumber+", "+follow.Name+", "+reduce+")");
            table.WriteReduce(stateNumber, follow.Name, reduce);
        }
    }

    /// <summary>Add an action to the table for the state with the given number.</summary>
    /// <param name="stateNumber">The state number for the state to add the action for.</param>
    /// <param name="action">The action to add to the table.</param>
    private void addActionForStateToTable(Table.Table table, int stateNumber, Action action) {
        string onItem = action.Item.Name;
        int gotoNo = action.State.Number;
        if (action.Item is Term) {
            this.log?.AddInfo("  WriteGoto("+stateNumber+", "+onItem+", "+gotoNo+")");
            table.WriteGoto(stateNumber, onItem, new Goto(gotoNo));
        } else {
            this.log?.AddInfo("  WriteShift("+stateNumber+", "+onItem+", "+gotoNo+")");
            table.WriteShift(stateNumber, onItem, new Shift(gotoNo));
        }
    }

    /// <summary>Add a state into the table.</summary>
    /// <param name="state">The state to add into the table.</param>
    private void addStateToTable(Table.Table table, State state) {
        if (state.HasAccept) {
            this.log?.AddInfo("  WriteAccept("+state.Number+", "+EofTokenName+")");
            table.WriteAccept(state.Number, EofTokenName, new Accept());
        }

        foreach (Fragment frag in state.Fragments)
            this.addFragmentForStateToTable(table, state.Number, frag);

        foreach (Action action in state.Actions)
            this.addActionForStateToTable(table, state.Number, action);
    }

    /// <summary>Fills a parse table with the information from the states.</summary>
    public Table.Table CreateTable() {
        this.log?.AddInfo("Creating Table");
        Table.Table table = new();
        foreach (State state in States)
            this.addStateToTable(table, state);
        return table;
    }

    #endregion

    /// <summary>Returns a human readable string for debugging of the parser being built.</summary>
    /// <returns>The debugging string for the builder.</returns>
    public override string ToString() {
        StringBuilder buf = new();
        if (this.log.Failed)
            buf.Append(this.log);
        foreach (State state in States)
            buf.Append(state.ToString());
        return buf.ToString();
    }
}
