using PetiteParser.Grammar;
using PetiteParser.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PetiteParser.ParseTree {

    /// <summary>
    /// The tree node containing reduced rule of the grammar
    /// filled out with tokens and other TreeNodes.
    /// </summary>
    public class RuleNode: ITreeNode {

        private const string treeStart  = "─";
        private const string treeBar    = "  │";
        private const string treeBranch = "  ├─";
        private const string treeSpace  = "   ";
        private const string treeLeaf   = "  └─";

        /// <summary>Creates a new tree node.</summary>
        /// <param name="rule">The rule for this node.</param>
        /// <param name="items">The children items for this node.</param>
        public RuleNode(Rule rule, params ITreeNode[] items) :
            this(rule, items as IEnumerable<ITreeNode>) { }

        /// <summary>Creates a new tree node.</summary>
        /// <param name="rule">The rule for this node.</param>
        /// <param name="items">The children items for this node.</param>
        public RuleNode(Rule rule, IEnumerable<ITreeNode> items) {
            this.Rule = rule;
            this.Items = new List<ITreeNode>(items);
        }

        /// <summary>The grammar rule for this node.</summary>
        public readonly Rule Rule;

        /// <summary>The list of items for this rule.</summary>
        public List<ITreeNode> Items { get; }

        /// <summary>Helps construct the debugging output of the tree.</summary>
        /// <param name="buf">The buffer to write test to.</param>
        /// <param name="indent">The indent for this node.</param>
        /// <param name="first">The indent for the first value in the node.</param>
        private void toTree(StringBuilder buf, string indent, string first) {
            buf.Append(first+'<'+this.Rule.Term.Name+'>');
            if (this.Items.Count > 0) {
                for (int i = 0; i < this.Items.Count - 1; ++i) {
                    ITreeNode item = this.Items[i];
                    string itemFirst = Environment.NewLine+indent+treeBranch;
                    if (item is RuleNode)
                        (item as RuleNode).toTree(buf, indent+treeBar, itemFirst);
                    else buf.Append(itemFirst+item.ToString());
                }

                ITreeNode lastItem = this.Items[^1];
                string lastItemFirst = Environment.NewLine+indent+treeLeaf;
                if (lastItem is RuleNode)
                    (lastItem as RuleNode).toTree(buf, indent+treeSpace, lastItemFirst);
                else buf.Append(lastItemFirst+lastItem.ToString());
            }
        }

        /// <summary>Processes this tree node with the given handles for the prompts to call.</summary>
        /// <param name="handles">The set of handles for the prompt to call.</param>
        public void Process(Dictionary<string, PromptHandle> handles) {
            Stack<ITreeNode> stack = new();
            stack.Push(this);
            PromptArgs args = new();
            while (stack.Count > 0) {
                ITreeNode node = stack.Pop();
                if (node is RuleNode rule) rule.Items.Reverse<ITreeNode>().Foreach(stack.Push);
                else if (node is TokenNode token) args.Tokens.Add(token.Token);
                else if (node is PromptNode prompt) {
                    string promptName = prompt.Prompt;
                    if (!handles.TryGetValue(promptName, out PromptHandle hndl))
                        throw new Misc.Exception("Failed to find the handle for the prompt: "+promptName);
                    args.Prompt = promptName;
                    hndl(args);
                }
            }
        }

        /// <summary>Gets a string for the tree node.</summary>
        /// <returns>The string tree of the rule.</returns>
        public override string ToString() {
            StringBuilder buf = new();
            this.toTree(buf, "", treeStart);
            return buf.ToString();
        }
    }
}
