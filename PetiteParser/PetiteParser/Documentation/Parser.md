# Parser




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

The tokenizer for that

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

