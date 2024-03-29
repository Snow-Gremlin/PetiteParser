﻿using PetiteParser.Grammar;
using PetiteParser.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PetiteParser.ParseTree;

/// <summary>
/// The tree node containing reduced rule of the grammar
/// filled out with tokens and other TreeNodes.
/// </summary>
sealed public class RuleNode : ITreeNode {

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
    public Rule Rule { get; }

    /// <summary>The list of items for this rule.</summary>
    public List<ITreeNode> Items { get; }

    /// <summary>Helps construct the debugging output of the tree.</summary>
    /// <param name="buffer">The buffer to write test to.</param>
    /// <param name="indent">The indent for this node.</param>
    /// <param name="first">The indent for the first value in the node.</param>
    private void toTree(StringBuilder buffer, string indent, string first) {
        buffer.Append(first+'<'+this.Rule.Term.Name+'>');
        if (this.Items.Count > 0) {
            for (int i = 0; i < this.Items.Count - 1; ++i) {
                ITreeNode item = this.Items[i];
                string itemFirst = Environment.NewLine+indent+treeBranch;
                if (item is RuleNode rule)
                    rule.toTree(buffer, indent+treeBar, itemFirst);
                else buffer.Append(itemFirst+item.ToString());
            }

            ITreeNode lastItem = this.Items[^1];
            string lastItemFirst = Environment.NewLine+indent+treeLeaf;
            if (lastItem is RuleNode lastRule)
                lastRule.toTree(buffer, indent+treeSpace, lastItemFirst);
            else buffer.Append(lastItemFirst+lastItem.ToString());
        }
    }

    /// <summary>Processes this tree node with the given handle for the prompts to call.</summary>
    /// <param name="promptHandle">The handler to call on each prompt.</param>
    /// <param name="args">The optional arguments to use when processing. If null then one will be created.</param>
    public void Process(PromptHandle promptHandle, PromptArgs? args = null) {
        Stack<ITreeNode> stack = new();
        stack.Push(this);
        args ??= new();
        while (stack.Count > 0 && !args.Cancel) {
            ITreeNode node = stack.Pop();
            if (node is RuleNode rule) rule.Items.Reverse<ITreeNode>().Foreach(stack.Push);
            else if (node is TokenNode token)   token.Process(promptHandle, args);
            else if (node is PromptNode prompt) prompt.Process(promptHandle, args);
        }
    }

    /// <summary>This returns this node and all inner items as an enumerable.</summary>
    public IEnumerable<ITreeNode> Nodes {
        get {
            yield return this;
            foreach (ITreeNode node in this.Items.SelectMany(i => i.Nodes))
                yield return node;
        }
    }

    /// <summary>Gets a string for the tree node.</summary>
    /// <returns>The string tree of the rule.</returns>
    public override string ToString() {
        StringBuilder buffer = new();
        this.toTree(buffer, "", treeStart);
        return buffer.ToString();
    }
}
