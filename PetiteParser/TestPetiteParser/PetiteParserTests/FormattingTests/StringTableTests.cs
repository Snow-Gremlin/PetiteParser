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

    static private StringTable twoByTwo() {
        StringTable table = new();
        table.Data[0, 0] = "I";
        table.Data[0, 1] = "II";
        table.Data[1, 0] = "III";
        table.Data[1, 1] = "IV";
        return table;
    }

    [TestMethod]
    public void TableEdgesNone() {
        StringTable table = twoByTwo();
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
        StringTable table = twoByTwo();
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
        StringTable table = twoByTwo();
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
        StringTable table = twoByTwo();
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
        StringTable table = twoByTwo();
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
        StringTable table = twoByTwo();
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
        StringTable table = twoByTwo();
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
        StringTable table = twoByTwo();
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
        StringTable table = twoByTwo();
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
        StringTable table = twoByTwo();
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
        StringTable table = twoByTwo();
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
        StringTable table = twoByTwo();
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
            "| I | II | III |",
            "|:---|:---|:---|",
            "| IV | V | VI |",
            "| VII | VIII | IX |");
        table.Alignments.SetAll(StringTable.Alignment.Center);
        assertMarkdown(table,
            "| I | II | III |",
            "|:---:|:---:|:---:|",
            "| IV | V | VI |",
            "| VII | VIII | IX |");
        table.Alignments.SetAll(StringTable.Alignment.Right);
        assertMarkdown(table,
            "| I | II | III |",
            "|---:|---:|---:|",
            "| IV | V | VI |",
            "| VII | VIII | IX |");

        table.Data[2, 0] = "|";
        table.Data[2, 1] = @"\|";
        table.Data[2, 2] = "A"+Environment.NewLine+"B";
        table.Alignments.SetAll(StringTable.Alignment.Left);
        assertMarkdown(table,
            @"| I | II | III |",
            @"|:---|:---|:---|",
            @"| IV | V | VI |",
            @"| \| | \\\| | A<br>B |");
    }

    [TestMethod]
    public void MultiLine() {
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
            "│  │",
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
        StringTable st1 = new();
        st1.Data[0, 0] = "TL";
        st1.Data[0, 6] = "TR";
        st1.Data[6, 0] = "BL";
        st1.Data[6, 6] = "BR";
        st1.SetAllEdges(StringTable.Edge.One);
        assertTable(st1,
            "┌────┬──┬──┬──┬──┬──┬────┐",
            "│ TL │  │  │  │  │  │ TR │",
            "├────┼──┼──┼──┼──┼──┼────┤",
            "│    │  │  │  │  │  │    │",
            "├────┼──┼──┼──┼──┼──┼────┤",
            "│    │  │  │  │  │  │    │",
            "├────┼──┼──┼──┼──┼──┼────┤",
            "│    │  │  │  │  │  │    │",
            "├────┼──┼──┼──┼──┼──┼────┤",
            "│    │  │  │  │  │  │    │",
            "├────┼──┼──┼──┼──┼──┼────┤",
            "│    │  │  │  │  │  │    │",
            "├────┼──┼──┼──┼──┼──┼────┤",
            "│ BL │  │  │  │  │  │ BR │",
            "└────┴──┴──┴──┴──┴──┴────┘");

        StringTable st2 = new();
        st2.Data[0, 0] = "TL";
        st2.Data[0, 3] = "TR";
        st2.Data[6, 0] = "BL";
        st2.Data[6, 3] = "BR";
        st2.SetAllEdges(StringTable.Edge.One);
        assertTable(st2,
            "┌────┬──┬──┬────┐",
            "│ TL │  │  │ TR │",
            "├────┼──┼──┼────┤",
            "│    │  │  │    │",
            "├────┼──┼──┼────┤",
            "│    │  │  │    │",
            "├────┼──┼──┼────┤",
            "│    │  │  │    │",
            "├────┼──┼──┼────┤",
            "│    │  │  │    │",
            "├────┼──┼──┼────┤",
            "│    │  │  │    │",
            "├────┼──┼──┼────┤",
            "│ BL │  │  │ BR │",
            "└────┴──┴──┴────┘");
        
        StringTable st3 = new();
        st3.Data[0, 0] = "TL";
        st3.Data[0, 6] = "TR";
        st3.Data[3, 0] = "BL";
        st3.Data[3, 6] = "BR";
        st3.SetAllEdges(StringTable.Edge.One);
        assertTable(st3,
            "┌────┬──┬──┬──┬──┬──┬────┐",
            "│ TL │  │  │  │  │  │ TR │",
            "├────┼──┼──┼──┼──┼──┼────┤",
            "│    │  │  │  │  │  │    │",
            "├────┼──┼──┼──┼──┼──┼────┤",
            "│    │  │  │  │  │  │    │",
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

    [TestMethod]
    public void PreSizedTable() {
        StringTable st = new(8, 3);
        st.Data[0, 0] = "Term";
        st.Data[0, 1] = "Firsts";
        st.Data[0, 2] = "λ";
        st.SetRowHeaderDefaultEdges();
        assertTable(st,
            "┌──────┬────────┬───┐",
            "│ Term │ Firsts │ λ │",
            "├──────┼────────┼───┤",
            "│      │        │   │", // 1
            "│      │        │   │", // 2
            "│      │        │   │", // 3
            "│      │        │   │", // 4
            "│      │        │   │", // 5
            "│      │        │   │", // 6
            "│      │        │   │", // 7
            "└──────┴────────┴───┘");
        
        for (int i = 1; i < 8; ++i) {
            st.Data[i, 0] = "term"+i;
            st.Data[i, 1] = "f, g, h";
            st.Data[i, 2] = i % 3 == 0 ? "x" : "";
        }
        assertTable(st,
            "┌───────┬─────────┬───┐",
            "│ Term  │ Firsts  │ λ │",
            "├───────┼─────────┼───┤",
            "│ term1 │ f, g, h │   │", // 1
            "│ term2 │ f, g, h │   │", // 2
            "│ term3 │ f, g, h │ x │", // 3
            "│ term4 │ f, g, h │   │", // 4
            "│ term5 │ f, g, h │   │", // 5
            "│ term6 │ f, g, h │ x │", // 6
            "│ term7 │ f, g, h │   │", // 7
            "└───────┴─────────┴───┘");
    }
}
