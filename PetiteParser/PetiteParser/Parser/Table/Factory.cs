using PetiteParser.Grammar;
using PetiteParser.Logger;
using PetiteParser.Parser.States;
using System.Collections.Generic;
using System.Linq;

namespace PetiteParser.Parser.Table;

/// <summary>A factory to create a parser table from parser states.</summary>
static internal class Factory {

    /// <summary>Fills a parse table with the information from the given states.</summary>
    /// <param name="states">The states to create the table with.</param>
    /// <param name="log">The optional logger to log the steps the builder has performed.</param>
    /// <returns>Returns the created table.</returns>
    static public Table CreateTable(ParserStates states, ILogger? log = null) {
        log?.AddInfo("Creating Table");
        Table table = new();
        foreach (State state in states.States)
            addStateToTable(table, state, log);
        return table;
    }

    /// <summary>Add a state into the table.</summary>
    /// <param name="table">The table to write to.</param>
    /// <param name="state">The state to add into the table.</param>
    /// <param name="log">The optional logger to log the steps the builder has performed.</param>
    static private void addStateToTable(Table table, State state, ILogger? log) {
        if (state.HasAccept) {
            log?.AddInfo("  WriteAccept("+state.Number+", "+ParserStates.EofTokenName+")");
            table.WriteAccept(state.Number, ParserStates.EofTokenName, new Accept());
        }

        foreach (Fragment frag in state.Fragments)
            addFragmentForStateToTable(table, state.Number, frag, log);

        foreach (Action action in state.Actions)
            addActionForStateToTable(table, state.Number, action, log);
    }

    /// <summary>Add an action to the table for the state with the given number.</summary>
    /// <param name="table">The table to write to.</param>
    /// <param name="stateNumber">The state number for the state to add the action for.</param>
    /// <param name="action">The action to add to the table.</param>
    /// <param name="log">The optional logger to log the steps the builder has performed.</param>
    static private void addActionForStateToTable(Table table, int stateNumber, Action action, ILogger? log) {
        string onItem = action.Item.Name;
        int gotoNo = action.State.Number;
        if (action.Item is Term) {
            log?.AddInfo("  WriteGoto("+stateNumber+", "+onItem+", "+gotoNo+")");
            table.WriteGoto(stateNumber, onItem, new Goto(gotoNo));
        } else {
            log?.AddInfo("  WriteShift("+stateNumber+", "+onItem+", "+gotoNo+")");
            table.WriteShift(stateNumber, onItem, new Shift(gotoNo));
        }
    }

    /// <summary>Add a fragment to the table for the state with the given number.</summary>
    /// <param name="table">The table to write to.</param>
    /// <param name="stateNumber">The state number for the state to add the fragments for.</param>
    /// <param name="frag">The fragment to add to the table.</param>
    /// <param name="log">The optional logger to log the steps the builder has performed.</param>
    static private void addFragmentForStateToTable(Table table, int stateNumber, Fragment frag, ILogger? log) {
        List<Item> items = frag.Rule.BasicItems.ToList();
        if (items.Count > frag.Index) return;

        Reduce reduce = new(frag.Rule);
        foreach (TokenItem follow in frag.Lookaheads) {
            log?.AddInfo("  WriteReduce("+stateNumber+", "+follow.Name+", "+reduce+")");
            table.WriteReduce(stateNumber, follow.Name, reduce);
        }
    }
}
