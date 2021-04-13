using Microsoft.VisualStudio.TestTools.UnitTesting;
using PetiteParser.Calculator;
using System;
using System.Collections.Generic;

namespace TestPetiteParser {

    [TestClass]
    public class CalculatorUnitTests {

        /// Checks that the given input to the given calculator will result in the expected lines on the stack.
        static private void checkCalc(Calculator calc, string input, params string[] expected) {
            calc.Clear();
            calc.Calculate(input);
            string result = calc.StackToString();
            string exp = string.Join(Environment.NewLine, expected);
            Assert.AreEqual(exp, result);
        }

        [TestMethod]
        public void Calculator1() {
            Calculator calc = new();
            checkCalc(calc, "", "no result");
            checkCalc(calc, "42", "42");
            checkCalc(calc, "2 * 3", "6");
            checkCalc(calc, "2 + 3", "5");
            checkCalc(calc, "2 * 3 + 5", "11");
            checkCalc(calc, "2 * (3 + 5)", "16");
            checkCalc(calc, "(2 * 3) + 5", "11");
            checkCalc(calc, "(2 * (3 + 5))", "16");
            checkCalc(calc, "2*5 + 5*2", "20");
            checkCalc(calc, "12 - 5", "7");
            checkCalc(calc, "12 + -5", "7");
            checkCalc(calc, "2*6 - 5", "7");
            checkCalc(calc, "2*2*3 + 5*(-1)", "7");
            checkCalc(calc, "2*2*3 + 5*(-1)", "7");
            checkCalc(calc, "2**3", "8");
            checkCalc(calc, "1100b", "12");
            checkCalc(calc, "0xF00A", "61450");
            checkCalc(calc, "77o", "63");
            checkCalc(calc, "42d", "42");
        }

        [TestMethod]
        public void Calculator2() {
            Calculator calc = new();
            checkCalc(calc, "3.14", "3.14");
            checkCalc(calc, "314e-2", "3.14");
            checkCalc(calc, "314.0e-2", "3.14");
            checkCalc(calc, "31.4e-1", "3.14");
            checkCalc(calc, "0.0314e2", "3.14");
            checkCalc(calc, "0.0314e+2", "3.14");
            checkCalc(calc, "2.0 * 3", "6.0");
            checkCalc(calc, "2 * 3.0", "6.0");
            checkCalc(calc, "2.0 * 3.0", "6.0");
            checkCalc(calc, "real(2) * 3", "6.0");
            checkCalc(calc, "2.0 - 3", "-1.0");
            checkCalc(calc, "2.0 ** 3", "8.0");
        }

        [TestMethod]
        public void Calculator3() {
            Calculator calc = new();
            checkCalc(calc, "min(2, 4, 3)", "2");
            checkCalc(calc, "max(2, 4, 3)", "4");
            checkCalc(calc, "sum(2, 4, 3)", "9");
            checkCalc(calc, "avg(2, 4, 3)", "3.0");
            checkCalc(calc, "min(2+4, 4-2, 1*3)", "2");
            checkCalc(calc, "floor(3.5)", "3");
            checkCalc(calc, "round(3.5)", "4");
            checkCalc(calc, "ceil(3.5)", "4");
        }

        [TestMethod]
        public void Calculator4() {
            Calculator calc = new();
            checkCalc(calc, "square(11)",
               "Errors in calculator input:",
               "   No function called square found.");

            calc.AddFunc("square", delegate (List<object> list) {
                if (list.Count != 1) throw new PetiteParser.Misc.Exception("Square may one and only one input.");
                Variant v = new(list[0]);
                return v.ImplicitInt ? (object)(v.AsInt*v.AsInt) :
                     v.ImplicitReal ? (object)(v.AsReal*v.AsReal) :
                     throw new PetiteParser.Misc.Exception("May only square an int or real number but got "+v+".");
            });

            checkCalc(calc, "square(11)", "121");
            checkCalc(calc, "square(-4.33)", "18.7489");
            checkCalc(calc, "square(\"cat\")",
               "Errors in calculator input:",
               "   May only square an int or real number but got String(cat).");
        }

        [TestMethod]
        public void Calculator5() {
            Calculator calc = new();
            checkCalc(calc, "\"cat\" + \"9\"", "cat9");
            checkCalc(calc, "\"cat\" + string(9)", "cat9");
            checkCalc(calc, "\"cat\" + string(6 + int(\"3\"))", "cat9");
            checkCalc(calc, "bin(42)", "101010b");
            checkCalc(calc, "oct(42)", "52o");
            checkCalc(calc, "hex(42)", "0x2A");
            checkCalc(calc, "upper(\"CAT-cat\")", "CAT-CAT");
            checkCalc(calc, "lower(\"CAT-cat\")", "cat-cat");

            checkCalc(calc, "sub(\"catch\", 0, 3)", "cat");
            checkCalc(calc, "sub(\"catch\", 1, 3)", "at");
            checkCalc(calc, "sub(\"catch\", 3, 5)", "ch");
            checkCalc(calc, "sub(\"catch\", 3, 3)", "");
            checkCalc(calc, "sub(\"catch\", 3, 1)",
               "Errors in calculator input:",
               "   Invalid substring range: 3..1");

            checkCalc(calc, "len(\"catch\")", "5");
            checkCalc(calc, "len(\"cat\")", "3");
            checkCalc(calc, "len(\"\\\"\")", "1");
            checkCalc(calc, "len(\"\")", "0");
            checkCalc(calc, "bool(\"tr\"+\"ue\")", "true");
        }

        [TestMethod]
        public void Calculator6() {
            Calculator calc = new();
            checkCalc(calc, "hex(0xFF00 & 0xF0F0)", "0xF000");
            checkCalc(calc, "hex(0xFF00 | 0xF0F0)", "0xFFF0");
            checkCalc(calc, "hex(0xFF00 ^ 0xF0F0)", "0xFF0");
            checkCalc(calc, "hex(~0xFF00 & 0x0FF0)", "0xF0");

            checkCalc(calc, "!true", "false");
            checkCalc(calc, "!false", "true");
            checkCalc(calc, "true & true", "true");
            checkCalc(calc, "true & false", "false");
            checkCalc(calc, "false & true", "false");
            checkCalc(calc, "false & false", "false");
            checkCalc(calc, "true | true", "true");
            checkCalc(calc, "true | false", "true");
            checkCalc(calc, "false | true", "true");
            checkCalc(calc, "false | false", "false");
            checkCalc(calc, "true ^ true", "false");
            checkCalc(calc, "true ^ false", "true");
            checkCalc(calc, "false ^ true", "true");
            checkCalc(calc, "false ^ false", "false");
        }

        [TestMethod]
        public void Calculator7() {
            Calculator calc = new();
            checkCalc(calc, "10 == 3", "false");
            checkCalc(calc, "3 == 3", "true");
            checkCalc(calc, "10 != 3", "true");
            checkCalc(calc, "3 != 3", "false");
            checkCalc(calc, "10 < 3", "false");
            checkCalc(calc, "3 < 3", "false");
            checkCalc(calc, "3 <= 3", "true");
            checkCalc(calc, "3 <= 10", "true");
            checkCalc(calc, "10 <= 3", "false");
            checkCalc(calc, "2 < 3", "true");
            checkCalc(calc, "10 > 3", "true");
            checkCalc(calc, "3 > 3", "false");
            checkCalc(calc, "3 >= 3", "true");
            checkCalc(calc, "10 >= 3", "true");
            checkCalc(calc, "3 >= 10", "false");
            checkCalc(calc, "3 > 2", "true");

            checkCalc(calc, "3 == 3.0", "true");
            checkCalc(calc, "\"3\" == 3", "false");
            checkCalc(calc, "\"3\" == string(3)", "true");
            checkCalc(calc, "true == false", "false");
            checkCalc(calc, "true != false", "true");
        }

        [TestMethod]
        public void Calculator8() {
            Calculator calc = new();
            checkCalc(calc, "(3 == 2) | (4 < 10)", "true");
            checkCalc(calc, "x := 4+5; y := 9; x == y; x+y", "true, 18");
            checkCalc(calc, "x", "9");
            checkCalc(calc, "z",
               "Errors in calculator input:",
               "   No constant called z found.");
            calc.SetVar("z", true);
            checkCalc(calc, "z", "true");
            checkCalc(calc, "e", "2.718281828459045");
            checkCalc(calc, "pi", "3.141592653589793");
            checkCalc(calc, "cos(pi)", "-1.0");
        }

        [TestMethod]
        public void Calculator9() {
            Calculator calc = new();
            checkCalc(calc, "padLeft(\"Hello\", 12)", "       Hello");
            checkCalc(calc, "padRight(\"Hello\", 12)", "Hello       ");
            checkCalc(calc, "padLeft(\"Hello\", 12, \"-\")", "-------Hello");
            checkCalc(calc, "padRight(\"Hello\", 12, \"-\")", "Hello-------");
            checkCalc(calc, "trim(\"   Hello   \")", "Hello");
            checkCalc(calc, "trimLeft(\"   Hello   \")", "Hello   ");
            checkCalc(calc, "trimRight(\"   Hello   \")", "   Hello");
            checkCalc(calc, "trim(str(1))",
               "Errors in calculator input:",
               "   No function called str found.");
            checkCalc(calc, "join(\"a\", \"b\", \"c\", \"d\")", "bacad");
        }
    }
}
