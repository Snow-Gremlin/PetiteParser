using PetiteParser.Analyzer;
using PetiteParser.Grammar;
using PetiteParser.Loader;
using PetiteParser.Logger;
using PetiteParser.Misc;
using PetiteParser.Parser;
using PetiteParser.Parser.States;
using PetiteParser.Parser.Table;
using PetiteParser.Tokenizer;

namespace LanguageTestingTool {
    public partial class MainForm : Form {

        private bool langValid;
        private Tokenizer tokenizer;
        private Grammar grammar;
        private Grammar normGrammar;
        private ParserStates states;
        private Table table;
        private Parser parser;

        public MainForm() {
            this.InitializeComponent();
            this.langValid = false;
        }

        #region Event Handling...

        private void boxLang_TextChanged(object sender, EventArgs e) {
            if (!this.debounceTimer.Enabled)
                this.debounceTimer.Start();
        }

        private void stateDropDown_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e) {
            if (this.langValid && int.TryParse(e.ClickedItem.Text, out int state))
                this.setState(state);
        }
        
        private void numState_ValueChanged(object sender, EventArgs e) =>
            this.setState((int)this.numState.Value);

        private void boxInput_TextChanged(object sender, EventArgs e) {
            if (this.langValid)
                this.inputUpdate();
        }

        private void debounceTimer_Tick(object sender, EventArgs e) {
            this.debounceTimer?.Stop();
            this.languageUpdate();
        }

        #endregion

        private void languageUpdate() {
            try {
                Loader loader = new();
                loader.Load(this.boxLang.Text);
                this.tokenizer = loader.Tokenizer;
                this.grammar = loader.Grammar;
                
                Buffered log = new();
                Analyzer.Validate(grammar, log);
                if (log.Failed) {
                    this.badLanguage(log.ToString());
                    return;
                }

                this.normGrammar = Analyzer.Normalize(this.grammar, log);
                if (log.Failed) {
                    this.badLanguage(log.ToString());
                    return;
                }

                this.states = new(this.normGrammar);
                if (log.Failed) {
                    this.badLanguage(log.ToString());
                    return;
                }

                this.table = this.states.CreateTable();

                this.parser = new Parser(this.table, this.normGrammar, this.tokenizer);
                this.languageGood();
            } catch (Exception ex) {
                this.badLanguage(ex.Message);
            }
        }

        private void badLanguage(string message) {
            this.langValid   = false;
            this.tokenizer   = null;
            this.grammar     = null;
            this.normGrammar = null;
            this.parser      = null;
            this.boxLangResult.Text = message;

            this.boxNorm.Text = "";

            this.numState.Value = 0;
            this.numState.Maximum = 0;
            this.boxStateActions.Text = "";
            
            this.inputUpdate();
        }

        private void languageGood() {
            this.langValid = true;
            this.boxLangResult.Text = "Success";

            this.boxNorm.Text = this.normGrammar.ToString();

            this.numState.Value = 0;
            this.numState.Maximum = this.states.States.Count;
            this.maxLabel.Text = "Max: "+this.states.States.Count;
            this.setState(0);

            this.inputUpdate();
        }

        private void setState(int state) {
            if (this.langValid && state >= 0 && state < this.states.States.Count) {
                this.boxStateFrags.Text = this.states.States[state].Fragments.JoinLines();
                this.boxStateActions.Text = this.states.States[state].Actions.JoinLines();
            } else {
                this.boxStateFrags.Text = "";
                this.boxStateActions.Text = "";
            }
        }

        private void inputUpdate() {
            try {
                if (!langValid) {
                    this.boxResultTree.Text = "";
                    return;
                }

                Result result = this.parser.Parse(this.boxInput.Text);
                this.boxResultTree.Text = result.ToString();
            } catch(Exception ex) {
                this.boxResultTree.Text = ex.Message;
            }
        }
    }
}