using System.Text;
using System;
using System.Linq;

namespace PetiteParser.Formatting;

/// <summary>A tool for creating a string for a table.</summary>
public class StringTable {

    /// <summary>The number of lines to draw between cells.</summary>
    public enum Edge {
        Zero = 0,
        One = 1,
        Two = 2
    }

    /// <summary>The horizontal alignment of text in a column.</summary>
    public enum Alignment {
        Left = default,
        Center,
        Right,
    }

    //  TODO: Test, comment, cleanup
    public StringTable(int rows, int columns) {
        MaximumColumnWidth = 100;
        MaximumRowHeight = 10;
        Rows = rows;
        Columns = columns;
        Alignments = new Alignment[columns];
        RowEdges = new Edge[rows + 1];
        ColumnEdges = new Edge[columns + 1];
        Data = new string[rows, columns];
    }

    public int MaximumColumnWidth { get; set; }

    public int MaximumRowHeight { get; set; }

    public int Rows { get; private set; }

    public int Columns { get; private set; }

    public Alignment[] Alignments { get; private set; }

    public Edge[] RowEdges { get; private set; }

    public Edge[] ColumnEdges { get; private set; }

    public string[,] Data { get; private set; }

    #region String Methods

    private static readonly string[] edgeIntersectionChars = new string[] {
        //0,0   0,1    0,2    1,0    1,1    1,2    2,0    2,1    2,2
        "",    " │ ", " ║ ", "───", "─┼─", "─╫─", "═══", "═╪═", "═╬═", // center, center
        "",     "│ ",  "║ ", "",     "├─",  "╟─",  "══",  "╞═",  "╠═", // center, left   side
        "",    " │",  " ║",  "",    "─┤",  "─╢",  "══",  "═╡",  "═╣",  // center, right  side

        "",    " │ ", " ║ ", "───", "─┬─", "─╥─", "═══", "═╤═", "═╦═", // top,    center side
        "",     "│ ",  "║ ", "",     "┌─",  "╓─",  "══",  "╒═",  "╔═", // top,    left   corner
        "",    " │",  " ║",  "",    "─┐",  "─╖",  "══ ", "═╕",  "═╗",  // top,    right  corner

        "",    " │ ", " ║ ", "───", "─┴─", "─╨─", "═══", "═╧═", "═╩═", // bottom, center side
        "",     "│ ",  "║ ", "",     "└─",  "╙─",  "══",  "╘═",  "╚═", // bottom, left   corner
        "",    " │",  " ║",  "",    "─┘",  "─╜",  "══",  "═╛",  "═╝",  // bottom, right  corner
    };

    private string edgeIntersection(int row, int column) {
        Edge rowEdge = RowEdges[row];
        Edge colEdge = ColumnEdges[column];
        int index = (int)rowEdge * 3 + (int)colEdge;
        if (column == 0) index += 9;
        else if (column == Columns) index += 18;
        if (row == 0) index += 27;
        else if (row == Rows) index += 54;
        return edgeIntersectionChars[index];
    }

    private static readonly char[] edgeHorizontalChar = new char[] {
        ' ', '─', '═',
    };

    private string edgeHorizontal(int row, int count) => new(edgeHorizontalChar[(int)RowEdges[row]], count);

    private static readonly string[] edgeVerticalChar = new string[] {
        " ", " │ ", " ║ ", // center
        "",   "│ ",  "║ ", // left side
        "",  " │",  " ║",  // right side
    };

    private string edgeVertical(int column) {
        int index = (int)ColumnEdges[column];
        if (column == 0) index += 3;
        else if (column == Columns) index += 6;
        return edgeVerticalChar[index];
    }

    private int[] maxWidths() {
        int totalMax = MaximumColumnWidth;
        if (totalMax < 3) totalMax = 3;

        int[] widths = new int[Columns];
        for (int j = 0; j < Columns; j++) {
            int maxWidth = 0;
            for (int i = 0; i < Rows; i++) {
                string text = Data[i, j];
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

    private string[][] columnLines(int row) {
        int totalMax = MaximumRowHeight;
        if (totalMax < 1) totalMax = 1;

        string[][] columns = new string[Columns][];
        for (int j = 0; j < Columns; j++) {
            string text = Data[row, j];
            if (!string.IsNullOrEmpty(text)) {
                string[] column = text.SplitLines();
                if (column.Length > totalMax)
                    column = column.Take(totalMax - 1).Append("...").ToArray();
                columns[j] = column;
            }
        }
        return columns;
    }

    private int maxHeight(string[][] columns) {
        int maxHeight = 0;
        for (int j = 0; j < Columns; j++) {
            int height = columns[j]?.Length ?? 0;
            if (height > maxHeight) maxHeight = height;
        }
        return maxHeight;
    }

    private void addHorizontal(StringBuilder result, int row, int[] widths) {
        if (RowEdges[row] == Edge.Zero) return;
        for (int i = 0; i < Columns; i++) {
            result.Append(edgeIntersection(row, i));
            result.Append(edgeHorizontal(row, widths[i]));
        }
        result.Append(edgeIntersection(row, Columns));
        result.AppendLine();
    }

    private string align(string[] lines, int lineNo, int column, int[] widths) {
        int width = widths[column];
        if (lines is null || lines.Length < lineNo) return "".PadRight(width);

        string line = lines[lineNo];
        int length = line.Length;
        if (length > width) {
            length = width;
            line = string.Concat(line.AsSpan(0, width - 3), "...");
        }

        switch (Alignments[column]) {
            case Alignment.Right: return line.PadLeft(width);
            case Alignment.Center: return line.PadLeft((width + length) / 2).PadRight(width);
            case Alignment.Left: break;
        }
        return line.PadRight(width); // Left
    }

    private void addRowText(StringBuilder result, int row, int[] widths) {
        string[][] columns = columnLines(row);
        int height = maxHeight(columns);
        for (int k = 0; k < height; k++) {
            for (int j = 0; j < Columns; j++) {
                result.Append(edgeVertical(j));
                result.Append(align(columns[j], k, j, widths));
            }
            result.Append(edgeVertical(Columns));
            result.AppendLine();
        }
    }

    /// <summary>Gets the table as a string.</summary>
    /// <returns>The string for this table.</returns>
    override public string ToString() {
        StringBuilder result = new();
        int[] widths = maxWidths();
        for (int j = 0; j < Rows; j++) {
            addHorizontal(result, j, widths);
            addRowText(result, j, widths);
        }
        addHorizontal(result, Rows, widths);
        return result.ToString();
    }

    #endregion String Methods
}
