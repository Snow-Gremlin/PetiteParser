# Tokenizer

Tokenizers are build out of states and character sets to transition between states.
The states can be assigned tokens or be consumed.

```Plain
> (Start);
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
- [Assigning Tokens](#assigning_tokens)
  - [Consuming Tokens](#consuming_tokens)
- [Examples](#examples)

## Start State

To set the state which the tokenizer starts at use the following.
Only one start should be indicated. If more than one is indicated
then the last defined one will be used as the start.

```Plain
> (Start);
```

## Character Transition

There are several ways to set a tokenizer transition between two states.
The states may be the same to create a loop for repeat transitions.
The transition occurs when a specified character is matched.
If there are duplicate state and character combinations the first defined transition will be taken.

This is a state transition on a single character.

```Plain
(State): '0' => (Next);
(State): '1' => (Next);
(Next):  '1' => (Next);
```

### Transition Range

This is a state transition on a range.
All characters between and including the start and stop characters (by unicode value).

```Plain
(State): '0'..'9' => (Next);
```

### Transition Set

The transition can also be defined by a set of characters.
All the individual characters in the set will transition
from the start to the end state. Repeats are ignored.

```Plain
(State): 'abcd' => (Next);
```

### Transition Not Characters

All or part of a transaction can be negated.
The NOT will match everything except for the values in the set and ranges.

```Plain
(State): !'abcd'   => (Next);
(State): !'a'..'d' => (Next);
```

### Transition On Any Character

A transition can also match any character.
Since the characters are matched in the order they are
defined, this can be used like an "else" or "otherwise".

```Plain
(State): * => (Next);
```

### Transition Combining

Several transitions between the same two states can be OR'ed together
by listing them and comma separating them.

```Plain
(State): 'a'..'z', 'A'..'Z' => (Next);
```

Negations only apply to the set or range it is next to.
The following will match anything that is not a lowercase letter and it matches lower case 'g'.

```Plain
(State): !'a'..'z', 'g' => (Next);
```

Negations can be combined negations with parenthesis.

```Plain
(State): !('a'..'z', 'A'..'Z', '_') => (Next);
```

### Special Transition Characters

The transitions can use either single quotes or double quotes.

```Plain
(State): "abcd" => (Next);
(State): "'"    => (Next);
(State): '"'    => (Next);
```

The characters can be defined as unicode characters (no modifiers).
The characters can be escaped to specify specific characters.
See the below table for the list of single character escapes.

| Notation | Ascii | character Description |
|:--------:|:-----:|:----------------------|
| \\'      | \\x27 | allow to enter a '    |
| \\"      | \\x22 | allow to enter a "    |
| \\\\     | \\x5c | allow to enter a \    |
| \\b      | \\x08 | back-space            |
| \\f      | \\x0c | form-feed (new page)  |
| \\n      | \\x0a | line-feed (new line)  |
| \\r      | \\x0d | carriage-return       |
| \\t      | \\x09 | tab (horizontal-tab)  |
| \\v      | \\x0b | vertical-tab          |

Apostrophes do not need to be escaped when inside quotes and
quotation marks do not need to be escaped when inside single quotes.

Additionally, the character can be an ASCII byte in hexadecimal (\\xFF) or a UTF-16 hexadecimal (\\uFFFF).

```Plain
(State): '\''     => (Next);
(State): "\""     => (Next);
(State): '\n\r'   => (Next);
(State): '\x0A'   => (Next);
(State): '\u2042' => (Next);
```

### Consuming Characters

A transition can be consumed and not outputted into the token result.
Add a hat (`^`) in the front of the transition to consume the letter that was matched.
The consume will apply to all comma separated sets and ranges,
however there may be consuming transitions and non-consuming transitions between the same two states.

```Plain
(State): ^'a'      => (Next)
(State): 'b'       => (Next)
(State): ^'c', 'd' => (Next)
```

## Chaining States

The following can be simplified by chaining transitions together.

```Plain
(Previous): 'a' => (State);
(State):    'b' => (Next);
```

In the following example, `(State)` is the ending state from the first transition
and used as the start for the second transition to get the same result as above.

```Plain
(Previous): 'a' => (State): 'b' => (Next);
```

## Assigning Tokens

// TODO: Talk about setting the token

// TODO: Talk about consuming the token

## Examples
