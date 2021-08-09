# Parser

Parser are built out of rules. This parser is a simple LR(1) parser.
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
  - [Rule Basics](#rule-basics)
- [Prompts](#prompts)

## Start Rule

To set the tule which the parser starts at use the following.
Only one start should be indicated. If more than one is indicated
then the last defined one will be used as the start.

```Plain
> <Start>;
```

## Rules

The rules defineone part of the grammar for the parsers's language.
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

### Rule Basics



## Prompts

