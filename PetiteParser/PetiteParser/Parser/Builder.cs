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
        private TokenSets tokenSets;

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

        /// <summary>Finds a state with the given fragment.</summary>
        /// <param name="fragment">The fragment to find.</param>
        /// <returns>The found state or null.</returns>
        public State FindState(Fragment fragment) {
            foreach (State state in this.States) {
                if (state.HasFragment(fragment)) return state;
            }
            return null;
        }

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
        /// <returns></The next states.returns>
        public List<State> NextStates(State state) {
            List<State> changed = new();
            foreach (Fragment fragment in state.Fragments) {
                Rule rule = fragment.Rule;
                int index = fragment.Index;
                List<Item> items = rule.BasicItems;
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

        /// <summary>Fills the parse table with the information from the states.</summary>
        public void FillTable() {
            foreach (State state in this.States) {
                if (state.HasAccept)
                    this.Table.WriteAccept(state.Number, EofTokenName, new Accept());

                foreach (Fragment frag in state.Fragments) {
                    List<Item> items = frag.Rule.BasicItems;
                    if (items.Count <= frag.Index) {
                        Reduce reduce = new(frag.Rule);
                        foreach (TokenItem follow in frag.Lookaheads)
                            this.Table.WriteReduce(state.Number, follow.Name, reduce);
                    }
                }

                foreach (Action action in state.Actions) {
                    string onItem = action.Item.Name;
                    int gotoNo = action.State.Number;
                    if (action.Item is Term)
                        this.Table.WriteGoto(state.Number, onItem, new Goto(gotoNo));
                    else this.Table.WriteShift(state.Number, onItem, new Shift(gotoNo));
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
