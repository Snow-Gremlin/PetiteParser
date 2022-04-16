using PetiteParser.Misc;
using System.Collections.Generic;
using System.Text;

namespace PetiteParser.Tokenizer {

    /// <summary>The helper is a collection of data and methods to help with tokenization.</summary>
    internal class Helper {
        private readonly Scanner.IScanner scanner;
        private int lastLength;
        private List<Rune> outText;
        private List<Rune> allInput;
        private List<Rune> retoken;
        private List<Scanner.Location> allLocs;
        private List<Scanner.Location> relocs;
        private Rune cur;
        private Scanner.Location curLoc;

        /// <summary>Creates a new tokenizer helper.</summary>
        /// <param name="scanner">The input to get the runes to tokenize.</param>
        public Helper(Scanner.IScanner scanner) {
            this.scanner    = scanner;
            this.lastLength = 0;
            this.outText    = new List<Rune>();
            this.allInput   = new List<Rune>();
            this.retoken    = new List<Rune>();
            this.allLocs    = new List<Scanner.Location>();
            this.relocs     = new List<Scanner.Location>();
        }

        /// <summary>Advances the tokenizer to the next character to tokenize.</summary>
        /// <remarks>If there are any characters which needed to be retoken, those are pulled from first.</remarks>
        /// <returns>True if there was more characters, false if done reading.</returns>
        public bool MoveNext() {
            if (this.retoken.Count > 0) {
                this.cur = this.retoken[0];
                this.retoken.RemoveAt(0);

                this.curLoc = this.relocs[0];
                this.relocs.RemoveAt(0);
            } else {
                if (!this.scanner.MoveNext()) return false;
                this.cur = this.scanner.Current;
                this.curLoc = this.scanner.Location;
            }
            this.allInput.Add(this.cur);
            this.allLocs.Add(this.curLoc);
            return true;
        }

        /// <summary>The current character being processed.</summary>
        public Rune Current => this.cur;

        /// <summary>The current location being processed.</summary>
        public Scanner.Location CurrentLocation => this.curLoc;

        /// <summary>Indicates if there are any characters which have ben preccessed but not tokenized yet.</summary>
        public bool HasAllInput => this.allInput.Count > 0;

        /// <summary>The number of characters which havebeen pushed back and not processed again.</summary>
        public int RetokenCount => this.retoken.Count;

        /// <summary>Adds the current character to the output string.</summary>
        public void AddCurrentToOutput() => this.outText.Add(this.cur);

        /// <summary>Gets the next token for the given token state and current state.</summary>
        /// <param name="state">The token state to produce a token with.</param>
        /// <returns>The next token to return.</returns>
        /// <exception cref="Exception">
        /// An expection will be thrown indicating that an input can not be
        /// tokenized is thrown if the given state is null.
        /// </exception>
        public Token GetToken(TokenState state) {
            string text = string.Concat(this.outText);
            Scanner.Location start = this.allLocs[0];
            Scanner.Location end   = this.allLocs[^1];
            this.lastLength = this.allInput.Count;
            return state is not null ? state.GetToken(text, start, end) :
                throw new Exception("Input is not tokenizable [state: " + state + ", "+
                    "location: (" + (start?.ToString() ?? "-") + "), "+
                    "length: " + this.lastLength + "]: \"" + Text.Escape(text) + "\"");
        }

        /// <summary>
        /// This will push all the characters not part of the prior token back into
        /// the characters to process so that they can be retokenized.
        /// </summary>
        public void Pushback() {
            this.allInput.RemoveRange(0, lastLength);
            this.retoken.AddRange(allInput);
            this.allInput.Clear();

            this.allLocs.RemoveRange(0, lastLength);
            this.relocs.AddRange(allLocs);
            this.allLocs.Clear();

            this.outText.Clear();
            this.lastLength = 0;
        }
    }
}
