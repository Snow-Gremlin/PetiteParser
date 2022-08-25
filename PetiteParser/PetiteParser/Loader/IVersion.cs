namespace PetiteParser.Loader {

    /// <summary>The interface for all loader versions.</summary>
    internal interface IVersion {

        /// <summary>Gets the version of the loader.</summary>
        public int Version { get; }

        /// <summary>
        /// Adds several blocks of definitions to the grammar and tokenizer
        /// which are being loaded via a list of characters containing the definition.
        /// </summary>
        /// <param name="grammar">Gets the grammar which is being loaded.</param>
        /// <param name="tokenizer">Gets the tokenizer which is being loaded.</param>
        /// <param name="input">The input language to read.</param>
        /// <returns>This loader so that calls can be chained.</returns>
        public void Load(Grammar.Grammar grammar, Tokenizer.Tokenizer tokenizer, Scanner.IScanner input);
    }
}
