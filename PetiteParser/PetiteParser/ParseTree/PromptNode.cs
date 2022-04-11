using System.Collections.Generic;

namespace PetiteParser.ParseTree {

    /// <summary>
    /// The tree node containing reduced rule of the grammar
    /// filled out with tokens and other TreeNodes.
    /// </summary>
    public class PromptNode: ITreeNode {

        /// <summary>The prompt name found at this point in the parse tree. </summary>
        public readonly string Prompt;

        /// <summary> Creates a new token parse tree node. </summary>
        /// <param name="prompt">The prompt name for this node.</param>
        public PromptNode(string prompt) {
            this.Prompt = prompt;
        }

        /// <summary>Processes this tree node with the given handles for the prompts to call.</summary>
        /// <param name="handles">The set of handles for the prompt to call.</param>
        /// <param name="args">The optional arguments to use when processing. If null then one will be created.</param>
        public void Process(Dictionary<string, PromptHandle> handles, PromptArgs args = null) {
            if (!handles.TryGetValue(this.Prompt, out PromptHandle hndl))
                throw new Misc.Exception("Failed to find the handle for the prompt: "+this.Prompt);
            args ??= new();
            args.Prompt = this.Prompt;
            hndl(args);
        }

        /// <summary>This returns this node as an enumerable.</summary>
        public IEnumerable<ITreeNode> Nodes { get { yield return this; } }

        /// <summary>Gets a string for this tree node.</summary>
        /// <returns>The string for this node.</returns>
        public override string ToString() => "{"+this.Prompt+"}";
    }
}
