using PetiteParser.Grammar;
using PetiteParser.Parser.States;
using System.Linq;

namespace PetiteParser.Parser.Table;

/// <summary>A factory to create a parser table from parser states.</summary>
static internal class Factory {

    /// <summary>Fills a parse table with the information from the given states.</summary>
    /// <param name="states">The states to create the table with.</param>
    /// <returns>Returns the created table.</returns>
    public static Table CreateTable(this ParserStates states) {
        Table table = new();
        foreach (State state in states.States)
            addStateToTable(table, state);
        return table;
    }

    /// <summary>Add a state into the table.</summary>
    /// <param name="table">The table to write to.</param>
    /// <param name="state">The state to add into the table.</param>
    static private void addStateToTable(Table table, State state) {
        if (state.HasAccept)
            table.Write(state.Number, ParserStates.EofTokenName, new Accept());

        foreach (Fragment frag in state.Fragments.Where(f => f.AtEnd))
            addFragmentForStateToTable(table, state, frag);

        foreach (StateAction action in state.Actions)
            addActionForStateToTable(table, state, action);
    }

    /// <summary>Add an action to the table for the state with the given number.</summary>
    /// <param name="table">The table to write to.</param>
    /// <param name="state">The state to add the fragments for.</param>
    /// <param name="action">The action to add to the table.</param>
    static private void addActionForStateToTable(Table table, State state, StateAction action) {
        int gotoNo = action.State.Number;
        if (action.Item is Term term)
            table.Write(state.Number, term.Name, new Goto(gotoNo));
        else if (action.Item is TokenItem token)
            table.Write(state.Number, token.Name, new Shift(gotoNo, action.Lookaheads));
        //else
    }

    /// <summary>Add a fragment to the table for the state with the given number.</summary>
    /// <param name="table">The table to write to.</param>
    /// <param name="state">The state to add the fragments for.</param>
    /// <param name="frag">The fragment to add to the table.</param>
    static private void addFragmentForStateToTable(Table table, State state, Fragment frag) {
        Reduce reduce = new(frag.Rule, frag.Lookaheads);
        foreach (TokenItem follow in frag.Lookaheads)
            table.Write(state.Number, follow.Name, reduce);
    }
}
