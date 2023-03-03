using PetiteParser.Formatting;

namespace PetiteParser.Logger;

/// <summary>This is a logger which writes to the given wrapped logger with an indent.</summary>
sealed public class Indented : BaseLog {

    /// <summary>The logger to pass all indented entries to.</summary>
    private readonly ILogger wrapped;

    /// <summary>The indent to apply to the entries.</summary>
    private readonly string indent;

    /// <summary>Creates a new logger which indents any entry before passing it to the wrapped logger.</summary>
    /// <param name="wrapped">The logger to pass all indented entries to.</param>
    /// <param name="indent">The indent to apply to the entries.</param>
    public Indented(ILogger wrapped, string indent = "  ") {
        this.wrapped = wrapped;
        this.indent  = indent;
    }

    /// <summary>Writes the added entry to the writer.</summary>
    /// <param name="entry">The entry to write.</param>
    protected override void OnAdd(Entry entry) =>
        this.wrapped.Add(new Entry(entry.Level, entry.Message.IndentLines(this.indent)));
}
