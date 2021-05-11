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

            Calculator.LoadParser();
            this.calc = new Calculator();


            //```
            //Calculator.Calculator.LoadParser();
            //Calculator.Calculator calc = new();

            //Console.WriteLine("Enter in an equation and press enter to calculate the result.");
            //Console.WriteLine("Type \"exit\" to exit. See documentation for more information.");

            //while (true) {
            //    Console.Write("> ");
            //    string input = Console.ReadLine();
            //    if (input.ToLower() == "exit") break;

            //    calc.Clear();
            //    calc.Calculate(input);
            //    Console.WriteLine(calc.StackToString());
            //}
            //```
        }

        private void btnCalcSolve_Click(object sender, EventArgs e) {

        }

        private void calcInputBox_KeyDown(object sender, KeyEventArgs e) {
            if (e.KeyCode == Keys.Enter) {

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
