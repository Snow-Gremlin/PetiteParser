using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PetiteParser.Parser.Table {

    /// <summary>
    /// This is a table to define the actions to take when
    /// a new token is added to the parse.
    /// </summary>
    sealed internal class Table {
        private readonly HashSet<string> shiftColumns;
        private readonly HashSet<string> gotoColumns;
        private readonly List<Dictionary<string, IAction>> shiftTable;
        private readonly List<Dictionary<string, IAction>> gotoTable;

        /// <summary>Creates a new parse table.</summary>
        public Table() {
            this.shiftColumns = new();
            this.gotoColumns  = new();
            this.shiftTable   = new();
            this.gotoTable    = new();
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
                    if (action is not null and not Error) result.Add(key);
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
        internal IAction ReadShift(int row, string column) =>
            read(row, column, this.shiftTable);

        /// <summary> Reads a goto action from the table, returns null if no action set.</summary>
        /// <param name="row">The row to read from.</param>
        /// <param name="column">The column to read from.</param>
        /// <returns>The action read from the goto table or null.</returns>
        internal IAction ReadGoto(int row, string column) =>
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
            if (rowData.TryGetValue(column, out IAction existing))
                throw new Exception("Trying to overwrite \""+existing+"\" with \""+value+"\" at "+row+" and \""+column+"\".");
            rowData[column] = value;
        }

        /// <summary>Writes a new shift action to the table.</summary>
        /// <param name="row">The row to write to.</param>
        /// <param name="column">The column to write to.</param>
        /// <param name="value">The value to write to the table.</param>
        internal void WriteShift(int row, string column, Shift value) =>
            write(row, column, value, this.shiftColumns, this.shiftTable);

        /// <summary>Writes a new goto action to the table.</summary>
        /// <param name="row">The row to write to.</param>
        /// <param name="column">The column to write to.</param>
        /// <param name="value">The value to write to the table.</param>
        internal void WriteGoto(int row, string column, Goto value) =>
            write(row, column, value, this.gotoColumns, this.gotoTable);

        /// <summary>Writes a new reduce action to the shift table.</summary>
        /// <param name="row">The row to write to.</param>
        /// <param name="column">The column to write to.</param>
        /// <param name="value">The value to write to the table.</param>
        internal void WriteReduce(int row, string column, Reduce value) =>
            write(row, column, value, this.shiftColumns, this.shiftTable);

        /// <summary>Writes a new accept action to the shift table.</summary>
        /// <param name="row">The row to write to.</param>
        /// <param name="column">The column to write to.</param>
        /// <param name="value">The value to write to the table.</param>
        internal void WriteAccept(int row, string column, Accept value) =>
            write(row, column, value, this.shiftColumns, this.shiftTable);

        /// <summary>Gets a string output of the table for debugging.</summary>
        /// <returns>The string of the table.</returns>
        public override string ToString() {
            const string emptyCell = "-";
            const string verticalSep = " | ";

            List<List<string>> grid = new();

            // Add Column labels
            List<string> columnLabels = new() { "state" };
            List<string> shiftColumns = this.shiftColumns.ToList();
            shiftColumns.Sort();
            for (int j = 0; j < shiftColumns.Count; ++j)
                columnLabels.Add("["+shiftColumns[j].ToString()+"]");
            List<string> gotoColumns = this.gotoColumns.ToList();
            gotoColumns.Sort();
            for (int j = 0; j < gotoColumns.Count; ++j)
                columnLabels.Add("<"+gotoColumns[j].ToString()+">");
            grid.Add(columnLabels);

            // Add all the data into the table
            int colCount = shiftColumns.Count + gotoColumns.Count + 1;
            int rowCount = Math.Max(this.shiftTable.Count, this.gotoTable.Count);
            for (int row = 0; row < rowCount; ++row) {
                List<string> values = new(colCount) { row.ToString() };
                for (int i = 0; i < shiftColumns.Count; ++i) {
                    IAction action = this.ReadShift(row, shiftColumns[i]);
                    if (action is null) values.Add(emptyCell);
                    else values.Add(action.ToString());
                }
                for (int i = 0; i < gotoColumns.Count; ++i) {
                    IAction action = this.ReadGoto(row, gotoColumns[i]);
                    if (action is null) values.Add(emptyCell);
                    else values.Add(action.ToString());
                }
                grid.Add(values);
            }

            // Measure all the items in a column to get the maximum column width
            List<int> widths = new(colCount);
            for (int j = 0; j < colCount; ++j) {
                int maxWidth = 0;
                for (int i = 0; i < rowCount; ++i)
                    maxWidth = Math.Max(maxWidth, grid[i][j].Length);
                widths.Add(maxWidth);
            }

            // Write the table
            StringBuilder buf = new();
            for (int i = 0; i < rowCount; ++i) {
                if (i > 0) buf.AppendLine();
                for (int j = 0; j < colCount; ++j) {
                    if (j > 0) buf.Append(verticalSep);
                    buf.Append(grid[i][j].PadRight(widths[j]));
                }
            }
            return buf.ToString();
        }
    }
}
