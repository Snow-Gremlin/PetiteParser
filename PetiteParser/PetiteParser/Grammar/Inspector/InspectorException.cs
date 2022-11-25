using System;

namespace PetiteParser.Grammar.Inspector;

/// <summary>An exception from Petite Parser's inspector.</summary>
internal class InspectorException : Exception {

    /// <summary>Creates a new inspector exception.</summary>
    /// <param name="message">The message for the exception.</param>
    public InspectorException(string message) : base(message) { }

    /// <summary>Creates a new inspector exception.</summary>
    /// <param name="message">The message for the exception.</param>
    /// <param name="inner">The inner exception to this exception.</param>
    public InspectorException(string message, Exception inner) : base(message, inner) { }
}
