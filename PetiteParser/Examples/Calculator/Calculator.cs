using PetiteParser.Formatting;
using PetiteParser.Loader;
using PetiteParser.Parser;
using PetiteParser.ParseTree;
using PetiteParser.Scanner;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Examples.Calculator;

/// <summary>
/// An implementation of a simple calculator language.
///
/// This is useful for allowing a text field with higher mathematic control
/// without exposing exploits via a full language input.
///
/// This is also an example of how to use petite parser to construct
/// a simple interpreted language.
/// </summary>
sealed public class Calculator {
    private static readonly Parser parser;
    private const string resourceName = "Examples.Calculator.Calculator.lang";

    /// <summary>Loads the parser used by the calculator.</summary>
    static Calculator() =>
        parser = Loader.LoadParser(DefaultScanner.FromResource(Assembly.GetExecutingAssembly(), resourceName));

    /// <summary>
    /// This parses the given calculation input and
    /// returns the results so that the input can be run multiple
    /// times without having to re-parse the program.
    /// </summary>
    /// <param name="input">The input to parse.</param>
    /// <returns>The parse tree and error(s) from parsing the input.</returns>
    static public Result? Parse(string input) {
        if (string.IsNullOrEmpty(input)) return null;

        try {
            return parser.Parse(input);
        } catch (Exception err) {
            return Result.Error(err.Message);
        }
    }

    private readonly Dictionary<string, PromptHandle> handles;
    private readonly Stack<object> stack;
    private readonly Dictionary<string, object> consts;
    private readonly Dictionary<string, object> vars;
    private readonly CalcFuncs funcs;

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

    /// <summary>
    /// This uses the pre-parsed input to calculate the result.
    /// This is useful when wanting to rerun the same code multiple
    /// times without having to re-parse the program.
    /// </summary>
    /// <param name="tree">The parse tree to run.</param>
    public void Calculate(ITreeNode tree) {
        try {
            tree.Process(this.handles);
        } catch (Exception err) {
            this.stack.Clear();
            this.Push(new CalcException("Errors in calculator input:" + Environment.NewLine +
                "   " + err.Message));
        }
    }

    /// <summary>
    /// This parses the given calculation input and
    /// puts the result on the top of the stack.
    /// </summary>
    /// <param name="input">The calculator program to parse and run.</param>
    public void Calculate(string input) {
        Result? result = Parse(input);
        if (result is not null) {
            if (result.Errors.Length > 0) {
                this.stack.Clear();
                this.Push(new CalcException("Errors in calculator input:" + Environment.NewLine +
                    "  " + result.Errors.JoinLines("  ")));
                return;
            }
            if (result.Tree is not null)
                this.Calculate(result.Tree);
        }
    }

    /// <summary>Get a string showing all the values in the stack.</summary>
    /// <returns>The string showing the result stack.</returns>
    public string StackToString() =>
        this.stack.Count <= 0 ? "no result" :
        this.stack.Reverse().Select(Text.ValueToString).Join(", ");

    /// <summary>
    /// Adds a new function that can be called by the language.
    /// Set to null to remove a function.
    /// </summary>
    /// <param name="name">The name for the new function.</param>
    /// <param name="hndl">The function handler to add or null.</param>
    public void AddFunc(string name, CalcFunc hndl) =>
        this.funcs.AddFunc(name, hndl);

    /// <summary>
    /// Adds a new constant value into the language.
    /// Set to null to remove the constant.
    /// </summary>
    /// <param name="name">The name of the constant.</param>
    /// <param name="value">The value for the constant or null.</param>
    public void AddConstant(string name, object value) {
        if (value is null) this.consts.Remove(name);
        else this.consts[name] = value;
    }

    /// <summary>
    /// Sets the value of a variable.
    /// Set to null to remove the variable.
    /// </summary>
    /// <param name="name">The name of the variable to get.</param>
    /// <param name="value">The value of the variable.</param>
    public void SetVar(string name, object value) {
        if (value is null) this.vars.Remove(name);
        else this.vars[name] = value;
    }

    /// <summary>Gets the value of a variable with the given name.</summary>
    /// <param name="name">The name of the variable to get.</param>
    /// <returns>The value of the variable.</returns>
    public object GetVar(string name) =>
        this.vars[name];

    /// <summary>Indicates if the stack is empty or not.</summary>
    public bool StackEmpty => this.stack.Count <= 0;

    /// <summary>Indicates if the stack contains at least one error value in it.</summary>
    public bool StackContainsError => this.stack.Any((object value) => value is Exception);

    /// <summary>Clears all the values from the stack.</summary>
    public void Clear() => this.stack.Clear();

    /// <summary>Removes the top value from the stack.</summary>
    public object Pop() => this.stack.Pop();

    /// <summary>Pushes a value onto the stack.</summary>
    /// <param name="value">Pushes a new value onto the stack.</param>
    public void Push(object value) => this.stack.Push(value);

    #region Parser Prompts...

    /// <summary>Handles calculating the sum of the top two items off of the stack.</summary>
    /// <param name="args">The prompt arguments with the tokens from the parse.</param>
    private void handleAdd(PromptArgs args) {
        Variant right = new(this.Pop());
        Variant left  = new(this.Pop());
        if      (left.ImplicitInt  && right.ImplicitInt)  this.Push(left.AsInt  + right.AsInt);
        else if (left.ImplicitReal && right.ImplicitReal) this.Push(left.AsReal + right.AsReal);
        else if (left.ImplicitStr  && right.ImplicitStr)  this.Push(left.AsStr  + right.AsStr);
        else throw new CalcException("Cannot Add "+left+" to "+right+".");
    }

    /// <summary>Handles ANDing the top two items off the stack.</summary>
    /// <param name="args">The prompt arguments with the tokens from the parse.</param>
    private void handleAnd(PromptArgs args) {
        Variant right = new(this.Pop());
        Variant left  = new(this.Pop());
        if      (left.ImplicitBool && right.ImplicitBool) this.Push(left.AsBool && right.AsBool);
        else if (left.ImplicitInt  && right.ImplicitInt)  this.Push(left.AsInt  &  right.AsInt);
        else throw new CalcException("Cannot And "+left+" with "+right+".");
    }

    /// <summary>Handles assigning an variable to the top value off of the stack.</summary>
    /// <param name="args">The prompt arguments with the tokens from the parse.</param>
    private void handleAssign(PromptArgs args) {
        object right = this.Pop();
        Variant left = new(this.Pop());
        if (!left.IsStr) throw new CalcException("Cannot Assign "+right+" to "+left+".");
        string text = left.AsStr;
        if (this.consts.ContainsKey(text))
            throw new CalcException("Cannot Assign "+right+" to the constant "+left+".");
        this.vars[text] = right;
    }

    /// <summary>Handles adding a binary integer value from the input tokens.</summary>
    /// <param name="args">The prompt arguments with the tokens from the parse.</param>
    private void handleBinary(PromptArgs args) {
        string text = args.LastText;
        args.Tokens.Clear();
        text = text[..^1]; // remove 'b'
        this.Push(Convert.ToInt32(text, 2));
    }

    /// <summary>Handles calling a function, taking it's parameters off the stack.</summary>
    /// <param name="args">The prompt arguments with the tokens from the parse.</param>
    private void handleCall(PromptArgs args) {
        List<object> methodArgs = new();
        object val = this.Pop();
        while (val is not CalcFunc) {
            methodArgs.Insert(0, val);
            val = this.Pop();
        }
        if (val is CalcFunc func)
            this.Push(func(methodArgs));
    }

    /// <summary>Handles adding a decimal integer value from the input tokens.</summary>
    /// <param name="args">The prompt arguments with the tokens from the parse.</param>
    private void handleDecimal(PromptArgs args) {
        string text = args.LastText;
        args.Tokens.Clear();
        if (text.EndsWith('d')) text = text[..^1];
        this.Push(int.Parse(text, CultureInfo.InvariantCulture));
    }

    /// <summary>Handles dividing the top two items on the stack.</summary>
    /// <param name="args">The prompt arguments with the tokens from the parse.</param>
    private void handleDivide(PromptArgs args) {
        Variant right = new(this.Pop());
        Variant left  = new(this.Pop());
        if      (left.ImplicitInt  && right.ImplicitInt)  this.Push(left.AsInt  / right.AsInt);
        else if (left.ImplicitReal && right.ImplicitReal) this.Push(left.AsReal / right.AsReal);
        else throw new CalcException("Cannot Divide "+left+" with "+right+".");
    }

    /// <summary>Handles checking if the two top items on the stack are equal.</summary>
    /// <param name="args">The prompt arguments with the tokens from the parse.</param>
    private void handleEqual(PromptArgs args) {
        Variant right = new(this.Pop());
        Variant left  = new(this.Pop());
        if      (left.ImplicitBool && right.ImplicitBool) this.Push(left.AsBool == right.AsBool);
        else if (left.ImplicitInt  && right.ImplicitInt)  this.Push(left.AsInt  == right.AsInt);
        else if (left.ImplicitReal && right.ImplicitReal) this.Push(left.AsReal == right.AsReal);
        else if (left.ImplicitStr  && right.ImplicitStr)  this.Push(left.AsStr  == right.AsStr);
        else this.Push(false);
    }

    /// <summary>Handles checking if the two top items on the stack are greater than or equal.</summary>
    /// <param name="args">The prompt arguments with the tokens from the parse.</param>
    private void handleGreaterEqual(PromptArgs args) {
        Variant right = new(this.Pop());
        Variant left  = new(this.Pop());
        if      (left.ImplicitInt  && right.ImplicitInt)  this.Push(left.AsInt  >= right.AsInt);
        else if (left.ImplicitReal && right.ImplicitReal) this.Push(left.AsReal >= right.AsReal);
        else throw new CalcException("Cannot Greater Than or Equals "+left+" and "+right+".");
    }

    /// <summary>Handles checking if the two top items on the stack are greater than.</summary>
    /// <param name="args">The prompt arguments with the tokens from the parse.</param>
    private void handleGreaterThan(PromptArgs args) {
        Variant right = new(this.Pop());
        Variant left  = new(this.Pop());
        if      (left.ImplicitInt  && right.ImplicitInt)  this.Push(left.AsInt  > right.AsInt);
        else if (left.ImplicitReal && right.ImplicitReal) this.Push(left.AsReal > right.AsReal);
        else throw new CalcException("Cannot Greater Than "+left+" and "+right+".");
    }

    /// <summary>Handles looking up a constant or variable value.</summary>
    /// <param name="args">The prompt arguments with the tokens from the parse.</param>
    private void handleId(PromptArgs args) {
        string text = args.LastText;
        args.Tokens.Clear();
        if (this.consts.TryGetValue(text, out object? constValue)) {
            this.stack.Push(constValue);
            return;
        }
        if (this.vars.TryGetValue(text, out object? varValue)) {
            this.stack.Push(varValue);
            return;
        }
        throw new CalcException("No constant called "+text+" found.");
    }

    /// <summary>Handles inverting the top value on the stack.</summary>
    /// <param name="args">The prompt arguments with the tokens from the parse.</param>
    private void handleInvert(PromptArgs args) {
        Variant top = new(this.Pop());
        if (top.IsInt) this.Push(~top.AsInt);
        else throw new CalcException("Cannot Invert "+top+".");
    }

    /// <summary>Handles checking if the two top items on the stack are less than or equal.</summary>
    /// <param name="args">The prompt arguments with the tokens from the parse.</param>
    private void handleLessEqual(PromptArgs args) {
        Variant right = new(this.Pop());
        Variant left  = new(this.Pop());
        if      (left.ImplicitInt  && right.ImplicitInt)  this.Push(left.AsInt  <= right.AsInt);
        else if (left.ImplicitReal && right.ImplicitReal) this.Push(left.AsReal <= right.AsReal);
        else throw new CalcException("Cannot Less Than or Equals "+left+" and "+right+".");
    }

    /// <summary>Handles checking if the two top items on the stack are less than.</summary>
    /// <param name="args">The prompt arguments with the tokens from the parse.</param>
    private void handleLessThan(PromptArgs args) {
        Variant right = new(this.Pop());
        Variant left  = new(this.Pop());
        if     (left.ImplicitInt   && right.ImplicitInt)  this.Push(left.AsInt  < right.AsInt);
        else if (left.ImplicitReal && right.ImplicitReal) this.Push(left.AsReal < right.AsReal);
        else throw new CalcException("Cannot Less Than "+left+" and "+right+".");
    }

    /// <summary>Handles adding a hexadecimal integer value from the input tokens.</summary>
    /// <param name="args">The prompt arguments with the tokens from the parse.</param>
    private void handleHexadecimal(PromptArgs args) {
        string text = args.LastText;
        args.Tokens.Clear();
        text = text[2..]; // remove '0x'
        this.Push(Convert.ToInt32(text, 16));
    }

    /// <summary>Handles calculating the multiplies of the top two items off of the stack.</summary>
    /// <param name="args">The prompt arguments with the tokens from the parse.</param>
    private void handleMultiply(PromptArgs args) {
        Variant right = new(this.Pop());
        Variant left  = new(this.Pop());
        if      (left.ImplicitInt  && right.ImplicitInt)  this.Push(left.AsInt  * right.AsInt);
        else if (left.ImplicitReal && right.ImplicitReal) this.Push(left.AsReal * right.AsReal);
        else throw new CalcException("Cannot Multiply "+left+" to "+right+".");
    }

    /// <summary>Handles negating the an integer or real value.</summary>
    /// <param name="args">The prompt arguments with the tokens from the parse.</param>
    private void handleNegate(PromptArgs args) {
        Variant top = new(this.Pop());
        if      (top.IsInt)  this.Push(-top.AsInt);
        else if (top.IsReal) this.Push(-top.AsReal);
        else throw new CalcException("Cannot Negate "+top+".");
    }

    /// <summary>Handles NOTing the Boolean values at the top of the stack.</summary>
    /// <param name="args">The prompt arguments with the tokens from the parse.</param>
    private void handleNot(PromptArgs args) {
        Variant top = new(this.Pop());
        if (top.IsBool) this.Push(!top.AsBool);
        else throw new CalcException("Cannot Not "+top+".");
    }

    /// <summary>Handles checking if the two top items on the stack are not equal.</summary>
    /// <param name="args">The prompt arguments with the tokens from the parse.</param>
    private void handleNotEqual(PromptArgs args) {
        Variant right = new(this.Pop());
        Variant left  = new(this.Pop());
        if      (left.ImplicitBool && right.ImplicitBool) this.Push(left.AsBool != right.AsBool);
        else if (left.ImplicitInt  && right.ImplicitInt)  this.Push(left.AsInt  != right.AsInt);
        else if (left.ImplicitReal && right.ImplicitReal) this.Push(left.AsReal != right.AsReal);
        else if (left.ImplicitStr  && right.ImplicitStr)  this.Push(left.AsStr  != right.AsStr);
        else this.Push(true);
    }

    /// <summary>Handles adding a octal integer value from the input tokens.</summary>
    /// <param name="args">The prompt arguments with the tokens from the parse.</param>
    private void handleOctal(PromptArgs args) {
        string text = args.LastText;
        args.Tokens.Clear();
        text = text[..^1]; // remove 'o'
        this.Push(Convert.ToInt32(text, 8));
    }

    /// <summary>Handles ORing the Boolean values at the top of the stack.</summary>
    /// <param name="args">The prompt arguments with the tokens from the parse.</param>
    private void handleOr(PromptArgs args) {
        Variant right = new(this.Pop());
        Variant left  = new(this.Pop());
        if      (left.ImplicitBool && right.ImplicitBool) this.Push(left.AsBool || right.AsBool);
        else if (left.ImplicitInt  && right.ImplicitInt)  this.Push(left.AsInt  |  right.AsInt);
        else throw new CalcException("Cannot Or "+left+" to "+right+".");
    }

    /// <summary>Handles calculating the power of the top two values on the stack.</summary>
    /// <param name="args">The prompt arguments with the tokens from the parse.</param>
    private void handlePower(PromptArgs args) {
        Variant right = new(this.Pop());
        Variant left  = new(this.Pop());
        if      (left.ImplicitInt  && right.ImplicitInt)  this.Push((int)Math.Pow(left.AsInt, right.AsInt));
        else if (left.ImplicitReal && right.ImplicitReal) this.Push(Math.Pow(left.AsReal, right.AsReal));
        else throw new CalcException("Cannot Power "+left+" and "+right+".");
    }

    /// <summary>
    /// Handles push an ID value from the input tokens
    /// which will be used later as a variable name.
    /// </summary>
    /// <param name="args">The prompt arguments with the tokens from the parse.</param>
    private void handlePushVar(PromptArgs args) {
        string text = args.LastText;
        args.Tokens.Clear();
        this.Push(text);
    }

    /// <summary>Handles adding a real value from the input tokens.</summary>
    /// <param name="args">The prompt arguments with the tokens from the parse.</param>
    private void handleReal(PromptArgs args) {
        string text = args.LastText;
        args.Tokens.Clear();
        this.Push(double.Parse(text, CultureInfo.InvariantCulture));
    }

    /// <summary>Handles starting a function call.</summary>
    /// <param name="args">The prompt arguments with the tokens from the parse.</param>
    private void handleStartCall(PromptArgs args) {
        string text = args.LastText.ToLower(CultureInfo.InvariantCulture);
        args.Tokens.Clear();
        CalcFunc? func = this.funcs.FindFunc(text);
        if (func is null) throw new CalcException("No function called "+text+" found.");
        this.Push(func);
    }

    /// <summary>Handles adding a string value from the input tokens.</summary>
    /// <param name="args">The prompt arguments with the tokens from the parse.</param>
    private void handleString(PromptArgs args) {
        string text = args.LastText;
        args.Tokens.Clear();
        this.Push(Text.Unescape(text));
    }

    /// <summary>Handles calculating the difference of the top two items off of the stack.</summary>
    /// <param name="args">The prompt arguments with the tokens from the parse.</param>
    private void handleSubtract(PromptArgs args) {
        Variant right = new(this.Pop());
        Variant left  = new(this.Pop());
        if      (left.ImplicitInt  && right.ImplicitInt)  this.Push(left.AsInt  - right.AsInt);
        else if (left.ImplicitReal && right.ImplicitReal) this.Push(left.AsReal - right.AsReal);
        else throw new CalcException("Cannot Subtract "+left+" to "+right+".");
    }

    /// <summary>Handles XORing the Boolean values at the top of the stack.</summary>
    /// <param name="args">The prompt arguments with the tokens from the parse.</param>
    private void handleXor(PromptArgs args) {
        Variant right = new(this.Pop());
        Variant left  = new(this.Pop());
        if      (left.ImplicitBool && right.ImplicitBool) this.Push(left.AsBool ^ right.AsBool);
        else if (left.ImplicitInt  && right.ImplicitInt)  this.Push(left.AsInt  ^ right.AsInt);
        else throw new CalcException("Cannot Multiply "+left+" to "+right+".");
    }

    #endregion
}
