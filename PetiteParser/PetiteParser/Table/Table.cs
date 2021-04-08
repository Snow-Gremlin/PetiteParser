using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PetiteParser.Table {

    /// <summary>
    /// This is a table to define the actions to take when
    /// a new token is added to the parse.
    /// </summary>
    internal class Table {
        private HashSet<string> shiftColumns;
        private HashSet<string> gotoColumns;
        private List<Dictionary<string, IAction>> shiftTable;
        private List<Dictionary<string, IAction>> gotoTable;

        /// <summary>Creates a new parse table.</summary>
        public Table() {
            this.shiftColumns = new HashSet<string>();
            this.gotoColumns  = new HashSet<string>();
            this.shiftTable   = new List<Dictionary<string, IAction>>();
            this.gotoTable    = new List<Dictionary<string, IAction>>();
        }

        /// <summary>Gets all the tokens for the row which are not null or error.</summary>
        /// <param name="row">The row to get all the tokens for.</param>
        /// <returns>The list of all the tokens.</returns>
        public List<string> GetAllTokens(int row) {
            List<string> result = new();
            if ((row >= 0) && (row < this.shiftTable.Count)) {
                Dictionary<string, IAction> rowData = this.shiftTable[row];
                foreach (string key in rowData.Keys) {
                    IAction action = rowData[key];
                    if (!(action is null) || !(action is Error)) result.Add(key);
                }
            }
            return result;
        }

        /// <summary>Reads an action from the table, returns null if no action set.</summary>
        /// <param name="row">The row to read from.</param>
        /// <param name="column">The column to read from.</param>
        /// <param name="table">The shift or goto table to read from.</param>
        /// <returns>The action read from the table or null.</returns>
        static private IAction read(int row, string column, List<Dictionary<string, IAction>> table) {
            if ((row >= 0) && (row < table.Count)) {
                Dictionary<string, IAction> rowData = table[row];
                if (rowData.TryGetValue(column, out IAction action)) return action;
            }
            return null;
        }

        /// <summary>Reads a shift action from the table, returns null if no action set. </summary>
        /// <param name="row">The row to read from.</param>
        /// <param name="column">The column to read from.</param>
        /// <returns>The action read from the shift table or null.</returns>
        public IAction ReadShift(int row, string column) =>
            read(row, column, this.shiftTable);

        /// <summary> Reads a goto action from the table, returns null if no action set.</summary>
        /// <param name="row">The row to read from.</param>
        /// <param name="column">The column to read from.</param>
        /// <returns>The action read from the goto table or null.</returns>
        public IAction ReadGoto(int row, string column) =>
            read(row, column, this.gotoTable);

        /// <summary>Writes a new action to the table.</summary>
        /// <param name="row">The row to write to.</param>
        /// <param name="column">The column to write to.</param>
        /// <param name="value">The value to write to the table.</param>
        /// <param name="columns">The columns for the table to write to.</param>
        /// <param name="table">The shift or goto table to read from.</param>
        static private void write(int row, string column, IAction value,
            HashSet<string> columns, List<Dictionary<string, IAction>> table) {
            if (row < 0) throw new ArgumentException("Row must be zero or more.");

            Dictionary<string, IAction> rowData = null;
            if (row < table.Count) rowData = table[row];
            else {
                while (row >= table.Count) {
                    rowData = new Dictionary<string, IAction>();
                    table.Add(rowData);
                }
            }

            if (!rowData.ContainsKey(column)) columns.Add(column);
            rowData[column] = value;
        }

        /// <summary>Writes a new shift action to the table.</summary>
        /// <param name="row">The row to write to.</param>
        /// <param name="column">The column to write to.</param>
        /// <param name="value">The value to write to the table.</param>
        public void WriteShift(int row, string column, IAction value) =>
            write(row, column, value, this.shiftColumns, this.shiftTable);

        /// <summary>Writes a new goto action to the table.</summary>
        /// <param name="row">The row to write to.</param>
        /// <param name="column">The column to write to.</param>
        /// <param name="value">The value to write to the table.</param>
        public void WriteGoto(int row, string column, IAction value) =>
            write(row, column, value, this.gotoColumns, this.gotoTable);

        /// <summary>Gets a string output of the table for debugging.</summary>
        /// <returns>The string of the table.</returns>
        public override string ToString() {
            List<List<string>> grid = new();

            // Add Column labels...
            List<string> columnLabels = new() {""}; // blank space for row labels
            List<string> shiftColumns = this.shiftColumns.ToList();
            shiftColumns.Sort();
            for (int j = 0; j < shiftColumns.Count; ++j)
                columnLabels.Add(shiftColumns[j].ToString());
            List<string> gotoColumns = this.gotoColumns.ToList();
            gotoColumns.Sort();
            for (int j = 0; j < gotoColumns.Count; ++j)
                columnLabels.Add(gotoColumns[j].ToString());
            grid.Add(columnLabels);

            // Add all the data into the table...
            int maxRowCount = Math.Max(this.shiftTable.Count, this.gotoTable.Count);
            for (int row = 0; row < maxRowCount; ++row) {
                List<string> values = new() { row.ToString() };
                for (int i = 0; i < shiftColumns.Count; ++i) {
                    IAction action = this.ReadShift(row, shiftColumns[i]);
                    if (action == null) values.Add("-");
                    else values.Add(action.ToString());
                }
                for (int i = 0; i < gotoColumns.Count; ++i) {
                    IAction action = this.ReadGoto(row, gotoColumns[i]);
                    if (action == null) values.Add("-");
                    else values.Add(action.ToString());
                }
                grid.Add(values);
            }

            // Make all the items in a column the same width...
            int colCount = shiftColumns.Count + gotoColumns.Count + 1;
            int rowCount = grid.Count;
            for (int j = 0; j < colCount; ++j) {
                int maxWidth = 0;
                for (int i = 0; i < rowCount; ++i)
                    maxWidth = Math.Max(maxWidth, grid[i][j].Length);
                for (int i = 0; i < rowCount; ++i)
                    grid[i][j] = grid[i][j].PadRight(maxWidth);
            }

            // Write the table...
            StringBuilder buf = new();
            for (int i = 0; i < rowCount; ++i) {
                if (i > 0) buf.AppendLine();
                for (int j = 0; j < colCount; ++j) {
                    if (j > 0) buf.Append('|');
                    buf.Append(grid[i][j]);
                }
            }
            return buf.ToString();
        }
    }
}
