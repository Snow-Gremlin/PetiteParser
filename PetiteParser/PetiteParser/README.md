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

#### Start State

To set the state which the tokenizer starts at use the following.
Only one start should be indicated. If more than one is indicated
then the last defined one will be used as the start.

```Plain
> (Start);
```

#### Character Transition

There are several ways to set a tokenizer transition between two states .
The transition occurs when a specified character is matched.
If there are duplicate state and character combinations the first defined transition will be taken.

This is a state transition on a single character.

```Plain
(State-1): '0' => (State-2);
(State-1): '1' => (State-2);
```

##### Transition Range

This is a state transition on a range.
All characters between and including the start and stop characters (by unicode value).

```Plain
(State-1): '0'..'9' => (State-3);
```

##### Transition Set

The transition can also be defined by a set of characters.
All the individual characters in the set will transition
from the start to the end state. Repeats are ignored.

```Plain
(State-1): 'abcd' => (State-4);
```

##### Transition Not Characters

All or part of a transaction can be negated.
The NOT will match everything except for the values in the set and ranges.

```Plain
(State-1): !'abcd' => (State-4);
(State-1): !'a'..'d' => (State-4);
```

##### Transition On Any Character

A transition can also match any character.
Since the characters are matched in the order they are
defined, this can be used like an "else" or "otherwise".

```Plain
(State-1): * => (State-5);
```

##### Transition Combining

Several transitions between the same two states can be OR'ed together
by listing them and comma separating them.

```Plain
(State-1): 'a'..'z', 'A'..'Z' => (State-5);
```

Negations only apply to the set or range it is next to.
The following will match anything that is not a lowercase letter and it matches lower case 'g'.

```Plain
(State-1): !'a'..'z', 'g' => (State-5);
```

Negations can be combined negations with parenthesis.

```Plain
(State-1): !('a'..'z', 'A'..'Z', '_') => (State-5);
```

##### Special Transition Characters

```Plain
(State-1): "abcd" => (State-4);
(State-1): "'" => (State-4);
(State-1): '"' => (State-4);
```

The characters can defined as unicode characters (no modifiers).
The characters can be escaped to specify specific characters.
Line returns (\\r), new lines (\\n), tabs (\\t), backslashes (\\\\), quotations marks (\\"),
and apostrophes (\\') can be added with single character escapes.
Apostrophes do not need to be escaped when inside quotes and
quotation marks do not need to be escaped when inside single quotes.
The character can be an ASCII byte in hexadecimal (\\xFF) or a 16 bit unicode (\\uFFFF).

```Plain
(State-1): '\'' => (State-2);
(State-1): "\"" => (State-2);
(State-1): '\n\r' => (State-2);
(State-1): '\x88' => (State-2);
(State-1): '\u2042' => (State-2);
```

##### Consuming Characters

// TODO: Talk about consuming a character

```Plain
(State-1): ^'a' => (State-5)
```

#### Chaining States


// TODO: Talk about chaining states


```Plain
(State-1): 'a' => (State-6): 'b' => (State-7);
```

// TODO: Talk about setting the token
// TODO: Talk about consuming the token


### Parser Statements
