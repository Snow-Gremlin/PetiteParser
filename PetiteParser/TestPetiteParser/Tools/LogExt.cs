using PetiteParser.Formatting;
using PetiteParser.Logger;

namespace TestPetiteParser.Tools;

static public class LogExt {
    
    /// <summary>Checks that the log got the specific given entries.</summary>
    /// <param name="log">The log to check.</param>
    /// <param name="exp">The expected lines in the log.</param>
    static public void Check(this Buffered log, params string[] exp) =>
        TestTools.AreEqual(exp.JoinLines(), log.ToString());
}
