using PetiteParser.Tokenizer;
using System.Collections.Generic;

namespace PetiteParser.ParseTree {

    /// <summary>
    /// The tree node containing reduced rule of the grammar
    /// filled out with tokens and other TreeNodes.
    /// </summary>
    public class TokenNode: ITreeNode {

        /// <summary>Creates a new token parse tree node.</summary>
        /// <param name="token">The token for this tree node.</param>
        public TokenNode(Token token) {
            this.Token = token;
        }

        /// <summary>The token found at this point in the parse tree.</summary>
        public readonly Token Token;

        /// <summary>Processes this tree node with the given handles for the triggers to call.</summary>
        /// <param name="handles">The handlers for the prompts.</param>
        public void Process(Dictionary<string, TriggerHandle> handles) {
            // Do Nothing, no prompt so there is no effect.
        }

        /// <summary>Gets a string for this tree node.</summary>
        /// <returns>The string for this node.</returns>
        public override string ToString() => "["+this.Token+"]";
    }
}
