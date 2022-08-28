using PetiteParser.Misc;
using System.Collections.Generic;
using System.Linq;

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
        /// <param name="format">The formatting string for the error.</param>
        /// <param name="args">The arguments to fill out the formatting string.</param>
        /// <returns>Returns the entry that was created.</returns>
        public Entry Add(Level level, string format, params object[] args) {
            Entry entry = new(level, string.Format(format, args));
            this.Add(entry);
            return entry;
        }

        /// <summary>Logs an error.</summary>
        /// <param name="format">The formatting string for the error.</param>
        /// <param name="args">The arguments to fill out the formatting string.</param>
        /// <returns>Returns the error entry that was created.</returns>
        public Entry AddError(string format, params object[] args) => this.Add(Level.Error, format, args);

        /// <summary>Logs a warning.</summary>
        /// <param name="format">The formatting string for the warning.</param>
        /// <param name="args">The arguments to fill out the formatting string.</param>
        /// <returns>Returns the warning entry that was created.</returns>
        public Entry AddWarning(string format, params object[] args) => this.Add(Level.Warning, format, args);

        /// <summary>Logs a notice.</summary>
        /// <param name="format">The formatting string for the notice.</param>
        /// <param name="args">The arguments to fill out the formatting string.</param>
        /// <returns>Returns the notice entry that was created.</returns>
        public Entry AddNotice(string format, params object[] args) => this.Add(Level.Notice, format, args);

        /// <summary>All the entries of log separated by newlines.</summary>
        /// <returns>The log that was collected as a string.</returns>
        public override string ToString() => this.Entries.JoinLines();
    }
}
