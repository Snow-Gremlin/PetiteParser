using Microsoft.VisualStudio.TestTools.UnitTesting;
using PetiteParser.Formatting;
using System;

namespace TestPetiteParser.PetiteParserTests.FormattingTests;

[TestClass]
sealed public class StringTableTests {

    static private void assertTable(StringTable table, params string[] expected) =>
        Assert.AreEqual(expected.JoinLines()+Environment.NewLine, table.ToString());

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

    // TODO: Add multi-line
    // TODO: Add too wide
    // TODO: Add alignment
    // TODO: Check Roe == 0 and/or Column == 0
}
