using PetiteParser.Formatting;
using PetiteParser.Logger;
using System;
using System.Collections.Generic;
using System.Text;

namespace PetiteParser.Tokenizer;

/// <summary>The helper is the actual tokenization functions to tokenize input.</summary>
sealed internal class Runner : IDisposable {
    private readonly Scanner.Rescanner scanner;
    private readonly State start;
    private readonly TokenState? errorTokenState;
    private readonly List<Rune> outText;
    private readonly HashSet<string> consume;
    private readonly ILogger? log;

    private Token? lastToken;
    private Token? errorToken;
    private State? state;
    private int lastLength;

    /// <summary>Creates a new tokenizer helper.</summary>
    /// <param name="scanner">The input to get the runes to tokenize.</param>
    /// <param name="start">The start state for the tokenizer.</param>
    /// <param name="errorToken">The token to use for an error.</param>
    /// <param name="consume">The set of tokens names to consume.</param>
    /// <param name="log">The optional logger to get feedback about the token state.</param>
    public Runner(Scanner.IScanner scanner, State? start, TokenState? errorToken, HashSet<string> consume, ILogger? log = null) {
        if (start is null)
            throw new TokenizerException("No start tokenizer state is defined.");

        this.scanner         = new Scanner.Rescanner(scanner);
        this.start           = start;
        this.errorTokenState = errorToken;
        this.outText         = new();
        this.consume         = consume;
        this.log             = log;

        this.lastToken  = null;
        this.errorToken = null;
        this.state      = null;
        this.lastLength = 0;
    }

    /// <summary>Disposes of the scanner used by this runner.</summary>
    public void Dispose() => this.scanner.Dispose();

    /// <summary>Performs a tokenization of the given input scanner and returns the tokens.</summary>
    /// <returns>The tokens found from tokenizing the given input.</returns>
    public IEnumerable<Token> Tokenize() {
        this.state = this.start;
        this.log?.AddInfo("Start Tokenizing");

        // If the start is an accept state, then prepare for an empty token for that state.
        if (this.start.Token is not null) this.setLastToken();

        // Step through all the characters, pushing back and rescanning when needed.
        while (true) {
            Token? token;
            if (this.scanner.MoveNext()) {
                Transition? trans = this.findTransition();
                token = trans is null ?
                    this.processNoTransition() :
                    this.processTransition(trans);
            } else if (this.scanner.ScannedCount > 0) {
                token = this.processNoTransition();
            } else break;
            if (token is not null) yield return token.Value;
        }

        // If an error token has been set, return it now.
        if (this.errorToken is not null) {
            yield return this.errorToken.Value;
            this.errorToken = null;
        }

        // If there is any token previously found, return it now.
        if (this.lastToken is not null) {
            bool consume = this.consume.Contains(this.lastToken.Value.Name);
            if (!consume) yield return this.lastToken.Value;
        }

        this.log?.AddInfo("Finished Tokenizing");
    }

    /// <summary>Find the transition from the current state with the current character.</summary>
    /// <returns>The next transaction or null if there are no transitions.</returns>
    private Transition? findTransition() {
        Transition? trans = this.state?.FindTransition(this.scanner.Current);
        this.log?.AddInfo("Step(state:"+(this.state?.Name ?? "-")+", rune:"+this.scanner.Current+", loc:"+this.scanner.Location+", "+
            (trans is null ? "target:-" : "target:"+trans.Target.Name+", consume:"+trans.Consume)+")");
        return trans;
    }

    /// <summary>
    /// Get the next token for the current token state and scan,
    /// then sets the last token with it.
    /// </summary>
    private void setLastToken() {
        this.lastLength = this.scanner.ScannedCount;
        this.lastToken = this.state?.Token?.GetToken(string.Concat(this.outText), this.scanner.StartLocation, this.scanner.Location);
        this.log?.AddInfo("SetToken(state:"+(this.state?.Name ?? "-")+", token:"+this.lastToken+")");
    }

    /// <summary>
    /// When no transition is found and no prior token has been set, then this is called to push the top
    /// character into the error state and attempt to restart tokenization, unless there is no error token
    /// in which case an exception will be thrown.
    /// </summary>
    private void pushToError() {
        Scanner.Location? start = this.scanner.StartLocation;
        if (this.errorTokenState is null)
            throw new TokenizerException("Input is not tokenizable [state: " + this.state + ", "+
                "location: (" + (start?.ToString() ?? "-") + "), "+
                "length: " + this.scanner.ScannedCount + "]: "+
                "\"" + Text.Escape(string.Concat(this.scanner.ScannedRunes)) + "\"");

        // Remove only one character and push back all the other characters.
        string newText = this.scanner.StartRune.ToString();
        this.scanner.Rescan(1);
        this.lastLength = 0;
        this.outText.Clear();
        this.state = this.start;

        // Creates the error token by extending any existing one.
        this.errorToken = this.errorToken is not null ?
            this.errorTokenState.GetToken(this.errorToken.Value.Text + newText, this.errorToken.Value.Start, start) :
            this.errorTokenState.GetToken(newText, start);
        this.log?.AddInfo("PushToError(token:"+this.errorToken+")");
    }

    /// <summary>Process the current character when the current state has no transition for it.</summary>
    /// <returns>Any tokens which need to be emitted.</returns>
    private Token? processNoTransition() {
        // No transition found with the current state and character.
        // Check if there was a prior token found.
        if (this.lastToken is null) {
            this.pushToError();
            return null;
        }

        // Reset to previous found token's state.
        Token? resultToken = this.lastToken;
        this.lastToken = null;
        this.scanner.Rescan(this.lastLength);
        this.lastLength = 0;
        this.outText.Clear();
        this.state = this.start;

        // Return the previous token, if it is not consumed.
        if (resultToken is null) return null;
        bool consume = this.consume.Contains(resultToken.Value.Name);
        this.log?.AddInfo("YieldAndRescan(retoken:"+this.scanner.RescanCount+", token:"+resultToken.Value.Name+
            ", text:"+resultToken.Value.Text+", loc:["+resultToken.Value.Start+".."+resultToken.Value.End+"], consume:"+consume+")");
        return consume ? null : resultToken;
    }

    /// <summary>Process the current character with the given transition.</summary>
    /// <param name="trans">The non-null transition that should be taken.</param>
    /// <returns>Any tokens which need to be emitted.</returns>
    private Token? processTransition(Transition trans) {
        // Concatenate the current character to the output text, if it isn't consumed.
        if (!trans.Consume) this.outText.Add(this.scanner.Current);

        // Transition to the next state and check if it is an acceptance state.
        this.state = trans.Target;
        if (this.state.Token is null) return null;

        // Set the last reached token.
        this.setLastToken();

        // If an error token has been set, return it now.
        Token? errorToken = this.errorToken;
        this.errorToken = null;
        return errorToken;
    }
}
