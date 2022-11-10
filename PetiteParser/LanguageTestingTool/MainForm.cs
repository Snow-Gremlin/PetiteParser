using PetiteParser.Analyzer;
using PetiteParser.Formatting;
using PetiteParser.Grammar;
using PetiteParser.Inspector;
using PetiteParser.Loader;
using PetiteParser.Logger;
using PetiteParser.Normalizer;
using PetiteParser.Parser;
using PetiteParser.Parser.States;
using PetiteParser.Parser.Table;
using PetiteParser.Tokenizer;
using System.Linq.Expressions;
using System.Text;

namespace LanguageTestingTool;

public partial class MainForm : Form {

    private bool langValid;
    private Tokenizer? tokenizer;
    private Grammar? grammar;
    private Grammar? normGrammar;
    private ParserStates? states;
    private Table? table;
    private Parser? parser;

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
            Inspector.Validate(grammar, log);
            if (log.Failed) {
                this.badLanguage(log.ToString());
                return;
            }

            this.normGrammar = Normalizer.GetNormal(this.grammar, log);
            if (log.Failed) {
                this.badLanguage(log.ToString());
                return;
            }

            this.states = new();
            this.states.DetermineStates(this.normGrammar, OnConflict.Panic);
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

        if (this.normGrammar is not null)
            this.boxNorm.Text = this.normGrammar.ToString();

        int stateCount = this.states is not null ? this.states.States.Count : 0;
        this.numState.Value = 0;
        this.numState.Maximum = stateCount;
        this.maxLabel.Text = "Max: "+stateCount;
        this.setState(0);

        this.inputUpdate();
    }

    private void setState(int state) {
        if (this.langValid && state >= 0 && this.states is not null && state < this.states.States.Count) {
            this.boxStateFrags.Text = this.states.States[state].Fragments.JoinLines();

            /*
            // TODO: FIX
            StringBuilder result = new();
            foreach (KeyValuePair<Item, StateAction> pair in this.states.States[state].Actions)
                result.AppendLine(pair.Key + ": " + pair.Value.Action);
            this.boxStateActions.Text = result.ToString();
            */
        } else {
            this.boxStateFrags.Text = "";
            this.boxStateActions.Text = "";
        }
    }

    private void inputUpdate() {
        try {
            if (!langValid || this.parser is null) {
                this.boxResultTree.Text = "";
                return;
            }

            Result result = this.parser.Parse(this.boxInput.Text);
            this.boxResultTree.Text = result.ToString();
        } catch (Exception ex) {
            this.boxResultTree.Text = ex.Message;
        }
    }
}
