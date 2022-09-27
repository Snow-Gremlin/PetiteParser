using System;

namespace PetiteParser.Scanner;

/// <summary>A location in the scanned value.</summary>
/// <param name="Name">The current name of the input.</param>
/// <param name="LineNumber">
/// The number of line separators from the beginning of the input.
/// This starts count with 1 for the first line.
/// </param>
/// <param name="Column">The offset from the beginning of the current line.</param>
/// <param name="Index">The offset from the beginning of the input.</param>
public readonly record struct Location(string Name, int LineNumber = 1, int Column = 0, int Index = 0) {

    /// <summary>Gets a human readable string for this location.</summary>
    /// <returns>The string for this location.</returns>
    public override string ToString() => this.Name+":"+this.LineNumber+", "+this.Column+", "+this.Index;

    /// <summary>Gets the hash code for the location.</summary>
    /// <returns>The location's hash code.</returns>
    public override int GetHashCode() => HashCode.Combine(this.Name, this.Index, this.LineNumber, this.Column);
}
