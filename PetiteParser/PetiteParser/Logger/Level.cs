namespace PetiteParser.Logger {

    /// <summary>The level of an entry in the log.</summary>
    public enum Level {

        /// <summary>Indicates that some state was reached.</summary>
        Notice,

        /// <summary>
        /// Indicates that a problem was found but it won't prevent
        /// from continuing, however the problem should be fixed.
        /// </summary>
        Warning,

        /// <summary>
        /// Indicate that an error that will prevent
        /// from continuing and must be fixed.
        /// </summary>
        Error
    }
}
