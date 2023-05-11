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
    private readonly List<Dictionary<string, int>> gotoTable;

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

    /// <summary>Reads a shift action from the table, returns null if no action set. </summary>
    /// <param name="stateNumber">The state number to read from.</param>
    /// <param name="tokenName">The name of token to read from.</param>
    /// <returns>The action read from the shift table or null.</returns>
    internal IAction? ReadShift(int stateNumber, string tokenName) {
        if ((stateNumber >= 0) && (stateNumber < this.shiftTable.Count)) {
            Dictionary<string, IAction> rowData = this.shiftTable[stateNumber];
            if (rowData.TryGetValue(tokenName, out IAction? action)) return action;
        }
        return null;
    }

    /// <summary>Reads a goto action from the table, returns null if no action set.</summary>
    /// <param name="stateNumber">The state number to read from.</param>
    /// <param name="termName">The name of term to read from.</param>
    /// <returns>The goto state read from the goto table or -1.</returns>
    internal int ReadGoto(int stateNumber, string termName) {
        if ((stateNumber >= 0) && (stateNumber < this.gotoTable.Count)) {
            Dictionary<string, int> rowData = this.gotoTable[stateNumber];
            if (rowData.TryGetValue(termName, out int action)) return action;
        }
        return -1;
    }

    /// <summary>Writes a new action to the table.</summary>
    /// <param name="stateNumber">The state number to write to.</param>
    /// <param name="itemName">The name of the term (Goto) or token (anything else) to write to.</param>
    /// <param name="action">The action to write to the table.</param>
    public void WriteShift(int stateNumber, string tokenName, IAction action) {
        if (stateNumber < 0) throw new ArgumentException("State Number must be zero or more.");
        while (stateNumber >= this.shiftTable.Count)
            this.shiftTable.Add(new Dictionary<string, IAction>());
        Dictionary<string, IAction> rowData = this.shiftTable[stateNumber];
        if (rowData.ContainsKey(tokenName))
            throw new ParserException("Table entry (" + stateNumber + ", " + tokenName + ") already has an assigned action.");
        rowData[tokenName] = action;
    }
    
    /// <summary>Writes a new goto to the table.</summary>
    /// <param name="stateNumber">The state number to write to.</param>
    /// <param name="termName">The name of the term to write to.</param>
    /// <param name="gotoState">The goto state to write to the table.</param>
    public void WriteGoto(int stateNumber, string termName, int gotoState) {
        if (stateNumber < 0) throw new ArgumentException("State Number must be zero or more.");
        while (stateNumber >= this.gotoTable.Count)
            this.gotoTable.Add(new Dictionary<string, int>());
        Dictionary<string, int> rowData = this.gotoTable[stateNumber];
        if (rowData.ContainsKey(termName))
            throw new ParserException("Table entry (" + stateNumber + ", " + termName + ") already has an assigned goto.");
        rowData[termName] = gotoState;
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
            Dictionary<string, IAction> data = this.shiftTable[i];
            foreach (KeyValuePair<string, IAction> pair in data) {
                int j = shiftColumns.IndexOf(pair.Key);
                grid.Data[i+1, j+1] = pair.Value?.ToString() ?? "null";
            }
        }
        
        // Add all the goto table data
        for (int i = this.gotoTable.Count-1; i >= 0; --i) {
            Dictionary<string, int> data = this.gotoTable[i];
            foreach (KeyValuePair<string, int> pair in data) {
                int j = gotoColumns.IndexOf(pair.Key);
                grid.Data[i+1, j+shiftCount+1] = "" + pair.Value;
            }
        }

        return grid.ToString();
    }
}
