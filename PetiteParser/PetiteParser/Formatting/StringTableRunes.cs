using System;
using System.Linq;

namespace PetiteParser.Formatting;

sealed public partial class StringTable {

    /// <summary>An exception used when static initialization of the table fails.</summary>
    sealed internal class StringTableException: Exception {
        public StringTableException(string message) : base(message) { }
    }

    /// <summary>The location of an intersection.</summary>
    private enum Location : int {

        /// <summary>The top or left of the table.</summary>
        Start = 0,

        /// <summary>The center of the table.</summary>
        Middle = 1,

        /// <summary>The bottom or right of the table.</summary>
        End = 2
    }

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
    
        void setInter(Edge rowEdge, Edge colEdge, string input) =>
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
}