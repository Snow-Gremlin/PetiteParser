using System;
using System.IO;

namespace PetiteParser.Logger;

/// <summary>This is a logger which writes to the given output or standard out.</summary>
sealed public class Writer : BaseLog {

    /// <summary>The writer to write entires out to.</summary>
    private readonly TextWriter? writer;

    /// <summary>Indicates log levels should be colored when writing to console.</summary>
    private readonly bool useColors;

    /// <summary>Creates a new writer logger.</summary>
    /// <param name="writer">
    /// The writer to write entries out to.
    /// If null, then this will output to standard out.
    /// </param>
    /// <param name="useColors">Indicates log levels should be colored when writing to console.</param>
    /// <param name="next">The next optional logger to pass entries onto.</param>
    public Writer(TextWriter? writer = null, bool useColors = true, ILogger? next = null) : base(next) {
        this.writer    = writer;
        this.useColors = useColors;
    }

    /// <summary>Writes the added entry to the writer.</summary>
    /// <param name="entry">The entry to write.</param>
    protected override void OnAdd(Entry entry) {
        if (this.writer is not null) this.writer.WriteLine(entry);
        else if (!this.useColors) Console.WriteLine(entry);
        else {
            ConsoleColor backupColor = Console.ForegroundColor;
            Console.ForegroundColor = entry.Level switch {
                Level.Notice  => ConsoleColor.Blue,
                Level.Warning => ConsoleColor.Yellow,
                Level.Error   => ConsoleColor.Red,
                _             => backupColor
            };
            Console.Out.WriteLine(entry);
            Console.ForegroundColor = backupColor;
        }
    }
}
