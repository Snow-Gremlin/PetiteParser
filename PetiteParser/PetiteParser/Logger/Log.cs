using PetiteParser.Misc;
using System.Collections.Generic;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace PetiteParser.Logger {

    /// <summary>This is the arguments for an inspector.</summary>
    public class Log {

        /// <summary>All the log entries.</summary>
        private readonly Queue<Entry> entries;

        /// <summary>Creates a new inspector argument.</summary>
        public Log() => entries = new();

        /// <summary>Removes all the entries from the logs.</summary>
        public void Clear() => entries.Clear();

        /// <summary>Indicates that at least one error has occurred.</summary>
        public bool Failed => entries.Any(e => e.Level == Level.Error);

        /// <summary>Gets all the log entries.</summary>
        public IEnumerable<Entry> Entries => entries;

        /// <summary>Adds the given entry to the logs.</summary>
        /// <param name="entry">The entry to add to the log.</param>
        public void Add(Entry entry) => entries.Enqueue(entry);

        /// <summary>Logs an entry to the log at the given level.</summary>
        /// <param name="level">The level of the entry to log.</param>
        /// <param name="text">The text string for the lines of the entry.</param>
        /// <returns>Returns the entry that was created.</returns>
        public Entry Add(Level level, params string[] text) {
            Entry entry = new(level, text.JoinLines());
            this.Add(entry);
            return entry;
        }

        /// <summary>Logs an error.</summary>
        /// <param name="text">The text string for the lines of the error.</param>
        /// <returns>Returns the error entry that was created.</returns>
        public Entry AddError(params string[] text) => this.Add(Level.Error, text);

        /// <summary>Logs a warning.</summary>
        /// <param name="text">The text string for the lines of the warning.</param>
        /// <returns>Returns the warning entry that was created.</returns>
        public Entry AddWarning(params string[] text) => this.Add(Level.Warning, text);

        /// <summary>Logs a notice.</summary>
        /// <param name="text">The text string for the lines of the notice.</param>
        /// <returns>Returns the notice entry that was created.</returns>
        public Entry AddNotice(params string[] text) => this.Add(Level.Notice, text);

        /// <summary>Logs some text for additional information.</summary>
        /// <param name="text">The text string for the lines of the information.</param>
        /// <returns>Returns the info entry that was created.</returns>
        public Entry AddInfo(params string[] text) => this.Add(Level.Info, text);

        /// <summary>Logs an entry to the log at the given level.</summary>
        /// <param name="level">The level of the entry to log.</param>
        /// <param name="format">The formatting string for the entry.</param>
        /// <param name="args">The arguments to fill out the formatting string.</param>
        /// <returns>Returns the entry that was created.</returns>
        public Entry AddF(Level level, string format, params object[] args) {
            Entry entry = new(level, string.Format(format, args));
            this.Add(entry);
            return entry;
        }

        /// <summary>Logs a formatted error.</summary>
        /// <param name="format">The formatting string for the error.</param>
        /// <param name="args">The arguments to fill out the formatting string.</param>
        /// <returns>Returns the error entry that was created.</returns>
        public Entry AddErrorF(string format, params object[] args) => this.AddF(Level.Error, format, args);

        /// <summary>Logs a formatted warning.</summary>
        /// <param name="format">The formatting string for the warning.</param>
        /// <param name="args">The arguments to fill out the formatting string.</param>
        /// <returns>Returns the warning entry that was created.</returns>
        public Entry AddWarningF(string format, params object[] args) => this.AddF(Level.Warning, format, args);

        /// <summary>Logs a formatted notice.</summary>
        /// <param name="format">The formatting string for the notice.</param>
        /// <param name="args">The arguments to fill out the formatting string.</param>
        /// <returns>Returns the notice entry that was created.</returns>
        public Entry AddNoticeF(string format, params object[] args) => this.AddF(Level.Notice, format, args);

        /// <summary>Logs some formatted text for additional information.</summary>
        /// <param name="format">The formatting string for the info.</param>
        /// <param name="args">The arguments to fill out the formatting string.</param>
        /// <returns>Returns the info entry that was created.</returns>
        public Entry AddInfoF(string format, params object[] args) => this.AddF(Level.Info, format, args);

        /// <summary>All the entries of log separated by newlines.</summary>
        /// <returns>The log that was collected as a string.</returns>
        public override string ToString() => this.Entries.JoinLines();
    }
}
