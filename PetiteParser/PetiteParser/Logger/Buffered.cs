using PetiteParser.Formatting;
using System.Collections.Generic;

namespace PetiteParser.Logger;

/// <summary>This is a buffered logger which holds onto the entires which were logged.</summary>
sealed public class Buffered : BaseLog {

    /// <summary>All the log entries.</summary>
    private readonly Queue<Entry> entries;

    /// <summary>Creates a new buffered logger.</summary>
    /// <param name="next">The next optional logger to pass entries onto.</param>
    public Buffered(ILogger? next = null) : base(next) => this.entries = new();

    /// <summary>Gets all the log entries.</summary>
    public IEnumerable<Entry> Entries => this.entries;

    /// <summary>Handles clearing he buffer when the log is cleared.</summary>
    protected override void OnClear() => this.entries.Clear();

    /// <summary>Handles adding the new entry into the buffer.</summary>
    /// <param name="entry">The entry to buffer.</param>
    protected override void OnAdd(Entry entry) => this.entries.Enqueue(entry);

    /// <summary>All the entries of log separated by newlines.</summary>
    /// <returns>The log that was collected as a string.</returns>
    public override string ToString() => this.Entries.JoinLines();
}
