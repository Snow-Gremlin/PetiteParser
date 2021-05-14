using Examples.Calculator;
using Examples.CodeColoring;
using System;
using System.Windows.Forms;

namespace ExamplesRunner
{
    public partial class MainForm: Form {

        private Calculator calc;

        public MainForm() {
            this.InitializeComponent();

            this.calc = new Calculator();

            this.colorLangBox.Items.Add(new Glsl());
            this.colorLangBox.SelectedIndex = 0;
        }

        #region Calculator Example

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

        private IColorer selectedColorer => this.colorLangBox.SelectedItem as IColorer;

        private void recolorCode() {

        }

        private void colorLangBox_SelectedIndexChanged(object sender, EventArgs e) {
            this.recolorCode();
        }

        private void btnColorExample_Click(object sender, EventArgs e) {
            this.codeColoringBox.Text = this.selectedColorer?.ExampleCode ?? "";
            this.recolorCode();
        }

        private void codeColoringBox_TextChanged(object sender, EventArgs e) {
            // TODO: Debounce
            this.recolorCode();
        }

        #endregion
    }
}
