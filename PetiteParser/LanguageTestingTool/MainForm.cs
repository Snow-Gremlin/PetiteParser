using PetiteParser.Analyzer;
using PetiteParser.Grammar;
using PetiteParser.Loader;
using PetiteParser.Logger;
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
        
        private void numState_ValueChanged(object sender, EventArgs e) {
            this.setState((int)this.numState.Value);
        }

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
                
                Log log = new();
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
            this.boxState.Text = "";
            
            this.inputUpdate();
        }

        private void languageGood() {
            this.langValid = true;
            this.boxLangResult.Text = "Success";

            this.boxNorm.Text = this.normGrammar.ToString();

            this.numState.Value = 0;
            this.numState.Maximum = this.states.States.Count;
            this.maxLabel.Text = "Max: "+this.states.States.Count;
            this.boxState.Text = this.states.States[0].ToString("", false);

            this.inputUpdate();
        }

        private void setState(int state) =>
            this.boxState.Text = this.langValid && state > 0 && state <= this.states.States.Count ?
                states.States[state].ToString("", false) : "";

        private void inputUpdate() {

        }
    }
}
