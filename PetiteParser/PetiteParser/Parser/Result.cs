using PetiteParser.Misc;
using PetiteParser.ParseTree;
using System;
using System.Text;

namespace PetiteParser.Parser {

    /// <summary>This is the result from a parse of a stream of tokens.</summary>
    public class Result {

        /// <summary>Creates a result with only an error.</summary>
        /// <param name="errors">The error message to wrap into a parser result.</param>
        /// <returns>The new error result with the given error message.</returns>
        static public Result Error(string error) =>
            new(null, new string[] { error });

        /// <summary>
        /// The tree of the parsed tokens into grammar rules.
        /// This will be null if there are any errors.
        /// </summary>
        public readonly ITreeNode Tree;

        /// <summary>Any errors which occurred during the parse.</summary>
        public readonly string[] Errors;

        /// <summary>Indicates if there were no errors.</summary>
        public bool Success => this.Errors.Length <= 0;

        /// <summary>Creates a new parser result.</summary>
        /// <param name="tree">The resulting parse tree.</param>
        /// <param name="errors">Any errors which occurred.</param>
        public Result(ITreeNode tree, string[] errors) {
            this.Tree   = tree;
            this.Errors = errors ?? Array.Empty<string>();
        }

        /// <summary>Gets the human-readable debug string for these results.</summary>
        /// <returns>The string for the result.</returns>
        public override string ToString() {
            StringBuilder buf = new();
            this.Errors.Foreach(buf.AppendLine);
            if (this.Tree is not null)
                buf.AppendLine(this.Tree.ToString());
            return buf.ToString().Trim();
        }
    }
}
