using Examples.Calculator;
using Examples.CodeColoring;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace ExamplesRunner
{
    public partial class MainForm: Form {

        public MainForm() {
            this.InitializeComponent();
            this.initializeCalc();
            this.initializeColoring();
        }

        #region Calculator Example

        private Calculator calc;

        private void initializeCalc() {
            this.calc = new Calculator();
        }

        private void solveCalc() {
            string input = this.calcInputBox.Text;
            if (!string.IsNullOrWhiteSpace(input)) {

                this.calc.Clear();
                this.calc.Calculate(input);
                string result = this.calc.StackToString();

                string nl = Environment.NewLine;
                this.calcResultBox.Text = ">" + input + nl + "   "+
                    result.Replace(nl, nl + "   ") + nl + nl + this.calcResultBox.Text;
            }
            this.calcInputBox.Clear();
        }

        private void btnCalcSolve_Click(object sender, EventArgs e) =>
            this.solveCalc();

        private void calcInputBox_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {
                this.solveCalc();
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        #endregion
        #region Code Coloring Example

        private bool debounceReady;

        private void initializeColoring() {
            this.debounceReady = true;
            this.colorLangBox.Items.Add(new Glsl());
            this.colorLangBox.SelectedIndex = 0;
        }

        private void debouncer_Tick(object sender, EventArgs e) => this.recolorCode();

        private IColorer selectedColorer => this.colorLangBox.SelectedItem as IColorer;

        private void setColor(int start, int length, Color color, Font font) {
            this.codeColoringBox.Select(start, length);
            this.codeColoringBox.SelectionColor = color;
            this.codeColoringBox.SelectionFont = font;
        }

        private void recolorCode() {
            this.debounceReady = false;
            this.debouncer.Stop();

            IColorer colorer = this.selectedColorer;
            if (colorer is null) return;

            // Suspend layout and store off the user's selection.
            this.codeColoringBox.SuspendLayout();
            int userStart = this.codeColoringBox.SelectionStart;
            int userLength = this.codeColoringBox.SelectionLength;

            // Set the colors of the text.
            int caret = 0;
            foreach (Formatting fmt in colorer.Colorize(this.codeColoringBox.Text)) {
                if (caret < fmt.Index)
                    this.setColor(caret, fmt.Index-caret, SystemColors.ControlText, this.codeColoringBox.Font);
                this.setColor(fmt.Index, fmt.Length, fmt.Color, fmt.Font);
                caret = fmt.Index+fmt.Length;
            }

            // Restore user's selection and resume layout.
            this.codeColoringBox.Select(userStart, userLength);
            this.codeColoringBox.ResumeLayout();
            this.debounceReady = true;
        }

        private void colorLangBox_SelectedIndexChanged(object sender, EventArgs e) {
            this.recolorCode();
        }

        private void btnColorExample_Click(object sender, EventArgs e) {
            this.codeColoringBox.Text = this.selectedColorer?.ExampleCode ?? "";
            this.recolorCode();
        }

        private void codeColoringBox_TextChanged(object sender, EventArgs e) {
            if (this.debounceReady) {
                this.debouncer.Start();
                this.debounceReady = false;
            }
        }

        #endregion
    }
}
