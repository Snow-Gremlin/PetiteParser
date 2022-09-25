namespace PetiteParser.Logger;

/// <summary>This is a logger which writes to the given output or standard out.</summary>
sealed public class Writer : BaseLog {

    /// <summary>The writer to write entires out to.</summary>
    private readonly System.IO.TextWriter writer;

    /// <summary>Creates a new writer logger.</summary>
    /// <param name="writer">
    /// The writer to write entries out to.
    /// If null, then this will output to standard out.
    /// </param>
    /// <param name="next">The next optional logger to pass entries onto.</param>
    public Writer(System.IO.TextWriter? writer = null, ILogger? next = null) : base(next) =>
        this.writer = writer ?? System.Console.Out;

    /// <summary>Writes the added entry to the writer.</summary>
    /// <param name="entry">The entry to write.</param>
    protected override void OnAdd(Entry entry) => this.writer.WriteLine(entry);
}
