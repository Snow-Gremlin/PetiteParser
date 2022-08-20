namespace PetiteParser.Analyzer {

    /// <summary>An action which can be performed on a grammar during analysis.</summary>
    public interface IAction {

        /// <summary>Performs this action on the given grammar.</summary>
        /// <param name="analyzer">The analyzer to perform this action on.</param>
        /// <returns>True if the grammar was changed.</returns>
        public bool Perform(Analyzer analyzer);
    }
}
