using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PetiteParser.Parser;

namespace TestPetiteParser {

    [TestClass]
    public class LoaderUnitTests {

        /// <summary>Checks the parser will parse the given input.</summary>
        static private void checkTokenizer(Parser parser, string input, params string[] expected) {
            Result parseResult = parser.Parse(input);
            string exp = string.Join(Environment.NewLine, expected);
            string result = parseResult.ToString();
            Assert.AreEqual(exp, result);
        }

        /// <summary>Checks the parser will parse the given input.</summary>
        static private void checkParser(Parser parser, string input, params string[] expected) {
            Result parseResult = parser.Parse(input);
            string exp = string.Join(Environment.NewLine, expected);
            string result = parseResult.ToString();
            Assert.AreEqual(exp, result);
        }

        [TestMethod]
        public void Loader1() {
            //Parser parser = new(
            //    "> (Start);");
            //checkParser(parser, "",
            //    "");




        }
    }
}
