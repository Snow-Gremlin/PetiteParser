using PetiteParser.Formatting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PetiteParser.Parser.Table;

/// <summary>
/// This is a table to define the actions to take when
/// a new token is added to the parse.
/// </summary>
sealed internal class Table {
    private readonly List<Dictionary<string, IAction>> shiftTable;
    private readonly List<Dictionary<string, IAction>> gotoTable;

    /// <summary>Creates a new parse table.</summary>
    public Table() {
        this.shiftTable = new();
        this.gotoTable  = new();
    }

    /// <summary>Gets all the tokens for the row which are not null or error.</summary>
    /// <param name="stateNumber">The state number to get all the tokens for.</param>
    /// <returns>The list of all the tokens.</returns>
    public List<string> GetAllTokens(int stateNumber) {
        List<string> result = new();
        if ((stateNumber >= 0) && (stateNumber < this.shiftTable.Count)) {
            Dictionary<string, IAction> rowData = this.shiftTable[stateNumber];
            foreach (string key in rowData.Keys) {
                IAction action = rowData[key];
                if (action is not null and not Error) result.Add(key);
            }
        }
        return result;
    }

    /// <summary>Reads an action from the table, returns null if no action set.</summary>
    /// <param name="stateNumber">The state number to read from.</param>
    /// <param name="itemName">The name of the term (Goto) or token (anything else) to write to.</param>
    /// <param name="table">The shift or goto table to read from.</param>
    /// <returns>The action read from the table or null.</returns>
    static private IAction? read(int stateNumber, string column, List<Dictionary<string, IAction>> table) {
        if ((stateNumber >= 0) && (stateNumber < table.Count)) {
            Dictionary<string, IAction> rowData = table[stateNumber];
            if (rowData.TryGetValue(column, out IAction? action)) return action;
        }
        return null;
    }

    /// <summary>Reads a shift action from the table, returns null if no action set. </summary>
    /// <param name="stateNumber">The state number to read from.</param>
    /// <param name="tokenName">The name of token to read from.</param>
    /// <returns>The action read from the shift table or null.</returns>
    internal IAction? ReadShift(int stateNumber, string tokenName) =>
        read(stateNumber, tokenName, this.shiftTable);

    /// <summary> Reads a goto action from the table, returns null if no action set.</summary>
    /// <param name="stateNumber">The state number to read from.</param>
    /// <param name="termName">The name of term to read from.</param>
    /// <returns>The action read from the goto table or null.</returns>
    internal IAction? ReadGoto(int stateNumber, string termName) =>
        read(stateNumber, termName, this.gotoTable);

    /// <summary>Writes a new action to the table.</summary>
    /// <param name="stateNumber">The state number to write to.</param>
    /// <param name="itemName">The name of the term (Goto) or token (anything else) to write to.</param>
    /// <param name="action">The action to write to the table.</param>
    public void Write(int stateNumber, string itemName, IAction action) {
        if (stateNumber < 0) throw new ArgumentException("State Number must be zero or more.");

        List<Dictionary<string, IAction>> table = action is Goto ? this.gotoTable : this.shiftTable;
        while (stateNumber >= table.Count)
            table.Add(new Dictionary<string, IAction>());
        Dictionary<string, IAction> rowData = table[stateNumber];

        if (rowData.ContainsKey(itemName))
            throw new ParserException("Table entry (" + stateNumber + ", " + itemName + ") already has an assigned action.");
        rowData[itemName] = action;
    }

    /// <summary>Gets a string output of the table for debugging.</summary>
    /// <returns>The string of the table.</returns>
    public override string ToString() {
        // Get all column labels
        List<string> shiftColumns = this.shiftTable.SelectMany(d => d.Keys).Distinct().ToList();
        List<string> gotoColumns  = this.gotoTable.SelectMany(d => d.Keys).Distinct().ToList();
        shiftColumns.Sort();
        gotoColumns.Sort();

        // Add column and row labels
        int shiftCount = shiftColumns.Count;
        int stateCount = Math.Max(this.shiftTable.Count, this.gotoTable.Count);
        StringTable grid = new(stateCount + 1, shiftCount + gotoColumns.Count + 1);
        grid.Data[0, 0] = "state";
        grid.RowEdges[1] = StringTable.Edge.One;
        for (int j = 0; j < shiftColumns.Count; ++j) {
            grid.Data[0, j+1] = "[" + shiftColumns[j] + "]";
            grid.ColumnEdges[j+1] = StringTable.Edge.One;
        }
        grid.ColumnEdges[1] = StringTable.Edge.Two;
        for (int j = 0; j < gotoColumns.Count; ++j) {
            grid.Data[0, j+shiftCount+1] = "<" + gotoColumns[j] + ">";
            grid.ColumnEdges[j+shiftCount+1] = StringTable.Edge.One;
        }
        grid.ColumnEdges[shiftCount+1] = StringTable.Edge.Two;
        for (int i = 0; i < stateCount; ++i)
            grid.Data[i+1, 0] = "" + i;

        // Add all the shift table data
        for (int i = this.shiftTable.Count-1; i >= 0; --i) {
            Dictionary<string, IAction> dic = this.shiftTable[i];
            foreach (KeyValuePair<string, IAction> pair in dic) {
                int j = shiftColumns.IndexOf(pair.Key);
                grid.Data[i+1, j+1] = pair.Value?.ToString() ?? "null";
            }
        }
        
        // Add all the goto table data
        for (int i = this.gotoTable.Count-1; i >= 0; --i) {
            Dictionary<string, IAction> dic = this.gotoTable[i];
            foreach (KeyValuePair<string, IAction> pair in dic) {
                int j = gotoColumns.IndexOf(pair.Key);
                grid.Data[i+1, j+shiftCount+1] = pair.Value?.ToString() ?? "null";
            }
        }

        return grid.ToString();
    }
}
