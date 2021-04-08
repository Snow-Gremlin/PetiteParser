namespace PetiteParser.Table {

    /// <summary>
    /// An accept indicates that the full input has been
    /// checked by the grammar and fits to the grammar.
    /// </summary>
    internal class Accept: IAction {

        /// <summary>Creates a new accept action.</summary>
        internal Accept() { }

        /// <summary>Gets the debug string for this action.</summary>
        /// <returns>The string for this action.</returns>
        public override string ToString() => "accept";
    }
}
