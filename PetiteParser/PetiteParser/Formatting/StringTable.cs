using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PetiteParser.Formatting;

/// <summary>A tool for creating a string for a table.</summary>
sealed public class StringTable {

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

    #region Private Static

    /// <summary>An exception used when static initialization of the table fails.</summary>
    sealed private class StringTableException : Exception {
        public StringTableException(string message) : base(message) { }
    }

    /// <summary>The location of an intersection.</summary>
    private enum Location : int {
        Start  = 0, // The top or left of the table.
        Middle = 1, // The center of the table.
        End    = 2  // The bottom or right of the table.
    }
    
    private const string ellispe = "…";
    private const string defaultPadding = " ";

    static private readonly string horizontals;
    static private readonly string verticals;
    static private readonly string[,] intersections;

    /// <summary>Initializes the box drawing character sets.</summary>
    /// <see cref="https://en.wikipedia.org/wiki/Box-drawing_character"/>
    static StringTable() {
        Edge[] values = Enum.GetValues<Edge>();
        int edgeCount = values.Length;
        for (int i = 0; i < edgeCount; ++i) {
            if (!values.Any(v => (int)v == i))
                throw new StringTableException("Failed to initialize: must have value "+i+" in the edge enumerator.");
        }
    
        static void setInter(Edge rowEdge, Edge colEdge, string input) =>
            intersections[(int)rowEdge, (int)colEdge] = input.Length == 9 ? input :
                throw new StringTableException("Failed to initialize: intersection string at ["+rowEdge+", "+colEdge+"] must have 9 characters.");

        horizontals = "  ─·┄┈╌━┅┉╍═";
        verticals   = "  │·┆┊╎┃┇┋╏║";
        intersections = new string[edgeCount, edgeCount];

        setInter(Edge.None,      Edge.None,      "         ");
        setInter(Edge.None,      Edge.Zero,      "         ");
        setInter(Edge.None,      Edge.One,       "         ");
        setInter(Edge.None,      Edge.Dot,       "         ");
        setInter(Edge.None,      Edge.Dot3,      "         ");
        setInter(Edge.None,      Edge.Dot4,      "         ");
        setInter(Edge.None,      Edge.Dash,      "         ");
        setInter(Edge.None,      Edge.OneHeavy,  "         ");
        setInter(Edge.None,      Edge.Dot3Heavy, "         ");
        setInter(Edge.None,      Edge.Dot4Heavy, "         ");
        setInter(Edge.None,      Edge.DashHeavy, "         ");
        setInter(Edge.None,      Edge.Two,       "         ");
        
        setInter(Edge.Zero,      Edge.None,      "         ");
        setInter(Edge.Zero,      Edge.Zero,      "         ");
        setInter(Edge.Zero,      Edge.One,       "╷╷╷│││╵╵╵");
        setInter(Edge.Zero,      Edge.Dot,       "·········");
        setInter(Edge.Zero,      Edge.Dot3,      "┆┆┆┆┆┆┆┆┆");
        setInter(Edge.Zero,      Edge.Dot4,      "┊┊┊┊┊┊┊┊┊");
        setInter(Edge.Zero,      Edge.Dash,      "╎╎╎╎╎╎╎╎╎");
        setInter(Edge.Zero,      Edge.OneHeavy,  "╻╻╻┃┃┃╹╹╹");
        setInter(Edge.Zero,      Edge.Dot3Heavy, "┇┇┇┇┇┇┇┇┇");
        setInter(Edge.Zero,      Edge.Dot4Heavy, "┋┋┋┋┋┋┋┋┋");
        setInter(Edge.Zero,      Edge.DashHeavy, "╏╏╏╏╏╏╏╏╏");
        setInter(Edge.Zero,      Edge.Two,       "║║║║║║║║║");

        setInter(Edge.One,       Edge.None,      "─────────");
        setInter(Edge.One,       Edge.Zero,      "╶─╴╶─╴╶─╴");
        setInter(Edge.One,       Edge.One,       "┌┬┐├┼┤└┴┘");
        setInter(Edge.One,       Edge.Dot,       "─────────");
        setInter(Edge.One,       Edge.Dot3,      "┌┬┐├┼┤└┴┘");
        setInter(Edge.One,       Edge.Dot4,      "┌┬┐├┼┤└┴┘");
        setInter(Edge.One,       Edge.Dash,      "┌┬┐├┼┤└┴┘");
        setInter(Edge.One,       Edge.OneHeavy,  "┎┰┒┠╂┨┖┸┚");
        setInter(Edge.One,       Edge.Dot3Heavy, "┎┰┒┠╂┨┖┸┚");
        setInter(Edge.One,       Edge.Dot4Heavy, "┎┰┒┠╂┨┖┸┚");
        setInter(Edge.One,       Edge.DashHeavy, "┎┰┒┠╂┨┖┸┚");
        setInter(Edge.One,       Edge.Two,       "╓╥╖╟╫╢╙╨╜");
        
        setInter(Edge.Dot,       Edge.None,      "·········");
        setInter(Edge.Dot,       Edge.Zero,      "·········");
        setInter(Edge.Dot,       Edge.One,       "┌┬┐│││└┴┘");
        setInter(Edge.Dot,       Edge.Dot,       "·········");
        setInter(Edge.Dot,       Edge.Dot3,      "┌┬┐┆┆┆└┴┘");
        setInter(Edge.Dot,       Edge.Dot4,      "┌┬┐┊┊┊└┴┘");
        setInter(Edge.Dot,       Edge.Dash,      "┌┬┐╎╎╎└┴┘");
        setInter(Edge.Dot,       Edge.OneHeavy,  "┎┰┒┃┃┃┖┸┚");
        setInter(Edge.Dot,       Edge.Dot3Heavy, "┎┰┒┇┇┇┖┸┚");
        setInter(Edge.Dot,       Edge.Dot4Heavy, "┎┰┒┋┋┋┖┸┚");
        setInter(Edge.Dot,       Edge.DashHeavy, "┎┰┒╏╏╏┖┸┚");
        setInter(Edge.Dot,       Edge.Two,       "╓╥╖║║║╙╨╜");
        
        setInter(Edge.Dot3,      Edge.None,      "┄┄┄┄┄┄┄┄┄");
        setInter(Edge.Dot3,      Edge.Zero,      "┄┄┄┄┄┄┄┄┄");
        setInter(Edge.Dot3,      Edge.One,       "┌┬┐├┼┤└┴┘");
        setInter(Edge.Dot3,      Edge.Dot,       "┄┄┄┄┄┄┄┄┄");
        setInter(Edge.Dot3,      Edge.Dot3,      "┌┬┐├┼┤└┴┘");
        setInter(Edge.Dot3,      Edge.Dot4,      "┌┬┐├┼┤└┴┘");
        setInter(Edge.Dot3,      Edge.Dash,      "┌┬┐├┼┤└┴┘");
        setInter(Edge.Dot3,      Edge.OneHeavy,  "┎┰┒┠╂┨┖┸┚");
        setInter(Edge.Dot3,      Edge.Dot3Heavy, "┎┰┒┠╂┨┖┸┚");
        setInter(Edge.Dot3,      Edge.Dot4Heavy, "┎┰┒┠╂┨┖┸┚");
        setInter(Edge.Dot3,      Edge.DashHeavy, "┎┰┒┠╂┨┖┸┚");
        setInter(Edge.Dot3,      Edge.Two,       "╓╥╖╟╫╢╙╨╜");

        setInter(Edge.Dot4,      Edge.None,      "┈┈┈┈┈┈┈┈┈");
        setInter(Edge.Dot4,      Edge.Zero,      "┈┈┈┈┈┈┈┈┈");
        setInter(Edge.Dot4,      Edge.One,       "┌┬┐├┼┤└┴┘");
        setInter(Edge.Dot4,      Edge.Dot,       "┈┈┈┈┈┈┈┈┈");
        setInter(Edge.Dot4,      Edge.Dot3,      "┌┬┐├┼┤└┴┘");
        setInter(Edge.Dot4,      Edge.Dot4,      "┌┬┐├┼┤└┴┘");
        setInter(Edge.Dot4,      Edge.Dash,      "┌┬┐├┼┤└┴┘");
        setInter(Edge.Dot4,      Edge.OneHeavy,  "┎┰┒┠╂┨┖┸┚");
        setInter(Edge.Dot4,      Edge.Dot3Heavy, "┎┰┒┠╂┨┖┸┚");
        setInter(Edge.Dot4,      Edge.Dot4Heavy, "┎┰┒┠╂┨┖┸┚");
        setInter(Edge.Dot4,      Edge.DashHeavy, "┎┰┒┠╂┨┖┸┚");
        setInter(Edge.Dot4,      Edge.Two,       "╓╥╖╟╫╢╙╨╜");

        setInter(Edge.Dash,      Edge.None,      "╌╌╌╌╌╌╌╌╌");
        setInter(Edge.Dash,      Edge.Zero,      "╌╌╌╌╌╌╌╌╌");
        setInter(Edge.Dash,      Edge.One,       "┌┬┐├┼┤└┴┘");
        setInter(Edge.Dash,      Edge.Dot,       "╌╌╌╌╌╌╌╌╌");
        setInter(Edge.Dash,      Edge.Dot3,      "┌┬┐├┼┤└┴┘");
        setInter(Edge.Dash,      Edge.Dot4,      "┌┬┐├┼┤└┴┘");
        setInter(Edge.Dash,      Edge.Dash,      "┌┬┐├┼┤└┴┘");
        setInter(Edge.Dash,      Edge.OneHeavy,  "┎┰┒┠╂┨┖┸┚");
        setInter(Edge.Dash,      Edge.Dot3Heavy, "┎┰┒┠╂┨┖┸┚");
        setInter(Edge.Dash,      Edge.Dot4Heavy, "┎┰┒┠╂┨┖┸┚");
        setInter(Edge.Dash,      Edge.DashHeavy, "┎┰┒┠╂┨┖┸┚");
        setInter(Edge.Dash,      Edge.Two,       "╓╥╖╟╫╢╙╨╜");

        setInter(Edge.OneHeavy,  Edge.None,      "━━━━━━━━━");
        setInter(Edge.OneHeavy,  Edge.Zero,      "╺━╸╺━╸╺━╸");
        setInter(Edge.OneHeavy,  Edge.One,       "┍┯┑┝┿┥┕┷┙");
        setInter(Edge.OneHeavy,  Edge.Dot,       "━━━━━━━━━");
        setInter(Edge.OneHeavy,  Edge.Dot3,      "┍┯┑┝┿┥┕┷┙");
        setInter(Edge.OneHeavy,  Edge.Dot4,      "┍┯┑┝┿┥┕┷┙");
        setInter(Edge.OneHeavy,  Edge.Dash,      "┍┯┑┝┿┥┕┷┙");
        setInter(Edge.OneHeavy,  Edge.OneHeavy,  "┏┳┓┣╋┫┗┻┛");
        setInter(Edge.OneHeavy,  Edge.Dot3Heavy, "┏┳┓┣╋┫┗┻┛");
        setInter(Edge.OneHeavy,  Edge.Dot4Heavy, "┏┳┓┣╋┫┗┻┛");
        setInter(Edge.OneHeavy,  Edge.DashHeavy, "┏┳┓┣╋┫┗┻┛");
        setInter(Edge.OneHeavy,  Edge.Two,       "╓╥╖╟╫╢╙╨╜");

        setInter(Edge.Dot3Heavy, Edge.None,      "┅┅┅┅┅┅┅┅┅");
        setInter(Edge.Dot3Heavy, Edge.Zero,      "┅┅┅┅┅┅┅┅┅");
        setInter(Edge.Dot3Heavy, Edge.One,       "┍┯┑┝┿┥┕┷┙");
        setInter(Edge.Dot3Heavy, Edge.Dot,       "┅┅┅┅┅┅┅┅┅");
        setInter(Edge.Dot3Heavy, Edge.Dot3,      "┍┯┑┝┿┥┕┷┙");
        setInter(Edge.Dot3Heavy, Edge.Dot4,      "┍┯┑┝┿┥┕┷┙");
        setInter(Edge.Dot3Heavy, Edge.Dash,      "┍┯┑┝┿┥┕┷┙");
        setInter(Edge.Dot3Heavy, Edge.OneHeavy,  "┏┳┓┣╋┫┗┻┛");
        setInter(Edge.Dot3Heavy, Edge.Dot3Heavy, "┏┳┓┣╋┫┗┻┛");
        setInter(Edge.Dot3Heavy, Edge.Dot4Heavy, "┏┳┓┣╋┫┗┻┛");
        setInter(Edge.Dot3Heavy, Edge.DashHeavy, "┏┳┓┣╋┫┗┻┛");
        setInter(Edge.Dot3Heavy, Edge.Two,       "╓╥╖╟╫╢╙╨╜");

        setInter(Edge.Dot4Heavy, Edge.None,      "┉┉┉┉┉┉┉┉┉");
        setInter(Edge.Dot4Heavy, Edge.Zero,      "┉┉┉┉┉┉┉┉┉");
        setInter(Edge.Dot4Heavy, Edge.One,       "┍┯┑┝┿┥┕┷┙");
        setInter(Edge.Dot4Heavy, Edge.Dot,       "┉┉┉┉┉┉┉┉┉");
        setInter(Edge.Dot4Heavy, Edge.Dot3,      "┍┯┑┝┿┥┕┷┙");
        setInter(Edge.Dot4Heavy, Edge.Dot4,      "┍┯┑┝┿┥┕┷┙");
        setInter(Edge.Dot4Heavy, Edge.Dash,      "┍┯┑┝┿┥┕┷┙");
        setInter(Edge.Dot4Heavy, Edge.OneHeavy,  "┏┳┓┣╋┫┗┻┛");
        setInter(Edge.Dot4Heavy, Edge.Dot3Heavy, "┏┳┓┣╋┫┗┻┛");
        setInter(Edge.Dot4Heavy, Edge.Dot4Heavy, "┏┳┓┣╋┫┗┻┛");
        setInter(Edge.Dot4Heavy, Edge.DashHeavy, "┏┳┓┣╋┫┗┻┛");
        setInter(Edge.Dot4Heavy, Edge.Two,       "╓╥╖╟╫╢╙╨╜");
        
        setInter(Edge.DashHeavy, Edge.None,      "╍╍╍╍╍╍╍╍╍");
        setInter(Edge.DashHeavy, Edge.Zero,      "╍╍╍╍╍╍╍╍╍");
        setInter(Edge.DashHeavy, Edge.One,       "┍┯┑┝┿┥┕┷┙");
        setInter(Edge.DashHeavy, Edge.Dot,       "╍╍╍╍╍╍╍╍╍");
        setInter(Edge.DashHeavy, Edge.Dot3,      "┍┯┑┝┿┥┕┷┙");
        setInter(Edge.DashHeavy, Edge.Dot4,      "┍┯┑┝┿┥┕┷┙");
        setInter(Edge.DashHeavy, Edge.Dash,      "┍┯┑┝┿┥┕┷┙");
        setInter(Edge.DashHeavy, Edge.OneHeavy,  "┏┳┓┣╋┫┗┻┛");
        setInter(Edge.DashHeavy, Edge.Dot3Heavy, "┏┳┓┣╋┫┗┻┛");
        setInter(Edge.DashHeavy, Edge.Dot4Heavy, "┏┳┓┣╋┫┗┻┛");
        setInter(Edge.DashHeavy, Edge.DashHeavy, "┏┳┓┣╋┫┗┻┛");
        setInter(Edge.DashHeavy, Edge.Two,       "╓╥╖╟╫╢╙╨╜");

        setInter(Edge.Two,       Edge.None,      "═════════");
        setInter(Edge.Two,       Edge.Zero,      "═════════");
        setInter(Edge.Two,       Edge.One,       "╒╤╕╞╪╡╘╧╛");
        setInter(Edge.Two,       Edge.Dot,       "═════════");
        setInter(Edge.Two,       Edge.Dot3,      "╒╤╕╞╪╡╘╧╛");
        setInter(Edge.Two,       Edge.Dot4,      "╒╤╕╞╪╡╘╧╛");
        setInter(Edge.Two,       Edge.Dash,      "╒╤╕╞╪╡╘╧╛");
        setInter(Edge.Two,       Edge.OneHeavy,  "╒╤╕╞╪╡╘╧╛");
        setInter(Edge.Two,       Edge.Dot3Heavy, "╒╤╕╞╪╡╘╧╛");
        setInter(Edge.Two,       Edge.Dot4Heavy, "╒╤╕╞╪╡╘╧╛");
        setInter(Edge.Two,       Edge.DashHeavy, "╒╤╕╞╪╡╘╧╛");
        setInter(Edge.Two,       Edge.Two,       "╔╦╗╠╬╣╚╩╝");

        if (horizontals.Length != edgeCount)
            throw new StringTableException("Failed to initialize: horizontals string must have "+edgeCount+" characters.");
        if (verticals.Length != edgeCount)
            throw new StringTableException("Failed to initialize: verticals string must have "+edgeCount+" characters.");

        // Make sure that all intersections have been set.
        for (int i = 0; i < edgeCount; ++i)
            for (int j = 0; j < edgeCount; ++j) {
                if (intersections[i, j].Length != 9)
                    throw new StringTableException("Failed to initialize: intersection string at ["+(Edge)i+", "+(Edge)j+"] must have 9 characters.");
            }
    }
    
    /// <summary>Gets the character to use for a horizontal edge.</summary>
    /// <param name="rowEdge">The row edge to get.</param>
    /// <returns>The character for the horizontal edge.</returns>
    static private char edgeHorizontal(Edge rowEdge) => horizontals[(int)rowEdge];
    
    /// <summary>Gets the character to use for a vertical edge.</summary>
    /// <param name="colEdge">The column edge to get.</param>
    /// <returns>The character for the vertical edge.</returns>
    static private char edgeVertical(Edge colEdge) => verticals[(int)colEdge];
    
    /// <summary>Gets the character to use for an intersection.</summary>
    /// <param name="rowEdge">The row edge to get.</param>
    /// <param name="colEdge">The column edge to get.</param>
    /// <param name="rowLoc">The row location of the intersection.</param>
    /// <param name="colLoc">The column location of the intersection.</param>
    /// <returns>The character for the intersection.</returns>
    static private char edgeIntersection(Edge rowEdge, Edge colEdge, Location rowLoc, Location colLoc) =>
        intersections[(int)rowEdge, (int)colEdge][3*(int)rowLoc + (int)colLoc];

    /// <summary>Gets the location for the given row and column range.</summary>
    /// <param name="i">The index of the intersection.</param>
    /// <param name="count">The number of rows or columns for the location.</param>
    /// <returns>The location for the given row or column.</returns>
    static private Location locInRange(int i, int count) =>
        i <= 0 ? Location.Start : i < count ? Location.Middle : Location.End;
        
    /// <summary>Pads the given intersection rune with the given padding based on the column condition.</summary>
    /// <param name="colEdge">The column edge for the intersection.</param>
    /// <param name="colLoc">The column location in the table.</param>
    /// <param name="rune">The intersection text to pad.</param>
    /// <param name="padding">The padding to add to the given intersection if needed.</param>
    /// <returns>The padded intersection string.</returns>
    private static string padEdge(Edge colEdge, Location colLoc, string rune, string padding) =>
       colEdge == Edge.None      ? (colLoc == Location.Middle ? rune : string.Empty) :
       colLoc  == Location.Start ? rune + padding :
       colLoc  == Location.End   ? padding + rune :
       padding + rune + padding;

    #endregion
    #region Data Classes

    /// <summary>
    /// An array which can be assigned at any index including outside of its current range.
    /// The array will be grown to include any index.
    /// This array will also grow the other components of the table.
    /// </summary>
    /// <typeparam name="T">The value to keep in the array.</typeparam>
    sealed public class VariadicArray<T>: IEnumerable<T> {
        private readonly StringTable table;
        private readonly bool isRow;
        private readonly List<T> values;
        private readonly T defaultValue;

        /// <summary>Creates a new table variadic array.</summary>
        /// <remarks>This should only be created by the string table.</remarks>
        /// <param name="table">The table for this array.</param>
        /// <param name="isRow">True if this array is for rows or false for columns.</param>
        /// <param name="defaultValue">The default value to use in the array.</param>
        internal VariadicArray(StringTable table, bool isRow, T defaultValue) {
            this.table  = table;
            this.isRow  = isRow;
            this.values = new();
            this.defaultValue = defaultValue;
        }

        /// <summary>The current length of the array.</summary>
        /// <remarks>This will either be the row or column length.</remarks>
        public int Length {
            get => (this.isRow ? this.table.Rows : this.table.Columns) + 1;
            internal set {
                if (value < this.values.Count)
                    this.values.RemoveRange(value, this.values.Count-value);
            }
        }

        /// <summary>Gets or sets a value in or outside the current array length.</summary>
        /// <param name="index">The index to set the value to.</param>
        /// <returns>The value in the array at the given index.</returns>
        public T this[int index] {
            get => index < this.values.Count ? this.values[index] : this.defaultValue;
            set {
                if (index < this.values.Count) {
                    this.values[index] = value;
                    return;
                }

                while (index > this.values.Count)
                    this.values.Add(this.defaultValue);
                this.values.Add(value);
                int count = this.values.Count;
                if (this.Length < count) {
                    if (this.isRow) this.table.Rows = count;
                    else this.table.Columns = count;
                }
            }
        }

        /// <summary>Gets or sets a value in or outside the current array length.</summary>
        /// <param name="index">The index to set the value to. When indexing from the end, the length is used.</param>
        /// <returns>The value in the array at the given index.</returns>
        public T this[Index index] {
            get => this[index.GetOffset(this.Length)];
            set => this[index.GetOffset(this.Length)] = value;
        }
        
        /// <summary>Sets all the values in the current length to a given value..</summary>
        /// <param name="value">The value to set to all locations.</param>
        public void SetAll(T value) {
            for (int i = this.values.Count-1; i >= 0; --i)
                this.values[i] = value;
            int count = this.Length;
            while (count >= this.values.Count)
                this.values.Add(value);
        }

        /// <summary>Sets a range to the given value.</summary>
        /// <param name="range">The range to set to the given value.</param>
        /// <param name="value">The value to set.</param>
        public void SetRange(Range range, T value) {
            (int offset, int length) = range.GetOffsetAndLength(this.Length);
            for (int i = length, j = length+offset-1; i >= 0; --i, --j)
                this.values[j] = value;
        }

        /// <summary>Sets a range to the given values starting from the given index.</summary>
        /// <param name="index">The index to start setting the values to.</param>
        /// <param name="values">The values to set from the index.</param>
        public void SetRange(int index, IEnumerable<T> values) {
            foreach (T value in values) {
                this[index] = value;
                ++index;
            }
        }
        
        /// <summary>Sets a range to the given values starting from the given index.</summary>
        /// <param name="index">The index to start setting the values to.</param>
        /// <param name="values">The values to set from the index.</param>
        public void SetRange(Index index, IEnumerable<T> values) =>
            this.SetRange(index.GetOffset(this.Length), values);

        /// <summary>Sets a range to the given values starting from the given index.</summary>
        /// <param name="index">The index to start setting the values to.</param>
        /// <param name="values">The values to set from the index.</param>
        public void SetRange(int index, params T[] values) => 
            this.SetRange(index, (IEnumerable<T>)values);
        
        /// <summary>Sets a range to the given values starting from the given index.</summary>
        /// <param name="index">The index to start setting the values to.</param>
        /// <param name="values">The values to set from the index.</param>
        public void SetRange(Index index, params T[] values) =>
            this.SetRange(index, (IEnumerable<T>)values);

        /// <summary>Adds the given value to the end the array.</summary>
        /// <param name="value">The value to add.</param>
        public void Add(T value) => this[this.Length] = value;

        /// <summary>Adds the range of values to the end of the array</summary>
        /// <param name="values">The values to add.</param>
        public void AddRange(IEnumerable<T> values) =>
            this.SetRange(this.Length, values);
        
        /// <summary>Adds the range of values to the end of the array</summary>
        /// <param name="values">The values to add.</param>
        public void AddRange(params T[] values) =>
            this.SetRange(this.Length, values);

        /// <summary>Gets an enumerator for all the array at the current size.</summary>
        /// <returns>The enumerator of all the values.</returns>
        public IEnumerator<T> GetEnumerator() {
            foreach (T value in this.values)
                yield return value;
            int count = this.Length;
            for (int i = this.values.Count; i < count; ++i)
                yield return this.defaultValue;
        }
        
        /// <summary>Gets an enumerator for all the array at the current size.</summary>
        /// <returns>The enumerator of all the values.</returns>
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }

    /// <summary>
    /// A two dimensional string array which can be assigned at any index including outside of its current range.
    /// The array will be grown to include any indices.
    /// This array will also grow the other components of the table.
    /// </summary>
    sealed public class Variadic2DArray {
        private readonly StringTable table;
        private readonly List<List<string>?> values;

        /// <summary>Creates a new variadic 2D string array.</summary>
        /// <param name="table">The table this data belongs to.</param>
        internal Variadic2DArray(StringTable table) {
            this.table = table;
            this.values = new();
        }

        /// <summary>The number of rows in the table.</summary>
        public int Rows {
            get => this.table.Rows;
            internal set {
                foreach (List<string>? col in this.values) {
                    if (col is not null && value < col.Count)
                        col.RemoveRange(value, col.Count-value);
                }
            }
        }

        /// <summary>The number of columns in the table.</summary>
        public int Columns {
            get => this.table.Columns;
            internal set {
                if (value < this.values.Count)
                    this.values.RemoveRange(value, this.values.Count-value);
            }
        }

        /// <summary>Gets or sets the string data for the table.</summary>
        /// <param name="column">The column to set.</param>
        /// <param name="row">The row to set.</param>
        /// <returns>The string at the given column and row.</returns>
        public string this[int column, int row] {
            get {
                if (column < this.values.Count) {
                    List<string>? col = this.values[column];
                    if (col is not null && row < col.Count) return col[row];
                }
                return "";
            }
            set {
                if (this.values.Count <= column) {
                    while (this.values.Count <= column)
                        this.values.Add(null);
                    if (this.Columns < this.values.Count)
                        this.table.Columns = this.values.Count;
                }

                List<string>? col = this.values[column];
                if (col is null) {
                    col = new List<string>(row+1);
                    this.values[column] = col;
                }

                if (col.Count <= row) {
                    while (col.Count <= row)
                        col.Add("");
                    if (this.Rows < col.Count)
                        this.table.Rows = col.Count;
                }

                col[row] = value;
            }
        }

        /// <summary>Gets or sets the string data for the table.</summary>
        /// <param name="column">The column to set.</param>
        /// <param name="row">The row to set.</param>
        /// <returns>The string at the given column and row.</returns>
        public string this[Index column, Index row] {
            get => this[column.GetOffset(this.Columns), row.GetOffset(this.Rows)];
            set => this[column.GetOffset(this.Columns), row.GetOffset(this.Rows)] = value;
        }

        /// <summary>Sets a row to the given values.</summary>
        /// <param name="row">The row to set the values to.</param>
        /// <param name="columns">The column of values to set.</param>
        /// <param name="columnStart">The column index to start setting at.</param>
        public void SetRow(int row, IEnumerable<string> columns, int columnStart = 0) {
            foreach (string value in columns) {
                this[columnStart, row] = value;
                ++columnStart;
            }
        }

        /// <summary>Sets a row to the given values.</summary>
        /// <param name="row">The row to set the values to.</param>
        /// <param name="columns">The column of values to set.</param>
        public void SetRow(int row, params string[] columns) =>
            this.SetRow(row, (IEnumerable<string>)columns);

        /// <summary>Adds a row with the given values.</summary>
        /// <param name="columns">The column of values to set.</param>
        /// <param name="columnStart">The column index to start setting at.</param>
        public void AddRow(IEnumerable<string> columns, int columnStart = 0) =>
            this.SetRow(this.Rows, columns, columnStart);
        
        /// <summary>Adds a row with the given values.</summary>
        /// <param name="columns">The column of values to set.</param>
        public void AddRow(params string[] columns) =>
            this.SetRow(this.Rows, columns);

        /// <summary>Sets a column to the given values.</summary>
        /// <param name="column">The column to set the values to.</param>
        /// <param name="rows">The row of values to set.</param>
        /// <param name="rowStart">The row index to start setting at.</param>
        public void SetColumn(int column, IEnumerable<string> rows, int rowStart = 0) {
            foreach (string value in rows) {
                this[column, rowStart] = value;
                ++rowStart;
            }
        }

        /// <summary>Sets a column to the given values.</summary>
        /// <param name="column">The column to set the values to.</param>
        /// <param name="rows">The row of values to set.</param>
        public void SetColumn(int column, params string[] rows) =>
            this.SetColumn(column, (IEnumerable<string>)rows);

        /// <summary>Adds a column with the given values.</summary>
        /// <param name="rows">The row of values to set.</param>
        /// <param name="rowStart">The row index to start setting at.</param>
        public void AddColumn(IEnumerable<string> rows, int rowStart = 0) =>
            this.SetColumn(this.Columns, rows, rowStart);
        
        /// <summary>Adds a column with the given values.</summary>
        /// <param name="rows">The row of values to set.</param>
        public void AddColumn(params string[] rows) =>
            this.SetColumn(this.Columns, rows);

        /// <summary>Gets an enumerator for a column at the current column count.</summary>
        /// <param name="column">The column to read.</param>
        /// <returns>The enumerator for the column.</returns>
        public IEnumerable<string> Column(int column) {
            int offset = 0;
            if (column < this.values.Count) {
                List<string>? col = this.values[column];
                if (col is not null) {
                    foreach (string value in col)
                        yield return value;
                    offset = col.Count;
                }
            }
            int rowCount = this.Rows;
            for (int i = offset; i < rowCount; ++i)
                yield return "";
        }
        
        /// <summary>Gets an enumerator for a row at the row column count.</summary>
        /// <param name="row">The row to read.</param>
        /// <returns>The enumerator for the row.</returns>
        public IEnumerable<string> Row(int row) {
            foreach (List<string>? col in this.values)
                yield return col is not null && row < col.Count ? col[row] : "";
            int columnCount = this.Columns;
            for (int i = this.values.Count; i < columnCount; ++i)
                yield return "";
        }
    }

    #endregion

    private int rows;
    private int columns;

    /// <summary>Creates a new string table of the given size.</summary>
    /// <param name="rows">The number of rows for the table.</param>
    /// <param name="columns">The number of columns for the table.</param>
    public StringTable(int rows = 0, int columns = 0) {
        this.MaximumColumnWidth = 100;
        this.MaximumRowHeight   = 10;
        this.rows        = rows;
        this.columns     = columns;
        this.Alignments  = new (this, false, Alignment.Left);
        this.RowEdges    = new (this, true,  Edge.None);
        this.ColumnEdges = new (this, false, Edge.None);
        this.Data        = new (this);
    }

    /// <summary>The maximum number of characters wide any column can get.</summary>
    public int MaximumColumnWidth { get; set; }
    
    /// <summary>The maximum number of lines tall any row can get.</summary>
    public int MaximumRowHeight { get; set; }
    
    /// <summary>Indicates if there is no information in the table.</summary>
    public bool Empty => this.Rows == 0 || this.Columns == 0;

    /// <summary>The number of rows in the table.</summary>
    public int Rows {
        get => this.rows;
        set {
            if (value != this.rows) {
                this.rows = value;
                this.Data.Rows = value;
                this.RowEdges.Length = value + 1;
            }
        }
    }

    /// <summary>The number of columns in the table.</summary>
    public int Columns {
        get => this.columns;
        set {
            if (value != this.columns) {
                this.columns = value;
                this.Data.Columns = value;
                this.Alignments.Length  = value + 1;
                this.ColumnEdges.Length = value + 1;
            }
        }
    }

    /// <summary>The horizontal alignments for each column.</summary>
    public VariadicArray<Alignment> Alignments { get; init; }

    /// <summary>The edges between each row.</summary>
    public VariadicArray<Edge> RowEdges { get; init; }

    /// <summary>The edges between each column.</summary>
    public VariadicArray<Edge> ColumnEdges { get; init; }

    /// <summary>The data for each cell in the table.</summary>
    public Variadic2DArray Data { get; init; }

    /// <summary>Sets the outer edges of the table the given edge type.</summary>
    /// <param name="edge">The type of edges to set the boarder.</param>
    public void SetBoarder(Edge edge) {
        if (this.Empty) return;
        this.RowEdges[0]  = edge;
        this.RowEdges[^1] = edge;
        this.ColumnEdges[0]  = edge;
        this.ColumnEdges[^1] = edge;
    }

    /// <summary>Sets all the edges for the rows and columns to the given edge type.</summary>
    /// <param name="edge">The type of the edges to set.</param>
    public void SetAllEdges(Edge edge) {
        if (this.Empty) return;
        this.RowEdges.SetAll(edge);
        this.ColumnEdges.SetAll(edge);
    }

    /// <summary>
    /// Sets the table edges such that there are lines between each row,
    /// lines around the table, and a line around the first row as a header.
    /// </summary>
    public void SetRowHeaderDefaultEdges() {
        if (this.Empty) return;
        this.ColumnEdges.SetAll(Edge.One);
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
        string rune    = edgeIntersection(rowEdge, colEdge, rowLoc, colLoc).ToString();
        string padding = edgeHorizontal(rowEdge).ToString();
        return padEdge(colEdge, colLoc, rune, padding);
    }

    /// <summary>Gets the horizontal edge string between rows.</summary>
    /// <param name="row">The row to get the edge along.</param>
    /// <param name="count">The width of the column.</param>
    /// <returns>The string for above the given rows.</returns>
    private string edgeHorizontal(int row, int count) =>
        new(edgeHorizontal(this.RowEdges[row]), count);

    /// <summary>Gets the vertical edge string beside columns.</summary>
    /// <param name="column">The column to get the left side of.</param>
    /// <returns>The string for the left of the given columns.</returns>
    private string edgeVertical(int column) {
        Edge colEdge = this.ColumnEdges[column];
        Location colLoc = locInRange(column, this.Columns);
        string rune = edgeVertical(colEdge).ToString();
        return padEdge(colEdge, colLoc, rune, defaultPadding);
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
                    column = column.Take(totalMax - 1).Append(ellispe).ToArray();
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
        if (lines is null || lines.Length <= lineNo) return string.Empty.PadRight(width);

        string line = lines[lineNo];
        int length = line.Length;
        if (length > width) {
            length = width;
            line = string.Concat(line.AsSpan(0, width - 3), ellispe);
        }

        switch (this.Alignments[column]) {
            case Alignment.Right:  return line.PadLeft(width);
            case Alignment.Center: return line.PadLeft((width + length) / 2).PadRight(width);
            case Alignment.Left:   break;
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
                result.Append(this.edgeVertical(j));
                result.Append(this.align(columns[j], k, j, widths));
            }
            result.Append(this.edgeVertical(this.Columns));
            result.AppendLine();
        }
    }

    /// <summary>Gets the table as a string.</summary>
    /// <returns>The string for this table.</returns>
    override public string ToString() {
        if (this.Empty) return string.Empty;
        StringBuilder result = new();
        int[] widths = this.maxWidths();
        for (int j = 0; j < this.Rows; j++) {
            this.addHorizontal(result, j, widths);
            this.addRowText(result, j, widths);
        }
        this.addHorizontal(result, this.Rows, widths);
        return result.ToString();
    }

    /// <summary>Escapes a markdown table entry.</summary>
    /// <param name="value">The value to markdown.</param>
    /// <returns>The escaped markdown table entry.</returns>
    static private string escapeMarkdown(string value) =>
        value.Replace("\\", "\\\\").
              Replace("|", "\\|").
              Replace("\r", "").
              Replace("\n", "<br>");

    /// <summary>Gets the table as a markdown table.</summary>
    /// <remarks>
    /// WARNING: This will not escape HTML tags so do not use with unescaped HTML tags.
    /// The edges will not be used and the first row is used as the header.
    /// </remarks>
    /// <returns>The string for this table.</returns>
    public string ToMarkdown() {
        if (this.Empty) return string.Empty;
        StringBuilder result = new();
        for (int i = 0; i < this.Rows; ++i) {
            result.Append("| ");
            result.AppendJoin(" | ", this.Data.Row(i).Select(escapeMarkdown));
            result.AppendLine(" |");
            if (i == 0) {
                for (int j = 0; j < this.Columns; ++j) {
                    result.Append(this.Alignments[j] switch {
                        Alignment.Left   => "|:---",
                        Alignment.Right  => "|---:",
                        Alignment.Center => "|:---:",
                        _                => "|---"
                    });
                }
                result.AppendLine("|");
            }
        }
        return result.ToString();
    }

    #endregion String Methods
}
