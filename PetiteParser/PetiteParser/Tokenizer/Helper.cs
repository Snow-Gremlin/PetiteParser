using PetiteParser.Misc;
using System;
using System.Collections.Generic;
using System.Text;

namespace PetiteParser.Tokenizer {

    /// <summary>The helper is the actual tokenization functions to tokenize input.</summary>
    internal class Helper {
        private readonly Scanner.Rescanner scanner;
        private readonly Watcher watcher;
        private readonly State start;
        private readonly TokenState errorTokenState;
        private readonly List<Rune> outText;
        private readonly HashSet<string> consume;

        private Token lastToken;
        private Token errorToken;
        private State state;
        private int lastLength;

        /// <summary>Creates a new tokenizer helper.</summary>
        /// <param name="scanner">The input to get the runes to tokenize.</param>
        public Helper(Scanner.IScanner scanner, Watcher watcher, State start, TokenState errorToken, HashSet<string> consume) {
            if (start is null)
                throw new Exception("No start tokenizer state is defined.");

            this.scanner         = new Scanner.Rescanner(scanner);
            this.watcher         = watcher;
            this.start           = start;
            this.errorTokenState = errorToken;
            this.outText         = new List<Rune>();
            this.consume         = consume;

            this.lastToken  = null;
            this.errorToken = null;
            this.state      = null;
            this.lastLength = 0;
        }

        /// <summary>Performs a tokenization of the given input scanner and returns the tokens.</summary>
        /// <returns>The tokens found from tokenizing the given input.</returns>
        public IEnumerable<Token> Tokenize() {
            this.state = this.start;
            this.watcher?.StartTokenization();

            // If the start is an accept state, then prepare for an empty token for that state.
            if (this.start.Token is not null) this.setLastToken();

            // Step through all the characters, pushing back and rescanning when needed.
            while (true) {
                Token token;
                if (this.scanner.MoveNext()) {
                    Transition trans = this.findTransition();
                    token = trans is null ?
                        this.processNoTransition() :
                        this.processTransition(trans);
                } else if (this.scanner.ScannedCount > 0) {
                    token = this.processNoTransition();
                } else break;
                if (token is not null) yield return token;
            }

            // If an error token has been set, return it now.
            if (this.errorToken is not null) {
                yield return this.errorToken;
                this.errorToken = null;
            }

            // If there is any token previously found, return it now.
            if (this.lastToken is not null) {
                bool consume = this.consume.Contains(this.lastToken.Name);
                if (!consume) yield return this.lastToken;
            }

            this.watcher?.FinishTokenization();
        }

        /// <summary>Find the transition from the current state with the current character.</summary>
        /// <returns>The next transaction or null if there are no transation.</returns>
        private Transition findTransition() {
            Transition trans = this.state.FindTransition(this.scanner.Current);
            this.watcher?.Step(this.state, this.scanner.Current, this.scanner.Location, trans);
            return trans;
        }

        /// <summary>
        /// Get the next token for the current token state and scan,
        /// then sets the last token with it.
        /// </summary>
        private void setLastToken() {
            this.lastLength = this.scanner.ScannedCount;
            this.lastToken = state.Token.GetToken(string.Concat(this.outText), this.scanner.StartLocation, this.scanner.Location);
            this.watcher?.SetToken(state, this.lastToken);
        }

        /// <summary>
        /// When no transition is found and no prior token has been set, then this is called to push the top
        /// character into the error state and attempt to restart tokenization, unless there is no error token
        /// in which case an exception will be thrown.
        /// </summary>
        private void pushToError() {
            Scanner.Location start = this.scanner.StartLocation;
            if (this.errorTokenState is null)
                throw new Exception("Input is not tokenizable [state: " + this.state + ", "+
                    "location: (" + (start?.ToString() ?? "-") + "), "+
                    "length: " + this.scanner.ScannedCount + "]: "+
                    "\"" + Text.Escape(string.Concat(this.scanner.ScannedRunes)) + "\"");

            string newText = this.scanner.StartRune.ToString();
            this.scanner.Rescan(1);
            this.lastLength = 0;
            this.outText.Clear();
            this.state = this.start;

            this.errorToken = this.errorToken is not null ?
                this.errorTokenState.GetToken(this.errorToken.Text + newText, this.errorToken.Start, start) :
                this.errorTokenState.GetToken(newText, start);
            this.watcher?.PushToError(this.errorToken);
        }

        /// <summary>Process the current character when the current state has no transition for it.</summary>
        /// <returns>Any tokens which need to be emitted.</returns>
        private Token processNoTransition() {
            // No transition found with the current state and character.
            // Check if there was a prior token found.
            if (this.lastToken is null) {
                this.pushToError();
                return null;
            }

            // Reset to previous found token's state.
            Token resultToken = this.lastToken;
            this.lastToken = null;
            this.scanner.Rescan(this.lastLength);
            this.lastLength = 0;
            this.outText.Clear();
            this.state = this.start;

            // Return the previous token, if it is not consumed.
            bool consume = this.consume.Contains(resultToken.Name);
            watcher?.YieldAndRescan(this.scanner.RescanCount, resultToken, consume);
            return consume ? null : resultToken;
        }

        /// <summary>Process the current character with the given transition.</summary>
        /// <param name="trans">The non-null transition that should be taken.</param>
        /// <returns>Any tokens which need to be emitted.</returns>
        private Token processTransition(Transition trans) {
            // Concatinate the current character to the output text, if it isn't consumed.
            if (!trans.Consume) this.outText.Add(this.scanner.Current);

            // Transition to the next state and check if it is an acceptance state.
            this.state = trans.Target;
            if (this.state.Token is null) return null;

            // Set the last reached token.
            this.setLastToken();

            // If an error token has been set, return it now.
            Token errorToken = this.errorToken;
            this.errorToken = null;
            return errorToken;
        }
    }
}
