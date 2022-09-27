using System;

namespace PetiteParser.Tokenizer;

/// <summary>An exception from Petite Parser's tokenizer.</summary>
internal class TokenizerException : Exception{

    /// <summary>Creates a new tokenizer exception.</summary>
    /// <param name="message">The message for the exception.</param>
    public TokenizerException(string message) : base(message) { }

    /// <summary>Creates a new tokenizer exception.</summary>
    /// <param name="message">The message for the exception.</param>
    /// <param name="inner">The inner exception to this exception.</param>
    public TokenizerException(string message, Exception inner) : base(message, inner) { }
}
