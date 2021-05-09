using System;

namespace CalculatorExample {

    static public class EntryPoint {

        static public void Main() {
            Calculator.Calculator.LoadParser();
            Calculator.Calculator calc = new();

            Console.WriteLine("Enter in an equation and press enter to calculate the result.");
            Console.WriteLine("Type \"exit\" to exit. See documentation for more information.");

            while (true) {
                Console.Write("> ");
                string input = Console.ReadLine();
                if (input.ToLower() == "exit") break;

                calc.Clear();
                calc.Calculate(input);
                Console.WriteLine(calc.StackToString());
            }
        }
    }
}
