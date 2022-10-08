using PetiteParser.Grammar;
using PetiteParser.Logger;
using PetiteParser.Misc;
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
    public static Table CreateTable(this ParserStates states, ILogger? log = null) {
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
            addFragmentForStateToTable(table, state, frag, log);

        foreach (Action action in state.Actions)
            addActionForStateToTable(table, state, action, log);
    }

    /// <summary>Add an action to the table for the state with the given number.</summary>
    /// <param name="table">The table to write to.</param>
    /// <param name="state">The state to add the fragments for.</param>
    /// <param name="action">The action to add to the table.</param>
    /// <param name="log">The optional logger to log the steps the builder has performed.</param>
    static private void addActionForStateToTable(Table table, State state, Action action, ILogger? log) {
        string onItem = action.Item.Name;
        int gotoNo = action.State.Number;
        if (action.Item is Term) {
            log?.AddInfo("  WriteGoto("+state.Number+", "+onItem+", "+gotoNo+")");
            table.WriteGoto(state.Number, onItem, new Goto(gotoNo));
        } else {
            log?.AddInfo("  WriteShift("+state.Number+", "+onItem+", "+gotoNo+", @ "+action.Lookaheads.Join(" ")+")");
            table.WriteShift(state.Number, onItem, new Shift(gotoNo, action.Lookaheads));
        }
    }

    /// <summary>Add a fragment to the table for the state with the given number.</summary>
    /// <param name="table">The table to write to.</param>
    /// <param name="state">The state to add the fragments for.</param>
    /// <param name="frag">The fragment to add to the table.</param>
    /// <param name="log">The optional logger to log the steps the builder has performed.</param>
    static private void addFragmentForStateToTable(Table table, State state, Fragment frag, ILogger? log) {
        List<Item> items = frag.Rule.BasicItems.ToList();
        if (items.Count > frag.Index) return;

        // TODO: Show these in the states too

        Reduce reduce = new(frag.Rule, frag.Lookaheads);
        foreach (TokenItem follow in frag.Lookaheads) {
            log?.AddInfo("  WriteReduce("+state.Number+", "+follow.Name+", "+reduce+")");
            table.WriteReduce(state.Number, follow.Name, reduce);
        }
    }
}
