using PetiteParser.Grammar;
using PetiteParser.Parser.States;
using PetiteParser.Parser.Table;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PetiteParser.Parser;

/// <summary>
/// When building a parser for a grammar there may be conflicts in the grammar.
/// This indicates how to handle conflicts while building the parser.
/// </summary>
sealed public class OnConflict {
    
    /// <summary>Throw an exception on conflict.</summary>
    static public readonly OnConflict Panic =
        new ("panic", data => throw new ParserException("Grammar conflict at state " + data.State.Number +
            " and " + data.Item + ": prior = " + data.Prior + ", next = " + data.Next + ":\n" + data.State.ToString()));

    /// <summary>The first parser action will be used.</summary>
    static public readonly OnConflict UseFirst =
        new ("use_first", data => data.Prior);

    /// <summary>The last parser action will be used.</summary>
    static public readonly OnConflict UseLast =
        new ("use_last", data => data.Next);

    /// <summary>A shift parser action will be preferred.</summary>
    static public readonly OnConflict PreferShift =
        new ("prefer_shift", data => data.Next is Shift ? data.Next : data.Prior);

    /// <summary>A reduce parser action will be preferred.</summary>
    static public readonly OnConflict PreferReduce =
        new ("prefer_reduce", data => data.Next is Reduce ? data.Next : data.Prior);

    /// <summary>Finds the conflict handler by the given name or returns null.</summary>
    /// <param name="name">The name of the conflict handler to find.</param>
    /// <returns>The conflict handler by the given name or null.</returns>
    static public OnConflict? Find(string name) => All.FirstOrDefault(oc =>
        string.Equals(oc.Name, name, StringComparison.OrdinalIgnoreCase));

    /// <summary>The enumeration of all the possible conflict options.</summary>
    static public IEnumerable<OnConflict> All =>
        new OnConflict[] { Panic, UseFirst, UseLast, PreferShift, PreferReduce };

    #region Implementation...
    
    /// <summary>The data used to describe a conflict.</summary>
    /// <param name="State">The state the conflict is in.</param>
    /// <param name="Item">The item which has a conflict.</param>
    /// <param name="Prior">The prior action in conflict.</param>
    /// <param name="Next">The next action in conflict.</param>
    internal readonly record struct ConflictData(State State, Item Item, IAction Prior, IAction Next);

    /// <summary>Creates a new conflict handler.</summary>
    /// <param name="name">The name of the conflict handling method.</param>
    /// <param name="handle">The method for performing the handle.</param>
    private OnConflict(string name, Func<ConflictData, IAction> handle) {
        this.Name   = name;
        this.handle = handle;
    }

    /// <summary>The name of this conflict handler.</summary>
    public readonly string Name;

    /// <summary>The method of handling the conflict.</summary>
    internal readonly Func<ConflictData, IAction> handle;

    /// <summary>Gets the name of the conflict handler.</summary>
    /// <returns>The given name.</returns>
    public override string ToString() => this.Name;

    #endregion
}
