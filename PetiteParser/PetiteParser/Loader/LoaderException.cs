using System;

namespace PetiteParser.Loader;

/// <summary>An exception from Petite Parser's loader.</summary>
internal class LoaderException : Exception {

    /// <summary>Creates a new loader exception.</summary>
    /// <param name="message">The message for the exception.</param>
    public LoaderException(string message) : base(message) { }

    /// <summary>Creates a new loader exception.</summary>
    /// <param name="message">The message for the exception.</param>
    /// <param name="inner">The inner exception to this exception.</param>
    public LoaderException(string message, Exception inner) : base(message, inner) { }
}
