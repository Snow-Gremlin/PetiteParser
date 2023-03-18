using System;
using System.Linq;
using System.Text;

namespace PetiteParser.Formatting;

/// <summary>A tool for creating a string for a table.</summary>
sealed public partial class StringTable {

    /// <summary>The number of lines to draw between cells.</summary>
    public enum Edge {
        None      = 0,
        Zero      = 1,
        One       = 2,
        Dot       = 3,
        Dot3      = 4,
        Dot4      = 5,
        Dash      = 6,
        OneHeavy  = 7,
        Dot3Heavy = 8,
        Dot4Heavy = 9,
        DashHeavy = 10,
        Two       = 11,
    }

    /// <summary>The horizontal alignment of text in a column.</summary>
    public enum Alignment {
        Left,
        Center,
        Right,
    }

    /// <summary>Creates a new string table of the given size.</summary>
    /// <param name="rows">The number of rows for the table.</param>
    /// <param name="columns">The number of columns for the table.</param>
    public StringTable(int rows, int columns) {
        this.MaximumColumnWidth = 100;
        this.MaximumRowHeight   = 10;
        this.Rows        = rows;
        this.Columns     = columns;
        this.Alignments  = new Alignment[columns];
        this.RowEdges    = new Edge[rows + 1];
        this.ColumnEdges = new Edge[columns + 1];
        this.Data        = new string[rows, columns];
    }

    /// <summary>The maximum number of characters wide any column can get.</summary>
    public int MaximumColumnWidth { get; set; }
    
    /// <summary>The maximum number of lines tall any row can get.</summary>
    public int MaximumRowHeight { get; set; }

    /// <summary>The set number of rows in the table.</summary>
    public int Rows { get; private set; }

    /// <summary>The set number of columns in the table.</summary>
    public int Columns { get; private set; }

    /// <summary>The horizontal alignments for each column.</summary>
    public Alignment[] Alignments { get; private set; }

    /// <summary>The edges between each row.</summary>
    public Edge[] RowEdges { get; private set; }

    /// <summary>The edges between each column.</summary>
    public Edge[] ColumnEdges { get; private set; }

    /// <summary>The data for each cell in the table.</summary>
    public string[,] Data { get; private set; }

    /// <summary>Sets the outer edges of the table the given edge type.</summary>
    /// <param name="edge">The type of edges to set the boarder.</param>
    public void SetBoarder(Edge edge) {
        this.RowEdges[0]         = edge;
        this.RowEdges[this.Rows] = edge;
        this.ColumnEdges[0]            = edge;
        this.ColumnEdges[this.Columns] = edge;
    }

    /// <summary>Sets all the edges for the rows to the given edge type.</summary>
    /// <param name="edge">The type of the edges to set.</param>
    public void SetAllRowEdges(Edge edge) {
        for (int i = this.RowEdges.Length-1; i >= 0; --i)
            this.RowEdges[i] = edge;
    }
    
    /// <summary>Sets all the edges for the columns to the given edge type.</summary>
    /// <param name="edge">The type of the edges to set.</param>
    public void SetAllColumnEdges(Edge edge) {
        for (int i = this.ColumnEdges.Length-1; i >= 0; --i)
            this.ColumnEdges[i] = edge;
    }

    /// <summary>Sets all the edges for the rows and columns to the given edge type.</summary>
    /// <param name="edge">The type of the edges to set.</param>
    public void SetAllEdges(Edge edge) {
        this.SetAllRowEdges(edge);
        this.SetAllColumnEdges(edge);
    }

    /// <summary>
    /// Sets the table edges such that there are lines between each row,
    /// lines around the table, and a line around the first row as a header.
    /// </summary>
    public void SetRowHeaderDefaultEdges() {
        this.SetAllColumnEdges(Edge.One);
        this.SetBoarder(Edge.One);
        this.RowEdges[1] = Edge.One;
    }

    #region String Methods

    /// <summary>Determines the string to put as an intersection edges between the given row and column.</summary>
    /// <param name="row">The row to get the edge to the left of.</param>
    /// <param name="column">The column to get the above edge.</param>
    /// <returns>The string for the intersecting edges.</returns>
    private string edgeIntersection(int row, int column) {
        Edge rowEdge = this.RowEdges[row];
        Edge colEdge = this.ColumnEdges[column];
        Location rowLoc = locInRange(row,    this.Rows);
        Location colLoc = locInRange(column, this.Columns);

        string rune = edgeIntersection(rowEdge, colEdge, rowLoc, colLoc).ToString();
        if (colEdge == Edge.None) return colLoc == Location.Middle ? rune : "";

        string padding = edgeHorizontal(rowEdge).ToString();
        return (colLoc != Location.Start ? padding : "") + rune + (colLoc != Location.End ? padding : "");
    }

    /// <summary>Gets the horizontal edge string between rows.</summary>
    /// <param name="row">The row to get the edge along.</param>
    /// <param name="count">The width of the column.</param>
    /// <returns>The string for above the given rows.</returns>
    private string edgeHorizontal(int row, int count) =>
        new(edgeHorizontal(this.RowEdges[row]), count);

    /// <summary>Gets the vertical edge string beside columns.</summary>
    /// <param name="row">The row to get the edge along.</param>
    /// <param name="column">The column to get the left side of.</param>
    /// <returns>The string for the left of the given columns.</returns>
    private string edgeVertical(int row, int column) {
        Edge colEdge = this.ColumnEdges[column];
        Location colLoc = locInRange(column, this.Columns);

        string rune = edgeVertical(colEdge).ToString();
        return colEdge == Edge.None ? (colLoc == Location.Middle ? rune : "") :
            (colLoc != Location.Start ? " " : "") + rune + (colLoc != Location.End ? " " : "");
    }

    /// <summary>Gets the maximum width for each column.</summary>
    /// <returns>The maximum widths for each column.</returns>
    private int[] maxWidths() {
        int totalMax = this.MaximumColumnWidth;
        if (totalMax < 3) totalMax = 3;

        int[] widths = new int[this.Columns];
        for (int j = 0; j < this.Columns; j++) {
            int maxWidth = 0;
            for (int i = 0; i < this.Rows; i++) {
                string text = this.Data[i, j];
                if (!string.IsNullOrEmpty(text)) {
                    foreach (string line in text.SplitLines()) {
                        int width = line.Length;
                        if (width > maxWidth) maxWidth = width;
                    }
                }
            }
            if (maxWidth > totalMax) maxWidth = totalMax;
            widths[j] = maxWidth;
        }
        return widths;
    }

    /// <summary>Gets all the lines for each column in a row.</summary>
    /// <param name="row">The row to get the lines for.</param>
    /// <returns>The lines for all the columns in the row.</returns>
    private string[][] columnLines(int row) {
        int totalMax = this.MaximumRowHeight;
        if (totalMax < 1) totalMax = 1;

        string[][] columns = new string[this.Columns][];
        for (int j = 0; j < this.Columns; j++) {
            string text = this.Data[row, j];
            if (!string.IsNullOrEmpty(text)) {
                string[] column = text.SplitLines();
                if (column.Length > totalMax)
                    column = column.Take(totalMax - 1).Append("...").ToArray();
                columns[j] = column;
            }
        }
        return columns;
    }

    /// <summary>Gets the maximum number of lines in a row.</summary>
    /// <param name="columns">The lines for all the columns in a row.</param>
    /// <returns>The maximum number of lines in a row.</returns>
    private int maxHeight(string[][] columns) {
        int maxHeight = 0;
        for (int j = 0; j < this.Columns; j++) {
            int height = columns[j]?.Length ?? 0;
            if (height > maxHeight) maxHeight = height;
        }
        return maxHeight;
    }

    /// <summary>Adds a horizontal line above a row as a divider.</summary>
    /// <param name="result">The string builder to write the table to.</param>
    /// <param name="row">The row to write the horizontal line above.</param>
    /// <param name="widths">The column widths.</param>
    private void addHorizontal(StringBuilder result, int row, int[] widths) {
        if (this.RowEdges[row] == Edge.None) return;
        for (int i = 0; i < this.Columns; i++) {
            result.Append(this.edgeIntersection(row, i));
            result.Append(this.edgeHorizontal(row, widths[i]));
        }
        result.Append(this.edgeIntersection(row, this.Columns));
        result.AppendLine();
    }

    /// <summary>Gets an aligned string for a single line in a cell.</summary>
    /// <param name="lines">The lines for a cell.</param>
    /// <param name="lineNo">The index for the current line to get.</param>
    /// <param name="column">The index for the column this cell belongs to.</param>
    /// <param name="widths">The column widths.</param>
    /// <returns>The string for a line in a cell.</returns>
    private string align(string[] lines, int lineNo, int column, int[] widths) {
        int width = widths[column];
        if (lines is null || lines.Length < lineNo) return "".PadRight(width);

        string line = lines[lineNo];
        int length = line.Length;
        if (length > width) {
            length = width;
            line = string.Concat(line.AsSpan(0, width - 3), "...");
        }

        switch (this.Alignments[column]) {
            case Alignment.Right: return line.PadLeft(width);
            case Alignment.Center: return line.PadLeft((width + length) / 2).PadRight(width);
            case Alignment.Left: break;
        }
        return line.PadRight(width); // Left
    }

    /// <summary>Adds a row of text to the given string buffer.</summary>
    /// <param name="result">The string buffer to add a row to.</param>
    /// <param name="row">The row to add.</param>
    /// <param name="widths">The column widths.</param>
    private void addRowText(StringBuilder result, int row, int[] widths) {
        string[][] columns = this.columnLines(row);
        int height = this.maxHeight(columns);
        for (int k = 0; k < height; k++) {
            for (int j = 0; j < this.Columns; j++) {
                result.Append(this.edgeVertical(k, j));
                result.Append(this.align(columns[j], k, j, widths));
            }
            result.Append(this.edgeVertical(k, this.Columns));
            result.AppendLine();
        }
    }

    /// <summary>Gets the table as a string.</summary>
    /// <returns>The string for this table.</returns>
    override public string ToString() {
        StringBuilder result = new();
        int[] widths = this.maxWidths();
        for (int j = 0; j < this.Rows; j++) {
            this.addHorizontal(result, j, widths);
            this.addRowText(result, j, widths);
        }
        this.addHorizontal(result, this.Rows, widths);
        return result.ToString();
    }

    #endregion String Methods
}
