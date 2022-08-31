using PetiteParser.Grammar;
using PetiteParser.Matcher;
using PetiteParser.Misc;
using PetiteParser.ParseTree;
using PetiteParser.Tokenizer;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace PetiteParser.Loader {

    /// <summary>The prompt arguments for processing V1 languages.</summary>
    internal class LoaderArgs : PromptArgs {

        /// <summary>The grammar that is being worked on and added to.</summary>
        public readonly Grammar.Grammar Grammar;

        /// <summary>The tokenizer that is being worked on and added to.</summary>
        public readonly Tokenizer.Tokenizer Tokenizer;

        /// <summary>The current feature flag mode for setting features.</summary>
        /// <remarks>This is values like, `enable`, `disable`, etc.</remarks>
        public string FeatureFlagMode;

        public readonly Features Features;

        public readonly List<TokenState> TokenStates;

        public readonly Stack<Term> Terms;

        public readonly Stack<TokenItem> TokenItems;

        public readonly Stack<Prompt> Prompts;

        public readonly List<Group> CurTransGroups;

        public bool CurTransConsume;

        public readonly List<string> ReplaceText;

        public Rule CurRule;

        /// <summary>Creates a new prompt arguments for V1.</summary>
        /// <param name="grammar">The grammar that is being worked on and added to.</param>
        /// <param name="tokenizer">The tokenizer that is being worked on and added to.</param>
        /// <param name="features">The optional features object for the features to use when parsing.</param>
        public LoaderArgs(Grammar.Grammar grammar, Tokenizer.Tokenizer tokenizer, Features features = null) {
            this.Grammar   = grammar;
            this.Tokenizer = tokenizer;
            this.Features  = features ?? new();

            this.TokenStates = new List<TokenState>();
            this.Terms       = new Stack<Term>();
            this.TokenItems  = new Stack<TokenItem>();
            this.Prompts     = new Stack<Prompt>();

            this.CurTransGroups  = new List<Group>();
            this.CurTransConsume = false;
            this.ReplaceText     = new List<string>();
            this.CurRule         = null;
        }

        public State PrevState { get; private set; }

        public State CurState { get; private set; }

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
        static private object getAsType(Type type, string value) {
            if (type == typeof(string)) return value;
            //
            if (type == typeof(bool))
                return bool.TryParse(value, out bool result) ? result :
                    throw new Exception("Unable to parse \""+value+"\" into bool.");
            //
            if (type == typeof(int))
                return int.TryParse(value, out int result) ? result :
                    throw new Exception("Unable to parse \""+value+"\" into int.");
            //
            if (type == typeof(double))
                return double.TryParse(value, out double result) ? result :
                    throw new Exception("Unable to parse \""+value+"\" into double.");
            //
            throw new Exception("Unable to set the feature of type " + type.Name + ". Expected string, bool, int or double.");
        }

        /// <summary>Sets the feature with the given name and value.</summary>
        /// <param name="name">The name of the feature to set.</param>
        /// <param name="value">The value to set to the feature.</param>
        public void SetFeatureValue(string name, string value) {
            FeatureEntry entry = FeatureEntry.FindFeature(this.Features, name);
            try {
                entry.SetValue(getAsType(entry.ValueType, value));
            } catch (Exception ex) {
                throw new Exception("Error setting feature " + name + ": " + ex.Message);
            }
        }

        /// <summary>Enabled or disables the feature with the given name.</summary>
        /// <param name="name">The name of the feature to enable and disable.</param>
        /// <param name="enabled">Indicates if the feature should be enabled or disabled.</param>
        public void EnableFeatureValue(string name, bool enabled) {
            FeatureEntry entry = FeatureEntry.FindFeature(this.Features, name);
            try {
                if (entry.ValueType != typeof(bool))
                    throw new Exception("May not enable or disable a flag unless it is boolean.");
                entry.SetValue(enabled);
            } catch (Exception ex) {
                throw new Exception("Error " + (enabled ? "enabling" : "disabling") + " a feature " + name + ": " + ex.Message);
            }
        }
    }
}
