using Examples.CodeColoring;
using Examples.CodeColoring.Glsl;
using Examples.CodeColoring.Json;
using Examples.CodeColoring.Petite;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PetiteParser.Formatting;
using PetiteParser.Tokenizer;
using System.Linq;
using TestPetiteParser.Tools;

namespace TestPetiteParser.ExamplesTests;

[TestClass]
sealed public class CodeColoringTests {

    static private void inColorerList<T>() where T: IColorer =>
        Assert.AreEqual(1, IColorer.Colorers.Where(c => c is T).Count());

    static private void assertTokens(IColorer c, string input, params string[] expTable) {
        Token[] tokens = c.Colorize(input).Select(f => f.Token).ToArray();
        StringTable table = new(tokens.Length+1, 2);
        table.SetRowHeaderDefaultEdges();
        table.Data[0, 0] = "Name";
        table.Data[0, 1] = "Text"; 
        for (int i = 0; i < tokens.Length; i++) {
            table.Data[i+1, 0] = tokens[i].Name;
            table.Data[i+1, 1] = tokens[i].Text;
        }
        TestTools.AreEqual(expTable.JoinLines(), table.ToString().Trim());
    }

    [TestMethod]
    public void Glsl() {
        inColorerList<Glsl>();
        Glsl c = new();
        Assert.IsTrue(c.ExampleCode.Length > 0);
        assertTokens(c, "uniform mat4 objMat;",
            "┌──────────┬─────────┐",
            "│ Name     │ Text    │",
            "├──────────┼─────────┤",
            "│ Reserved │ uniform │",
            "│ Type     │ mat4    │",
            "│ Id       │ objMat  │",
            "│ Symbol   │ ;       │",
            "└──────────┴─────────┘");
        assertTokens(c, "vec3 n = normalize(objMat*vec4(normAttr, 0.0)).xyz;",
            "┌────────┬───────────┐",
            "│ Name   │ Text      │",
            "├────────┼───────────┤",
            "│ Type   │ vec3      │",
            "│ Id     │ n         │",
            "│ Symbol │ =         │",
            "│ Id     │ normalize │",
            "│ Symbol │ (         │",
            "│ Id     │ objMat    │",
            "│ Symbol │ *         │",
            "│ Type   │ vec4      │",
            "│ Symbol │ (         │",
            "│ Id     │ normAttr  │",
            "│ Symbol │ ,         │",
            "│ Num    │ 0.0       │",
            "│ Symbol │ )).       │",
            "│ Id     │ xyz       │",
            "│ Symbol │ ;         │",
            "└────────┴───────────┘");
    }

    [TestMethod]
    public void Json() {
        inColorerList<Json>();
        Json c = new();
        Assert.IsTrue(c.ExampleCode.Length > 0);
        assertTokens(c, "{ \"name\": \"john\", \"age\": 45, \"kids\": [] }",
            "┌─────────────┬────────┐",
            "│ Name        │ Text   │",
            "├─────────────┼────────┤",
            "│ OpenObject  │ {      │",
            "│ String      │ \"name\" │",
            "│ Colon       │ :      │",
            "│ String      │ \"john\" │",
            "│ Comma       │ ,      │",
            "│ String      │ \"age\"  │",
            "│ Colon       │ :      │",
            "│ Integer     │ 45     │",
            "│ Comma       │ ,      │",
            "│ String      │ \"kids\" │",
            "│ Colon       │ :      │",
            "│ OpenArray   │ [      │",
            "│ CloseArray  │ ]      │",
            "│ CloseObject │ }      │",
            "└─────────────┴────────┘");
    }

    [TestMethod]
    public void Petite() {
        inColorerList<Petite>();
        Petite c = new();
        Assert.IsTrue(c.ExampleCode.Length > 0);
        assertTokens(c, "(start): ',|!^*_;' => [symbol];",
            "┌──────────────┬───────────┐",
            "│ Name         │ Text      │",
            "├──────────────┼───────────┤",
            "│ openParen    │ (         │",
            "│ id           │ start     │",
            "│ closeParen   │ )         │",
            "│ symbol       │ :         │",
            "│ string       │ ',|!^*_;' │",
            "│ symbol       │ =>        │",
            "│ openBracket  │ [         │",
            "│ id           │ symbol    │",
            "│ closeBracket │ ]         │",
            "│ symbol       │ ;         │",
            "└──────────────┴───────────┘");
        assertTokens(c, "> <start> := _ | <start> <part>;",
            "┌────────────┬───────┐",
            "│ Name       │ Text  │",
            "├────────────┼───────┤",
            "│ closeAngle │ >     │",
            "│ openAngle  │ <     │",
            "│ id         │ start │",
            "│ closeAngle │ >     │",
            "│ symbol     │ :=    │",
            "│ symbol     │ _     │",
            "│ symbol     │ |     │",
            "│ openAngle  │ <     │",
            "│ id         │ start │",
            "│ closeAngle │ >     │",
            "│ openAngle  │ <     │",
            "│ id         │ part  │",
            "│ closeAngle │ >     │",
            "│ symbol     │ ;     │",
            "└────────────┴───────┘");
    }
}
