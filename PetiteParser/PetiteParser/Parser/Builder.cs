using PetiteParser.Grammar;
using PetiteParser.Misc;
using PetiteParser.Table;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PetiteParser.Parser {

    /// <summary>This is a builder used to generate a parser giving a grammar.</summary>
    internal class Builder {
        public const string StartTerm    = "$StartTerm";
        public const string EofTokenName = "$EOFToken";

        private readonly Grammar.Grammar grammar;
        private readonly HashSet<Item> items;
        private readonly Analyzer.Analyzer analyzer;
        private readonly Logger.Log log;

        /// <summary>Constructs of a new parser builder.</summary>
        /// <param name="grammar">The grammar to build.</param>
        /// <param name="log">The optional logger to log the steps the builder has performed.</param>
        public Builder(Grammar.Grammar grammar, Logger.Log log = null) {
            this.grammar  = grammar;
            Term oldStart = this.grammar.StartTerm;
            this.grammar.Start(StartTerm);
            this.grammar.NewRule(StartTerm).AddTerm(oldStart.Name).AddToken(EofTokenName);
            this.States   = new();
            this.items    = new();
            this.Table    = new();
            this.BuildLog = new();
            this.analyzer = new(this.grammar);
            this.log      = log;

            foreach (Term term in this.grammar.Terms) {
                this.items.Add(term);
                foreach (Rule rule in term.Rules) {
                    foreach (Item item in rule.Items) {
                        if (item is not Prompt) this.items.Add(item);
                    }
                }
            }
        }

        /// <summary>Gets the error log for any errors which occurred during the build.</summary>
        public readonly Logger.Log BuildLog;

        /// <summary>The table from the builder.</summary>
        public readonly Table.Table Table;

        /// <summary>The set of states for the parser.</summary>
        public readonly List<State> States;

        /// <summary>Finds a state with the given fragment.</summary>
        /// <param name="fragment">The fragment to find.</param>
        /// <returns>The found state or null.</returns>
        public State FindState(Fragment fragment) =>
            this.States.FirstOrDefault(state => state.HasFragment(fragment));

        /// <summary>Determines all the parser states for the grammar.</summary>
        public void DetermineStates() {
            // Create the first state, state 0.
            State startState = new(0);
            TokenItem eof = new(EofTokenName);
            foreach (Rule rule in this.grammar.StartTerm.Rules)
                startState.AddFragment(new Fragment(rule, 0, eof), this.analyzer);
            this.States.Add(startState);
            this.log?.AddInfo("Created initial start state:" +
                System.Environment.NewLine + "  " + startState.ToString("  "));

            // Fill out all other states.
            HashSet<State> changed = new() { startState };
            while (changed.Count > 0) {
                State state = changed.First();
                changed.Remove(state);
                this.NextStates(state).Foreach(changed.Add);
            }
        }

        /// <summary>Determines the next state with the given fragment from the given state.</summary>
        /// <param name="state">The state that is being followed.</param>
        /// <param name="fragment">The fragment from the state to follow.</param>
        /// <param name="changed">The states which have been changed.</param>
        private void determineNextStateFragment(State state, Fragment fragment, HashSet<State> changed) {
            Rule rule = fragment.Rule;
            int index = fragment.Index;

            // If there are any items left in this fragment get it or leave.
            Item item = rule.BasicItems.ElementAtOrDefault(index);
            if (item is null) return;

            // If this item is the EOF token then we have found the grammar accept.
            if ((item is TokenItem) && (item.Name == EofTokenName)) {
                state.SetAccept();
                return;
            }

            // Create a new fragment for the action.
            Fragment nextFrag = new(rule, index+1, fragment.Lookaheads);
            this.log?.AddInfo("  Created fragment: {0}", nextFrag);

            // Get or create a new state for the target of the action.
            State next = state.FindActionTarget(item);
            if (next is null) {
                next = this.FindState(nextFrag);
                if (next is null) {
                    next = new State(this.States.Count);
                    this.States.Add(next);
                }
                state.AddAction(new Action(item, next));
            }
            this.log?.AddInfo("    Adding fragment to state {0}.", next.Number);

            // Try to add the fragment and indicate a change if it was changed.
            if (next.AddFragment(nextFrag, this.analyzer))
                changed.Add(next);
        }

        /// <summary>Determines the next states following the given state.</summary>
        /// <param name="state">The state to follow.</param>
        /// <returns>The next states.</returns>
        public HashSet<State> NextStates(State state) {
            this.log?.AddInfo("Next States from state {0}.", state.Number);
            HashSet<State> changed = new();
            // Use fragment count instead of for-each because fragments will be added to the list,
            // this means we also need to increment and check count on each loop.
            for (int i = 0; i < state.Fragments.Count; ++i)
                this.determineNextStateFragment(state, state.Fragments[i], changed);
            return changed;
        }

        /// <summary>Add a fragment to the table for the state with the given number.</summary>
        /// <param name="stateNumber">The state number for the state to add the fragments for.</param>
        /// <param name="frag">The fragment to add to the table.</param>
        private void addFragmentForStateToTable(int stateNumber, Fragment frag) {
            List<Item> items = frag.Rule.BasicItems.ToList();
            if (items.Count > frag.Index) return;

            Reduce reduce = new(frag.Rule);
            foreach (TokenItem follow in frag.Lookaheads)
                this.Table.WriteReduce(stateNumber, follow.Name, reduce);
        }

        /// <summary>Add an action to the table for the state with the given number.</summary>
        /// <param name="stateNumber">The state number for the state to add the action for.</param>
        /// <param name="action">The action to add to the table.</param>
        private void addActionForStateToTable(int stateNumber, Action action) {
            string onItem = action.Item.Name;
            int gotoNo = action.State.Number;
            if (action.Item is Term)
                this.Table.WriteGoto(stateNumber, onItem, new Goto(gotoNo));
            else this.Table.WriteShift(stateNumber, onItem, new Shift(gotoNo));
        }

        /// <summary>Add a state into the table.</summary>
        /// <param name="state">The state to add into the table.</param>
        private void addStateToTable(State state) {
            if (state.HasAccept)
                this.Table.WriteAccept(state.Number, EofTokenName, new Accept());

            foreach (Fragment frag in state.Fragments)
                this.addFragmentForStateToTable(state.Number, frag);

            foreach (Action action in state.Actions)
                this.addActionForStateToTable(state.Number, action);
        }

        /// <summary>Fills the parse table with the information from the states.</summary>
        public void FillTable() {
            foreach (State state in this.States)
                this.addStateToTable(state);
        }

        /// <summary>Returns a human readable string for debugging of the parser being built.</summary>
        /// <returns>The debugging string for the builder.</returns>
        public override string ToString() => this.ToString(true, true, true);

        /// <summary>Returns a human readable string for debugging of the parser being built.</summary>
        /// <param name="showState">Indicates the state should be shown.</param>
        /// <param name="showTable">Indicates the table should be shown.</param>
        /// <param name="showError">Indicates the errors should be shown.</param>
        /// <returns>The debugging string for the builder.</returns>
        public string ToString(bool showState = true, bool showTable = true, bool showError = true) {
            StringBuilder buf = new();
            if (showError && this.BuildLog.Failed) {
                if (buf.Length > 0) buf.AppendLine();
                buf.Append(this.BuildLog);
            }

            if (showState) {
                foreach (State state in this.States)
                    buf.Append(state.ToString());
            }

            if (showTable) {
                if (buf.Length > 0) buf.AppendLine();
                buf.AppendLine(this.Table.ToString());
            }
            return buf.ToString();
        }
    }
}
