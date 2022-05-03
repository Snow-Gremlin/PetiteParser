# Petite Parser

- [Parsing Language](#parsing_language)
  - [Tokenizer Statements](#tokenizer_statements)
  - [Parser Statements](#parser_statements)

For help debugging problems with a language file, see [Tips](./Tips.md).

## Parsing Language

The language definition may have a comment on any line.
A comment starts with `#` and continues for the rest of the line.

```Plain
# Comment
```

Whitespace is ignored other than being used as a separator.

There are four main parts to the definition.
There are `(States)`, `[Tokens]`, `<Terms>`, and `{Prompts}`.

- `(States)` are state nodes in the tokenizer.
- `[Tokens]` are the result of the tokenizer which are passed into the parser.
- `<Terms>` are parts of the grammar used for defining the rules of the language.
- `{Prompts}` can be added to the rules to invoke custom actions later.

The names for the state, token, term, or prompts may be any combination of numbers (0-9),
letter (a-z, A-Z), `_`, `.`, or `-`. Here are some states for example `(12)`, `(cat)`, `(animal.cat)`,
`(-1st-)`, `(__cat__)`, `(cat-12)`, and `(cat12)`.
Whitespace and comments may also inserted around the name, for example `( cat )`.

All statements end with a semicolon, `;`.

### Tokenizer Statements

Tokenizers are state machines build out of states and character sets to transition between states.
The states can be assigned tokens or can be consumed.

See [Tokenizer.md](./Tokenizer.md) for more information.

#### Example Tokenizer

```Plain
> (Start);
(Start): ^'"' => (inString): !'"' => (inString): ^'"' => [String];
(Start): '+' => [Concatenate];
(Start): '=' => [Assignment];
(Start): 'a'..'z', 'A'..'Z', '_' => (Identifier): 'a'..'z', 'A'..'Z', '_', '0'..'9' => [Identifier];
(Start): ' \n\r\t' => ^[Whitespace];
```

### Parser Statements

PetiteParser uses a CLR(1) parser meaning that it can handle a large variety of language grammars
and a single token look ahead. The parser not only defines the language rules but
can also be annotated with prompts to method calls for processing the a parsed result.

See [Parser.md](./Parser.md) for more information.

#### Example Parser

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
