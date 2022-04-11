using PetiteParser.Misc;
using PetiteParser.ParseTree;

namespace PetiteParser.Parser {

    /// <summary>This is the result from a parse of a stream of tokens.</summary>
    public class Result {

        /// <summary>
        /// The tree of the parsed tokens into grammar rules.
        /// This will be null if there are any errors.
        /// </summary>
        public readonly ITreeNode Tree;

        /// <summary>Any errors which occurred during the parse.</summary>
        public readonly string[] Errors;

        /// <summary>Creates a new parser result.</summary>
        /// <param name="tree">The resulting parse tree.</param>
        /// <param name="errors">Any errors which occurred.</param>
        public Result(ITreeNode tree, params string[] errors) {
            this.Tree = tree;
            this.Errors = errors;
        }

        /// <summary>Gets the human-readable debug string for these results.</summary>
        /// <returns>The string for the result.</returns>
        public override string ToString() {
            string result = this.Errors.JoinLines();
            if (this.Tree is not null) result += this.Tree.ToString();
            return result;
        }
    }
}
