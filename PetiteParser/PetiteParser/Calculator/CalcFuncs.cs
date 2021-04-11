using System;
using System.Collections.Generic;

namespace PetiteParser.Calculator {

    /// <summary>This is the signature for functions which can be called by the calculator.</summary>
    /// <param name="args">The argument values for the function.</param>
    /// <returns>The resulting value.</returns>
    public delegate object CalcFunc(List<object> args);

    /// <summary>This is a collection of functions for the calculator.</summary>
    public class CalcFuncs {
        private Random rand;
        private Dictionary<string, CalcFunc> funcs;

        /// <summary>Creates a new collection of calculator function.</summary>
        public CalcFuncs(int randomSeed = 0) {
            this.rand = new Random(randomSeed);
            this.funcs = new Dictionary<string, CalcFunc>() {
                { "abs",       funcAbs },
                { "acos",      funcAcos },
                { "asin",      funcAsin },
                { "atan",      funcAtan },
                { "atan2",     funcAtan2 },
                { "avg",       funcAvg },
                { "bin",       funcBin },
                { "bool",      funcBool },
                { "ceil",      funcCeil },
                { "cos",       funcCos },
                { "floor",     funcFloor },
                { "hex",       funcHex },
                { "int",       funcInt },
                { "join",      funcJoin },
                { "len",       funcLen },
                { "log",       funcLog },
                { "log2",      funcLog2 },
                { "log10",     funcLog10 },
                { "lower",     funcLower },
                { "ln",        funcLn },
                { "max",       funcMax },
                { "min",       funcMin },
                { "oct",       funcOct },
                { "padleft",   funcPadLeft },
                { "padright",  funcPadRight },
                { "rand",      this.funcRand },
                { "real",      funcReal },
                { "round",     funcRound },
                { "sin",       funcSin },
                { "sqrt",      funcSqrt },
                { "string",    funcString },
                { "sub",       funcSub },
                { "sum",       funcSum },
                { "tan",       funcTan },
                { "trim",      funcTrim },
                { "trimleft",  funcTrimLeft },
                { "trimright", funcTrimRight },
                { "upper",     funcUpper },
            };
        }

        /// <summary>
        /// Adds a new function that can be called by the language.
        /// Set to null to remove a function.
        /// </summary>
        /// <param name="name">The name of the function.</param>
        /// <param name="hndl">The new function handle or null.</param>
        public void AddFunc(string name, CalcFunc hndl) {
            if (hndl == null) this.funcs.Remove(name);
            else this.funcs[name] = hndl;
        }

        /// <summary>Finds the function with the given name.</summary>
        /// <param name="name">The name of the function to look up.</param>
        /// <returns>The function for the given name.</returns>
        public CalcFunc FindFunc(string name) => 
            this.funcs.ContainsKey(name) ? this.funcs[name] : null;

        /// <summary>This checks that the specified number of arguments has been given.</summary>
        /// <param name="name">The name of the function being checked.</param>
        /// <param name="args">The arguments for the function.</param>
        /// <param name="count">The expected number of arguments.</param>
        static private void argCount(string name, List<object> args, int count) {
            if (args.Count != count)
                throw new Exception("The function "+name+" requires "+count+" arguments but got "+args.Count+".");
        }

        #region Function Definitions...

        /// This function puts a random number onto the stack.
        private object funcRand(List<object> args) {
            argCount("rand", args, 0);
            return this.rand.NextDouble();
        }

        /// This function gets the absolute value of the given integer or real.
        static private object funcAbs(List<object> args) {
            argCount("abs", args, 1);
            Variant arg = new(args[0]);
            return arg.ImplicitInt ? Math.Abs(arg.AsInt) :
                arg.ImplicitReal ? Math.Abs(arg.AsReal) :
                throw new Exception("Can not use "+arg+" in abs(int) or abs(real).");
        }

        /// This function gets the arccosine of the given real.
        static private object funcAcos(List<object> args) {
            argCount("acos", args, 1);
            Variant arg = new(args[0]);
            return arg.ImplicitReal ? Math.Acos(arg.AsReal) :
                throw new Exception("Can not use "+arg+" in acos(real).");
        }

        /// This function gets the arcsine of the given real.
        static private object funcAsin(List<object> args) {
            argCount("asin", args, 1);
            Variant arg = new(args[0]);
            return arg.ImplicitReal ? Math.Asin(arg.AsReal) :
                throw new Exception("Can not use "+arg+" in asin(real).");
        }

        /// This function gets the arctangent of the given real.
        static private object funcAtan(List<object> args) {
            argCount("atan", args, 1);
            Variant arg = new(args[0]);
            return arg.ImplicitReal ? Math.Atan(arg.AsReal) :
                throw new Exception("Can not use "+arg+" in atan(real).");
        }

        /// This function gets the arctangent of the two given reals.
        static private object funcAtan2(List<object> args) {
            argCount("atan2", args, 2);
            Variant left  = new(args[0]);
            Variant right = new(args[1]);
            return left.ImplicitReal && right.ImplicitReal ? Math.Atan2(left.AsReal, right.AsReal) :
                throw new Exception("Can not use "+left+" and "+right+" in atan2(real, real).");
        }

        /// This function gets the average of one or more reals.
        static private object funcAvg(List<object> args) {
            if (args.Count <= 0)
                throw new Exception("The function avg requires at least one argument.");
            double sum = 0.0;
            foreach (object arg in args) {
                Variant value = new(arg);
                if (value.ImplicitReal) sum += value.AsReal;
                else throw new Exception("Can not use "+value+" in avg(real, real, ...).");
            }
            return sum / args.Count;
        }

        /// This function gets the binary formatted integer as a string.
        static private object funcBin(List<object> args) {
            argCount("bin", args, 1);
            Variant arg = new(args[0]);
            return arg.ImplicitInt ? Convert.ToString(arg.AsInt, 2)+"b" :
                throw new Exception("Can not use "+arg+" in bin(int)");
        }

        /// This function casts the given value into a Boolean value.
        static private object funcBool(List<object> args) {
            argCount("bool", args, 1);
            Variant arg = new(args[0]);
            return arg.AsBool;
        }

        /// This function gets the ceiling of the given real.
        static private object funcCeil(List<object> args) {
            argCount("ceil", args, 1);
            Variant arg = new(args[0]);
            return arg.ImplicitReal ? (int)Math.Ceiling(arg.AsReal) :
                throw new Exception("Can not use "+arg+" to ceil(real) or already an int.");
        }

        /// This function gets the cosine of the given real.
        static private object funcCos(List<object> args) {
            argCount("cos", args, 1);
            Variant arg = new(args[0]);
            return arg.ImplicitReal ? Math.Cos(arg.AsReal) :
                throw new Exception("Can not use "+arg+" in cos(real).");
        }

        /// This function gets the floor of the given real.
        static private object funcFloor(List<object> args) {
            argCount("floor", args, 1);
            Variant arg = new(args[0]);
            return arg.ImplicitReal ? (int)Math.Floor(arg.AsReal) :
                throw new Exception("Can not use "+arg+" to floor(real) or already an int.");
        }

        /// This function gets the hexadecimal formatted integer as a string.
        static private object funcHex(List<object> args) {
            argCount("hex", args, 1);
            Variant arg = new(args[0]);
            return arg.ImplicitInt ? "0x"+Convert.ToString(arg.AsInt, 16).ToUpper() :
                throw new Exception("Can not use "+arg+" to hex(int).");
        }

        /// This function casts the given value into an integer value.
        static private object funcInt(List<object> args) {
            argCount("int", args, 1);
            Variant arg = new(args[0]);
            return arg.AsInt;
        }

        /// This function joins the given strings.
        static private object funcJoin(List<object> args) {
            if (args.Count <= 0)
                throw new Exception("The function join requires at least one argument.");
            Variant sep = new(args[0]);
            if (!sep.ImplicitStr)
                throw new Exception("Can not use "+sep+" in join(string, string, ...)");
            List<string> parts = new();
            for (int i = 1; i < args.Count; ++i) {
                Variant value = new(args[i]);
                if (value.ImplicitStr) parts.Add(value.AsStr);
                else throw new Exception("Can not use "+value+" in join(string, string, ...).");
            }
            return string.Join(sep.AsStr, parts);
        }

        /// This function gets the length of a string.
        static private object funcLen(List<object> args) {
            argCount("len", args, 1);
            Variant arg = new(args[0]);
            return arg.ImplicitStr ? arg.AsStr.Length :
                throw new Exception("Can not use "+arg+" to len(string).");
        }

        /// This function gets the log of the given real with the base of another real.
        static private object funcLog(List<object> args) {
            argCount("log", args, 2);
            Variant left  = new(args[0]);
            Variant right = new(args[1]);
            return left.ImplicitReal && right.ImplicitReal ? Math.Log(left.AsReal, right.AsReal) :
                throw new Exception("Can not use "+left+" and "+right+" in log(real, real).");
        }

        /// This function gets the log base 2 of the given real.
        static private object funcLog2(List<object> args) {
            argCount("log2", args, 1);
            Variant arg = new(args[0]);
            return arg.ImplicitReal ? Math.Log2(arg.AsReal) :
                throw new Exception("Can not use "+arg+" in log2(real).");
        }

        /// This function gets the log base 10 of the given real.
        static private object funcLog10(List<object> args) {
            argCount("log10", args, 1);
            Variant arg = new(args[0]);
            return arg.ImplicitReal ? Math.Log10(arg.AsReal) :
                throw new Exception("Can not use "+arg+" in log10(real).");
        }

        /// This function gets the lower case of the given string.
        static private object funcLower(List<object> args) {
            argCount("lower", args, 1);
            Variant arg = new(args[0]);
            return arg.ImplicitStr ? arg.AsStr.ToLower() :
                throw new Exception("Can not use "+arg+" in lower(string).");
        }

        /// This function gets the natural log of the given real.
        static private object funcLn(List<object> args) {
            argCount("ln", args, 1);
            Variant arg = new(args[0]);
            return arg.ImplicitReal ? Math.Log(arg.AsReal) :
                throw new Exception("Can not use "+arg+" in ln(real).");
        }

        /// This function gets the maximum value of one or more integers or reals.
        static private object funcMax(List<object> args) {
            if (args.Count <= 0)
                throw new Exception("The function max requires at least one argument.");
            bool allInt = true;
            foreach (object arg in args) {
                Variant value = new(arg);
                if (value.ImplicitInt) continue;
                allInt = false;
                if (value.ImplicitReal) continue;
                throw new Exception("Can not use "+arg+" in max(real, real, ...) or max(int, int, ...).");
            }

            if (allInt) {
                int value = new Variant(args[0]).AsInt;
                for (int i = args.Count - 1; i > 0; --i)
                    value = Math.Max(value, new Variant(args[i]).AsInt);
                return value;
            } else {
                double value = new Variant(args[0]).AsReal;
                for (int i = args.Count - 1; i > 0; --i)
                    value = Math.Max(value, new Variant(args[i]).AsReal);
                return value;
            }
        }

        /// This function gets the minimum value of one or more integers or reals.
        static private object funcMin(List<object> args) {
            if (args.Count <= 0)
                throw new Exception("The function min requires at least one argument.");
            bool allInt = true;
            foreach (object arg in args) {
                Variant value = new(arg);
                if (value.ImplicitInt) continue;
                allInt = false;
                if (value.ImplicitReal) continue;
                throw new Exception("Can not use "+arg+" in min(real, real, ...) or min(int, int, ...).");
            }

            if (allInt) {
                int value = new Variant(args[0]).AsInt;
                for (int i = args.Count - 1; i > 0; --i)
                    value = Math.Min(value, new Variant(args[i]).AsInt);
                return value;
            } else {
                double value = new Variant(args[0]).AsReal;
                for (int i = args.Count - 1; i > 0; --i)
                    value = Math.Min(value, new Variant(args[i]).AsReal);
                return value;
            }
        }

        /// This function gets the octal formatted integer as a string.
        static private object funcOct(List<object> args) {
            argCount("oct", args, 1);
            Variant arg = new(args[0]);
            return arg.ImplicitInt ? Convert.ToString(arg.AsInt, 8)+"o" :
                throw new Exception("Can not use "+arg+" to oct(int).");
        }

        /// This function pads the string on the left side with an optional character
        /// until the string's length is equal to a specified length.
        static private object funcPadLeft(List<object> args) {
            if (args.Count < 2 || args.Count > 3)
                throw new Exception("The function padLeft requires 2 or 3 arguments but got "+args.Count+".");
            Variant arg0 = new(args[0]);
            Variant arg1 = new(args[1]);
            Variant arg2 = new((args.Count == 3) ? args[2] : " ");
            if (arg0.ImplicitStr && arg1.ImplicitInt && arg2.ImplicitStr) {
                string padding = arg2.AsStr;
                return arg0.AsStr.PadLeft(arg1.AsInt, padding.Length > 0 ? padding[0] : ' ');
            }
            throw new Exception("Can not use "+arg0+", "+arg1+", and "+arg2+" in padLeft(string, int, [string]).");
        }

        /// This function pads the string on the right side with an optional character
        /// until the string's length is equal to a specified length.
        static private object funcPadRight(List<object> args) {
            if (args.Count < 2 || args.Count > 3)
                throw new Exception("The function padRight requires 2 or 3 arguments but got "+args.Count+".");
            Variant arg0 = new(args[0]);
            Variant arg1 = new(args[1]);
            Variant arg2 = new((args.Count == 3) ? args[2] : " ");
            if (arg0.ImplicitStr && arg1.ImplicitInt && arg2.ImplicitStr) {
                string padding = arg2.AsStr;
                return arg0.AsStr.PadRight(arg1.AsInt, padding.Length > 0 ? padding[0] : ' ');
            }
            throw new Exception("Can not use "+arg0+", "+arg1+", and "+arg2+" in padRight(string, int, [string]).");
        }

        /// This function casts the given value into a real value.
        static private object funcReal(List<object> args) {
            argCount("real", args, 1);
            Variant arg = new(args[0]);
            return arg.AsReal;
        }

        /// This function gets the round of the given real.
        static private object funcRound(List<object> args) {
            argCount("round", args, 1);
            Variant arg = new(args[0]);
            return arg.ImplicitReal ? (int)Math.Round(arg.AsReal) :
                throw new Exception("Can not use "+arg+" in round(real).");
        }

        /// This function gets the sine of the given real.
        static private object funcSin(List<object> args) {
            argCount("sin", args, 1);
            Variant arg = new(args[0]);
            return arg.ImplicitReal ? Math.Sin(arg.AsReal) :
                throw new Exception("Can not use "+arg+" in sin(real).");
        }

        /// This function gets the square root of the given real.
        static private object funcSqrt(List<object> args) {
            argCount("sqrt", args, 1);
            Variant arg = new(args[0]);
            return arg.ImplicitReal ? Math.Sqrt(arg.AsReal) :
                throw new Exception("Can not use "+arg+" in sqrt(real).");
        }

        /// This function casts the given value into a string value.
        static private object funcString(List<object> args) {
            argCount("string", args, 1);
            Variant arg = new(args[0]);
            return arg.AsStr;
        }

        /// This function gets a substring for a given string with a start and stop integer.
        static private object funcSub(List<object> args) {
            argCount("sub", args, 3);
            Variant arg0 = new(args[0]);
            Variant arg1 = new(args[1]);
            Variant arg2 = new(args[2]);
            if (arg0.ImplicitStr && arg1.ImplicitInt && arg2.ImplicitInt) {
                string str = arg0.AsStr;
                int start = arg1.AsInt, stop = arg2.AsInt;
                return start >= 0 && stop <= str.Length && start <= stop ? str[start..stop] :
                    throw new Exception("Invalid substring range: "+start+".."+stop);
            }
            throw new Exception("Can not use "+arg0+", "+arg1+", and "+arg2+" in sub(string, int, int).");
        }

        /// This function gets the sum of zero or more integers or reals.
        static private object funcSum(List<object> args) {
            bool allInt = true;
            foreach (object arg in args) {
                Variant value = new(arg);
                if (value.ImplicitInt) continue;
                allInt = false;
                if (value.ImplicitReal) continue;
                throw new Exception("Can not use "+arg+" in sum(real, real, ...) or sum(int, int, ...).");
            }

            if (allInt) {
                int value = 0;
                foreach (object arg in args) value += new Variant(arg).AsInt;
                return value;
            } else {
                double value = 0.0;
                foreach (object arg in args) value += new Variant(arg).AsReal;
                return value;
            }
        }

        /// This function gets the tangent of the given real.
        static private object funcTan(List<object> args) {
            argCount("tan", args, 1);
            Variant arg = new(args[0]);
            return arg.ImplicitReal ? Math.Tan(arg.AsReal) :
                throw new Exception("Can not use "+arg+" in tan(real).");
        }

        /// This function trims the left and right of a string.
        static private object funcTrim(List<object> args) {
            argCount("trim", args, 1);
            Variant arg = new(args[0]);
            return arg.ImplicitStr ? arg.AsStr.Trim() :
                throw new Exception("Can not use "+arg+" in trim(string).");
        }

        /// This function trims the left of a string.
        static private object funcTrimLeft(List<object> args) {
            argCount("trimLeft", args, 1);
            Variant arg = new(args[0]);
            return arg.ImplicitStr ? arg.AsStr.TrimStart() :
                throw new Exception("Can not use "+arg+" in trimLeft(string).");
        }

        /// This function trims the right of a string.
        static private object funcTrimRight(List<object> args) {
            argCount("trimRight", args, 1);
            Variant arg = new(args[0]);
            return arg.ImplicitStr ? arg.AsStr.TrimEnd() :
                throw new Exception("Can not use "+arg+" in trimRight(string).");
        }

        /// This function gets the upper case of the given string.
        static private object funcUpper(List<object> args) {
            argCount("upper", args, 1);
            Variant arg = new(args[0]);
            return arg.ImplicitStr ? arg.AsStr.ToUpper() :
                throw new Exception("Can not use "+arg+" in upper(string).");
        }

        #endregion
    }
}
