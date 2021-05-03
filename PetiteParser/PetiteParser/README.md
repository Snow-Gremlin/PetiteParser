# Petite Parser

- [Parsing Language](#parsing_language)
  - [Tokenizer Statements](#tokenizer_statements)
  - [Parser Statements](#parser_statements)

## Parsing Language

The language definition may have a comment on any line.
A comment starts with `#` and continues for the rest of the line.

```Plain
# Comment
```

Whitespace is ignored other than being used a separator.

There are four main parts to the definition.
There are `(States)`, `[Tokens]`, `<Terms>`, and `{Prompts}`.

- `(States)` are state nodes in the tokenizer.
- `[Tokens]` are the result of the tokenizer which are passed into the parser.
- `<Terms>` are parts of the grammar used for defining the rules of the language.
- `{Prompts}` can be added to the rules to invoke custom actions later.

The names for the state, token, term, or prompts may be any combination of numbers (0-9),
letter (a-z, A-Z), `_`, `.`, or `-`. Here are some states for example `(12)`, `(cat)`, `(animal.cat)`,
`(-1st-)`, `(__cat__)`, `(cat-12)`, and `(cat12)`.

All statements end with a semicolon, `;`.

### Tokenizer Statements

Tokenizers are build out of states and character sets to transition between states.
The states can be assigned tokens or be consumed.

```Plain
> (State);
(Start): ^'"' => (inString): !'"' => (inString): ^'"' => [String];
(Start): '+' => [Concatenate];
(Start): '=' => [Assignment];
(Start): 'a'..'z', 'A'..'Z', '_' => (Identifier): 'a'..'z', 'A'..'Z', '_', '0'..'9' => [Identifier];
(Start): ' \n\r\t' => ^[Whitespace];
```

See [Tokenizer.md](./Tokenizer.md) for more information.

### Parser Statements
