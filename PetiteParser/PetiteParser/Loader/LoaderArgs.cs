﻿using PetiteParser.Grammar;
using PetiteParser.ParseTree;
using PetiteParser.Tokenizer;
using PetiteParser.Tokenizer.Matcher;
using System;
using System.Collections.Generic;

namespace PetiteParser.Loader;

/// <summary>The prompt arguments for processing V1 languages.</summary>
sealed internal class LoaderArgs : PromptArgs {

    /// <summary>The grammar that is being worked on and added to.</summary>
    public readonly Grammar.Grammar Grammar;

    /// <summary>The tokenizer that is being worked on and added to.</summary>
    public readonly Tokenizer.Tokenizer Tokenizer;

    /// <summary>The current feature flag mode for setting features.</summary>
    /// <remarks>This is values like, `enable`, `disable`, etc.</remarks>
    public string FeatureFlagMode;

    /// <summary>The features object for storing the features and settings used for parsing.</summary>
    public readonly Features Features;

    /// <summary>The token states list of tokens being processed.</summary>
    public readonly List<TokenState> TokenStates;

    /// <summary>The stack of terms used while loading rules.</summary>
    public readonly Stack<Term> Terms;

    /// <summary>The stack of tokens used with rules and errors.</summary>
    public readonly Stack<TokenItem> TokenItems;

    /// <summary>The stack of prompts used while loading rules.</summary>
    public readonly Stack<Prompt> Prompts;

    /// <summary>The current transition groups being setup.</summary>
    public readonly List<Group> CurTransGroups;

    /// <summary>True if the current transition is to be consumed.</summary>
    public bool CurTransConsume;

    /// <summary>The list of replacement text for token replacement.</summary>
    public readonly List<string> ReplaceText;

    /// <summary>The current rule that is being worked on.</summary>
    public Rule? CurRule;

    /// <summary>Creates a new prompt arguments for V1.</summary>
    /// <param name="grammar">The grammar that is being worked on and added to.</param>
    /// <param name="tokenizer">The tokenizer that is being worked on and added to.</param>
    /// <param name="features">The optional features object for the features to use when parsing.</param>
    public LoaderArgs(Grammar.Grammar grammar, Tokenizer.Tokenizer tokenizer, Features? features = null) {
        this.Grammar   = grammar;
        this.Tokenizer = tokenizer;
        this.Features  = features ?? new();

        this.TokenStates = new();
        this.Terms       = new();
        this.TokenItems  = new();
        this.Prompts     = new();

        this.FeatureFlagMode = "";
        this.CurTransGroups  = new();
        this.CurTransConsume = false;
        this.ReplaceText     = new();
        this.CurRule         = null;
    }

    /// <summary>Clears all the argument data.</summary>
    public void Clear() {
        this.Tokens.Clear();

        this.PrevState = null;
        this.CurState = null;
        this.CurTransConsume = false;
        this.CurRule = null;

        this.TokenStates.Clear();
        this.Terms.Clear();
        this.TokenItems.Clear();
        this.Prompts.Clear();
        this.CurTransGroups.Clear();
        this.ReplaceText.Clear();
    }

    /// <summary>The second state on the state stack, the one prior to the current state.</summary>
    public State? PrevState { get; private set; }

    /// <summary>The top state on the state stack, the most current state.</summary>
    public State? CurState { get; private set; }

    /// <summary>Pushes a state onto the state stack.</summary>
    /// <param name="state">The new state stack.</param>
    public void PushState(State state) {
        this.PrevState = this.CurState;
        this.CurState = state;
    }

    /// <summary>Gets the top matcher group in the current transitions.</summary>
    /// <remarks>If there are no groups then one is added.</remarks>
    public Group TopTransGroup {
        get {
            if (this.CurTransGroups.Count <= 0)
                this.CurTransGroups.Add(new Group());
            return this.CurTransGroups[^1];
        }
    }

    /// <summary>Gets the string parsed into the given type.</summary>
    /// <param name="type">The type to parse the string into.</param>
    /// <param name="value">The value to parse into the given type.</param>
    /// <returns>The value in the given type.</returns>
    static private object getAsType(Type type, string value) =>
        type == typeof(bool)   ? getAsBool(value) :
        type == typeof(int)    ? getAsInt(value) :
        type == typeof(double) ? getAsDouble(value) :
        type == typeof(string) ? (object)value :
        throw new LoaderException("Unable to set the feature of type " + type.Name + ". Expected string, bool, int or double.");

    /// <summary>Gets the given value as a boolean.</summary>
    /// <param name="value">The value to parse into a boolean.</param>
    /// <returns>The parsed boolean value.</returns>
    static private bool getAsBool(string value) =>
        bool.TryParse(value, out bool result) ? result :
            throw new LoaderException("Unable to parse \""+value+"\" into bool.");
    
    /// <summary>Gets the given value as an integer.</summary>
    /// <param name="value">The value to parse into an integer.</param>
    /// <returns>The parsed integer value.</returns>
    static private int getAsInt(string value) =>
        int.TryParse(value, out int result) ? result :
            throw new LoaderException("Unable to parse \""+value+"\" into int.");
    
    /// <summary>Gets the given value as a double.</summary>
    /// <param name="value">The value to parse into a double.</param>
    /// <returns>The parsed double value.</returns>
    static private double getAsDouble(string value) =>
        double.TryParse(value, out double result) ? result :
            throw new LoaderException("Unable to parse \""+value+"\" into double.");

    /// <summary>Sets the feature with the given name and value.</summary>
    /// <param name="name">The name of the feature to set.</param>
    /// <param name="value">The value to set to the feature.</param>
    public void SetFeatureValue(string name, string value) {
        FeatureEntry entry = FeatureEntry.FindFeature(this.Features, name);
        try {
            entry.SetValue(getAsType(entry.ValueType, value));
        } catch (Exception ex) {
            throw new LoaderException("Error setting feature " + name + ": " + ex.Message);
        }
    }

    /// <summary>Enabled or disables the feature with the given name.</summary>
    /// <param name="name">The name of the feature to enable and disable.</param>
    /// <param name="enabled">Indicates if the feature should be enabled or disabled.</param>
    public void EnableFeatureValue(string name, bool enabled) {
        FeatureEntry entry = FeatureEntry.FindFeature(this.Features, name);
        try {
            if (entry.ValueType != typeof(bool))
                throw new LoaderException("May not enable or disable a flag unless it is boolean.");
            entry.SetValue(enabled);
        } catch (Exception ex) {
            throw new LoaderException("Error " + (enabled ? "enabling" : "disabling") + " a feature " + name + ": " + ex.Message);
        }
    }
}
