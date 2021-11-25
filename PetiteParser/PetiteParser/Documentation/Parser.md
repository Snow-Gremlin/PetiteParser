# Parser

Parser are built out of rules. This parser is a simple CLR(1) parser.
The parser finds the corrects rules to pick based on the given tokens.
The collection of rules which define the parser's language is called the grammar.

```Plain
> <Expression>;

<Expression> := <Term>
    | <Expression> [Pos] <Term> {Add}
    | <Expression> [Neg] <Term> {Subtract};

<Term> := <Factor>
    | <Term> [Mul] <Factor> {Multiply}
    | <Term> [Div] <Factor> {Divide};

<Factor> := <Value>
    | [Open] <Expression> [Close]
    | [Neg] <Factor> {Negate}
    | [Pos] <Factor>;

<Value> := [Id] {PushId}
    | [Int] {PushInt}
    | [Float] {PushFloat};
```

The tokenizer for that grammar.

```Plain
> (Start);
(Start): '+' => [Pos];
(Start): '-' => [Neg];
(Start): '*' => [Mul];
(Start): '/' => [Div];
(Start): '(' => [Open];
(Start): ')' => [Close];
(Start): '0'..'9' => (Int): '0'..'9' => [Int];
(Int): '.' => (Float-Start): '0'..'9' => (Float): '0'..'9' => [Float];
(Start): 'a'..'z', 'A'..'Z', '_' => (Id): 'a'..'z', 'A'..'Z', '_', '0'..'9' => [Id];
(Start): ' ' => (Space): ' ' => ^[Space];
```

- [Start Rule](#start_rule)
- [Rules](#rules)
  - [Rule Basics](#rule_basics)
- [Parse Tree](#parse_tree)
  - [Example 1](#example_1)
  - [Example 2](#example_2)
  - [Example 3](#example_3)
  - [Prompts](#prompts)

## Start Rule

To set the tule which the parser starts at use the following.
Only one start should be indicated. If more than one is indicated
then the last defined one will be used as the start.

```Plain
> <Start>;
```

## Rules

The rules define one part of the grammar for the parsers's language.
A Rule must start with the term, `<Term>`, to add the rule too
followed by `:=` and any number of terms, tokens, and prompts.

```
<Term> := <Factor>;
<Term> := <Term> [Mul] <Factor>;
<Term> := <Term> [Div] <Factor>;
```

Multiple rules for the same term can use `|` to short hand them together.

```
<Term> := <Factor>
    | <Term> [Mul] <Factor>
    | <Term> [Div] <Factor>;
```

Some rules should be empty with no terms or tokens, these are lambda rules.
To add a lambda rule use `_`.

```
<Term> := _;
<Factor> := _
    | [Open] <Expression> [Close];
```

### Rule Basics

Rules may create loops but must have at least one token in all the rules
of the loop that tells the parser which rule to take. For exmple, the following
won't work because it will create a loop with no token to choose the rules.

```
<First> := <Second>;
<Second> := <First>;
```

The following is okay and will not make a loop. It may look like you can take
the second rule, `<Term> [Mul] <Factor>`, to loop since it is a rule for `<Term>`.
However, the only reason the parser would choose that rule is if it new that
the token, `[Mul]`, will be found. That's the beauty of an RL(1) parser.

```
<Term> := <Factor>
    | <Term> [Mul] <Factor>
    | <Term> [Div] <Factor>;
```

It is strongly recommended that you read more about rule design from sites like
[Practical LR(k) Parser Construction](http://david.tribble.com/text/lrk_parsing.html) and
[Canonical LR parser Wiki](https://en.wikipedia.org/wiki/Canonical_LR_parser).

## Parse Tree

The result of a parse is a parse tree.
The parse tree is the rules taken to parse the input tokens.
The root of the tree is one rule from the starting term.
Any time there is a term in a rule, the term will be replaced with the chosen rule for that term.

### Example 1

For a simple example, using the example grammar at the top of the page,
parse a simple string, `5`. The resulting parse tree is as follows.

```
─<Expression>
  └─<Term>
     └─<Factor>
        └─<Value>
           ├─[Int:(Unnamed:1, 1, 1):"5"]
           └─{PushInt}
```

In the above tree you can see the grammar starts with `<Expression>`.
The first rule, `<Expression> := <Term>`, was chosen.
`<Term>` had it's first rule, `<Term> := <Factor>`, chosen.
Then the first rule, `<Factor> := <Value>`, for `<Factor>` was chosen.
Finally, the second rule, `<Value> := [Int] {PushInt}`, for `<Value>` was chosen.

The token, `[Int:(Unnamed:1, 1, 1):"5"]` indicates the value was tokenized as an `[Int]`,
it came from a file/source called "Unnamed" on the first line, first character of the line,
and first character from the start of the source. The value of the token was the string `"5"`.

`{PushInt}` is a prompt which will be discussed in the [Prompts subsection](#prompts).

### Example 2

For a more complicated example, using the example grammar at the top of the page,
parse a simple string, `5 + -2`. The resulting parse tree is as follows.

```
─<Expression>
  ├─<Expression>
  │  └─<Term>
  │     └─<Factor>
  │        └─<Value>
  │           ├─[Int:(Unnamed:1, 1, 1):"5"]
  │           └─{PushInt}
  ├─[Pos:(Unnamed:1, 3, 3):"+"]
  ├─<Term>
  │  └─<Factor>
  │     ├─[Neg:(Unnamed:1, 5, 5):"-"]
  │     ├─<Factor>
  │     │  └─<Value>
  │     │     ├─[Int:(Unnamed:1, 6, 6):"2"]
  │     │     └─{PushInt}
  │     └─{Negate}
  └─{Add}
```

This results can show that the first rule taken was `<Expression> := <Expression> [Pos] <Term> {Add}`.
To the left of the `[Pos]` expands out to "5". The right side expands out to
`<Factor> := [Neg] <Factor> {Negate}` and then out to "2". The branches of the resulting
tree indicate the order the to apply the rules on the token, i.e. the order of operations.
That means that the negative should be applied to the 2 prior to being added to 5, `(5 + (-2))`.
See the next example for more about the order of operations.

### Example 3

For another complicated example, using the example grammar at the top of the page,
parse a simple string, `5 * 2 + 3`. The resulting parse tree is as follows.

```
─<Expression>
  ├─<Expression>
  │  └─<Term>
  │     ├─<Term>
  │     │  └─<Factor>
  │     │     └─<Value>
  │     │        ├─[Int:(Unnamed:1, 1, 1):"5"]
  │     │        └─{PushInt}
  │     ├─[Mul:(Unnamed:1, 3, 3):"*"]
  │     ├─<Factor>
  │     │  └─<Value>
  │     │     ├─[Int:(Unnamed:1, 5, 5):"2"]
  │     │     └─{PushInt}
  │     └─{Multiply}
  ├─[Pos:(Unnamed:1, 7, 7):"+"]
  ├─<Term>
  │  └─<Factor>
  │     └─<Value>
  │        ├─[Int:(Unnamed:1, 9, 9):"3"]
  │        └─{PushInt}
  └─{Add}
```

Just like in [Example 2](#example_2) the rules of the grammar define the order of operations.
Left hand side of the `[Pos]` contains the `[Mul]`, this means that "5" and "2" should be multiplied
first before "3" is added to it, `((5 * 2) + 3)`.

### Prompts

A unique feature of PetiteParser is the ability to add propts into the grammar.
Prompts will not effect how rules are choosen by the parser at all.
The parser tree returned from a successful parse can be difficult to use when
compiling or interpreting the input. By placing prompts in the grammar the parser tree can
be used to call a set of methods for each prompt by name.

For example, given the parser tree from [Example 3](#example_3).
There are three prompts: `{PushInt}`, `{Multiply}`, and `{Add}`.
The complier or interpretor should prepare handlers for all the prompts
and then use those handlers to process the parse tree.
The following code is an example of how those handlers could be setup.

```CSharp
using System;
using System.Collections.Generic;

class Compiler {
    private Dictionary<string, PromptHandle> handles;
    private Parser parser;

    public Compiler() {
        this.parser = Loader.LoadParser(languageDefinition);
        this.handles = new Dictionary<string, PromptHandle>() {
            { "Add",      this.handleAdd },
            { "Multiply", this.handleMultiply },
            { "PushInt",  this.handlePushInt },
        };
    }
        
    public void Compile(String input) {
        Result result = this.parser.Parse(input);
        if (result is not null) {
            if (result.Errors.Length > 0)
                Console.WriteLine(string.Join(Environment.NewLine, result.Errors));
            else result.Tree.Process(this.handles);
        }
    }
    
    private void handleAdd(PromptArgs args) =>
        Console.WriteLine("ADD");
    
    private void handleMultiply(PromptArgs args) =>
        Console.WriteLine("MUL");
    
    private void handlePushInt(PromptArgs args) =>
        Console.WriteLine("PUSH "+ args.LastText);
}
```

When [Example 3](#example_3)'s parse tree is processed by this code the output would be.

```
PUSH 5
PUSH 2
MUL
PUSH 3
ADD
```

It can be seen that those handlers quickly created what is similar to the compiled assembly.
This is because the parse tree is walked depth first and call all the prompts when one is reached.
The `PromptArgs` contains the tokens which have been passed while processing the tree.
For the first "{PushInt}" the only token in the arguments is `[Int:(Unnamed:1, 1, 1):"5"]`.
When "{Mul}" is reached the arguemts contain`[Int:(Unnamed:1, 1, 1):"5"]`, `[Mul:(Unnamed:1, 3, 3):"*"]`,
and `[Int:(Unnamed:1, 5, 5):"2"]`. By placing the prompts in specific spots of the grammar
the parse tree can be easily turned into useful instructions for running the parsed code.

For a more detailed example see the [Calculator Example's code](../../Examples/Calculator).
