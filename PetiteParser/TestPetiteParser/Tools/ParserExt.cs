using PetiteParser.Formatting;
using PetiteParser.Parser;
using PetiteParser.Parser.States;
using PetiteParser.Parser.Table;

namespace TestPetiteParser.Tools;

static internal class ParserExt {

    /// <summary>Checks the parser will parse the given input.</summary>
    static public void Check(this Parser parser, string input, params string[] expected) =>
        TestTools.AreEqual(expected.JoinLines(), parser.Parse(input).ToString());

    /// <summary>Checks the states generated from this grammar.</summary>
    static public void Check(this ParserStates states, params string[] expected) =>
        TestTools.AreEqual(expected.JoinLines(), states.States.JoinLines().Trim());

    /// <summary>Checks the table generated from this grammar.</summary>
    static public void Check(this Table table, params string[] expected) =>
        TestTools.AreEqual(expected.JoinLines(), table.ToString().Trim());
}
