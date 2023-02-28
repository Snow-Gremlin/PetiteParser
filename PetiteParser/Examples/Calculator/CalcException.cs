using System;

namespace Examples.Calculator;

/// <summary>An exception from the calculator example.</summary>
sealed public class CalcException : Exception {

    /// <summary>Creates a new exception.</summary>
    /// <param name="message">The message for the exception.</param>
    public CalcException(string message) : base(message) {}
}
