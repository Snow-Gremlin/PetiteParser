using System;

namespace PetiteParser.Parser;

/// <summary>An exception from Petite Parser's parser.</summary>
internal class ParserException : Exception {

    /// <summary>Creates a new parser exception.</summary>
    /// <param name="message">The message for the exception.</param>
    public ParserException(string message) : base(message) { }

    /// <summary>Creates a new parser exception.</summary>
    /// <param name="message">The message for the exception.</param>
    /// <param name="inner">The inner exception to this exception.</param>
    public ParserException(string message, Exception inner) : base(message, inner) { }
}
