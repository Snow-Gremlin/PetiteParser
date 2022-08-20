using System.Linq;

namespace PetiteParser.Analyzer.Inspectors {

    /// <summary>An inspector to check for grammars which have nothing defined.</summary>
    internal class CheckForEmptyDefinitions : IInspector {

        /// <summary>Performs this inspection on the given grammar.</summary>
        /// <param name="grammar">The grammar being validated.</param>
        /// <param name="log">The log to write errors and warnings out to.</param>
        public void Inspect(Grammar.Grammar grammar, InspectorLog log) {
            if (!grammar.Terms.Any())  log.LogError("No terms are defined.");
            if (!grammar.Tokens.Any()) log.LogError("No tokens are defined.");
        }
    }
}
