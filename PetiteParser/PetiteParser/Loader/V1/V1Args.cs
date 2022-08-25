using PetiteParser.Grammar;
using PetiteParser.Matcher;
using PetiteParser.ParseTree;
using PetiteParser.Tokenizer;
using System.Collections.Generic;

namespace PetiteParser.Loader.V1 {


    internal class V1Args: PromptArgs {

        public readonly Grammar.Grammar Grammar;
        public readonly Tokenizer.Tokenizer Tokenizer;



        public readonly List<Tokenizer.State> States;
        public readonly List<TokenState> TokenStates;
        public readonly Stack<Term> Terms;
        public readonly Stack<TokenItem> TokenItems;
        public readonly Stack<Prompt> Prompts;
        public readonly List<Group> CurTransGroups;
        public bool CurTransConsume;
        public readonly List<string> ReplaceText;
        public Rule CurRule;

        public V1Args(Grammar.Grammar grammar, Tokenizer.Tokenizer tokenizer) {
            this.Grammar = grammar;
            this.Tokenizer = tokenizer;


            this.States      = new List<Tokenizer.State>();
            this.TokenStates = new List<TokenState>();
            this.Terms       = new Stack<Term>();
            this.TokenItems  = new Stack<TokenItem>();
            this.Prompts     = new Stack<Prompt>();

            this.CurTransGroups  = new List<Group>();
            this.CurTransConsume = false;
            this.ReplaceText     = new List<string>();
            this.CurRule         = null;
        }


        public void Clear() {
            this.Tokens.Clear();
            this.States.Clear();
            this.TokenStates.Clear();
            this.Terms.Clear();
            this.TokenItems.Clear();
            this.Prompts.Clear();
            this.CurTransGroups.Clear();
            this.CurTransConsume = false;
            this.ReplaceText.Clear();
            this.CurRule = null;
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
