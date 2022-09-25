namespace PetiteParser.Logger;

/// <summary>This is a static logger.</summary>
/// <remarks>
/// This shouldn't be used except when trouble shooting a bug so that
/// the logs aren't filled with entries not useful for debugging.
/// </remarks>
static public class GlobalLog {

    /// <summary>The instance of the logger that is being used.</summary>
    static private ILogger log = new Null();

    /// <summary>This gets or sets the logger to use globally.</summary>
    /// <returns>The logger which is currently being used.</returns>
    static public ILogger Log {
        get => log;
        set => log = value ?? new Null();
    }

    /// <summary>
    /// Removes all the entries from the logs which can
    /// be removed and resets the failed flag.
    /// </summary>
    static public void Clear() => log.Clear();

    /// <summary>Indicates that at least one error has occurred.</summary>
    static public bool Failed => log.Failed;

    /// <summary>Logs an error, if a global log is set.</summary>
    /// <param name="text">The text string for the lines of the error.</param>
    /// <returns>Returns the error entry that was created.</returns>
    static public Entry AddError(params string[] text) => log.AddError(text);

    /// <summary>Logs a warning, if a global log is set.</summary>
    /// <param name="text">The text string for the lines of the warning.</param>
    /// <returns>Returns the warning entry that was created.</returns>
    static public Entry AddWarning(params string[] text) => log.AddWarning(text);

    /// <summary>Logs a notice, if a global log is set.</summary>
    /// <param name="text">The text string for the lines of the notice.</param>
    /// <returns>Returns the notice entry that was created.</returns>
    static public Entry AddNotice(params string[] text) => log.AddNotice(text);

    /// <summary>Logs some text for additional information, if a global log is set.</summary>
    /// <param name="text">The text string for the lines of the information.</param>
    /// <returns>Returns the info entry that was created.</returns>
    static public Entry AddInfo(params string[] text) => log.AddInfo(text);

    /// <summary>Logs a formatted error, if a global log is set.</summary>
    /// <param name="format">The formatting string for the error.</param>
    /// <param name="args">The arguments to fill out the formatting string.</param>
    /// <returns>Returns the error entry that was created.</returns>
    static public Entry AddErrorF(string format, params object[] args) => log.AddErrorF(format, args);

    /// <summary>Logs a formatted warning, if a global log is set.</summary>
    /// <param name="format">The formatting string for the warning.</param>
    /// <param name="args">The arguments to fill out the formatting string.</param>
    /// <returns>Returns the warning entry that was created.</returns>
    static public Entry AddWarningF(string format, params object[] args) => log.AddWarningF(format, args);

    /// <summary>Logs a formatted notice, if a global log is set.</summary>
    /// <param name="format">The formatting string for the notice.</param>
    /// <param name="args">The arguments to fill out the formatting string.</param>
    /// <returns>Returns the notice entry that was created.</returns>
    static public Entry AddNoticeF(string format, params object[] args) => log.AddNoticeF(format, args);

    /// <summary>Logs some formatted text for additional information, if a global log is set.</summary>
    /// <param name="format">The formatting string for the info.</param>
    /// <param name="args">The arguments to fill out the formatting string.</param>
    /// <returns>Returns the info entry that was created.</returns>
    static public Entry AddInfoF(string format, params object[] args) => log.AddInfoF(format, args);
}
