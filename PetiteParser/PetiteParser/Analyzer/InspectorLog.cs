using PetiteParser.Misc;
using System.Collections.Generic;
using System.Linq;

namespace PetiteParser.Analyzer {

    /// <summary>This is the arguments for an inspector.</summary>
    public class InspectorLog {

        /// <summary>This is an entry in the log containing a single message.</summary>
        /// <param name="Message">This is the message of the entry.</param>
        /// <param name="IsError">This indicates if the entry is for an error or warning.</param>
        public readonly record struct Entry(string Message, bool IsError);

        /// <summary>All the log entries.</summary>
        private readonly List<Entry> entries;

        /// <summary>Creates a new inspector argument.</summary>
        public InspectorLog() => this.entries = new();

        /// <summary>Removes all the entries from the logs.</summary>
        public void Clear() => this.entries.Clear();

        /// <summary>Indicates that at least one error has occurred.</summary>
        public bool Failed => this.entries.Any(e => e.IsError);

        /// <summary>Gets all the log entries.</summary>
        public IEnumerable<Entry> Entries => this.entries;

        /// <summary>Gets all the warnings and errors which have occurred.</summary>
        public IEnumerable<string> All =>
            this.entries.Select(e => e.Message);

        /// <summary>Gets all the errors which have occurred.</summary>
        public IEnumerable<string> Errors =>
            this.entries.Where(e => e.IsError).Select(e => e.Message);

        /// <summary>Gets all the warnings which have occurred.</summary>
        public IEnumerable<string> Warnings =>
            this.entries.WhereNot(e => e.IsError).Select(e => e.Message);

        /// <summary>Logs an error.</summary>
        /// <param name="format">The formatting string for the error.</param>
        /// <param name="args">The arguments to fill out the formatting string.</param>
        public void LogError(string format, params object[] args) =>
            this.entries.Add(new Entry(string.Format(format, args), true));

        /// <summary>Logs a warning.</summary>
        /// <param name="format">The formatting string for the warning.</param>
        /// <param name="args">The arguments to fill out the formatting string.</param>
        public void LogWarning(string format, params object[] args) =>
            this.entries.Add(new Entry(string.Format(format, args), false));

        /// <summary>All the entries of log separated by newlines.</summary>
        /// <returns>The log that was collected as a string.</returns>
        public override string ToString() =>
            this.All.Join(System.Environment.NewLine);
    }
}
