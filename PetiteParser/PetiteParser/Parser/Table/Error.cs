namespace PetiteParser.Parser.Table {

    /// <summary>
    /// An error indicates that the given token can not
    /// be processed from the current state.
    /// A null action is a generic error, this one gives specific information.
    /// </summary>
    sealed internal class Error: IAction {

        /// <summary>The error message to return for this action.</summary>
        public readonly string Message;

        /// <summary>Creates a new error action.</summary>
        /// <param name="message">The error message for this action.</param>
        internal Error(string message) => this.Message = message;

        /// <summary>Gets the debug string for this action.</summary>
        /// <returns>The string for this action.</returns>
        public override string ToString() => "error "+this.Message;
    }
}
