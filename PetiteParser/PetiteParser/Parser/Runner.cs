using PetiteParser.Formatting;
using PetiteParser.Grammar;
using PetiteParser.Logger;
using PetiteParser.Parser.Table;
using PetiteParser.ParseTree;
using PetiteParser.Tokenizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PetiteParser.Parser;

/// <summary>The runner performs a parse step by step as tokens are added.</summary>
sealed internal class Runner {

    /// <summary>The maximum attempts to add a single token.</summary>
    /// <remarks>
    /// When adding a token a reduce action will need the add the token again with the new stack.
    /// If something goes wrong and that re-adding the same token gets stuck in a loop, this will kill it.
    /// </remarks>
    private const int maxAddAttempts = 300;

    private readonly Table.Table table;
    private readonly TokenItem? errTokenItem;
    private readonly int errorCap;
    private readonly ILogger? log;

    private readonly List<string> errors;
    private readonly Stack<ITreeNode> itemStack;
    private readonly Stack<int> stateStack;
    private bool reworkToken;
    private bool accepted;

    /// <summary>Creates a new runner, only the parser may create a runner.</summary>
    /// <param name="table">The table to read from.</param>
    /// <param name="errTokenItem">The token item to use for error tokens, or null for no error tokens handling.</param>
    /// <param name="errorCap">The limit to the number of errors to allow before stopping.</param>
    /// <param name="log">Is the optional log to record information about the run with.</param>
    public Runner(Table.Table table, TokenItem? errTokenItem, int errorCap = 0, ILogger? log = null) {
        this.table        = table;
        this.errTokenItem = errTokenItem;
        this.errorCap     = errorCap;
        this.log          = log;

        this.errors       = new();
        this.itemStack    = new();
        this.stateStack   = new();
        this.reworkToken  = false;
        this.accepted     = false;

        this.stateStack.Push(0);
        this.log?.AddInfo("Starting parse");
    }

    /// <summary>Gets the results from the runner.</summary>
    public Result Result {
        get {
            this.log?.AddInfo("Getting parse result");
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

    /// <summary>This adds an error to the errors from the parse.</summary>
    /// <param name="message">The message for the error.</param>
    private void addError(string message) {
        this.errors.Add(message);
        this.log?.AddInfoF("    Add Error: {0}", message);
    }

    /// <summary>Handles when a default error action has been reached.</summary>
    /// <param name="curState">The current state.</param>
    /// <param name="token">The current token.</param>
    /// <returns>True to continue, false to stop.</returns>
    private bool nullAction(int curState, Token token) {
        List<string> tokens = this.table.GetAllTokens(curState);
        this.addError("Unexpected item, ["+token+"], in state "+curState+". Expected: "+tokens.Join(", ")+".");
        return !this.errorLimitReached;
    }

    /// <summary>Handles when a specified error action has been reached.</summary>
    /// <param name="action">The error action being processed.</param>
    /// <returns>True to continue, false to stop.</returns>
    private bool errorAction(Error action) {
        this.addError(action.Message);
        return !this.errorLimitReached;
    }

    /// <summary>Handles when a shift action has been reached.</summary>
    /// <param name="action">The shift action being processed.</param>
    /// <param name="token">The current token.</param>
    /// <returns>Always returns true.</returns>
    private bool shiftAction(Shift action, Token token) {
        this.itemStack.Push(new TokenNode(token));
        this.stateStack.Push(action.State);
        this.log?.AddInfoF("    Shift to State {0} for {1}.", action.State, token);
        return true;
    }

    /// <summary>Handles when a reduce action has been reached.</summary>
    /// <param name="action">The reduce action being processed.</param>
    /// <param name="token">The current token.</param>
    /// <returns>True to continue, false to stop.</returns>
    private bool reduceAction(Reduce action, Token token) {
        this.log?.AddInfoF("    Reduce with rule {0} for {1}", action.Rule, token);

        // Pop the items off the stack for this action.
        // Also check that the items match the expected rule.
        int count = action.Rule.Items.Count;
        List<ITreeNode> items = new();
        for (int i = count - 1; i >= 0; --i) {
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
                if (item is RuleNode ruleNode) {
                    if (ruleItem.Name != ruleNode.Rule.Term.Name)
                        throw new ParserException("The action, "+action+", could not reduce item "+i+", "+item+": the term names did not match.");
                    // else found a rule with the correct name, continue.
                } else throw new ParserException("The action "+action+" could not reduce item "+i+", "+item+": the item is not a rule node.");
            } else { // if (ruleItem is Grammar.TokenItem) {
                if (item is TokenNode tokenNode) {
                    if (ruleItem.Name != tokenNode.Token.Name)
                        throw new ParserException("The action "+action+" could not reduce item "+i+", "+item+": the token names did not match.");
                    // else found a token with the correct name, continue.
                } else throw new ParserException("The action "+action+" could not reduce item "+i+", "+item+": the item is not a token node.");
            }
        }

        // Create a new item with the items for this
        // rule in it and put it onto the stack.
        RuleNode node = new(action.Rule, items);
        this.itemStack.Push(node);

        // Use the state that was reduced back to, and the new item to seek,
        // via the goto table, the next state to continue from.
        int gotoState = this.table.ReadGoto(this.stateStack.Peek(), node.Rule.Term.Name);
        if (gotoState >= 0) {
            this.stateStack.Push(gotoState);
            this.log?.AddInfoF("    Goto state {0} for {1}", gotoState, node.Rule.Term);
        }

        // Continue with parsing the current token.
        this.reworkToken = true;
        return true;
    }

    /// <summary>Handles when an accept has been reached.</summary>
    /// <returns>Always returns true.</returns>
    private bool acceptAction() {
        this.accepted = true;
        this.log?.AddInfo("    Accepted");
        return true;
    }

    /// <summary>Handles when there is a conflict in the table.</summary>
    /// <param name="conflict">The conflict action.</param>
    /// <param name="curState">The current state being run.</param>
    /// <param name="token">The token currently being processed.</param>
    /// <returns>True to continue parsing, false to stop.</returns>
    private bool conflictAction(Conflict conflict, int curState, Token token) {

        // TODO: Need to handle conflicts better than just taking only the first pass.
        //       Maybe make a copy of the runner at this point and run it with the first
        //       action, if that fails, rollback then try the other action.

        IAction action = conflict.Actions.First().Value;
        this.log?.AddInfoF("    Conflict: {0}", conflict);
        this.log?.AddInfoF("      Taking: {0}", action);
        return this.performAction(action, curState, token);
    }

    /// <summary>Performs the given action from the table.</summary>
    /// <param name="action">The action to perform.</param>
    /// <param name="curState">The current state being run.</param>
    /// <param name="token">The token currently being processed.</param>
    /// <returns>True to continue parsing, false to stop.</returns>
    private bool performAction(IAction? action, int curState, Token token) =>
        action is null              ? this.nullAction(curState, token) :
        action is Shift    shift    ? this.shiftAction(shift, token) :
        action is Reduce   reduce   ? this.reduceAction(reduce, token) :
        action is Accept            ? this.acceptAction() :
        action is Error    error    ? this.errorAction(error) :
        action is Conflict conflict ? this.conflictAction(conflict, curState, token) :
        throw new ParserException("Unexpected action type: "+action);

    /// <summary>Inserts the next look ahead token into the parser.</summary>
    /// <param name="token">The token to add.</param>
    /// <returns>True to continue, false to stop.</returns>
    public bool Add(Token token) {
        if (this.accepted) {
            this.addError("unexpected token after end: "+token);
            return false;
        }

        if (this.errTokenItem is not null && token.Name == this.errTokenItem.Name) {
            this.addError("received an error token: "+token);
            return true;
        }

        int attempts = maxAddAttempts;
        do {

            int curState = this.stateStack.Peek();
            this.log?.AddInfoF("  Adding {0} while in State {1}", token, curState);
            IAction? action = this.table.ReadShift(curState, token.Name);
            this.reworkToken = false;

            bool keepParsing = this.performAction(action, curState, token);
            if (!keepParsing) return false;
            if (!this.reworkToken) return true;

            --attempts;
        } while (attempts > 0);

        throw new ParserException("Too many attempts ("+maxAddAttempts+") while "+
            "reworking token, "+token+", at state "+this.stateStack.Peek()+".");
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
                    item is null                  ? "null" :
                    item is RuleNode   ruleNode   ? "<"+ruleNode.Rule.Term.Name+">" :
                    item is TokenNode  tokenNode  ? "["+tokenNode.Token.Name+"]" :
                    item is PromptNode promptNode ? "{"+promptNode.Prompt+"}" :
                    "unknown");
            }
        }
        return buf.ToString();
    }
}
