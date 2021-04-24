using PetiteParser.Grammar;
using PetiteParser.Table;
using System.Collections.Generic;
using System.Text;

namespace PetiteParser.Parser {

    /// <summary>This is a builder used to generate a parser giving a grammar.</summary>
    internal class Builder {
        public const string StartTerm    = "$StartTerm";
        public const string EofTokenName = "$EOFToken";

        private Grammar.Grammar grammar;
        private HashSet<Item> items;
        private StringBuilder errors;

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

            foreach (Term term in this.grammar.Terms) {
                this.items.Add(term);
                foreach (Rule rule in term.Rules) {
                    foreach (Item item in rule.Items) {
                        if (!(item is Prompt)) this.items.Add(item);
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

        /// <summary>Finds a state with the given offset index for the given rule.</summary>
        /// <param name="index">The index to find.</param>
        /// <param name="rule">The rule to find.</param>
        /// <returns>The found state or null.</returns>
        public State Find(int index, Rule rule) {
            foreach (State state in this.States) {
                for (int i = 0; i < state.Indices.Count; ++i) {
                    if ((state.Indices[i] == index) && (state.Rules[i] == rule)) return state;
                }
            }
            return null;
        }

        /// <summary>Determines all the parser states for the grammar.</summary>
        public void DetermineStates() {
            // Create the first state, state 0.
            State startState = new(0);
            foreach (Rule rule in this.grammar.StartTerm.Rules)
                startState.AddRule(EofTokenName, 0, rule);
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
        /// <returns></The next states.returns>
        public List<State> NextStates(State state) {
            List<State> changed = new();
            for (int i = 0; i < state.Indices.Count; i++) {
                string token = state.Tokens[i];
                int index = state.Indices[i];
                Rule rule = state.Rules[i];

                List<Item> items = rule.BasicItems;
                if (index < items.Count) {
                    Item item = items[index];

                    if ((item is TokenItem) && (item.Name == EofTokenName))
                        state.SetAccept();
                    else {
                        State next = state.FindGoto(item);
                        if (next is null) {
                            next = this.Find(index+1, rule);
                            if (next is null) {
                                next = new State(this.States.Count);
                                this.States.Add(next);
                            }
                            state.AddGoto(item, next);
                        }

                        if (next.AddRule(token, index+1, rule))
                            changed.Add(next);
                    }
                }
            }
            return changed;
        }

        /// <summary>Fills the parse table with the information from the states.</summary>
        public void FillTable() {
            foreach (State state in this.States) {
                if (state.HasAccept)
                    this.Table.WriteShift(state.Number, EofTokenName, new Accept());

                for (int i = 0; i < state.Rules.Count; ++i) {
                    Rule rule = state.Rules[i];
                    int index = state.Indices[i];
                    List<Item> items = rule.BasicItems;
                    if (items.Count <= index) {

                        // Add the reduce action to all the follow items.
                        Reduce reduce = new(rule);
                        foreach (TokenItem follow in rule.Term.Follows)
                            this.Table.WriteShift(state.Number, follow.Name, reduce);
                    }
                }

                for (int i = 0; i < state.Gotos.Count; ++i) {
                    Item onItem = state.OnItems[i];
                    int gotoNo = state.Gotos[i].Number;
                    if (onItem is Term)
                        this.Table.WriteGoto(state.Number, onItem.Name, new Goto(gotoNo));
                    else this.Table.WriteShift(state.Number, onItem.Name, new Shift(gotoNo));
                }
            }

            // Check for goto loops.
            foreach (Term term in this.grammar.Terms) {
                List<int> checkedState = new();
                for (int i = 0; i< this.States.Count; i++) {
                    if (checkedState.Contains(i)) continue;
                    checkedState.Add(i);

                    IAction action = this.Table.ReadGoto(i, term.Name);
                    List<int> reached = new();
                    while (action is Goto) {
                        reached.Add(i);
                        checkedState.Add(i);
                        i = (action as Goto).State;
                        if (reached.Contains(i)) {
                            int index = reached.IndexOf(i);
                            List<int> loop = reached.GetRange(index, reached.Count-index);
                            this.errors.AppendLine("Infinite goto loop found in term "+term.Name+
                                " between the state(s) ["+string.Join(", ", loop)+"].");
                            break;
                        }
                        action = this.Table.ReadGoto(i, term.Name);
                    }
                }
            }
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
            if (showState) {
                foreach (State state in this.States)
                    buf.Append(state.ToString());
            }

            if (showTable) {
                if (buf.Length > 0) buf.AppendLine();
                buf.AppendLine(this.Table.ToString());
            }

            if ((showError) && (this.errors.Length > 0)) {
                if (buf.Length > 0) buf.AppendLine();
                buf.Append(this.errors);
            }
            return buf.ToString();
        }
    }
}
