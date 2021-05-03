# Tokenizer

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

- [Start State](#start_state)
- [Character Transition](#character_transition)
  - [Transition Range](#transition_range)
  - [Transition Set](#transition_set)
  - [Transition Not Characters](#transition_not_characters)
  - [Transition On Any Character](#transition_on_any_character)
  - [Transition Combining](#transition_combining)
  - [Special Transition Characters](#special_transition_characters)
  - [Consuming Characters](#consuming_characters)
- [Chaining States](#chaining_states)

## Start State

To set the state which the tokenizer starts at use the following.
Only one start should be indicated. If more than one is indicated
then the last defined one will be used as the start.

```Plain
> (Start);
```

## Character Transition

There are several ways to set a tokenizer transition between two states .
The transition occurs when a specified character is matched.
If there are duplicate state and character combinations the first defined transition will be taken.

This is a state transition on a single character.

```Plain
(State-1): '0' => (State-2);
(State-1): '1' => (State-2);
```

### Transition Range

This is a state transition on a range.
All characters between and including the start and stop characters (by unicode value).

```Plain
(State-1): '0'..'9' => (State-3);
```

### Transition Set

The transition can also be defined by a set of characters.
All the individual characters in the set will transition
from the start to the end state. Repeats are ignored.

```Plain
(State-1): 'abcd' => (State-4);
```

### Transition Not Characters

All or part of a transaction can be negated.
The NOT will match everything except for the values in the set and ranges.

```Plain
(State-1): !'abcd' => (State-4);
(State-1): !'a'..'d' => (State-4);
```

### Transition On Any Character

A transition can also match any character.
Since the characters are matched in the order they are
defined, this can be used like an "else" or "otherwise".

```Plain
(State-1): * => (State-5);
```

### Transition Combining

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

### Special Transition Characters

The transitions can use either single quotes or double quotes.

```Plain
(State-1): "abcd" => (State-4);
(State-1): "'" => (State-4);
(State-1): '"' => (State-4);
```

The characters can be defined as unicode characters (no modifiers).
The characters can be escaped to specify specific characters.
See the below table for the list of single character escapes.

| Short Notation | UTF-16 | character Description |
|:--------------:|:------:|:----------------------|
| \\'            | \\x27  | allow to enter a '    |
| \\"            | \\x22  | allow to enter a "    |
| \\\\           | \\x5c  | allow to enter a \    |
| \\b            | \\x08  | back-space            |
| \\f            | \\x0c  | form-feed (new page)  |
| \\n            | \\x0a  | line-feed (new line)  |
| \\r            | \\x0d  | carriage-return       |
| \\t            | \\x09  | tab (horizontal-tab)  |
| \\v            | \\x0b  | vertical-tab          |

Apostrophes do not need to be escaped when inside quotes and
quotation marks do not need to be escaped when inside single quotes.

Additionally, the character can be an ASCII byte in hexadecimal (\\xFF) or a 16 bit unicode (\\uFFFF).

```Plain
(State-1): '\'' => (State-2);
(State-1): "\"" => (State-2);
(State-1): '\n\r' => (State-2);
(State-1): '\x88' => (State-2);
(State-1): '\u2042' => (State-2);
```

### Consuming Characters

// TODO: Talk about consuming a character

```Plain
(State-1): ^'a' => (State-5)
```

## Chaining States


// TODO: Talk about chaining states


```Plain
(State-1): 'a' => (State-6): 'b' => (State-7);
```

// TODO: Talk about setting the token
// TODO: Talk about consuming the token
