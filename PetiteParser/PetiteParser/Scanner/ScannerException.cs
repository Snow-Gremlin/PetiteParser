using System;

namespace PetiteParser.Scanner;

/// <summary>An exception from Petite Parser's scanner.</summary>
internal class ScannerException : Exception{

    /// <summary>Creates a new scanner exception.</summary>
    /// <param name="message">The message for the exception.</param>
    public ScannerException(string message) : base(message) { }

    /// <summary>Creates a new scanner exception.</summary>
    /// <param name="message">The message for the exception.</param>
    /// <param name="inner">The inner exception to this exception.</param>
    public ScannerException(string message, Exception inner) : base(message, inner) { }
}
