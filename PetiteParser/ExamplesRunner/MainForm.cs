using Examples.Calculator;
using Examples.CodeColoring;
using Examples.CodeColoring.Glsl;
using PetiteParser.Diff;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace ExamplesRunner {
    public partial class MainForm: Form {

        public MainForm() {
            this.InitializeComponent();
            this.initializeCalc();
            this.initializeColoring();
            this.initializeDiff();
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

        private bool colorDebounceReady;
        private List<Formatting> prevFmt;

        private void initializeColoring() {
            this.colorDebounceReady = true;
            this.prevFmt = new List<Formatting>();

            foreach (IColorer colorer in IColorer.Colorers)
                this.colorLangBox.Items.Add(colorer);
            this.colorLangBox.SelectedIndex = 0;
        }

        private void colorDebouncer_Tick(object sender, EventArgs e) => this.recolorCode();

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

        private int fromEnd(int start, int minLen, List<Formatting> curFmt) {
            for (int i = start, j = 1; i < minLen; ++i, ++j) {
                if (!this.prevFmt[^j].Same(curFmt[^j]))
                    return curFmt.Count-j;
            }
            return curFmt.Count-1-minLen;
        }

        private void recolorCode() {
            this.colorDebounceReady = false;
            this.colorDebouncer.Stop();

            IColorer colorer = this.selectedColorer;
            if (colorer is null) {
                this.colorDebounceReady = true;
                return;
            }

            // Suspend layout and store off the user's selection.
            this.codeColoringBox.SuspendLayout();
            int userStart  = this.codeColoringBox.SelectionStart;
            int userLength = this.codeColoringBox.SelectionLength;

            // Get the new coloring and find what is different.
            string text = this.codeColoringBox.Text;
            List<Formatting> curFmt = colorer.Colorize(text).ToList();
            if (curFmt.Count > 0) {
                int minLen = Math.Min(this.prevFmt.Count, curFmt.Count);
                int start  = this.fromStart(minLen, curFmt);
                int end    = this.fromEnd(start, minLen, curFmt);

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

                this.prevFmt = curFmt;
            }

            // Restore user's selection and resume layout.
            this.codeColoringBox.Select(userStart, userLength);
            this.codeColoringBox.ResumeLayout();
            this.colorDebounceReady = true;
        }

        private void colorLangBox_SelectedIndexChanged(object sender, EventArgs e) =>
            this.recolorCode();

        private void btnColorExample_Click(object sender, EventArgs e) {
            this.codeColoringBox.Text = this.selectedColorer?.ExampleCode ?? "";
            this.recolorCode();
        }

        private void codeColoringBox_TextChanged(object sender, EventArgs e) {
            if (this.colorDebounceReady) {
                this.colorDebouncer.Start();
                this.colorDebounceReady = false;
            }
        }

        #endregion
        #region Diff Example

        private bool diffDebounceReady;
        private Diff diffInstance;

        private void initializeDiff() {
            this.diffDebounceReady = true;
            // Use a single instance to reuse previously allocated memory.
            this.diffInstance = Diff.Default();

            // The following texts come from https://en.wikipedia.org/wiki/Diff
            this.textBoxAdded.Lines = new string[] {
                "This part of the",
                "document has stayed the",
                "same from version to",
                "version.  It shouldn't",
                "be shown if it doesn't",
                "change.  Otherwise, that",
                "would not be helping to",
                "compress the size of the",
                "changes.",
                "",
                "This paragraph contains",
                "text that is outdated.",
                "It will be deleted in the",
                "near future.",
                "",
                "It is important to spell",
                "check this dokument. On",
                "the other hand, a",
                "misspelled word isn't",
                "the end of the world.",
                "Nothing in the rest of",
                "this paragraph needs to",
                "be changed. Things can",
                "be added after it."};

            this.textBoxRemoved.Lines = new string[] {
                "This is an important",
                "notice! It should",
                "therefore be located at",
                "the beginning of this",
                "document!",
                "",
                "This part of the",
                "document has stayed the",
                "same from version to",
                "version.  It shouldn't",
                "be shown if it doesn't",
                "change.  Otherwise, that",
                "would not be helping to",
                "compress the size of the",
                "changes.",
                "",
                "It is important to spell",
                "check this document. On",
                "the other hand, a",
                "misspelled word isn't",
                "the end of the world.",
                "Nothing in the rest of",
                "this paragraph needs to",
                "be changed. Things can",
                "be added after it.",
                "",
                "This paragraph contains",
                "important new additions",
                "to this document."};
        }

        private void diffDebouncer_Tick(object sender, EventArgs e) => this.performDiff();

        private void inputDiff_TextChanged(object sender, EventArgs e) {
            if (this.diffDebounceReady) {
                this.diffDebouncer.Start();
                this.diffDebounceReady = false;
            }
        }

        private void performDiff() {
            this.diffDebounceReady = false;
            this.diffDebouncer.Stop();

            string[] added = this.textBoxAdded.Lines;
            string[] removed = this.textBoxRemoved.Lines;
            string[] diff = this.diffInstance.PlusMinus(added, removed).ToArray();
            this.textBoxDiff.Lines = diff;

            this.diffDebounceReady = true;
        }

        #endregion
    }
}
