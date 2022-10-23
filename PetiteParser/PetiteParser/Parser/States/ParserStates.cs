using PetiteParser.Formatting;
using PetiteParser.Grammar;
using PetiteParser.Logger;
using PetiteParser.Misc;
using PetiteParser.Parser.Table;
using System.Collections.Generic;
using System.Linq;
using System;

namespace PetiteParser.Parser.States;

/// <summary>This is a builder used to generate the LR parser states for a giving a grammar.</summary>
internal class ParserStates {
    static public readonly string StartTerm    = "$StartTerm";
    static public readonly string EofTokenName = "$EOFToken";

    /// <summary>Constructs of a new parser state collection.</summary>
    /// <param name="grammar">The grammar to build states for. The grammar may be modified.</param>
    /// <param name="log">The optional logger to log the steps the builder has performed.</param>
    public ParserStates(Grammar.Grammar grammar, ILogger? log = null) {
        this.States = new();
        Analyzer.Analyzer analyzer = new(grammar);
        Term startTerm = prepareGrammar(grammar);
        this.createInitialState(startTerm, analyzer, log);
        this.determineStates(analyzer, log);
    }

    /// <summary>The set of states for the parser.</summary>
    public readonly List<State> States;

    /// <summary>Finds a state with the given fragment.</summary>
    /// <param name="fragment">The fragment to find.</param>
    /// <returns>The found state or null.</returns>
    private State? findState(Fragment fragment) =>
        this.States.FirstOrDefault(state => state.HasFragment(fragment));

    /// <summary>Prepare the grammar with a single start and EOF.</summary>
    /// <param name="grammar">The grammar to build states for.</param>
    /// <returns>The start term from the grammar.</returns>+
    static private Term prepareGrammar(Grammar.Grammar grammar) {
        Term? startTerm = grammar.StartTerm;
        if (startTerm is null)
            throw new ParserException("Grammar did not have start term set.");

        // Check if the grammar has already been decorated with the StartTerm and EofTokenName,
        // if not then add them. Always ensure the StartTerm is set as the start term.
        if (grammar.Terms.FindItemByName(StartTerm) is null) {
            Term oldStart = startTerm;
            grammar.NewRule(StartTerm).AddTerm(oldStart.Name).AddToken(EofTokenName);
        }
        return grammar.Start(StartTerm);
    }

    /// <summary>Create the first state, state 0.</summary>
    /// <param name="startTerm">The start term of the grammar.</param>
    /// <param name="analyzer">The analyzer for the grammar being used to create the states.</param>
    /// <param name="log">The optional logger to log the steps the builder has performed.</param>
    private void createInitialState(Term startTerm, Analyzer.Analyzer analyzer, ILogger? log) {
        State startState = new(0);
        TokenItem eof = new(EofTokenName);
        foreach (Rule rule in startTerm.Rules)
            startState.AddFragment(new Fragment(rule, 0, eof), analyzer);
        this.States.Add(startState);
        log?.AddInfo("Created initial start state:",
            "  " + startState.ToString().IndentLines("  "));
    }

    /// <summary>Determines and fills out all the parser states for the grammar.</summary>
    /// <param name="analyzer">The analyzer for the grammar being used to create the states.</param>
    /// <param name="log">The optional logger to log the steps the builder has performed.</param>
    private void determineStates(Analyzer.Analyzer analyzer, ILogger? log) {
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
    private HashSet<State> nextStates(State state, Analyzer.Analyzer analyzer, ILogger? log) {
        log?.AddInfoF("Next States from state {0}.", state.Number);
        HashSet<State> changed = new();
        // Use fragment count instead of for-each because fragments will be added to the list,
        // this means we also need to increment and check count on each loop.
        for (int i = 0; i < state.Fragments.Count; ++i)
            this.determineNextStateFragment(state, state.Fragments[i], changed, analyzer, log);
        return changed;
    }

    /// <summary>Determines the next state with the given fragment from the given state.</summary>
    /// <param name="state">The state that is being followed.</param>
    /// <param name="fragment">The fragment from the state to follow.</param>
    /// <param name="changed">The states which have been changed.</param>
    /// <param name="analyzer">The analyzer for the grammar being created.</param>
    /// <param name="log">The optional logger to log the steps the builder has performed.</param>
    private void determineNextStateFragment(State state, Fragment fragment, HashSet<State> changed, Analyzer.Analyzer analyzer, ILogger? log) {
        Rule rule = fragment.Rule;
        int index = fragment.Index;

        // If there are any items left in this fragment get it or leave.
        Item? item = rule.BasicItems.ElementAtOrDefault(index);
        if (item is null) {
            foreach (TokenItem token in fragment.Lookaheads) {
                //
                // TODO: Determine the lookaheads for this rule and index specifically.
                //       Use the lookaheads for the action so that on conflict, the
                //       next token can be used to determine which path to take.
                //

                TokenItem[] lookaheads = Array.Empty<TokenItem>();
                state.AddAction(token, new Reduce(rule, lookaheads), log);
            }
            return;
        }

        // If this item is the EOF token then we have found the grammar accept.
        if (item is TokenItem && item.Name == EofTokenName) {
            Item eofToken = analyzer.Grammar.Token(EofTokenName);
            state.AddAction(eofToken, new Accept(), log);
            return;
        }

        // Create a new fragment for the action.
        Fragment nextFrag = new(rule, index + 1, fragment.Lookaheads);
        log?.AddInfoF("  Created fragment: {0}", nextFrag);

        // Get or create a new state for the target of the action.
        State? next = state.NextState(item);
        if (next is null) {
            next = this.findState(nextFrag);
            if (next is null) {
                next = new State(this.States.Count);
                this.States.Add(next);
            }
            
            state.ConnectToState(item, next);
            if (item is Term) state.AddGotoState(item, next.Number);
            else {
                //
                // TODO: FIX
                //

                TokenItem[] lookaheads = analyzer.ClosureLookAheads(nextFrag.Rule, 0, nextFrag.Lookaheads);
                state.AddAction(item, new Shift(next.Number, lookaheads), log);
            }
        }
        log?.AddInfoF("    Adding fragment to state {0}.", next.Number);

        // Try to add the fragment and indicate a change if it was changed.
        if (next.AddFragment(nextFrag, analyzer))
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
    public override string ToString() => this.States.Join();
}
