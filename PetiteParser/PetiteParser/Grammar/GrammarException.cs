using System;

namespace PetiteParser.Grammar;

/// <summary>An exception from Petite Parser's grammar.</summary>
internal class GrammarException : Exception{

    /// <summary>Creates a new grammar exception.</summary>
    /// <param name="message">The message for the exception.</param>
    public GrammarException(string message) : base(message) { }

    /// <summary>Creates a new grammar exception.</summary>
    /// <param name="message">The message for the exception.</param>
    /// <param name="inner">The inner exception to this exception.</param>
    public GrammarException(string message, Exception inner) : base(message, inner) { }
}
