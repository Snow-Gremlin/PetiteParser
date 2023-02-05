using PetiteParser.Formatting;
using PetiteParser.Logger;
using PetiteParser.Parser;
using PetiteParser.Parser.States;
using PetiteParser.Parser.Table;
using System.Linq;

namespace TestPetiteParser.Tools;

static internal class ParserExt {

    /// <summary>Checks the parser will parse the given input.</summary>
    static public void Check(this Parser parser, string input, params string[] expected) =>
        TestTools.AreEqual(expected.JoinLines(), parser.Parse(input, 0, new Writer()).ToString());

    /// <summary>Checks the states generated from this grammar.</summary>
    static public void Check(this ParserStates states, params string[] expected) =>
        TestTools.AreEqual(expected.JoinLines(), states.States.JoinLines().Trim());

    /// <summary>Checks one state generated from this grammar.</summary>
    static public void CheckState(this ParserStates states, int stateNumber, params string[] expected) =>
        TestTools.AreEqual(expected.JoinLines(), states.States[stateNumber].ToString().Trim());
    
    /// <summary>Checks the states actions generated from this grammar.</summary>
    static public void CheckActions(this ParserStates states, params string[] expected) =>
        TestTools.AreEqual(expected.JoinLines(), states.States.Select(s => s.ToString(false, true, false)).JoinLines().Trim());

    /// <summary>Checks the table generated from this grammar.</summary>
    static public void Check(this Table table, params string[] expected) =>
        TestTools.AreEqual(expected.JoinLines(), table.ToString().Trim());
}
