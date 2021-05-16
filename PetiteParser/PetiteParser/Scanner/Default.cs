using PetiteParser.Misc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace PetiteParser.Scanner {

    /// <summary>A scanner for scanning a bunch of strings or runes.</summary>
    public class Default: IScanner {

        /// <summary>Reads the given resource file from the properties assembly.</summary>
        /// <param name="resourceName">The name of the resource.</param>
        /// <returns>The new scannar.</returns>
        static public Default FromResource(string resourceName) {
            Assembly assembly = Assembly.GetExecutingAssembly();
            using Stream stream = assembly.GetManifestResourceStream(resourceName);
            using StreamReader reader = new(stream);
            Default scanner = new(reader.ReadToEnd());
            scanner.Name = resourceName;
            return scanner;
        }

        /// <summary>Reads the given text file.</summary>
        /// <param name="filePath">The path to the text file to read.</param>
        /// <returns>The new scanner.</returns>
        static public Default FromFile(string filePath) {
            Default scanner = new(File.ReadAllText(filePath));
            scanner.Name = filePath;
            return scanner;
        }

        /// <summary>The enumerator to process and return from this scanner.</summary>
        private IEnumerator<Rune> runes;

        /// <summary>The location helper for this input.</summary>
        private LocationHelper loc;

        /// <summary>Creates a simple scanner for multiple strings.</summary>
        /// <param name="input">The input string to scan.</param>
        public Default(params string[] input) :
            this(input as IEnumerable<string>) { }

        /// <summary>Creates a simple scanner for multiple strings.</summary>
        /// <param name="input">The input strings to scan.</param>
        public Default(IEnumerable<string> input) :
            this(input.Select((i) => i.EnumerateRunes() as IEnumerable<Rune>).Combine()) { }

        /// <summary>Creates a simple scanner for runes.</summary>
        /// <param name="runes">The input runes to scan.</param>
        public Default(IEnumerable<Rune> runes) {
            this.runes = runes.GetEnumerator();
            this.loc = new LocationHelper();
        }

        /// <summary>The current name for the input data.</summary>
        /// <remarks>This can be set to a filepath to set the name in the location of tokens.</remarks>
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
