using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Examples;
using Examples.Calculator;

namespace ExamplesRunner {
    public partial class MainForm: Form {

        private Calculator calc;

        public MainForm() {
            InitializeComponent();

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

        private void colorLangBox_SelectedIndexChanged(object sender, EventArgs e) {

        }

        private void btnColorExample_Click(object sender, EventArgs e) {

        }

        private void codeColoringBox_TextChanged(object sender, EventArgs e) {

        }
    }
}
