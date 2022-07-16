using PetiteParser.Misc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace PetiteParser.Scanner {

    /// <summary>A scanner for scanning several strings or runes.</summary>
    public class Default: IScanner {

        /// <summary>The default name to use for scanners.</summary>
        public const string DefaultName = "Unnamed";

        /// <summary>Reads the given resource file from the properties assembly.</summary>
        /// <param name="resourceName">The name of the resource.</param>
        /// <returns>The new scanner.</returns>
        static public Default FromResource(Assembly assembly, string resourceName) {
            using Stream stream = assembly.GetManifestResourceStream(resourceName);
            return FromStream(stream, resourceName);
        }

        /// <summary>Reads the given stream for this scanner.</summary>
        /// <param name="name">The name for this stream.</param>
        /// <returns>The new scanner.</returns>
        static public Default FromStream(Stream stream, string name = DefaultName) {
            using StreamReader reader = new(stream);
            return FromTextReader(reader, name);
        }

        /// <summary>Reads the given text reader for this scanner.</summary>
        /// <param name="name">The name for this reader.</param>
        /// <returns>The new scanner.</returns>
        static public Default FromTextReader(TextReader reader, string name = DefaultName) =>
            new(reader.ReadToEnd()) { Name = name };

        /// <summary>Reads the given text file.</summary>
        /// <param name="filePath">The path to the text file to read.</param>
        /// <returns>The new scanner.</returns>
        static public Default FromFile(string filePath) =>
            new(File.ReadAllText(filePath)) { Name = filePath };

        /// <summary>The enumerator to process and return from this scanner.</summary>
        private readonly IEnumerator<Rune> runes;

        /// <summary>The location helper for this input.</summary>
        private readonly LocationHelper loc;

        /// <summary>Creates a simple scanner for multiple strings.</summary>
        /// <remarks>This will join strings together with newlines.</remarks>
        /// <param name="input">The input string to scan.</param>
        public Default(params string[] input) :
            this(input as IEnumerable<string>) { }

        /// <summary>Creates a simple scanner for multiple strings.</summary>
        /// <param name="input">The input strings to scan.</param>
        /// <param name="name">The name of the input.</param>
        /// <param name="separator">The string to join the inputs with, by default this is a newline.</param>
        public Default(IEnumerable<string> input, string name = DefaultName, string separator = "\n") :
            this(input.Join(separator).EnumerateRunes(), name) { }

        /// <summary>Creates a simple scanner for runes.</summary>
        /// <param name="runes">The input runes to scan.</param>
        /// <param name="name">The name of the input.</param>
        public Default(IEnumerable<Rune> runes, string name = DefaultName) {
            this.runes = runes.GetEnumerator();
            this.loc = new LocationHelper();
            this.Name = name;
        }

        /// <summary>The current name for the input data.</summary>
        /// <remarks>This can be set to a file path to set the name in the location of tokens.</remarks>
        public string Name {
            get => this.loc.Name;
            set => this.loc.Name = value;
        }

        /// <summary>Moves to the next rune.</summary>
        /// <returns>True if there is another rune, false if at the end.</returns>
        public bool MoveNext() {
            if (!this.runes.MoveNext()) return false;
            this.loc.Step(this.runes.Current);
            return true;
        }

        /// <summary>Resets the scan.</summary>
        public void Reset() {
            this.runes.Reset();
            this.loc.Reset();
        }

        /// <summary>This disposes the scanner.</summary>
        public void Dispose() {
            this.runes.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>Gets the current rune.</summary>
        public Rune Current => this.runes.Current;

        /// <summary>Gets the current rune.</summary>
        object IEnumerator.Current => this.runes.Current;

        /// <summary>Get the current location.</summary>
        public Location Location => this.loc.Location;
    }
}
