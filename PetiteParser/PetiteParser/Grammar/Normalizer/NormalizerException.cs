using System;

namespace PetiteParser.Grammar.Normalizer;

/// <summary>An exception from Petite Parser's normalizer.</summary>
internal class NormalizerException : Exception {

    /// <summary>Creates a new normalizer exception.</summary>
    /// <param name="message">The message for the exception.</param>
    public NormalizerException(string message) : base(message) { }

    /// <summary>Creates a new normalizer exception.</summary>
    /// <param name="message">The message for the exception.</param>
    /// <param name="inner">The inner exception to this exception.</param>
    public NormalizerException(string message, Exception inner) : base(message, inner) { }
}
