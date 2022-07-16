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
        private readonly StringBuilder errors;
        private readonly TokenSets tokenSets;

        /// <summary>Constructs of a new parser builder.</summary>
        /// <param name="grammar">The grammar to build.</param>
        public Builder(Grammar.Grammar grammar) {
            this.grammar = grammar;
            Term oldStart = this.grammar.StartTerm;
            this.grammar.Start(StartTerm);
            this.grammar.NewRule(StartTerm).AddTerm(oldStart.Name).AddToken(EofTokenName);

            this.States = new List<State>();
            this.items  = new HashSet<Item>();
            this.Table  = new Table.Table();
            this.errors = new StringBuilder();
            this.tokenSets = new TokenSets(this.grammar);

            foreach (Term term in this.grammar.Terms) {
                this.items.Add(term);
                foreach (Rule rule in term.Rules) {
                    foreach (Item item in rule.Items) {
                        if (item is not Prompt) this.items.Add(item);
                    }
                }
            }
        }

        /// <summary>
        /// Gets all the error which occurred during the build,
        /// or an empty string if no error occurred.
        /// </summary>
        public string BuildErrors => this.errors.ToString();

        /// <summary>The table from the builder.</summary>
        public Table.Table Table { get; }

        /// <summary>The set of states for the parser.</summary>
        public List<State> States { get; }

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
                startState.AddFragment(new Fragment(rule, 0, eof), this.tokenSets);
            this.States.Add(startState);

            // Fill out all other states.
            List<State> changed = new() { startState };
            while (changed.Count > 0) {
                State state = changed[^1];
                changed.RemoveAt(changed.Count-1);
                changed.AddRange(this.NextStates(state));
            }
        }

        /// <summary>Determines the next states following the given state.</summary>
        /// <param name="state">The state to follow.</param>
        /// <returns>The next states.</returns>
        public List<State> NextStates(State state) {
            List<State> changed = new();
            for (int i = 0; i < state.Fragments.Count; ++i) {
                Fragment fragment = state.Fragments[i];
                Rule rule = fragment.Rule;
                int index = fragment.Index;
                List<Item> items = rule.BasicItems.ToList();
                if (index < items.Count) {
                    Item item = items[index];

                    if ((item is TokenItem) && (item.Name == EofTokenName))
                        state.SetAccept();
                    else {
                        State next = state.FindActionTarget(item);
                        Fragment nextFrag = new(rule, index+1, fragment.Lookaheads);

                        if (next is null) {
                            next = this.FindState(nextFrag);
                            if (next is null) {
                                next = new State(this.States.Count);
                                this.States.Add(next);
                            }
                            state.AddAction(new Action(item, next));
                        }

                        if (next.AddFragment(nextFrag, this.tokenSets))
                            changed.Add(next);
                    }
                }
            }
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

        /// <summary>Adds an error for a found goto loop to the list of errors found.</summary>
        /// <param name="termName">The name of the term which contains the goto loop.</param>
        /// <param name="loop">The set of states that are included in the loop.</param>
        private void addLoopError(string termName, List<int> loop) =>
            this.errors.AppendLine("Infinite goto loop found in term " + termName + " between the state(s) [" + loop.Join(", ") + "].");

        /// <summary>Check if there is a loop for an action for the given state and term.</summary>
        /// <param name="stateNumber">The number for the state to start at.</param>
        /// <param name="termName">The name of the term to start at and check.</param>
        /// <param name="checkedState">The set of states which have already been checked.</param>
        private void checkActionForGotoLoop(int stateNumber, string termName, HashSet<int> checkedState) {
            IAction action = this.Table.ReadGoto(stateNumber, termName);
            List<int> reached = new();
            while (action is Goto gotoAction) {
                reached.Add(stateNumber);
                checkedState.Add(stateNumber);
                stateNumber = gotoAction.State;
                if (reached.Contains(stateNumber)) {
                    // Loop has been found because we have already reached this state before. 
                    int index = reached.IndexOf(stateNumber);
                    List<int> loop = reached.GetRange(index, reached.Count-index);
                    this.addLoopError(termName, loop);
                    break;
                }
                action = this.Table.ReadGoto(stateNumber, termName);
            }

        }

        /// <summary>Check if there is any loops in the given term.</summary>
        /// <param name="term">The term to check for loops in the table with.</param>
        private void checkTermForLoops(Term term) {
            HashSet<int> checkedState = new();
            for (int i = 0; i< this.States.Count; i++) {
                if (checkedState.Contains(i)) continue;
                checkedState.Add(i);

                this.checkActionForGotoLoop(i, term.Name, checkedState);
            }
        }

        /// <summary>Fills the parse table with the information from the states.</summary>
        public void FillTable() {
            foreach (State state in this.States)
                this.addStateToTable(state);

            // Check for goto loops.
            foreach (Term term in this.grammar.Terms)
                this.checkTermForLoops(term);
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
            if ((showError) && (this.errors.Length > 0)) {
                if (buf.Length > 0) buf.AppendLine();
                buf.Append(this.errors);
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
