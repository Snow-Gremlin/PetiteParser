using System;

namespace PetiteParser.Scanner {

    /// <summary>A location in the scanned value.</summary>
    public class Location {

        /// <summary>The current name of the input.</summary>
        /// <remarks>This maybe the filename the input came from.</remarks>
        readonly public string Name;

        /// <summary>The number of line separators from the beginning of the input.</summary>
        /// <remarks>This starts count with 1 for the first line.</remarks>
        readonly public int LineNumber;

        /// <summary>The offset since the last line separator.</summary>
        readonly public int Column;

        /// <summary>The offset from the beginning of the input.</summary>
        readonly public int Index;

        /// <summary>Creates a new location.</summary>
        /// <param name="name">The current name of the input.</param>
        /// <param name="lineNumber">The line number.</param>
        /// <param name="column">The offset from the beginning of the current line.</param>
        /// <param name="index">The offset from the beginning of the input.</param>
        public Location(string name, int lineNumber = 1, int column = 0, int index = 0) {
            this.Name = name;
            this.LineNumber = lineNumber;
            this.Column = column;
            this.Index = index;
        }

        /// <summary>Gets a human readable string for this location.</summary>
        /// <returns>The string for this location.</returns>
        public override string ToString() =>
            this.Name+":"+this.LineNumber+", "+this.Column+", "+this.Index;

        /// <summary>Checks if the given object is equal to this location.</summary>
        /// <param name="obj">The object to check.</param>
        /// <returns>True if they are equal, false otherwise.</returns>
        public override bool Equals(object obj) =>
            obj is Location loc &&
            this.Name == loc.Name &&
            this.LineNumber == loc.LineNumber &&
            this.Column == loc.Column &&
            this.Index == loc.Index;

        /// <summary>Gets the hash code for the location.</summary>
        /// <returns>The location's hash code.</returns>
        public override int GetHashCode() =>
            HashCode.Combine(this.Name, this.Index, this.LineNumber, this.Column);
    }
}
