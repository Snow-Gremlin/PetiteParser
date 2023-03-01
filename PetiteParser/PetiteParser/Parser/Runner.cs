using PetiteParser.Formatting;
using PetiteParser.Grammar;
using PetiteParser.ParseTree;
using PetiteParser.Table;
using PetiteParser.Tokenizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PetiteParser.Parser;

/// <summary>The runner performs a parse step by step as tokens are added.</summary>
internal class Runner {
    private readonly Table.Table table;
    private readonly TokenItem errTokenItem;
    private readonly int errorCap;
    private readonly List<string> errors;
    private readonly Stack<ITreeNode> itemStack;
    private readonly Stack<int> stateStack;
    private bool accepted;

    /// <summary>Creates a new runner, only the parser may create a runner.</summary>
    /// <param name="table">The table to read from.</param>
    /// <param name="errTokenItem">The token item to use for error tokens, or null for no error tokens handling.</param>
    /// <param name="errorCap">The limit to the number of errors to allow before stopping.</param>
    public Runner(Table.Table table, TokenItem errTokenItem, int errorCap = 0) {
        this.table        = table;
        this.errTokenItem = errTokenItem;
        this.errorCap     = errorCap;
        this.errors       = new List<string>();
        this.itemStack    = new Stack<ITreeNode>();
        this.stateStack   = new Stack<int>();
        this.stateStack.Push(0);
        this.accepted     = false;
    }

    /// <summary>Gets the results from the runner.</summary>
    public Result Result {
        get {
            if (!this.accepted) {
                this.errors.Add("Unexpected end of input.");
                return new Result(null, this.errors.ToArray());
            }
            return new Result(this.itemStack.Pop(), this.errors.ToArray());
        }
    }

    /// <summary>Determines if the error limit has been reached.</summary>
    private bool errorLimitReached =>
        (this.errorCap > 0) && (this.errors.Count >= this.errorCap);

    /// <summary>Handles when a default error action has been reached.</summary>
    /// <param name="curState">The current state.</param>
    /// <param name="token">The current token.</param>
    /// <returns>True to continue, false to stop.</returns>
    private bool nullAction(int curState, Token token) {
        List<string> tokens = this.table.GetAllTokens(curState);
        this.errors.Add("Unexpected item, ["+token+"], in state "+
            curState+". Expected: "+tokens.Join(", ")+".");
        return !this.errorLimitReached;
    }

    /// <summary>Handles when a specified error action has been reached.</summary>
    /// <param name="action">The error action being processed.</param>
    /// <returns>True to continue, false to stop.</returns>
    private bool errorAction(Error action) {
        this.errors.Add(action.Message);
        return !this.errorLimitReached;
    }

    /// <summary>Handles when a shift action has been reached.</summary>
    /// <param name="action">The shift action being processed.</param>
    /// <param name="token">The current token.</param>
    /// <returns>Always returns true.</returns>
    private bool shiftAction(Shift action, Token token) {
        this.itemStack.Push(new TokenNode(token));
        this.stateStack.Push(action.State);
        return true;
    }

    /// <summary>Handles when a reduce action has been reached.</summary>
    /// <param name="action">The reduce action being processed.</param>
    /// <param name="token">The current token.</param>
    /// <returns>True to continue, false to stop.</returns>
    private bool reduceAction(Reduce action, Token token) {
        // Pop the items off the stack for this action.
        // Also check that the items match the expected rule.
        int count = action.Rule.Items.Count;
        List<ITreeNode> items = new();
        for (int i = count - 1; i >= 0; i--) {
            Item ruleItem = action.Rule.Items[i];
            if (ruleItem is Prompt) {
                items.Insert(0, new PromptNode(ruleItem.Name));
                continue;
            }

            // Pop one item off the stack and push it into the items.
            this.stateStack.Pop();
            ITreeNode item = this.itemStack.Pop();
            items.Insert(0, item);

            // Check if the popped value is valid.
            if (ruleItem is Term) {
                if (item is RuleNode) {
                    if (ruleItem.Name != (item as RuleNode).Rule.Term.Name)
                        throw new Exception("The action, "+action+", could not reduce item "+i+", "+item+": the term names did not match.");
                    // else found a rule with the correct name, continue.
                } else throw new Exception("The action "+action+" could not reduce item "+i+", "+item+": the item is not a rule node.");
            } else { // if (ruleItem is Grammar.TokenItem) {
                if (item is TokenNode) {
                    if (ruleItem.Name != (item as TokenNode).Token.Name)
                        throw new Exception("The action "+action+" could not reduce item "+i+", "+item+": the token names did not match.");
                    // else found a token with the correct name, continue.
                } else throw new Exception("The action "+action+" could not reduce item "+i+", "+item+": the item is not a token node.");
            }
        }

        // Create a new item with the items for this
        // rule in it and put it onto the stack.
        RuleNode node = new(action.Rule, items);
        this.itemStack.Push(node);

        // Use the state reduced back to and the new item to seek,
        // via the goto table, the next state to continue from.
        IAction nextAction = this.table.ReadGoto(this.stateStack.Peek(), node.Rule.Term.Name);
        if (nextAction is not null) {
            if (nextAction is Goto gotoAction) this.stateStack.Push(gotoAction.State);
            else throw new Exception("Unexpected goto type: "+nextAction);
        }

        // Continue with parsing the current token.
        return this.Add(token);
    }

    /// <summary>Handles when an accept has been reached.</summary>
    /// <returns>Always returns true.</returns>
    private bool acceptAction() {
        this.accepted = true;
        return true;
    }

    /// <summary>Inserts the next look ahead token into the parser.</summary>
    /// <param name="token">The token to add.</param>
    /// <returns>True to continue, false to stop.</returns>
    public bool Add(Token token) {
        if (this.accepted) {
            this.errors.Add("unexpected token after end: "+token);
            return false;
        }

        if (this.errTokenItem is not null && token.Name == this.errTokenItem.Name) {
            this.errors.Add("received an error token: "+token);
            return true;
        }

        int curState = this.stateStack.Peek();
        IAction action = this.table.ReadShift(curState, token.Name);

        return action is null ? this.nullAction(curState, token) :
            action is Shift   ? this.shiftAction(action as Shift, token) :
            action is Reduce  ? this.reduceAction(action as Reduce, token) :
            action is Accept  ? this.acceptAction() :
            action is Error   ? this.errorAction(action as Error) :
            throw new Exception("Unexpected action type: "+action);
    }

    /// <summary>Gets a string for the current parser stack.</summary>
    /// <returns>The debug string for the runner.</returns>
    public override string ToString() {
        StringBuilder buf = new();
        int[] states = this.stateStack.ToArray();
        ITreeNode[] items = this.itemStack.ToArray();

        int max = Math.Max(items.Length, states.Length);
        for (int i = 0; i < max; ++i) {
            if (i != 0) buf.Append(", ");
            bool hasState = false;
            if (i < states.Length) {
                buf.Append(states[i]);
                hasState = true;
            }
            if (i < items.Length) {
                if (hasState) buf.Append(':');
                ITreeNode item = items[i];
                buf.Append(
                    item is null       ? "null" :
                    item is RuleNode   ? "<"+(item as RuleNode).Rule.Term.Name+">" :
                    item is TokenNode  ? "["+(item as TokenNode).Token.Name+"]" :
                    item is PromptNode ? "{"+(item as PromptNode).Prompt+"}" :
                    "unknown");
            }
        }
        return buf.ToString();
    }
}
