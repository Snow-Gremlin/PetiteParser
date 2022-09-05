using PetiteParser.Loader;
using PetiteParser.Parser;

namespace LanguageTestingTool {
    public partial class MainForm : Form {

        private bool langValid;
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
                this.parser = Loader.LoadParser(this.boxLang.Text);
                this.langValid = true;
                
            } catch (Exception ex) {
                this.langValid = false;
                this.boxLangResult.Text = ex.Message;
            }
        }

        private void setState(int state) {

        }

        private void inputUpdate() {

        }
    }
}