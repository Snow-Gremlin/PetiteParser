# Tips

This is a collection of common mistakes made and solutions to them.

- [Infinite Loop In Rule](#infinite-loop-in-rule)

## Infinite Loop In Rule

If you get an error similar to the following:

`Infinite goto loop found in term part between the state(s) [X, Y, Z].`

This means that the language has a path through it which can loop infinitly
without requiring another token from the tokenizer.

Typically this is caused by a language defined which is written like an LL1
language with recursion on the left hand side of the rule when it should
be written like an LR1 language with recursion on the right hand side.

### Infinite Loop Example

For example, the following language snippet will cause the following error:

```text
> (Start): 'a' => [A];
(Start): 'b' => [B];
> <Start> := _ | <Part> <Start>;
<Part> := [A] | [B];
```

#### Infinite Loop Error

The following error will be returned from the loader.
It contains the LR1 parser states being used to generate the parser table.
These states are specific to the LR1 parser's internals and shouldn't be confused
with the tokenizer states.

```text
System.Exception: Errors while building parser:
state 0:
  <$StartTerm> → • <Start> [$EOFToken] @ [$EOFToken]
  <Start> → • @ [$EOFToken]
  <Start> → • <Part> <Start> @ [$EOFToken]
  <Part> → • [A] @ [A] [B] [$EOFToken]
  <Part> → • [B] @ [A] [B] [$EOFToken]
  <Start>: goto state 1
  <Part>: goto state 2
  [A]: shift state 3
  [B]: shift state 4
state 1:
  <$StartTerm> → <Start> • [$EOFToken] @ [$EOFToken]
state 2:
  <Start> → <Part> • <Start> @ [$EOFToken]
  <Start> → • @ [$EOFToken]
  <Start> → • <Part> <Start> @ [$EOFToken]
  <Part> → • [A] @ [A] [B] [$EOFToken]
  <Part> → • [B] @ [A] [B] [$EOFToken]
  <Start>: goto state 5
  <Part>: goto state 2
  [A]: shift state 3
  [B]: shift state 4
state 3:
  <Part> → [A] • @ [A] [B] [$EOFToken]
state 4:
  <Part> → [B] • @ [A] [B] [$EOFToken]
state 5:
  <Start> → <Part> <Start> • @ [$EOFToken]

Infinite goto loop found in term Part between the state(s) [2].

Stack Trace: 
  Parser.ctor(Grammar grammar, Tokenizer tokenizer) line 45
  Loader.get_Parser() line 349
  Loader.LoadParser(String[] input) line 23
  ParserLoaderUnitTests.ParserLoader08() line 282
```

The dot `•` in the state's rules is the index into the rule for the next step.
The tokens following after `@` are the follow tokens for that index in the rule.
The implicit main wrapping rule, `<$StartTerm>`, defines how to end
parse with the end-of-file token `[$EOFToken]`.
(Although it is called the end-of-file token, this is the token that is returned
when the scanner had finished scanning all the input for the parser.)

Each parser state also has the shift and goto transitions defined.
Shift transitions, like `[A]: shift state 3` from `state 2`, consume the token,
(`[A]`) when stepping from the current state (`state 2`) into the specified
stated (`state 3`). A goto transition, like `<Start>: goto state 5` from `state 2`,
is the state to transition to when the top of the current parse is a given
rule (`<Start>`) to step from the current state (`state 2`) into the specified
state (`state 5`). When a goto is taken, nothing is consumed.

(If you need to get the parser's internal states without getting this error
you can use `Parser.GetDebugStateString`.)

The error message says it trasnitions on a term, like `<Part>`, because it
will cause a goto transition to be taken through the given states.
If the states are `[X, Y, Z]` then state `X` has a goto to `Y` for the specified
term, then state `Y` has a goto `Z`, and `Z` has a goto back to `X`.
Since goto transitions don't consume anything, this is an infinite loop.

In this example there is only one state specificed, `[2]`, meaning the
loop from state 2 back to state 2. We can see that in state 2 is the
goto `<Part>: goto state 2` for the term `<Part>`.
The rules with the index prior to the term, `<Start> → • <Part> <Start`,
is rule that is problematic.

This gets more complicated to determine the more rules and states involved in the
loop, so you may have to break the language down to a smaller subset when debugging it.

#### Infinite Loop Fix

To fix this problem, change the rule for the term `Part`
from left recursion, `<Start> := <Part> <Start>;`,
to right recursion, ``<Start> := <Start> <Part>;`.

Here is the langague with the fix in it:

```text
> (Start): 'a' => [A];
(Start): 'b' => [B];
> <Start> := _ | <Start> <Part>;
<Part> := [A] | [B];
```

Some loops take a little more effort to figure out the 
