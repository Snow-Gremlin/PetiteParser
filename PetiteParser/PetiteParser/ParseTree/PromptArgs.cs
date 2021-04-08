using PetiteParser.Tokenizer;
using System.Collections.Generic;

namespace PetiteParser.ParseTree {

    /// <summary>The argument passed into the prompt handler when it is being called.</summary>
    public class PromptArgs {

        /// <summary>Creates a new prompt argument.</summary>
        public PromptArgs() {
            this.Tokens = new List<Token>();
        }

        /// <summary>The list of recent tokens while processing a tree node.</summary>
        public List<Token> Tokens { get; }

        /// <summary>Gets the recent token offset from most recent by the given index.</summary>
        /// <param name="index">The index from the top of the stack or recent values.</param>
        /// <returns>The top of the stack offset by the index, otherwise null if out-of-bounds.</returns>
        public Token Recent(int index) =>
            (index > 0) && (index <= this.Tokens.Count) ? this.Tokens[^index] : null;
    }
}
