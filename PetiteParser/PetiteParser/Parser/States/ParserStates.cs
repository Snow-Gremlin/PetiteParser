using PetiteParser.Formatting;
using PetiteParser.Grammar;
using PetiteParser.Logger;
using PetiteParser.Misc;
using PetiteParser.Parser.Table;
using System.Collections.Generic;
using System.Linq;

namespace PetiteParser.Parser.States;

/// <summary>This is a builder used to generate the LR parser states for a giving a grammar.</summary>
internal class ParserStates {
    static public readonly string StartTerm    = "$StartTerm";
    static public readonly string EofTokenName = "$EOFToken";
    
    /// <summary>Constructs of a new parser state collection.</summary>
    public ParserStates() => this.States = new();

    /// <summary>Determines all the states from the given grammar.</summary>
    /// <param name="grammar">The grammar to build states for. The grammar may be modified.</param>
    /// <param name="log">The optional logger to log the steps the builder has performed.</param>
    public void DetermineStates(Grammar.Grammar grammar, ILogger? log = null) {
        this.States.Clear();
        Grammar.Analyzer.Analyzer analyzer = new(grammar);
        Term startTerm = prepareGrammar(grammar);
        this.createInitialState(startTerm, analyzer, log);
        this.determineStates(analyzer, log);
    }

    /// <summary>The set of states for the parser.</summary>
    public readonly List<State> States;

    /// <summary>Creates and adds a new state to this set of states.</summary>
    /// <param name="log">The optional logger to log the steps the builder has performed.</param>
    /// <returns>The new state that was created.</returns>
    private State newState(ILogger? log) {
        State state = new(this.States.Count);
        log?.AddInfoF("Created new state {0}.", state.Number);
        this.States.Add(state);
        return state;
    }

    /// <summary>Finds a state with the given fragment.</summary>
    /// <param name="fragment">The fragment to find.</param>
    /// <returns>The found state or null.</returns>
    private State? findState(Fragment fragment) =>
        this.States.FirstOrDefault(state => state.HasFragment(fragment));

    /// <summary>Prepare the grammar with a single start and EOF.</summary>
    /// <param name="grammar">The grammar to build states for.</param>
    /// <returns>The start term from the grammar.</returns>+
    static private Term prepareGrammar(Grammar.Grammar grammar) {
        Term? givenStartTerm = grammar.StartTerm;
        if (givenStartTerm is null)
            throw new ParserException("Grammar did not have start term set.");

        // Check if the grammar has already been decorated with the StartTerm and EofTokenName,
        // if not then add them. Always ensure the StartTerm is set as the start term.
        if (grammar.Terms.FindItemByName(StartTerm) is null)
            grammar.NewRule(StartTerm).AddTerm(givenStartTerm.Name).AddToken(EofTokenName);
        return grammar.Start(StartTerm);
    }

    /// <summary>Create the first state, state 0.</summary>
    /// <param name="startTerm">The start term of the grammar.</param>
    /// <param name="analyzer">The analyzer for the grammar being used to create the states.</param>
    /// <param name="log">The optional logger to log the steps the builder has performed.</param>
    private void createInitialState(Term startTerm, Grammar.Analyzer.Analyzer analyzer, ILogger? log) {
        State startState = this.newState(log);
        ILogger? log2 = log?.Indent();
        foreach (Rule rule in startTerm.Rules) {
            Fragment frag = Fragment.NewRoot(rule);
            startState.AddFragment(frag, analyzer, log2);
        }
    }

    /// <summary>Determines and fills out all the parser states for the grammar.</summary>
    /// <param name="analyzer">The analyzer for the grammar being used to create the states.</param>
    /// <param name="log">The optional logger to log the steps the builder has performed.</param>
    private void determineStates(Grammar.Analyzer.Analyzer analyzer, ILogger? log) {
        HashSet<State> changed = new(this.States);
        while (changed.Count > 0) {
            State state = changed.First();
            changed.Remove(state);
            this.nextStates(state, analyzer, log).Foreach(changed.Add);
        }
    }

    /// <summary>Determines the next states following the given state.</summary>
    /// <param name="state">The state to follow.</param>
    /// <param name="analyzer">The analyzer for the grammar being used to create the states.</param>
    /// <param name="log">The optional logger to log the steps the builder has performed.</param>
    /// <returns>The next states.</returns>
    private HashSet<State> nextStates(State state, Grammar.Analyzer.Analyzer analyzer, ILogger? log) {
        log?.AddInfoF("Next States from state {0}.", state.Number);
        ILogger? log2 = log?.Indent();
        HashSet<State> changed = new();
        // Use fragment count instead of for-each because fragments will be added to the list,
        // this means we also need to increment and check count on each loop.
        for (int i = 0; i < state.Fragments.Count; ++i)
            this.determineNextStateFragment(state, i, changed, analyzer, log2);
        return changed;
    }

    /// <summary>Determines the next state with the given fragment from the given state.</summary>
    /// <param name="state">The state that is being followed.</param>
    /// <param name="fragmentNum">The fragment number from the state to follow.</param>
    /// <param name="changed">The states which have been changed.</param>
    /// <param name="analyzer">The analyzer for the grammar being created.</param>
    /// <param name="log">The optional logger to log the steps the builder has performed.</param>
    private void determineNextStateFragment(State state, int fragmentNum, HashSet<State> changed, Grammar.Analyzer.Analyzer analyzer, ILogger? log) {
        Fragment fragment = state.Fragments[fragmentNum];
        log?.AddInfoF("Determining next state from fragment #{0}: {1}", fragmentNum, fragment);
        ILogger? log2 = log?.Indent();

        // If there are any items left in this fragment get it or leave with a reduction.
        Item? item = fragment.NextItem;
        if (item is null) {
            TokenItem[] lookaheads = fragment.Lookaheads;

            // TODO: Remove any lookaheads used for reducing which were caused by the same term's lookahead

            log2?.AddInfoF("Adding reductions to state {0} for {1}.", state.Number, lookaheads.Join(" "));
            foreach (TokenItem token in lookaheads)
                state.AddAction(token, new Reduce(fragment.Rule));
            return;
        }

        // If this item is the EOF token then we have found the grammar accept.
        if (item is TokenItem && item.Name == EofTokenName) {
            Item eofToken = analyzer.Grammar.Token(EofTokenName);
            log2?.AddInfoF("Adding accept to state {0}.", state.Number);
            state.AddAction(eofToken, new Accept());
            return;
        }

        // Create a new fragment for the action.
        Fragment nextFrag = Fragment.NextFragment(fragment);
        log2?.AddInfoF("Created fragment: {0}", nextFrag);

        // Get or create a new state for the target of the action.
        State? next = state.NextState(item);
        if (next is null) {
            next = this.findState(nextFrag) ?? this.newState(log2);

            log2?.AddInfoF("Adding connection between state {0} and {1}.", next.Number, item);
            state.ConnectToState(item, next);
            if (item is Term) state.AddGotoState(item, next.Number);
            else state.AddAction(item, new Shift(next.Number));
        }

        // Try to add the fragment and indicate a change if it was changed.
        if (next.AddFragment(nextFrag, analyzer, log2))
            changed.Add(next);
    }

    /// <summary>Fills a parse table with the information from the given states.</summary>
    /// <returns>Returns the created table.</returns>
    public Table.Table CreateTable() {
        Table.Table table = new();
        foreach (State state in this.States)
            state.WriteToTable(table);
        return table;
    }

    /// <summary>Returns a human readable string for debugging of the parser being built.</summary>
    /// <returns>The debugging string for the builder.</returns>
    public override string ToString() => this.States.JoinLines();
}
