using PetiteParser.Tokenizer;
using System.Collections.Generic;

namespace PetiteParser.ParseTree;

/// <summary>
/// The tree node containing reduced rule of the grammar
/// filled out with tokens and other TreeNodes.
/// </summary>
sealed public class TokenNode : ITreeNode {

    /// <summary>Creates a new token parse tree node.</summary>
    /// <param name="token">The token for this tree node.</param>
    public TokenNode(Token token) => this.Token = token;

    /// <summary>The token found at this point in the parse tree.</summary>
    public readonly Token Token;

    /// <summary>Processes this tree node with the given handle for the prompts to call.</summary>
    /// <param name="handle">The handler to call on each prompt.</param>
    /// <param name="args">The optional arguments to use when processing. If null then one will be created.</param>
    public void Process(PromptHandle handle, PromptArgs args = null) =>
        args?.Tokens?.Add(this.Token);

    /// <summary>This returns this node as an enumerable.</summary>
    public IEnumerable<ITreeNode> Nodes { get { yield return this; } }

    /// <summary>Gets a string for this tree node.</summary>
    /// <returns>The string for this node.</returns>
    public override string ToString() => "["+this.Token+"]";
}
