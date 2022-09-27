using System;

namespace PetiteParser.Analyzer;

/// <summary>An exception from Petite Parser's analyzer.</summary>
internal class AnalyzerException : Exception{

    /// <summary>Creates a new analyzer exception.</summary>
    /// <param name="message">The message for the exception.</param>
    public AnalyzerException(string message) : base(message) { }

    /// <summary>Creates a new analyzer exception.</summary>
    /// <param name="message">The message for the exception.</param>
    /// <param name="inner">The inner exception to this exception.</param>
    public AnalyzerException(string message, Exception inner) : base(message, inner) { }
}
