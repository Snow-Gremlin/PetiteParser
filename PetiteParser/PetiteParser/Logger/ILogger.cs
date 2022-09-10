using PetiteParser.Misc;

namespace PetiteParser.Logger {

    /// <summary>The interface for all logging tools.</summary>
    public interface ILogger {

        /// <summary>
        /// Removes all the entries from the logs which can
        /// be removed and resets the failed flag.
        /// </summary>
        public void Clear();

        /// <summary>Indicates that at least one error has occurred.</summary>
        public bool Failed { get; }

        /// <summary>Adds the given entry to the logs.</summary>
        /// <param name="entry">The entry to add to the log.</param>
        public void Add(Entry entry);

        /// <summary>Logs an entry to the log at the given level.</summary>
        /// <param name="level">The level of the entry to log.</param>
        /// <param name="text">The text string for the lines of the entry.</param>
        /// <returns>Returns the entry that was created.</returns>
        public Entry Add(Level level, params string[] text);

        /// <summary>Logs an error.</summary>
        /// <param name="text">The text string for the lines of the error.</param>
        /// <returns>Returns the error entry that was created.</returns>
        public Entry AddError(params string[] text);

        /// <summary>Logs a warning.</summary>
        /// <param name="text">The text string for the lines of the warning.</param>
        /// <returns>Returns the warning entry that was created.</returns>
        public Entry AddWarning(params string[] text);

        /// <summary>Logs a notice.</summary>
        /// <param name="text">The text string for the lines of the notice.</param>
        /// <returns>Returns the notice entry that was created.</returns>
        public Entry AddNotice(params string[] text);

        /// <summary>Logs some text for additional information.</summary>
        /// <param name="text">The text string for the lines of the information.</param>
        /// <returns>Returns the info entry that was created.</returns>
        public Entry AddInfo(params string[] text);

        /// <summary>Logs an entry to the log at the given level.</summary>
        /// <param name="level">The level of the entry to log.</param>
        /// <param name="format">The formatting string for the entry.</param>
        /// <param name="args">The arguments to fill out the formatting string.</param>
        /// <returns>Returns the entry that was created.</returns>
        public Entry AddF(Level level, string format, params object[] args);

        /// <summary>Logs a formatted error.</summary>
        /// <param name="format">The formatting string for the error.</param>
        /// <param name="args">The arguments to fill out the formatting string.</param>
        /// <returns>Returns the error entry that was created.</returns>
        public Entry AddErrorF(string format, params object[] args);

        /// <summary>Logs a formatted warning.</summary>
        /// <param name="format">The formatting string for the warning.</param>
        /// <param name="args">The arguments to fill out the formatting string.</param>
        /// <returns>Returns the warning entry that was created.</returns>
        public Entry AddWarningF(string format, params object[] args);

        /// <summary>Logs a formatted notice.</summary>
        /// <param name="format">The formatting string for the notice.</param>
        /// <param name="args">The arguments to fill out the formatting string.</param>
        /// <returns>Returns the notice entry that was created.</returns>
        public Entry AddNoticeF(string format, params object[] args);

        /// <summary>Logs some formatted text for additional information.</summary>
        /// <param name="format">The formatting string for the info.</param>
        /// <param name="args">The arguments to fill out the formatting string.</param>
        /// <returns>Returns the info entry that was created.</returns>
        public Entry AddInfoF(string format, params object[] args);
    }
}
