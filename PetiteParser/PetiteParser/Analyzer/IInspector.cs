namespace PetiteParser.Analyzer {

    /// <summary>The interface for inspection as part of validating a grammar.</summary>
    /// <remarks>These are not allowed to modify the grammar.</remarks>
    public interface IInspector {

        /// <summary>Performs this inspection on the given grammar.</summary>
        /// <param name="grammar">The grammar being validated.</param>
        /// <param name="log">The log to write errors and warnings out to.</param>
        public void Inspect(Grammar.Grammar grammar, Logger.ILogger log);
    }
}
