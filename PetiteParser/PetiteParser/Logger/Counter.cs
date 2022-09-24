namespace PetiteParser.Logger;

/// <summary>
/// This is an entry counter logger which only keeps count of the types of entries
/// that are logged to it but does not keep nor write the entries itself.
/// </summary>
sealed public class Counter : BaseLog {

    /// <summary>Creates a new buffered logger.</summary>
    /// <param name="next">The next optional logger to pass entries onto.</param>
    public Counter(ILogger next = null) : base(next) => this.OnClear();

    /// <summary>The number of error entries which have been logged.</summary>
    public int ErrorCount { get; private set; }

    /// <summary>The number of warning entries which have been logged.</summary>
    public int WarningCount { get; private set; }

    /// <summary>The number of notice entries which have been logged.</summary>
    public int NoticeCount { get; private set; }

    /// <summary>The number of info entries which have been logged.</summary>
    public int InfoCount { get; private set; }

    /// <summary>The total count of all entries that have been logged.</summary>
    public int TotalCount { get; private set; }

    /// <summary>This gets the count for the given level.</summary>
    /// <param name="level">The level to get the count for.</param>
    /// <returns>The count for the given level.</returns>
    public int Count(Level level) =>
        level switch {
            Level.Error   => this.ErrorCount,
            Level.Warning => this.WarningCount,
            Level.Notice  => this.NoticeCount,
            Level.Info    => this.InfoCount,
            _             => 0
        };

    /// <summary>Handles clearing he buffer when the log is cleared.</summary>
    protected override void OnClear() {
        this.ErrorCount   = 0;
        this.WarningCount = 0;
        this.NoticeCount  = 0;
        this.InfoCount    = 0;
        this.TotalCount   = 0;
    }

    /// <summary>Handles adding the new entry into the buffer.</summary>
    /// <param name="entry">The entry to buffer.</param>
    protected override void OnAdd(Entry entry) {
        ++this.TotalCount;
        switch (entry.Level) {
            case Level.Error: ++this.ErrorCount; break;
            case Level.Warning: ++this.WarningCount; break;
            case Level.Notice: ++this.NoticeCount; break;
            case Level.Info: ++this.InfoCount; break;
        }
    }

    /// <summary>All the entries of log separated by newlines.</summary>
    /// <returns>The log that was collected as a string.</returns>
    public override string ToString() {
        System.Text.StringBuilder buf = new();
        buf.Append(this.TotalCount + " Total");
        if (this.ErrorCount   > 0) buf.Append(", " + this.ErrorCount   + " Errors");
        if (this.WarningCount > 0) buf.Append(", " + this.WarningCount + " Warnings");
        if (this.NoticeCount  > 0) buf.Append(", " + this.NoticeCount  + " Notices");
        if (this.InfoCount    > 0) buf.Append(", " + this.InfoCount    + " Info");
        return buf.ToString();
    }
}
