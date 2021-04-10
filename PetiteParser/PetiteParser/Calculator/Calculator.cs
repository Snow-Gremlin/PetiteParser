using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PetiteParser.Parser;
using PetiteParser.ParseTree;

namespace PetiteParser.Calculator {

    /// <summary>
    /// An implementation of a simple calculator language.
    ///
    /// This is useful for allowing a text field with higher mathematic control
    /// without exposing exploits via a full language input.
    ///
    /// This is also an example of how to use petite parser to construct
    /// a simple interpreted language.
    /// </summary>
    public class Calculator {
        static private Parser.Parser parser;

        /// <summary>Loads the parser used by the calculator.</summary>
        /// <remarks>This will be loaded on first parse or can be called earlier.</remarks>
        static public void LoadParser() {
            if (parser == null)
                parser = new Parser.Parser(Encoding.UTF8.GetString(Resource.Calculator));
        }

        private Dictionary<string, PromptHandle> handles;
        private Stack<object> stack;
        private Dictionary<string, object> consts;
        private Dictionary<string, object> vars;
        private CalcFuncs funcs;

        /// <summary>Creates a new calculator instance.</summary>
        /// <param name="randomSeed">The random seed to use for the calculators random function.</param>
        public Calculator(int randomSeed = 0) {
            this.handles = new Dictionary<string, PromptHandle>() {
                { "Add",          this.handleAdd },
                { "And",          this.handleAnd },
                { "Assign",       this.handleAssign },
                { "Binary",       this.handleBinary },
                { "Call",         this.handleCall },
                { "Decimal",      this.handleDecimal },
                { "Divide",       this.handleDivide },
                { "Equal",        this.handleEqual },
                { "GreaterEqual", this.handleGreaterEqual },
                { "GreaterThan",  this.handleGreaterThan },
                { "Hexadecimal",  this.handleHexadecimal },
                { "Id",           this.handleId },
                { "Invert",       this.handleInvert },
                { "LessEqual",    this.handleLessEqual },
                { "LessThan",     this.handleLessThan },
                { "Multiply",     this.handleMultiply },
                { "Negate",       this.handleNegate },
                { "Not",          this.handleNot },
                { "NotEqual",     this.handleNotEqual },
                { "Octal",        this.handleOctal },
                { "Or",           this.handleOr },
                { "Power",        this.handlePower },
                { "PushVar",      this.handlePushVar },
                { "Real",         this.handleReal },
                { "StartCall",    this.handleStartCall },
                { "String",       this.handleString },
                { "Subtract",     this.handleSubtract },
                { "Xor",          this.handleXor },
            };
            this.stack = new Stack<object>();
            this.consts = new Dictionary<string, object>() {
                { "pi",    Math.PI },
                { "tau",   Math.Tau },
                { "e",     Math.E },
                { "true",  true },
                { "false", false },
            };
            this.vars = new Dictionary<string, object>();
            this.funcs = new CalcFuncs(randomSeed);
        }


        /// This parses the given calculation input and
        /// returns the results so that the input can be run multiple
        /// times without having to re-parse the program.
        static public Result Parse(string input) {
            if (string.IsNullOrEmpty(input)) return null;
            if (parser is null) LoadParser();

            try {
                return parser.Parse(input);
            } catch (Exception err) {
                return new Result(null,
                  "Errors in calculator input:"+Environment.NewLine+err.Message);
            }
        }

        /// This uses the pre-parsed input to calculate the result.
        /// This is useful when wanting to rerun the same code multiple
        /// times without having to re-parse the program.
        public void CalculateNode(ITreeNode tree) {
            try {
                tree.Process(this.handles);
            } catch (Exception err) {
                this.stack.Clear();
                this.Push("Errors in calculator input:"+Environment.NewLine+err.Message);
            }
        }

        /// This parses the given calculation input and
        /// puts the result on the top of the stack.
        void calculate(string input) {
            Result result = Parse(input);
            if (result != null) {
                if (result.Errors.Length > 0) {
                    this.stack.Clear();
                    this.Push("Errors in calculator input:"+Environment.NewLine+
                        "  "+string.Join(Environment.NewLine+"  ", result.Errors));
                    return;
                }
                this.CalculateNode(result.Tree);
            }
        }

        /// Get a string showing all the values in the stack.
        public string StackToString() {
            if (this.stack.Count <= 0) return "no result";
            List<string> parts = new();
            foreach (object val in this.stack) parts.Add(val.ToString());
            return string.Join(", ", parts);
        }

        /// Adds a new function that can be called by the language.
        /// Set to null to remove a function.
        public void AddFunc(string name, CalcFunc hndl) =>
            this.funcs.AddFunc(name, hndl);

        /// Adds a new constant value into the language.
        /// Set to null to remove the constant.
        public void AddConstant(string name, object value) {
            if (value is null) this.consts.Remove(name);
            else this.consts[name] = value;
        }

        /// Sets the value of a variable.
        /// Set to null to remove the variable.
        public void SetVar(string name, object value) {
            if (value is null) this.vars.Remove(name);
            else this.vars[name] = value;
        }

        /// Indicates if the stack is empty or not.
        public bool StackEmpty => this.stack.Count <= 0;

        /// Clears all the values from the stack.
        public void Clear() => this.stack.Clear();

        /// Removes the top value from the stack.
        public object Pop() => this.stack.Pop();

        /// Pushes a value onto the stack.
        public void Push(object value) => this.stack.Push(value);

        #region Parser Prompts...

        /// Handles calculating the sum of the top two items off of the stack.
        private void handleAdd(PromptArgs args) {
            Variant right = new(this.Pop());
            Variant left  = new(this.Pop());
            if (left.ImplicitInt  && right.ImplicitInt) this.Push(left.AsInt + right.AsInt);
            else if (left.ImplicitReal && right.ImplicitReal) this.Push(left.AsReal + right.AsReal);
            else if (left.ImplicitStr  && right.ImplicitStr) this.Push(left.AsStr + right.AsStr);
            else throw new Exception("Can not Add "+left+" to "+right+".");
        }

        /// Handles ANDing the top two items off the stack.
        private void handleAnd(PromptArgs args) {
            Variant right = new(this.Pop());
            Variant left  = new(this.Pop());
            if (left.ImplicitBool && right.ImplicitBool) this.Push(left.AsBool && right.AsBool);
            else if (left.ImplicitInt  && right.ImplicitInt) this.Push(left.AsInt & right.AsInt);
            else throw new Exception("Can not And "+left+" with "+right+".");
        }

        /// Handles assigning an variable to the top value off of the stack.
        private void handleAssign(PromptArgs args) {
            object right = this.Pop();
            Variant left = new(this.Pop());
            if (!left.IsStr) throw new Exception("Can not Assign "+right+" to "+left+".");
            string text = left.AsStr;
            if (this.consts.ContainsKey(text))
                throw new Exception("Can not Assign "+right+" to the constant "+left+".");
            this.vars[text] = right;
        }

        /// Handles adding a binary integer value from the input tokens.
        private void handleBinary(PromptArgs args) {
            string text = args.Recent(1).Text;
            args.Tokens.Clear();
            text = text[..^1]; // remove 'b'
            this.Push(Convert.ToInt32(text, 2));
        }

        /// Handles calling a function, taking it's parameters off the stack.
        private void handleCall(PromptArgs args) {
            List<object> methodArgs = new();
            object val = this.Pop();
            while (!(val is CalcFunc)) {
                methodArgs.Insert(0, val);
                val = this.Pop();
            }
            this.Push((val as CalcFunc)(methodArgs));
        }

        /// Handles adding a decimal integer value from the input tokens.
        private void handleDecimal(PromptArgs args) {
            String text = args.Recent(1).Text;
            args.Tokens.Clear();
            if (text.EndsWith('d')) text = text[..^1];
            this.Push(int.Parse(text));
        }

        /// Handles dividing the top two items on the stack.
        private void handleDivide(PromptArgs args) {
            Variant right = new(this.Pop());
            Variant left  = new(this.Pop());
            if (left.ImplicitInt && right.ImplicitInt) this.Push(left.AsInt / right.AsInt);
            else if (left.ImplicitReal && right.ImplicitReal) this.Push(left.AsReal / right.AsReal);
            else throw new Exception("Can not Divide "+left+" with "+right+".");
        }

        /// Handles checking if the two top items on the stack are equal.
        private void handleEqual(PromptArgs args) {
            Variant right = new(this.Pop());
            Variant left  = new(this.Pop());
            if (left.ImplicitBool && right.ImplicitBool) this.Push(left.AsBool == right.AsBool);
            else if (left.ImplicitInt && right.ImplicitInt) this.Push(left.AsInt == right.AsInt);
            else if (left.ImplicitReal && right.ImplicitReal) this.Push(left.AsReal == right.AsReal);
            else if (left.ImplicitStr && right.ImplicitStr) this.Push(left.AsStr == right.AsStr);
            else this.Push(false);
        }

        /// Handles checking if the two top items on the stack are greater than or equal.
        private void handleGreaterEqual(PromptArgs args) {
            Variant right = new(this.Pop());
            Variant left  = new(this.Pop());
            if (left.ImplicitInt && right.ImplicitInt) this.Push(left.AsInt >= right.AsInt);
            else if (left.ImplicitReal && right.ImplicitReal) this.Push(left.AsReal >= right.AsReal);
            else throw new Exception("Can not Greater Than or Equals "+left+" and "+right+".");
        }

        /// Handles checking if the two top items on the stack are greater than.
        private void handleGreaterThan(PromptArgs args) {
            Variant right = new(this.Pop());
            Variant left  = new(this.Pop());
            if (left.ImplicitInt && right.ImplicitInt) this.Push(left.AsInt > right.AsInt);
            else if (left.ImplicitReal && right.ImplicitReal) this.Push(left.AsReal > right.AsReal);
            else throw new Exception("Can not Greater Than "+left+" and "+right+".");
        }

        /// Handles looking up a constant or variable value.
        private void handleId(PromptArgs args) {
            string text = args.Recent(1).Text;
            args.Tokens.Clear();
            if (this.consts.ContainsKey(text)) {
                this.stack.Push(this.consts[text]);
                return;
            }
            if (this.vars.ContainsKey(text)) {
                this.stack.Push(this.vars[text]);
                return;
            }
            throw new Exception("No constant called "+text+" found.");
        }

        /// Handles inverting the top value on the stack.
        private void handleInvert(PromptArgs args) {
            Variant top = new(this.Pop());
            if (top.IsInt) this.Push(~top.AsInt);
            else throw new Exception("Can not Invert "+top+".");
        }

        /// Handles checking if the two top items on the stack are less than or equal.
        private void handleLessEqual(PromptArgs args) {
            Variant right = new(this.Pop());
            Variant left  = new(this.Pop());
            if (left.ImplicitInt && right.ImplicitInt) this.Push(left.AsInt <= right.AsInt);
            else if (left.ImplicitReal && right.ImplicitReal) this.Push(left.AsReal <= right.AsReal);
            else throw new Exception("Can not Less Than or Equals "+left+" and "+right+".");
        }

        /// Handles checking if the two top items on the stack are less than.
        private void handleLessThan(PromptArgs args) {
            Variant right = new(this.Pop());
            Variant left  = new(this.Pop());
            if (left.ImplicitInt && right.ImplicitInt) this.Push(left.AsInt < right.AsInt);
            else if (left.ImplicitReal && right.ImplicitReal) this.Push(left.AsReal < right.AsReal);
            else throw new Exception("Can not Less Than "+left+" and "+right+".");
        }

        /// Handles adding a hexadecimal integer value from the input tokens.
        private void handleHexadecimal(PromptArgs args) {
            string text = args.Recent(1).Text;
            args.Tokens.Clear();
            text = text[2..]; // remove '0x'
            this.Push(Convert.ToInt32(text, 16));
        }

        //=============== Continue from here down

        /// Handles calculating the multiplies of the top two items off of the stack.
        private void handleMultiply(PromptArgs args) {
            Variant right = new Variant(this.Pop());
            Variant left  = new Variant(this.Pop());
            if (left.ImplicitInt  && right.ImplicitInt) this.Push(left.AsInt  * right.AsInt);
            else if (left.ImplicitReal && right.ImplicitReal) this.Push(left.AsReal * right.AsReal);
            else throw new Exception('Can not Multiply $left to $right.');
        }

        /// Handles negating the an integer or real value.
        private void handleNegate(PromptArgs args) {
            Variant top = new Variant(this.Pop());
            if (top.isInt) this.Push(-top.AsInt);
            else if (top.isReal) this.Push(-top.AsReal);
            else throw new Exception('Can not Negate $top.');
        }

        /// Handles NOTing the Boolean values at the top of the the stack.
        private void handleNot(PromptArgs args) {
            Variant top = new Variant(this.Pop());
            if (top.isBool) this.Push(!top.AsBool);
            else throw new Exception('Can not Not $top.');
        }

        /// Handles checking if the two top items on the stack are not equal.
        private void handleNotEqual(PromptArgs args) {
            Variant right = new Variant(this.Pop());
            Variant left  = new Variant(this.Pop());
            if (left.ImplicitBool && right.ImplicitBool) this.Push(left.AsBool != right.AsBool);
            else if (left.ImplicitInt  && right.ImplicitInt) this.Push(left.AsInt  != right.AsInt);
            else if (left.ImplicitReal && right.ImplicitReal) this.Push(left.AsReal != right.AsReal);
            else if (left.ImplicitStr  && right.ImplicitStr) this.Push(left.AsStr  != right.AsStr);
            else this.Push(true);
        }

        /// Handles adding a octal integer value from the input tokens.
        private void handleOctal(PromptArgs args) {
            String text = args.recent(1).text;
            args.tokens.clear();
            text = text.substring(0, text.length-1); // remove 'o'
            this.Push(int.parse(text, radix: 8));
        }

        /// Handles ORing the Boolean values at the top of the the stack.
        private void handleOr(PromptArgs args) {
            Variant right = new Variant(this.Pop());
            Variant left  = new Variant(this.Pop());
            if (left.ImplicitBool && right.ImplicitBool) this.Push(left.AsBool || right.AsBool);
            else if (left.ImplicitInt  && right.ImplicitInt) this.Push(left.AsInt  |  right.AsInt);
            else throw new Exception('Can not Or $left to $right.');
        }

        /// Handles calculating the power of the top two values on the stack.
        private void handlePower(PromptArgs args) {
            Variant right = new Variant(this.Pop());
            Variant left  = new Variant(this.Pop());
            if (left.ImplicitInt  && right.ImplicitInt) this.Push(math.pow(left.AsInt, right.AsInt).toInt());
            else if (left.ImplicitReal && right.ImplicitReal) this.Push(math.pow(left.AsReal, right.AsReal));
            else throw new Exception('Can not Power $left and $right.');
        }

        /// Handles push an ID value from the input tokens
        /// which will be used later as a variable name.
        private void handlePushVar(PromptArgs args) {
            String text = args.recent(1).text;
            args.tokens.clear();
            this.Push(text);
        }

        /// Handles adding a real value from the input tokens.
        private void handleReal(PromptArgs args) {
            String text = args.recent(1).text;
            args.tokens.clear();
            this.Push(double.parse(text));
        }

        /// Handles starting a function call.
        private void handleStartCall(PromptArgs args) {
            String text = args.recent(1).text.toLowerCase();
            args.tokens.clear();
            CalcFunc func = this._funcs.findFunc(text);
            if (func == null) throw new Exception('No function called $text found.');
            this.Push(func);
        }

        /// Handles adding a string value from the input tokens.
        private void handleString(PromptArgs args) {
            String text = args.recent(1).text;
            args.tokens.clear();
            this.Push(Parser.Loader.unescapeString(text));
        }

        /// Handles calculating the difference of the top two items off of the stack.
        private void handleSubtract(PromptArgs args) {
            Variant right = new Variant(this.Pop());
            Variant left  = new Variant(this.Pop());
            if (left.ImplicitInt  && right.ImplicitInt) this.Push(left.AsInt  - right.AsInt);
            else if (left.ImplicitReal && right.ImplicitReal) this.Push(left.AsReal - right.AsReal);
            else throw new Exception('Can not Subtract $left to $right.');
        }

        /// Handles XORing the Boolean values at the top of the the stack.
        private void handleXor(PromptArgs args) {
            Variant right = new Variant(this.Pop());
            Variant left  = new Variant(this.Pop());
            if (left.ImplicitBool && right.ImplicitBool) this.Push(left.AsBool ^ right.AsBool);
            else if (left.ImplicitInt && right.ImplicitInt) this.Push(left.AsInt ^ right.AsInt);
            else throw new Exception('Can not Multiply $left to $right.');
        }

        #endregion
    }
}
