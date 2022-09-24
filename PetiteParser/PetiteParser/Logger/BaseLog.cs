using PetiteParser.Misc;

namespace PetiteParser.Logger;

/// <summary>The base logger to help build other logging tools.</summary>
public abstract class BaseLog : ILogger {

    /// <summary>Creates a new base logger.</summary>
    /// <param name="next">The next optional logger to pass entries onto.</param>
    protected BaseLog(ILogger next) {
        this.Failed = false;
        this.Next = next;
    }

    /// <summary>
    /// This is called when clear is called so that the inheriting log can
    /// add specific tasks to perform on clear by overriding this method.
    /// This method is empty, so does not need to be called by the overriding method.
    /// </summary>
    virtual protected void OnClear() { }

    /// <summary>
    /// This is called when an entry is added so that the inheriting log can
    /// add specific tasks to perform on added entries by overriding this method.
    /// This method is empty, so does not need to be called by the overriding method.
    /// </summary>
    /// <param name="entry">The entry to that was added.</param>
    virtual protected void OnAdd(Entry entry) { }

    /// <summary>This is the next logger in a chain of loggers or nil.</summary>
    public ILogger Next;

    /// <summary>
    /// Removes all the entries from the logs which can
    /// be removed and resets the failed flag.
    /// </summary>
    public void Clear() {
        this.Failed = false;
        this.OnClear();
        this.Next?.Clear();
    }

    /// <summary>Indicates that at least one error has occurred.</summary>
    public bool Failed { get; private set; }

    /// <summary>Adds the given entry to the logs.</summary>
    /// <param name="entry">The entry to add to the log.</param>
    public void Add(Entry entry) {
        if (entry.Level == Level.Error) this.Failed = true;
        this.OnAdd(entry);
        this.Next?.Add(entry);
    }

    /// <summary>Adds a new entry and returns the new entry.</summary>
    /// <param name="level">The level of the entry to log.</param>
    /// <param name="message">The message for the entry.</param>
    /// <returns>The new added entry.</returns>
    private Entry addNewEntry(Level level, string message) {
        Entry entry = new(level, message);
        this.Add(entry);
        return entry;
    }

    /// <summary>Logs an entry to the log at the given level.</summary>
    /// <param name="level">The level of the entry to log.</param>
    /// <param name="lines">The text string for the lines of the entry.</param>
    /// <returns>Returns the entry that was created.</returns>
    public Entry Add(Level level, params string[] lines) => this.addNewEntry(level, lines.JoinLines());

    /// <summary>Logs an error.</summary>
    /// <param name="lines">The text string for the lines of the error.</param>
    /// <returns>Returns the error entry that was created.</returns>
    public Entry AddError(params string[] lines) => this.Add(Level.Error, lines);

    /// <summary>Logs a warning.</summary>
    /// <param name="lines">The text string for the lines of the warning.</param>
    /// <returns>Returns the warning entry that was created.</returns>
    public Entry AddWarning(params string[] lines) => this.Add(Level.Warning, lines);

    /// <summary>Logs a notice.</summary>
    /// <param name="lines">The text string for the lines of the notice.</param>
    /// <returns>Returns the notice entry that was created.</returns>
    public Entry AddNotice(params string[] lines) => this.Add(Level.Notice, lines);

    /// <summary>Logs some text for additional information.</summary>
    /// <param name="lines">The text string for the lines of the information.</param>
    /// <returns>Returns the info entry that was created.</returns>
    public Entry AddInfo(params string[] lines) => this.Add(Level.Info, lines);

    /// <summary>Logs an entry to the log at the given level.</summary>
    /// <param name="level">The level of the entry to log.</param>
    /// <param name="format">The formatting string for the entry.</param>
    /// <param name="args">The arguments to fill out the formatting string.</param>
    /// <returns>Returns the entry that was created.</returns>
    public Entry AddF(Level level, string format, params object[] args) => this.addNewEntry(level, string.Format(format, args));

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
}
