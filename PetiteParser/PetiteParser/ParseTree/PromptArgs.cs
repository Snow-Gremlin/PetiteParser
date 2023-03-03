using PetiteParser.Scanner;
using PetiteParser.Tokenizer;
using System.Collections.Generic;

namespace PetiteParser.ParseTree;

/// <summary>The argument passed into the prompt handler when it is being called.</summary>
/// <remarks>
/// This is not sealed so that it can be inherited and
/// used to store state information during processing of handles.
/// </remarks>
public class PromptArgs {

    /// <summary>Creates a new prompt argument.</summary>
    public PromptArgs() {
        this.Cancel = false;
        this.Prompt = "";
        this.Tokens = new List<Token>();
    }

    /// <summary>Setting this to true will stop a running prompt processing.</summary>
    public bool Cancel { get; set; }

    /// <summary>The name of the prompt which was last called.</summary>
    /// <remarks>This will be set when the prompt handler is called.</remarks>
    public string Prompt { get; set; }

    /// <summary>The list of recent tokens while processing a tree node.</summary>
    /// <remarks>
    /// This list is editable to inject tokens, remove tokens, or clear the list when
    /// the current tokens are no longer needed, tor example at the end of a statement
    /// when a new statement is starting the list can be cleared since none of the tokens
    /// from the previous statement will be needed anymore.
    /// </remarks>
    public List<Token> Tokens { get; }

    /// <summary>This is end location of the most recent token, so the most recently processed location.</summary>
    /// <remarks>This will return null if there are no tokens in the tokens list.</remarks>
    public Location? LastLocation => this.Recent()?.End ?? null;

    /// <summary>This is the text from the most recent token.</summary>
    /// <remarks>This will return an empty string if there are no tokens in the token list.</remarks>
    public string LastText => this.Recent()?.Text ?? "";

    /// <summary>Gets the recent token offset from most recent by the given index.</summary>
    /// <param name="index">
    /// The index from the top of the stack or recent values.
    /// Where 0 is the most recent, 1 is the next most recent and so on.
    /// </param>
    /// <returns>The top of the stack offset by the index, otherwise null if out-of-bounds.</returns>
    public Token? Recent(int index = 0) =>
        (index >= 0) && (index < this.Tokens.Count) ? this.Tokens[^(index+1)] : null;
}
