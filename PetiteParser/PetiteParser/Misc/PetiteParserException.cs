using System;

namespace PetiteParser.Misc;

/// <summary>An exception from Petite Parser.</summary>
internal class PetiteParserException : Exception{

    /// <summary>Creates a new parser exception.</summary>
    /// <param name="message">The message for the exception.</param>
    public PetiteParserException(string message) : base(message) { }

    /// <summary>Creates a new parser exception.</summary>
    /// <param name="message">The message for the exception.</param>
    /// <param name="inner">The inner exception to this exception.</param>
    public PetiteParserException(string message, Exception inner) : base(message, inner) { }
}
