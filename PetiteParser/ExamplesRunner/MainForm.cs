using Examples.Calculator;
using Examples.CodeColoring;
using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

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

        private void initializeCalc() =>
            this.calc = new Calculator();

        private void solveCalc() {
            string input = this.calcInputBox.Text;
            if (!string.IsNullOrWhiteSpace(input)) {

                this.calc.Clear();
                this.calc.Calculate(input);
                string result = this.calc.StackToString();

                string nl = Environment.NewLine;
                this.calcResultBox.Text = ">" + input + nl + "   "+
                    result.Replace(nl, nl + "   ") + nl + nl + this.calcResultBox.Text;

                if (!this.calc.StackContainsError) this.calcInputBox.Clear();
            }
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
        private List<Formatting> prevFmt;

        private void initializeColoring() {
            this.debounceReady = true;
            this.prevFmt = new List<Formatting>();

            this.colorLangBox.Items.Add(new Glsl());
            this.colorLangBox.SelectedIndex = 0;
        }

        private void debouncer_Tick(object sender, EventArgs e) => this.recolorCode();

        private IColorer selectedColorer => this.colorLangBox.SelectedItem as IColorer;

        private void setColor(int start, int length, Color color, Font font) {
            this.codeColoringBox.Select(start-1, length);
            this.codeColoringBox.SelectionColor = color;
            this.codeColoringBox.SelectionFont  = font;
        }

        private int fromStart(int minLen, List<Formatting> curFmt) {
            for (int i = 0; i < minLen; ++i) {
                if (!this.prevFmt[i].Same(curFmt[i]))
                    return i;
            }
            return minLen;
        }

        private int fromEnd(int start, int minLen, List<Formatting> prevFmt, List<Formatting> curFmt) {
            for (int i = start, j = 1; i < minLen; ++i, ++j) {
                if (!this.prevFmt[^j].Same(curFmt[^j]))
                    return curFmt.Count-j;
            }
            return curFmt.Count-1-minLen;
        }

        private void recolorCode() {
            this.debounceReady = false;
            this.debouncer.Stop();

            IColorer colorer = this.selectedColorer;
            if (colorer is null) {
                this.debounceReady = true;
                return;
            }

            // Suspend layout and store off the user's selection.
            this.codeColoringBox.SuspendLayout();
            int userStart  = this.codeColoringBox.SelectionStart;
            int userLength = this.codeColoringBox.SelectionLength;

            // Get the new coloring and find what is different.
            string text = this.codeColoringBox.Text;
            List<Formatting> curFmt = new(colorer.Colorize(text));
            if (curFmt.Count <= 0) {
                this.setColor(1, text.Length, SystemColors.ControlText, this.codeColoringBox.Font);
            } else {
                int minLen = Math.Min(this.prevFmt.Count, curFmt.Count);
                int start  = this.fromStart(minLen, curFmt);
                int end    = this.fromEnd(start, minLen, this.prevFmt, curFmt);

                // Set the colors of the text.
                int caret = 0;
                bool notFirst = false;
                for (int i = start; i <= end; ++i) {
                    Formatting fmt = curFmt[i];
                    int index = fmt.Token.Start.Index;
                    int length = fmt.Token.End.Index-index+1;
                    if (notFirst && (caret < index))
                        this.setColor(caret, index-caret, SystemColors.ControlText, this.codeColoringBox.Font);
                    this.setColor(index, length, fmt.Color, fmt.Font);
                    caret = index+length;
                    notFirst = true;
                }
            }
            this.prevFmt = curFmt;

            // Restore user's selection and resume layout.
            this.codeColoringBox.Select(userStart, userLength);
            this.codeColoringBox.ResumeLayout();
            this.debounceReady = true;
        }

        private void colorLangBox_SelectedIndexChanged(object sender, EventArgs e) =>
            this.recolorCode();

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
