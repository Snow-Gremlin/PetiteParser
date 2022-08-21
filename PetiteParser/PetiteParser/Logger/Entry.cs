namespace PetiteParser.Logger {

    /// <summary>This is an entry in the log containing a single message.</summary>
    /// <param name="Level">The level of this entry.</param>
    /// <param name="Message">This is the message of the entry.</param>
    public readonly record struct Entry(Level Level, string Message) {

        /// <summary>Gets a string for the given entry.</summary>
        /// <returns>The string for the given entry.</returns>
        override public string ToString() => this.Level.ToString() + ": "+ this.Message;
    }
}
