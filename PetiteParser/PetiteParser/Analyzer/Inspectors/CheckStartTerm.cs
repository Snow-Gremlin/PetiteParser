using System.Linq;

namespace PetiteParser.Analyzer.Inspectors {

    /// <summary>An inspector to check that the grammar has a valid start term.</summary>
    internal class CheckStartTerm : IInspector {

        /// <summary>Performs this inspection on the given grammar.</summary>
        /// <param name="grammar">The grammar being validated.</param>
        /// <param name="log">The log to write errors and warnings out to.</param>
        public void Inspect(Grammar.Grammar grammar, Logger.ILogger log) {
            Grammar.Term start = grammar.StartTerm;
            if (start is null)
                log.AddErrorF("The start term is not set.");
            else if (!grammar.Terms.Contains(start))
                log.AddErrorF("The start term, {0}, was not found in the set of terms.", start);
        }
    }
}
