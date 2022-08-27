using PetiteParser.Grammar;
using PetiteParser.Matcher;
using PetiteParser.ParseTree;
using PetiteParser.Tokenizer;
using System.Collections.Generic;

namespace PetiteParser.Loader {

    /// <summary>The prompt arguments for processing V1 languages.</summary>
    internal class LoaderArgs : PromptArgs {

        /// <summary>The grammar that is being worked on and added to.</summary>
        public readonly Grammar.Grammar Grammar;

        /// <summary>The tokenizer that is being worked on and added to.</summary>
        public readonly Tokenizer.Tokenizer Tokenizer;

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
        public LoaderArgs(Grammar.Grammar grammar, Tokenizer.Tokenizer tokenizer) {
            this.Grammar   = grammar;
            this.Tokenizer = tokenizer;
            this.Features  = new();

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
    }
}
