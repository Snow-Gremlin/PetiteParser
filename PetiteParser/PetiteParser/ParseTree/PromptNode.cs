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

        /// Processes this tree node with the given handles for the prompts to call.
        public void Process(Dictionary<string, PromptHandle> handles) {
            if (!handles.TryGetValue(this.Prompt, out PromptHandle hndl))
                throw new Misc.Exception("Failed to find the handle for the prompt: "+this.Prompt);
            hndl(new PromptArgs(this.Prompt));
        }

        /// <summary>Gets a string for this tree node.</summary>
        /// <returns>The string for this node.</returns>
        public override string ToString() => "{"+this.Prompt+"}";
    }
}
