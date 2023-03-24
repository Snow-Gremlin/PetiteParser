using Microsoft.VisualStudio.TestTools.UnitTesting;
using PetiteParser.Formatting;
using System;

namespace TestPetiteParser.PetiteParserTests.FormattingTests;

[TestClass]
sealed public class StringTableTests {

    static private void assertTable(StringTable table, params string[] expected) =>
        Assert.AreEqual(expected.JoinLines()+Environment.NewLine, table.ToString());
    
    static private void assertMarkdown(StringTable table, params string[] expected) =>
        Assert.AreEqual(expected.JoinLines()+Environment.NewLine, table.ToMarkdown());

    static private StringTable twoBytwo() {
        StringTable table = new();
        table.Data[0, 0] = "I";
        table.Data[0, 1] = "II";
        table.Data[1, 0] = "III";
        table.Data[1, 1] = "IV";
        return table;
    }

    [TestMethod]
    public void TableEdgesNone() {
        StringTable table = twoBytwo();
        table.ColumnEdges.SetAll(StringTable.Edge.None);
        table.RowEdges.SetAll(StringTable.Edge.None);
        assertTable(table,
            "I   II",
            "III IV");
        table.RowEdges.SetAll(StringTable.Edge.Zero);
        assertTable(table,
            "      ",
            "I   II",
            "      ",
            "III IV",
            "      ");
        table.RowEdges.SetAll(StringTable.Edge.One);
        assertTable(table,
            "──────",
            "I   II",
            "──────",
            "III IV",
            "──────");
        table.RowEdges.SetAll(StringTable.Edge.Dot);
        assertTable(table,
            "······",
            "I   II",
            "······",
            "III IV",
            "······");
        table.RowEdges.SetAll(StringTable.Edge.Dot3);
        assertTable(table,
            "┄┄┄┄┄┄",
            "I   II",
            "┄┄┄┄┄┄",
            "III IV",
            "┄┄┄┄┄┄");
        table.RowEdges.SetAll(StringTable.Edge.Dot4);
        assertTable(table,
            "┈┈┈┈┈┈",
            "I   II",
            "┈┈┈┈┈┈",
            "III IV",
            "┈┈┈┈┈┈");
        table.RowEdges.SetAll(StringTable.Edge.Dash);
        assertTable(table,
            "╌╌╌╌╌╌",
            "I   II",
            "╌╌╌╌╌╌",
            "III IV",
            "╌╌╌╌╌╌");
        table.RowEdges.SetAll(StringTable.Edge.OneHeavy);
        assertTable(table,
            "━━━━━━",
            "I   II",
            "━━━━━━",
            "III IV",
            "━━━━━━");
        table.RowEdges.SetAll(StringTable.Edge.Dot3Heavy);
        assertTable(table,
            "┅┅┅┅┅┅",
            "I   II",
            "┅┅┅┅┅┅",
            "III IV",
            "┅┅┅┅┅┅");
        table.RowEdges.SetAll(StringTable.Edge.Dot4Heavy);
        assertTable(table,
            "┉┉┉┉┉┉",
            "I   II",
            "┉┉┉┉┉┉",
            "III IV",
            "┉┉┉┉┉┉");
        table.RowEdges.SetAll(StringTable.Edge.DashHeavy);
        assertTable(table,
            "╍╍╍╍╍╍",
            "I   II",
            "╍╍╍╍╍╍",
            "III IV",
            "╍╍╍╍╍╍");
        table.RowEdges.SetAll(StringTable.Edge.Two);
        assertTable(table,
            "══════",
            "I   II",
            "══════",
            "III IV",
            "══════");
    }

    [TestMethod]
    public void TableEdgesZero() {
        StringTable table = twoBytwo();
        table.ColumnEdges.SetAll(StringTable.Edge.Zero);
        table.RowEdges.SetAll(StringTable.Edge.None);
        assertTable(table,
            "  I     II  ",
            "  III   IV  ");
        table.RowEdges.SetAll(StringTable.Edge.Zero);
        assertTable(table,
            "            ",
            "  I     II  ",
            "            ",
            "  III   IV  ",
            "            ");
        table.RowEdges.SetAll(StringTable.Edge.One);
        assertTable(table,
            "╶──────────╴",
            "  I     II  ",
            "╶──────────╴",
            "  III   IV  ",
            "╶──────────╴");
        table.RowEdges.SetAll(StringTable.Edge.Dot);
        assertTable(table,
            "············",
            "  I     II  ",
            "············",
            "  III   IV  ",
            "············");
        table.RowEdges.SetAll(StringTable.Edge.Dot3);
        assertTable(table,
            "┄┄┄┄┄┄┄┄┄┄┄┄",
            "  I     II  ",
            "┄┄┄┄┄┄┄┄┄┄┄┄",
            "  III   IV  ",
            "┄┄┄┄┄┄┄┄┄┄┄┄");
        table.RowEdges.SetAll(StringTable.Edge.Dot4);
        assertTable(table,
            "┈┈┈┈┈┈┈┈┈┈┈┈",
            "  I     II  ",
            "┈┈┈┈┈┈┈┈┈┈┈┈",
            "  III   IV  ",
            "┈┈┈┈┈┈┈┈┈┈┈┈");
        table.RowEdges.SetAll(StringTable.Edge.Dash);
        assertTable(table,
            "╌╌╌╌╌╌╌╌╌╌╌╌",
            "  I     II  ",
            "╌╌╌╌╌╌╌╌╌╌╌╌",
            "  III   IV  ",
            "╌╌╌╌╌╌╌╌╌╌╌╌");
        table.RowEdges.SetAll(StringTable.Edge.OneHeavy);
        assertTable(table,
            "╺━━━━━━━━━━╸",
            "  I     II  ",
            "╺━━━━━━━━━━╸",
            "  III   IV  ",
            "╺━━━━━━━━━━╸");
        table.RowEdges.SetAll(StringTable.Edge.Dot3Heavy);
        assertTable(table,
            "┅┅┅┅┅┅┅┅┅┅┅┅",
            "  I     II  ",
            "┅┅┅┅┅┅┅┅┅┅┅┅",
            "  III   IV  ",
            "┅┅┅┅┅┅┅┅┅┅┅┅");
        table.RowEdges.SetAll(StringTable.Edge.Dot4Heavy);
        assertTable(table,
            "┉┉┉┉┉┉┉┉┉┉┉┉",
            "  I     II  ",
            "┉┉┉┉┉┉┉┉┉┉┉┉",
            "  III   IV  ",
            "┉┉┉┉┉┉┉┉┉┉┉┉");
        table.RowEdges.SetAll(StringTable.Edge.DashHeavy);
        assertTable(table,
            "╍╍╍╍╍╍╍╍╍╍╍╍",
            "  I     II  ",
            "╍╍╍╍╍╍╍╍╍╍╍╍",
            "  III   IV  ",
            "╍╍╍╍╍╍╍╍╍╍╍╍");
        table.RowEdges.SetAll(StringTable.Edge.Two);
        assertTable(table,
            "════════════",
            "  I     II  ",
            "════════════",
            "  III   IV  ",
            "════════════");
    }

    [TestMethod]
    public void TableEdgesOne() {
        StringTable table = twoBytwo();
        table.ColumnEdges.SetAll(StringTable.Edge.One);
        table.RowEdges.SetAll(StringTable.Edge.None);
        assertTable(table,
            "│ I   │ II │",
            "│ III │ IV │");
        table.RowEdges.SetAll(StringTable.Edge.Zero);
        assertTable(table,
            "╷     ╷    ╷",
            "│ I   │ II │",
            "│     │    │",
            "│ III │ IV │",
            "╵     ╵    ╵");
        table.RowEdges.SetAll(StringTable.Edge.One);
        assertTable(table,
            "┌─────┬────┐",
            "│ I   │ II │",
            "├─────┼────┤",
            "│ III │ IV │",
            "└─────┴────┘");
        table.RowEdges.SetAll(StringTable.Edge.Dot);
        assertTable(table,
            "┌·····┬····┐",
            "│ I   │ II │",
            "│·····│····│",
            "│ III │ IV │",
            "└·····┴····┘");
        table.RowEdges.SetAll(StringTable.Edge.Dot3);
        assertTable(table,
            "┌┄┄┄┄┄┬┄┄┄┄┐",
            "│ I   │ II │",
            "├┄┄┄┄┄┼┄┄┄┄┤",
            "│ III │ IV │",
            "└┄┄┄┄┄┴┄┄┄┄┘");
        table.RowEdges.SetAll(StringTable.Edge.Dot4);
        assertTable(table,
            "┌┈┈┈┈┈┬┈┈┈┈┐",
            "│ I   │ II │",
            "├┈┈┈┈┈┼┈┈┈┈┤",
            "│ III │ IV │",
            "└┈┈┈┈┈┴┈┈┈┈┘");
        table.RowEdges.SetAll(StringTable.Edge.Dash);
        assertTable(table,
            "┌╌╌╌╌╌┬╌╌╌╌┐",
            "│ I   │ II │",
            "├╌╌╌╌╌┼╌╌╌╌┤",
            "│ III │ IV │",
            "└╌╌╌╌╌┴╌╌╌╌┘");
        table.RowEdges.SetAll(StringTable.Edge.OneHeavy);
        assertTable(table,
            "┍━━━━━┯━━━━┑",
            "│ I   │ II │",
            "┝━━━━━┿━━━━┥",
            "│ III │ IV │",
            "┕━━━━━┷━━━━┙");
        table.RowEdges.SetAll(StringTable.Edge.Dot3Heavy);
        assertTable(table,
            "┍┅┅┅┅┅┯┅┅┅┅┑",
            "│ I   │ II │",
            "┝┅┅┅┅┅┿┅┅┅┅┥",
            "│ III │ IV │",
            "┕┅┅┅┅┅┷┅┅┅┅┙");
        table.RowEdges.SetAll(StringTable.Edge.Dot4Heavy);
        assertTable(table,
            "┍┉┉┉┉┉┯┉┉┉┉┑",
            "│ I   │ II │",
            "┝┉┉┉┉┉┿┉┉┉┉┥",
            "│ III │ IV │",
            "┕┉┉┉┉┉┷┉┉┉┉┙");
        table.RowEdges.SetAll(StringTable.Edge.DashHeavy);
        assertTable(table,
            "┍╍╍╍╍╍┯╍╍╍╍┑",
            "│ I   │ II │",
            "┝╍╍╍╍╍┿╍╍╍╍┥",
            "│ III │ IV │",
            "┕╍╍╍╍╍┷╍╍╍╍┙");
        table.RowEdges.SetAll(StringTable.Edge.Two);
        assertTable(table,
            "╒═════╤════╕",
            "│ I   │ II │",
            "╞═════╪════╡",
            "│ III │ IV │",
            "╘═════╧════╛");
    }

    [TestMethod]
    public void TableEdgeDot() {
        StringTable table = twoBytwo();
        table.ColumnEdges.SetAll(StringTable.Edge.Dot);
        table.RowEdges.SetAll(StringTable.Edge.None);
        assertTable(table,
            "· I   · II ·",
            "· III · IV ·");
        table.RowEdges.SetAll(StringTable.Edge.Zero);
        assertTable(table,
            "·     ·    ·",
            "· I   · II ·",
            "·     ·    ·",
            "· III · IV ·",
            "·     ·    ·");
        table.RowEdges.SetAll(StringTable.Edge.One);
        assertTable(table,
            "────────────",
            "· I   · II ·",
            "────────────",
            "· III · IV ·",
            "────────────");
        table.RowEdges.SetAll(StringTable.Edge.Dot);
        assertTable(table,
            "············",
            "· I   · II ·",
            "············",
            "· III · IV ·",
            "············");
        table.RowEdges.SetAll(StringTable.Edge.Dot3);
        assertTable(table,
            "┄┄┄┄┄┄┄┄┄┄┄┄",
            "· I   · II ·",
            "┄┄┄┄┄┄┄┄┄┄┄┄",
            "· III · IV ·",
            "┄┄┄┄┄┄┄┄┄┄┄┄");
        table.RowEdges.SetAll(StringTable.Edge.Dot4);
        assertTable(table,
            "┈┈┈┈┈┈┈┈┈┈┈┈",
            "· I   · II ·",
            "┈┈┈┈┈┈┈┈┈┈┈┈",
            "· III · IV ·",
            "┈┈┈┈┈┈┈┈┈┈┈┈");
        table.RowEdges.SetAll(StringTable.Edge.Dash);
        assertTable(table,
            "╌╌╌╌╌╌╌╌╌╌╌╌",
            "· I   · II ·",
            "╌╌╌╌╌╌╌╌╌╌╌╌",
            "· III · IV ·",
            "╌╌╌╌╌╌╌╌╌╌╌╌");
        table.RowEdges.SetAll(StringTable.Edge.OneHeavy);
        assertTable(table,
            "━━━━━━━━━━━━",
            "· I   · II ·",
            "━━━━━━━━━━━━",
            "· III · IV ·",
            "━━━━━━━━━━━━");
        table.RowEdges.SetAll(StringTable.Edge.Dot3Heavy);
        assertTable(table,
            "┅┅┅┅┅┅┅┅┅┅┅┅",
            "· I   · II ·",
            "┅┅┅┅┅┅┅┅┅┅┅┅",
            "· III · IV ·",
            "┅┅┅┅┅┅┅┅┅┅┅┅");
        table.RowEdges.SetAll(StringTable.Edge.Dot4Heavy);
        assertTable(table,
            "┉┉┉┉┉┉┉┉┉┉┉┉",
            "· I   · II ·",
            "┉┉┉┉┉┉┉┉┉┉┉┉",
            "· III · IV ·",
            "┉┉┉┉┉┉┉┉┉┉┉┉");
        table.RowEdges.SetAll(StringTable.Edge.DashHeavy);
        assertTable(table,
            "╍╍╍╍╍╍╍╍╍╍╍╍",
            "· I   · II ·",
            "╍╍╍╍╍╍╍╍╍╍╍╍",
            "· III · IV ·",
            "╍╍╍╍╍╍╍╍╍╍╍╍");
        table.RowEdges.SetAll(StringTable.Edge.Two);
        assertTable(table,
            "════════════",
            "· I   · II ·",
            "════════════",
            "· III · IV ·",
            "════════════");
    }
    
    [TestMethod]
    public void TableEdgeDot3() {
        StringTable table = twoBytwo();
        table.ColumnEdges.SetAll(StringTable.Edge.Dot3);
        table.RowEdges.SetAll(StringTable.Edge.None);
        assertTable(table,
            "┆ I   ┆ II ┆",
            "┆ III ┆ IV ┆");
        table.RowEdges.SetAll(StringTable.Edge.Zero);
        assertTable(table,
            "┆     ┆    ┆",
            "┆ I   ┆ II ┆",
            "┆     ┆    ┆",
            "┆ III ┆ IV ┆",
            "┆     ┆    ┆");
        table.RowEdges.SetAll(StringTable.Edge.One);
        assertTable(table,
            "┌─────┬────┐",
            "┆ I   ┆ II ┆",
            "├─────┼────┤",
            "┆ III ┆ IV ┆",
            "└─────┴────┘");
        table.RowEdges.SetAll(StringTable.Edge.Dot);
        assertTable(table,
            "┌·····┬····┐",
            "┆ I   ┆ II ┆",
            "┆·····┆····┆",
            "┆ III ┆ IV ┆",
            "└·····┴····┘");
        table.RowEdges.SetAll(StringTable.Edge.Dot3);
        assertTable(table,
            "┌┄┄┄┄┄┬┄┄┄┄┐",
            "┆ I   ┆ II ┆",
            "├┄┄┄┄┄┼┄┄┄┄┤",
            "┆ III ┆ IV ┆",
            "└┄┄┄┄┄┴┄┄┄┄┘");
        table.RowEdges.SetAll(StringTable.Edge.Dot4);
        assertTable(table,
            "┌┈┈┈┈┈┬┈┈┈┈┐",
            "┆ I   ┆ II ┆",
            "├┈┈┈┈┈┼┈┈┈┈┤",
            "┆ III ┆ IV ┆",
            "└┈┈┈┈┈┴┈┈┈┈┘");
        table.RowEdges.SetAll(StringTable.Edge.Dash);
        assertTable(table,
            "┌╌╌╌╌╌┬╌╌╌╌┐",
            "┆ I   ┆ II ┆",
            "├╌╌╌╌╌┼╌╌╌╌┤",
            "┆ III ┆ IV ┆",
            "└╌╌╌╌╌┴╌╌╌╌┘");
        table.RowEdges.SetAll(StringTable.Edge.OneHeavy);
        assertTable(table,
            "┍━━━━━┯━━━━┑",
            "┆ I   ┆ II ┆",
            "┝━━━━━┿━━━━┥",
            "┆ III ┆ IV ┆",
            "┕━━━━━┷━━━━┙");
        table.RowEdges.SetAll(StringTable.Edge.Dot3Heavy);
        assertTable(table,
            "┍┅┅┅┅┅┯┅┅┅┅┑",
            "┆ I   ┆ II ┆",
            "┝┅┅┅┅┅┿┅┅┅┅┥",
            "┆ III ┆ IV ┆",
            "┕┅┅┅┅┅┷┅┅┅┅┙");
        table.RowEdges.SetAll(StringTable.Edge.Dot4Heavy);
        assertTable(table,
            "┍┉┉┉┉┉┯┉┉┉┉┑",
            "┆ I   ┆ II ┆",
            "┝┉┉┉┉┉┿┉┉┉┉┥",
            "┆ III ┆ IV ┆",
            "┕┉┉┉┉┉┷┉┉┉┉┙");
        table.RowEdges.SetAll(StringTable.Edge.DashHeavy);
        assertTable(table,
            "┍╍╍╍╍╍┯╍╍╍╍┑",
            "┆ I   ┆ II ┆",
            "┝╍╍╍╍╍┿╍╍╍╍┥",
            "┆ III ┆ IV ┆",
            "┕╍╍╍╍╍┷╍╍╍╍┙");
        table.RowEdges.SetAll(StringTable.Edge.Two);
        assertTable(table,
            "╒═════╤════╕",
            "┆ I   ┆ II ┆",
            "╞═════╪════╡",
            "┆ III ┆ IV ┆",
            "╘═════╧════╛");
    }

    [TestMethod]
    public void TableEdgeDot4() {
        StringTable table = twoBytwo();
        table.ColumnEdges.SetAll(StringTable.Edge.Dot4);
        table.RowEdges.SetAll(StringTable.Edge.None);
        assertTable(table,
            "┊ I   ┊ II ┊",
            "┊ III ┊ IV ┊");
        table.RowEdges.SetAll(StringTable.Edge.Zero);
        assertTable(table,
            "┊     ┊    ┊",
            "┊ I   ┊ II ┊",
            "┊     ┊    ┊",
            "┊ III ┊ IV ┊",
            "┊     ┊    ┊");
        table.RowEdges.SetAll(StringTable.Edge.One);
        assertTable(table,
            "┌─────┬────┐",
            "┊ I   ┊ II ┊",
            "├─────┼────┤",
            "┊ III ┊ IV ┊",
            "└─────┴────┘");
        table.RowEdges.SetAll(StringTable.Edge.Dot);
        assertTable(table,
            "┌·····┬····┐",
            "┊ I   ┊ II ┊",
            "┊·····┊····┊",
            "┊ III ┊ IV ┊",
            "└·····┴····┘");
        table.RowEdges.SetAll(StringTable.Edge.Dot3);
        assertTable(table,
            "┌┄┄┄┄┄┬┄┄┄┄┐",
            "┊ I   ┊ II ┊",
            "├┄┄┄┄┄┼┄┄┄┄┤",
            "┊ III ┊ IV ┊",
            "└┄┄┄┄┄┴┄┄┄┄┘");
        table.RowEdges.SetAll(StringTable.Edge.Dot4);
        assertTable(table,
            "┌┈┈┈┈┈┬┈┈┈┈┐",
            "┊ I   ┊ II ┊",
            "├┈┈┈┈┈┼┈┈┈┈┤",
            "┊ III ┊ IV ┊",
            "└┈┈┈┈┈┴┈┈┈┈┘");
        table.RowEdges.SetAll(StringTable.Edge.Dash);
        assertTable(table,
            "┌╌╌╌╌╌┬╌╌╌╌┐",
            "┊ I   ┊ II ┊",
            "├╌╌╌╌╌┼╌╌╌╌┤",
            "┊ III ┊ IV ┊",
            "└╌╌╌╌╌┴╌╌╌╌┘");
        table.RowEdges.SetAll(StringTable.Edge.OneHeavy);
        assertTable(table,
            "┍━━━━━┯━━━━┑",
            "┊ I   ┊ II ┊",
            "┝━━━━━┿━━━━┥",
            "┊ III ┊ IV ┊",
            "┕━━━━━┷━━━━┙");
        table.RowEdges.SetAll(StringTable.Edge.Dot3Heavy);
        assertTable(table,
            "┍┅┅┅┅┅┯┅┅┅┅┑",
            "┊ I   ┊ II ┊",
            "┝┅┅┅┅┅┿┅┅┅┅┥",
            "┊ III ┊ IV ┊",
            "┕┅┅┅┅┅┷┅┅┅┅┙");
        table.RowEdges.SetAll(StringTable.Edge.Dot4Heavy);
        assertTable(table,
            "┍┉┉┉┉┉┯┉┉┉┉┑",
            "┊ I   ┊ II ┊",
            "┝┉┉┉┉┉┿┉┉┉┉┥",
            "┊ III ┊ IV ┊",
            "┕┉┉┉┉┉┷┉┉┉┉┙");
        table.RowEdges.SetAll(StringTable.Edge.DashHeavy);
        assertTable(table,
            "┍╍╍╍╍╍┯╍╍╍╍┑",
            "┊ I   ┊ II ┊",
            "┝╍╍╍╍╍┿╍╍╍╍┥",
            "┊ III ┊ IV ┊",
            "┕╍╍╍╍╍┷╍╍╍╍┙");
        table.RowEdges.SetAll(StringTable.Edge.Two);
        assertTable(table,
            "╒═════╤════╕",
            "┊ I   ┊ II ┊",
            "╞═════╪════╡",
            "┊ III ┊ IV ┊",
            "╘═════╧════╛");
    }
    
    [TestMethod]
    public void TableEdgeDash() {
        StringTable table = twoBytwo();
        table.ColumnEdges.SetAll(StringTable.Edge.Dash);
        table.RowEdges.SetAll(StringTable.Edge.None);
        assertTable(table,
            "╎ I   ╎ II ╎",
            "╎ III ╎ IV ╎");
        table.RowEdges.SetAll(StringTable.Edge.Zero);
        assertTable(table,
            "╎     ╎    ╎",
            "╎ I   ╎ II ╎",
            "╎     ╎    ╎",
            "╎ III ╎ IV ╎",
            "╎     ╎    ╎");
        table.RowEdges.SetAll(StringTable.Edge.One);
        assertTable(table,
            "┌─────┬────┐",
            "╎ I   ╎ II ╎",
            "├─────┼────┤",
            "╎ III ╎ IV ╎",
            "└─────┴────┘");
        table.RowEdges.SetAll(StringTable.Edge.Dot);
        assertTable(table,
            "┌·····┬····┐",
            "╎ I   ╎ II ╎",
            "╎·····╎····╎",
            "╎ III ╎ IV ╎",
            "└·····┴····┘");
        table.RowEdges.SetAll(StringTable.Edge.Dot3);
        assertTable(table,
            "┌┄┄┄┄┄┬┄┄┄┄┐",
            "╎ I   ╎ II ╎",
            "├┄┄┄┄┄┼┄┄┄┄┤",
            "╎ III ╎ IV ╎",
            "└┄┄┄┄┄┴┄┄┄┄┘");
        table.RowEdges.SetAll(StringTable.Edge.Dot4);
        assertTable(table,
            "┌┈┈┈┈┈┬┈┈┈┈┐",
            "╎ I   ╎ II ╎",
            "├┈┈┈┈┈┼┈┈┈┈┤",
            "╎ III ╎ IV ╎",
            "└┈┈┈┈┈┴┈┈┈┈┘");
        table.RowEdges.SetAll(StringTable.Edge.Dash);
        assertTable(table,
            "┌╌╌╌╌╌┬╌╌╌╌┐",
            "╎ I   ╎ II ╎",
            "├╌╌╌╌╌┼╌╌╌╌┤",
            "╎ III ╎ IV ╎",
            "└╌╌╌╌╌┴╌╌╌╌┘");
        table.RowEdges.SetAll(StringTable.Edge.OneHeavy);
        assertTable(table,
            "┍━━━━━┯━━━━┑",
            "╎ I   ╎ II ╎",
            "┝━━━━━┿━━━━┥",
            "╎ III ╎ IV ╎",
            "┕━━━━━┷━━━━┙");
        table.RowEdges.SetAll(StringTable.Edge.Dot3Heavy);
        assertTable(table,
            "┍┅┅┅┅┅┯┅┅┅┅┑",
            "╎ I   ╎ II ╎",
            "┝┅┅┅┅┅┿┅┅┅┅┥",
            "╎ III ╎ IV ╎",
            "┕┅┅┅┅┅┷┅┅┅┅┙");
        table.RowEdges.SetAll(StringTable.Edge.Dot4Heavy);
        assertTable(table,
            "┍┉┉┉┉┉┯┉┉┉┉┑",
            "╎ I   ╎ II ╎",
            "┝┉┉┉┉┉┿┉┉┉┉┥",
            "╎ III ╎ IV ╎",
            "┕┉┉┉┉┉┷┉┉┉┉┙");
        table.RowEdges.SetAll(StringTable.Edge.DashHeavy);
        assertTable(table,
            "┍╍╍╍╍╍┯╍╍╍╍┑",
            "╎ I   ╎ II ╎",
            "┝╍╍╍╍╍┿╍╍╍╍┥",
            "╎ III ╎ IV ╎",
            "┕╍╍╍╍╍┷╍╍╍╍┙");
        table.RowEdges.SetAll(StringTable.Edge.Two);
        assertTable(table,
            "╒═════╤════╕",
            "╎ I   ╎ II ╎",
            "╞═════╪════╡",
            "╎ III ╎ IV ╎",
            "╘═════╧════╛");
    }
    
    [TestMethod]
    public void TableEdgeOneHeavy() {
        StringTable table = twoBytwo();
        table.ColumnEdges.SetAll(StringTable.Edge.OneHeavy);
        table.RowEdges.SetAll(StringTable.Edge.None);
        assertTable(table,
            "┃ I   ┃ II ┃",
            "┃ III ┃ IV ┃");
        table.RowEdges.SetAll(StringTable.Edge.Zero);
        assertTable(table,
            "╻     ╻    ╻",
            "┃ I   ┃ II ┃",
            "┃     ┃    ┃",
            "┃ III ┃ IV ┃",
            "╹     ╹    ╹");
        table.RowEdges.SetAll(StringTable.Edge.One);
        assertTable(table,
            "┎─────┰────┒",
            "┃ I   ┃ II ┃",
            "┠─────╂────┨",
            "┃ III ┃ IV ┃",
            "┖─────┸────┚");
        table.RowEdges.SetAll(StringTable.Edge.Dot);
        assertTable(table,
            "┎·····┰····┒",
            "┃ I   ┃ II ┃",
            "┃·····┃····┃",
            "┃ III ┃ IV ┃",
            "┖·····┸····┚");
        table.RowEdges.SetAll(StringTable.Edge.Dot3);
        assertTable(table,
            "┎┄┄┄┄┄┰┄┄┄┄┒",
            "┃ I   ┃ II ┃",
            "┠┄┄┄┄┄╂┄┄┄┄┨",
            "┃ III ┃ IV ┃",
            "┖┄┄┄┄┄┸┄┄┄┄┚");
        table.RowEdges.SetAll(StringTable.Edge.Dot4);
        assertTable(table,
            "┎┈┈┈┈┈┰┈┈┈┈┒",
            "┃ I   ┃ II ┃",
            "┠┈┈┈┈┈╂┈┈┈┈┨",
            "┃ III ┃ IV ┃",
            "┖┈┈┈┈┈┸┈┈┈┈┚");
        table.RowEdges.SetAll(StringTable.Edge.Dash);
        assertTable(table,
            "┎╌╌╌╌╌┰╌╌╌╌┒",
            "┃ I   ┃ II ┃",
            "┠╌╌╌╌╌╂╌╌╌╌┨",
            "┃ III ┃ IV ┃",
            "┖╌╌╌╌╌┸╌╌╌╌┚");
        table.RowEdges.SetAll(StringTable.Edge.OneHeavy);
        assertTable(table,
            "┏━━━━━┳━━━━┓",
            "┃ I   ┃ II ┃",
            "┣━━━━━╋━━━━┫",
            "┃ III ┃ IV ┃",
            "┗━━━━━┻━━━━┛");
        table.RowEdges.SetAll(StringTable.Edge.Dot3Heavy);
        assertTable(table,
            "┏┅┅┅┅┅┳┅┅┅┅┓",
            "┃ I   ┃ II ┃",
            "┣┅┅┅┅┅╋┅┅┅┅┫",
            "┃ III ┃ IV ┃",
            "┗┅┅┅┅┅┻┅┅┅┅┛");
        table.RowEdges.SetAll(StringTable.Edge.Dot4Heavy);
        assertTable(table,
            "┏┉┉┉┉┉┳┉┉┉┉┓",
            "┃ I   ┃ II ┃",
            "┣┉┉┉┉┉╋┉┉┉┉┫",
            "┃ III ┃ IV ┃",
            "┗┉┉┉┉┉┻┉┉┉┉┛");
        table.RowEdges.SetAll(StringTable.Edge.DashHeavy);
        assertTable(table,
            "┏╍╍╍╍╍┳╍╍╍╍┓",
            "┃ I   ┃ II ┃",
            "┣╍╍╍╍╍╋╍╍╍╍┫",
            "┃ III ┃ IV ┃",
            "┗╍╍╍╍╍┻╍╍╍╍┛");
        table.RowEdges.SetAll(StringTable.Edge.Two);
        assertTable(table,
            "╒═════╤════╕",
            "┃ I   ┃ II ┃",
            "╞═════╪════╡",
            "┃ III ┃ IV ┃",
            "╘═════╧════╛");
    }
    
    [TestMethod]
    public void TableEdgeDot3Heavy() {
        StringTable table = twoBytwo();
        table.ColumnEdges.SetAll(StringTable.Edge.Dot3Heavy);
        table.RowEdges.SetAll(StringTable.Edge.None);
        assertTable(table,
            "┇ I   ┇ II ┇",
            "┇ III ┇ IV ┇");
        table.RowEdges.SetAll(StringTable.Edge.Zero);
        assertTable(table,
            "┇     ┇    ┇",
            "┇ I   ┇ II ┇",
            "┇     ┇    ┇",
            "┇ III ┇ IV ┇",
            "┇     ┇    ┇");
        table.RowEdges.SetAll(StringTable.Edge.One);
        assertTable(table,
            "┎─────┰────┒",
            "┇ I   ┇ II ┇",
            "┠─────╂────┨",
            "┇ III ┇ IV ┇",
            "┖─────┸────┚");
        table.RowEdges.SetAll(StringTable.Edge.Dot);
        assertTable(table,
            "┎·····┰····┒",
            "┇ I   ┇ II ┇",
            "┇·····┇····┇",
            "┇ III ┇ IV ┇",
            "┖·····┸····┚");
        table.RowEdges.SetAll(StringTable.Edge.Dot3);
        assertTable(table,
            "┎┄┄┄┄┄┰┄┄┄┄┒",
            "┇ I   ┇ II ┇",
            "┠┄┄┄┄┄╂┄┄┄┄┨",
            "┇ III ┇ IV ┇",
            "┖┄┄┄┄┄┸┄┄┄┄┚");
        table.RowEdges.SetAll(StringTable.Edge.Dot4);
        assertTable(table,
            "┎┈┈┈┈┈┰┈┈┈┈┒",
            "┇ I   ┇ II ┇",
            "┠┈┈┈┈┈╂┈┈┈┈┨",
            "┇ III ┇ IV ┇",
            "┖┈┈┈┈┈┸┈┈┈┈┚");
        table.RowEdges.SetAll(StringTable.Edge.Dash);
        assertTable(table,
            "┎╌╌╌╌╌┰╌╌╌╌┒",
            "┇ I   ┇ II ┇",
            "┠╌╌╌╌╌╂╌╌╌╌┨",
            "┇ III ┇ IV ┇",
            "┖╌╌╌╌╌┸╌╌╌╌┚");
        table.RowEdges.SetAll(StringTable.Edge.OneHeavy);
        assertTable(table,
            "┏━━━━━┳━━━━┓",
            "┇ I   ┇ II ┇",
            "┣━━━━━╋━━━━┫",
            "┇ III ┇ IV ┇",
            "┗━━━━━┻━━━━┛");
        table.RowEdges.SetAll(StringTable.Edge.Dot3Heavy);
        assertTable(table,
            "┏┅┅┅┅┅┳┅┅┅┅┓",
            "┇ I   ┇ II ┇",
            "┣┅┅┅┅┅╋┅┅┅┅┫",
            "┇ III ┇ IV ┇",
            "┗┅┅┅┅┅┻┅┅┅┅┛");
        table.RowEdges.SetAll(StringTable.Edge.Dot4Heavy);
        assertTable(table,
            "┏┉┉┉┉┉┳┉┉┉┉┓",
            "┇ I   ┇ II ┇",
            "┣┉┉┉┉┉╋┉┉┉┉┫",
            "┇ III ┇ IV ┇",
            "┗┉┉┉┉┉┻┉┉┉┉┛");
        table.RowEdges.SetAll(StringTable.Edge.DashHeavy);
        assertTable(table,
            "┏╍╍╍╍╍┳╍╍╍╍┓",
            "┇ I   ┇ II ┇",
            "┣╍╍╍╍╍╋╍╍╍╍┫",
            "┇ III ┇ IV ┇",
            "┗╍╍╍╍╍┻╍╍╍╍┛");
        table.RowEdges.SetAll(StringTable.Edge.Two);
        assertTable(table,
            "╒═════╤════╕",
            "┇ I   ┇ II ┇",
            "╞═════╪════╡",
            "┇ III ┇ IV ┇",
            "╘═════╧════╛");
    }
    
    [TestMethod]
    public void TableEdgeDot4Heavy() {
        StringTable table = twoBytwo();
        table.ColumnEdges.SetAll(StringTable.Edge.Dot4Heavy);
        table.RowEdges.SetAll(StringTable.Edge.None);
        assertTable(table,
            "┋ I   ┋ II ┋",
            "┋ III ┋ IV ┋");
        table.RowEdges.SetAll(StringTable.Edge.Zero);
        assertTable(table,
            "┋     ┋    ┋",
            "┋ I   ┋ II ┋",
            "┋     ┋    ┋",
            "┋ III ┋ IV ┋",
            "┋     ┋    ┋");
        table.RowEdges.SetAll(StringTable.Edge.One);
        assertTable(table,
            "┎─────┰────┒",
            "┋ I   ┋ II ┋",
            "┠─────╂────┨",
            "┋ III ┋ IV ┋",
            "┖─────┸────┚");
        table.RowEdges.SetAll(StringTable.Edge.Dot);
        assertTable(table,
            "┎·····┰····┒",
            "┋ I   ┋ II ┋",
            "┋·····┋····┋",
            "┋ III ┋ IV ┋",
            "┖·····┸····┚");
        table.RowEdges.SetAll(StringTable.Edge.Dot3);
        assertTable(table,
            "┎┄┄┄┄┄┰┄┄┄┄┒",
            "┋ I   ┋ II ┋",
            "┠┄┄┄┄┄╂┄┄┄┄┨",
            "┋ III ┋ IV ┋",
            "┖┄┄┄┄┄┸┄┄┄┄┚");
        table.RowEdges.SetAll(StringTable.Edge.Dot4);
        assertTable(table,
            "┎┈┈┈┈┈┰┈┈┈┈┒",
            "┋ I   ┋ II ┋",
            "┠┈┈┈┈┈╂┈┈┈┈┨",
            "┋ III ┋ IV ┋",
            "┖┈┈┈┈┈┸┈┈┈┈┚");
        table.RowEdges.SetAll(StringTable.Edge.Dash);
        assertTable(table,
            "┎╌╌╌╌╌┰╌╌╌╌┒",
            "┋ I   ┋ II ┋",
            "┠╌╌╌╌╌╂╌╌╌╌┨",
            "┋ III ┋ IV ┋",
            "┖╌╌╌╌╌┸╌╌╌╌┚");
        table.RowEdges.SetAll(StringTable.Edge.OneHeavy);
        assertTable(table,
            "┏━━━━━┳━━━━┓",
            "┋ I   ┋ II ┋",
            "┣━━━━━╋━━━━┫",
            "┋ III ┋ IV ┋",
            "┗━━━━━┻━━━━┛");
        table.RowEdges.SetAll(StringTable.Edge.Dot3Heavy);
        assertTable(table,
            "┏┅┅┅┅┅┳┅┅┅┅┓",
            "┋ I   ┋ II ┋",
            "┣┅┅┅┅┅╋┅┅┅┅┫",
            "┋ III ┋ IV ┋",
            "┗┅┅┅┅┅┻┅┅┅┅┛");
        table.RowEdges.SetAll(StringTable.Edge.Dot4Heavy);
        assertTable(table,
            "┏┉┉┉┉┉┳┉┉┉┉┓",
            "┋ I   ┋ II ┋",
            "┣┉┉┉┉┉╋┉┉┉┉┫",
            "┋ III ┋ IV ┋",
            "┗┉┉┉┉┉┻┉┉┉┉┛");
        table.RowEdges.SetAll(StringTable.Edge.DashHeavy);
        assertTable(table,
            "┏╍╍╍╍╍┳╍╍╍╍┓",
            "┋ I   ┋ II ┋",
            "┣╍╍╍╍╍╋╍╍╍╍┫",
            "┋ III ┋ IV ┋",
            "┗╍╍╍╍╍┻╍╍╍╍┛");
        table.RowEdges.SetAll(StringTable.Edge.Two);
        assertTable(table,
            "╒═════╤════╕",
            "┋ I   ┋ II ┋",
            "╞═════╪════╡",
            "┋ III ┋ IV ┋",
            "╘═════╧════╛");
    }
    
    [TestMethod]
    public void TableEdgeDashHeavy() {
        StringTable table = twoBytwo();
        table.ColumnEdges.SetAll(StringTable.Edge.DashHeavy);
        table.RowEdges.SetAll(StringTable.Edge.None);
        assertTable(table,
            "╏ I   ╏ II ╏",
            "╏ III ╏ IV ╏");
        table.RowEdges.SetAll(StringTable.Edge.Zero);
        assertTable(table,
            "╏     ╏    ╏",
            "╏ I   ╏ II ╏",
            "╏     ╏    ╏",
            "╏ III ╏ IV ╏",
            "╏     ╏    ╏");
        table.RowEdges.SetAll(StringTable.Edge.One);
        assertTable(table,
            "┎─────┰────┒",
            "╏ I   ╏ II ╏",
            "┠─────╂────┨",
            "╏ III ╏ IV ╏",
            "┖─────┸────┚");
        table.RowEdges.SetAll(StringTable.Edge.Dot);
        assertTable(table,
            "┎·····┰····┒",
            "╏ I   ╏ II ╏",
            "╏·····╏····╏",
            "╏ III ╏ IV ╏",
            "┖·····┸····┚");
        table.RowEdges.SetAll(StringTable.Edge.Dot3);
        assertTable(table,
            "┎┄┄┄┄┄┰┄┄┄┄┒",
            "╏ I   ╏ II ╏",
            "┠┄┄┄┄┄╂┄┄┄┄┨",
            "╏ III ╏ IV ╏",
            "┖┄┄┄┄┄┸┄┄┄┄┚");
        table.RowEdges.SetAll(StringTable.Edge.Dot4);
        assertTable(table,
            "┎┈┈┈┈┈┰┈┈┈┈┒",
            "╏ I   ╏ II ╏",
            "┠┈┈┈┈┈╂┈┈┈┈┨",
            "╏ III ╏ IV ╏",
            "┖┈┈┈┈┈┸┈┈┈┈┚");
        table.RowEdges.SetAll(StringTable.Edge.Dash);
        assertTable(table,
            "┎╌╌╌╌╌┰╌╌╌╌┒",
            "╏ I   ╏ II ╏",
            "┠╌╌╌╌╌╂╌╌╌╌┨",
            "╏ III ╏ IV ╏",
            "┖╌╌╌╌╌┸╌╌╌╌┚");
        table.RowEdges.SetAll(StringTable.Edge.OneHeavy);
        assertTable(table,
            "┏━━━━━┳━━━━┓",
            "╏ I   ╏ II ╏",
            "┣━━━━━╋━━━━┫",
            "╏ III ╏ IV ╏",
            "┗━━━━━┻━━━━┛");
        table.RowEdges.SetAll(StringTable.Edge.Dot3Heavy);
        assertTable(table,
            "┏┅┅┅┅┅┳┅┅┅┅┓",
            "╏ I   ╏ II ╏",
            "┣┅┅┅┅┅╋┅┅┅┅┫",
            "╏ III ╏ IV ╏",
            "┗┅┅┅┅┅┻┅┅┅┅┛");
        table.RowEdges.SetAll(StringTable.Edge.Dot4Heavy);
        assertTable(table,
            "┏┉┉┉┉┉┳┉┉┉┉┓",
            "╏ I   ╏ II ╏",
            "┣┉┉┉┉┉╋┉┉┉┉┫",
            "╏ III ╏ IV ╏",
            "┗┉┉┉┉┉┻┉┉┉┉┛");
        table.RowEdges.SetAll(StringTable.Edge.DashHeavy);
        assertTable(table,
            "┏╍╍╍╍╍┳╍╍╍╍┓",
            "╏ I   ╏ II ╏",
            "┣╍╍╍╍╍╋╍╍╍╍┫",
            "╏ III ╏ IV ╏",
            "┗╍╍╍╍╍┻╍╍╍╍┛");
        table.RowEdges.SetAll(StringTable.Edge.Two);
        assertTable(table,
            "╒═════╤════╕",
            "╏ I   ╏ II ╏",
            "╞═════╪════╡",
            "╏ III ╏ IV ╏",
            "╘═════╧════╛");
    }
    
    [TestMethod]
    public void TableEdgesTwo() {
        StringTable table = twoBytwo();
        table.ColumnEdges.SetAll(StringTable.Edge.Two);
        table.RowEdges.SetAll(StringTable.Edge.None);
        assertTable(table,
            "║ I   ║ II ║",
            "║ III ║ IV ║");
        table.RowEdges.SetAll(StringTable.Edge.Zero);
        assertTable(table,
            "║     ║    ║",
            "║ I   ║ II ║",
            "║     ║    ║",
            "║ III ║ IV ║",
            "║     ║    ║");
        table.RowEdges.SetAll(StringTable.Edge.One);
        assertTable(table,
            "╓─────╥────╖",
            "║ I   ║ II ║",
            "╟─────╫────╢",
            "║ III ║ IV ║",
            "╙─────╨────╜");        
        table.RowEdges.SetAll(StringTable.Edge.Dot);
        assertTable(table,
            "╓·····╥····╖",
            "║ I   ║ II ║",
            "║·····║····║",
            "║ III ║ IV ║",
            "╙·····╨····╜");
        table.RowEdges.SetAll(StringTable.Edge.Dot3);
        assertTable(table,
            "╓┄┄┄┄┄╥┄┄┄┄╖",
            "║ I   ║ II ║",
            "╟┄┄┄┄┄╫┄┄┄┄╢",
            "║ III ║ IV ║",
            "╙┄┄┄┄┄╨┄┄┄┄╜");
        table.RowEdges.SetAll(StringTable.Edge.Dot4);
        assertTable(table,
            "╓┈┈┈┈┈╥┈┈┈┈╖",
            "║ I   ║ II ║",
            "╟┈┈┈┈┈╫┈┈┈┈╢",
            "║ III ║ IV ║",
            "╙┈┈┈┈┈╨┈┈┈┈╜");
        table.RowEdges.SetAll(StringTable.Edge.Dash);
        assertTable(table,
            "╓╌╌╌╌╌╥╌╌╌╌╖",
            "║ I   ║ II ║",
            "╟╌╌╌╌╌╫╌╌╌╌╢",
            "║ III ║ IV ║",
            "╙╌╌╌╌╌╨╌╌╌╌╜");
        table.RowEdges.SetAll(StringTable.Edge.OneHeavy);
        assertTable(table,
            "╓━━━━━╥━━━━╖",
            "║ I   ║ II ║",
            "╟━━━━━╫━━━━╢",
            "║ III ║ IV ║",
            "╙━━━━━╨━━━━╜");
        table.RowEdges.SetAll(StringTable.Edge.Dot3Heavy);
        assertTable(table,
            "╓┅┅┅┅┅╥┅┅┅┅╖",
            "║ I   ║ II ║",
            "╟┅┅┅┅┅╫┅┅┅┅╢",
            "║ III ║ IV ║",
            "╙┅┅┅┅┅╨┅┅┅┅╜");
        table.RowEdges.SetAll(StringTable.Edge.Dot4Heavy);
        assertTable(table,
            "╓┉┉┉┉┉╥┉┉┉┉╖",
            "║ I   ║ II ║",
            "╟┉┉┉┉┉╫┉┉┉┉╢",
            "║ III ║ IV ║",
            "╙┉┉┉┉┉╨┉┉┉┉╜");
        table.RowEdges.SetAll(StringTable.Edge.DashHeavy);
        assertTable(table,
            "╓╍╍╍╍╍╥╍╍╍╍╖",
            "║ I   ║ II ║",
            "╟╍╍╍╍╍╫╍╍╍╍╢",
            "║ III ║ IV ║",
            "╙╍╍╍╍╍╨╍╍╍╍╜");
        table.RowEdges.SetAll(StringTable.Edge.Two);
        assertTable(table,
            "╔═════╦════╗",
            "║ I   ║ II ║",
            "╠═════╬════╣",
            "║ III ║ IV ║",
            "╚═════╩════╝");
    }

    [TestMethod]
    public void TableBorders() {
        StringTable table = new();
        table.Data[0, 0] = "I";
        table.Data[0, 1] = "II";
        table.Data[0, 2] = "III";
        table.Data[1, 0] = "IV";
        table.Data[1, 1] = "V";
        table.Data[1, 2] = "VI";
        table.Data[2, 0] = "VII";
        table.Data[2, 1] = "VIII";
        table.Data[2, 2] = "IX";
        assertTable(table,
            "I   II   III",
            "IV  V    VI ",
            "VII VIII IX ");
        table.SetBoarder(StringTable.Edge.One);
        assertTable(table,
            "┌──────────────┐",
            "│ I   II   III │",
            "│ IV  V    VI  │",
            "│ VII VIII IX  │",
            "└──────────────┘");
        table.SetBoarder(StringTable.Edge.Two);
        assertTable(table,
            "╔══════════════╗",
            "║ I   II   III ║",
            "║ IV  V    VI  ║",
            "║ VII VIII IX  ║",
            "╚══════════════╝");
        table.SetRowHeaderDefaultEdges();
        assertTable(table,
            "┌─────┬──────┬─────┐",
            "│ I   │ II   │ III │",
            "├─────┼──────┼─────┤",
            "│ IV  │ V    │ VI  │",
            "│ VII │ VIII │ IX  │",
            "└─────┴──────┴─────┘");
    }

    [TestMethod]
    public void MarkDown() {
        StringTable table = new();
        table.Data[0, 0] = "I";
        table.Data[0, 1] = "II";
        table.Data[0, 2] = "III";
        table.Data[1, 0] = "IV";
        table.Data[1, 1] = "V";
        table.Data[1, 2] = "VI";
        table.Data[2, 0] = "VII";
        table.Data[2, 1] = "VIII";
        table.Data[2, 2] = "IX";
        assertMarkdown(table,
            "| I | IV | VII |",
            "|:---|:---|:---|",
            "| II | V | VIII |",
            "| III | VI | IX |");
        table.Alignments.SetAll(StringTable.Alignment.Center);
        assertMarkdown(table,
            "| I | IV | VII |",
            "|:---:|:---:|:---:|",
            "| II | V | VIII |",
            "| III | VI | IX |");
        table.Alignments.SetAll(StringTable.Alignment.Right);
        assertMarkdown(table,
            "| I | IV | VII |",
            "|---:|---:|---:|",
            "| II | V | VIII |",
            "| III | VI | IX |");

        table.Data[2, 0] = "|";
        table.Data[2, 1] = @"\|";
        table.Data[2, 2] = "A"+Environment.NewLine+"B";
        table.Alignments.SetAll(StringTable.Alignment.Left);
        assertMarkdown(table,
            @"| I | IV | \| |",
            @"|:---|:---|:---|",
            @"| II | V | \\\| |",
            @"| III | VI | A<br>B |");
    }

    [TestMethod]
    public void Multiline() {
        StringTable table = new();
        table.Data[0, 0] = "A\nB\nC";
        table.Data[0, 1] = "D\nE";
        table.Data[0, 2] = "F";
        table.Data[1, 0] = "G";
        table.Data[1, 1] = "G\nI\nJ\nK";
        table.Data[1, 2] = "L";
        table.Data[2, 0] = "M";
        table.Data[2, 1] = "N O";
        table.Data[2, 2] = "P\nQ R";
        table.SetAllEdges(StringTable.Edge.One);
        assertTable(table,
            "┌───┬─────┬─────┐",
            "│ A │ D   │ F   │",
            "│ B │ E   │     │",
            "│ C │     │     │",
            "├───┼─────┼─────┤",
            "│ G │ G   │ L   │",
            "│   │ I   │     │",
            "│   │ J   │     │",
            "│   │ K   │     │",
            "├───┼─────┼─────┤",
            "│ M │ N O │ P   │",
            "│   │     │ Q R │",
            "└───┴─────┴─────┘");
        table.MaximumRowHeight = 2;
        assertTable(table,
            "┌───┬─────┬─────┐",
            "│ A │ D   │ F   │",
            "│ … │ E   │     │",
            "├───┼─────┼─────┤",
            "│ G │ G   │ L   │",
            "│   │ …   │     │",
            "├───┼─────┼─────┤",
            "│ M │ N O │ P   │",
            "│   │     │ Q R │",
            "└───┴─────┴─────┘");
    }

    [TestMethod]
    public void Alignment() {
        StringTable table = new();
        table.Data[0, 0] = "All";
        table.Data[0, 1] = "the";
        table.Data[0, 2] = "bananas";
        table.Data[1, 0] = "around";
        table.Data[1, 1] = "your";
        table.Data[1, 2] = "bushel";
        table.Data[2, 0] = "are";
        table.Data[2, 1] = "rotten";
        table.Data[2, 2] = "now";
        table.SetAllEdges(StringTable.Edge.One);
        table.Alignments.SetAll(StringTable.Alignment.Left);
        assertTable(table,
            "┌────────┬────────┬─────────┐",
            "│ All    │ the    │ bananas │",
            "├────────┼────────┼─────────┤",
            "│ around │ your   │ bushel  │",
            "├────────┼────────┼─────────┤",
            "│ are    │ rotten │ now     │",
            "└────────┴────────┴─────────┘");
        table.Alignments.SetAll(StringTable.Alignment.Right);
        assertTable(table,
            "┌────────┬────────┬─────────┐",
            "│    All │    the │ bananas │",
            "├────────┼────────┼─────────┤",
            "│ around │   your │  bushel │",
            "├────────┼────────┼─────────┤",
            "│    are │ rotten │     now │",
            "└────────┴────────┴─────────┘");
        table.Alignments.SetAll(StringTable.Alignment.Center);
        assertTable(table,
            "┌────────┬────────┬─────────┐",
            "│  All   │  the   │ bananas │",
            "├────────┼────────┼─────────┤",
            "│ around │  your  │ bushel  │",
            "├────────┼────────┼─────────┤",
            "│  are   │ rotten │   now   │",
            "└────────┴────────┴─────────┘");
        table.Alignments[0] = StringTable.Alignment.Left;
        table.Alignments[2] = StringTable.Alignment.Right;
        assertTable(table,
            "┌────────┬────────┬─────────┐",
            "│ All    │  the   │ bananas │",
            "├────────┼────────┼─────────┤",
            "│ around │  your  │  bushel │",
            "├────────┼────────┼─────────┤",
            "│ are    │ rotten │     now │",
            "└────────┴────────┴─────────┘");
    }

    [TestMethod]
    public void SingleValue() {
        StringTable table = new();
        table.Data[0, 0] = "";
        table.RowEdges.SetAll(StringTable.Edge.One);
        table.ColumnEdges.SetAll(StringTable.Edge.One);
        assertTable(table,
            "┌──┐",
            "└──┘");
    }

    [TestMethod]
    public void Empty() {
        StringTable table = new();
        Assert.AreEqual(table.ToString(), "");
        Assert.AreEqual(table.ToMarkdown(), "");
        table.SetRowHeaderDefaultEdges();
        Assert.AreEqual(table.ToString(), "");
        Assert.AreEqual(table.ToMarkdown(), "");
    }

    [TestMethod]
    public void Skips() {
        StringTable table = new();
        table.Data[0, 0] = "TL";
        table.Data[0, 6] = "TR";
        table.Data[6, 0] = "BL";
        table.Data[6, 6] = "BR";
        table.SetAllEdges(StringTable.Edge.One);
        assertTable(table,
            "┌────┬──┬──┬──┬──┬──┬────┐",
            "│ TL │  │  │  │  │  │ TR │",
            "├────┼──┼──┼──┼──┼──┼────┤",
            "├────┼──┼──┼──┼──┼──┼────┤",
            "├────┼──┼──┼──┼──┼──┼────┤",
            "├────┼──┼──┼──┼──┼──┼────┤",
            "├────┼──┼──┼──┼──┼──┼────┤",
            "├────┼──┼──┼──┼──┼──┼────┤",
            "│ BL │  │  │  │  │  │ BR │",
            "└────┴──┴──┴──┴──┴──┴────┘");
    }

    [TestMethod]
    public void TooWide() {
        StringTable table = new();
        table.Data[0, 0] = "This is the song that never ends";
        table.Data[0, 1] = "Yes it goes on and on my friend";
        table.Data[1, 0] = "Some people started singing it\nnot knowing what it was";
        table.Data[1, 1] = "But people kept singing it just because,";
        table.SetAllEdges(StringTable.Edge.One);
        table.MaximumColumnWidth = 100;
        assertTable(table,
            "┌──────────────────────────────────┬──────────────────────────────────────────┐",
            "│ This is the song that never ends │ Yes it goes on and on my friend          │",
            "├──────────────────────────────────┼──────────────────────────────────────────┤",
            "│ Some people started singing it   │ But people kept singing it just because, │",
            "│ not knowing what it was          │                                          │",
            "└──────────────────────────────────┴──────────────────────────────────────────┘");
        table.MaximumColumnWidth = 28;
        assertTable(table,
            "┌──────────────────────────────┬──────────────────────────────┐",
            "│ This is the song that nev…   │ Yes it goes on and on my …   │",
            "├──────────────────────────────┼──────────────────────────────┤",
            "│ Some people started singi…   │ But people kept singing i…   │",
            "│ not knowing what it was      │                              │",
            "└──────────────────────────────┴──────────────────────────────┘");
        table.MaximumColumnWidth = 10;
        assertTable(table,
            "┌────────────┬────────────┐",
            "│ This is…   │ Yes it …   │",
            "├────────────┼────────────┤",
            "│ Some pe…   │ But peo…   │",
            "│ not kno…   │            │",
            "└────────────┴────────────┘");
    }
}
