# Petite Parser

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

The names for the state, token, term, or prompts may be any combination of numbers,
letter, `_`, `.`, or `-`. Here are some states for example `(12)`, `(cat)`, `(animal.cat)`,
`(-1st-)`, `(__cat__)`, `(cat-12)`, and `(cat12)`.

All statements end with a semicolon, `;`.

### Tokenizer Statements

To set the state which the tokenizer starts at use the following.

```Plain
> (Start);
```

```Plain
(State-1): '0' => (State-2);
```

```Plain
(State-1): '0'..'9' => (State-3);
```

```Plain
(State-1): 'abcd' => (State-4);
```

```Plain
(State-1): ^'a' => (State-5)
```

```Plain
(State-1): * => (State-5)
```

```Plain
(State-1): 'a' => (State-6): 'b' => (State-7);
```




### Parser Statements
