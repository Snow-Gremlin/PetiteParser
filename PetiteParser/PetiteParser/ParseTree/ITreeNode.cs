using System.Collections.Generic;

namespace PetiteParser.ParseTree {

    /// <summary>The handler signature for a method to call for a specific prompt.</summary>
    /// <param name="args">The argument for handling a prompt in the node tree.</param>
    public delegate void PromptHandle(PromptArgs args);

    /// <summary>
    /// The tree node containing reduced rule of the grammar
    /// filled out with tokens and other TreeNodes.
    /// </summary>
    public interface ITreeNode {

        /// <summary>Processes this tree node with the given handles for the prompts to call.</summary>
        /// <param name="handles">The set of handles for the prompt to call.</param>
        /// <param name="args">The optional arguments to use when processing. If null then one will be created.</param>
        void Process(Dictionary<string, PromptHandle> handles, PromptArgs args = null);

        /// <summary>This returns this node and all inner items as an enumerable.</summary>
        IEnumerable<ITreeNode> Nodes { get; }
    }
}
