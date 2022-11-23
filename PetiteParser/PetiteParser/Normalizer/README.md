# Normalizer

Normalization converts a grammar into a form which can be used by the CLR parser.
The main requirements are to have left-recursion eliminated and to left-factor the grammar.
The other precepts are useful for simplifying the grammar and removing rules which
complicate the process of performing the other precepts.

_:warning: For the following all prompts are ignored. An attempt should be made to
keep the prompts in the changed grammar,
such that they will still be called at the same points in a set of tokens which
is accepted by the original grammar.
However, some prompts will be removed if they exist in unproductive rules._

- [Resources](#resources)
- [Eliminating direct left-recursion](#eliminating-direct-left-recursion)
- [Eliminating indirect left-recursion](#eliminating-indirect-left-recursion)

## Resources

- [Left recursion Wiki](https://handwiki.org/wiki/Left_recursion)
- [Left Recursion & Elimination](https://www.gatevidyalay.com/left-recursion-left-recursion-elimination/)

## Eliminating direct left-recursion

A term is direct left-recursive if it has one or more rules which starts with that term,
`<A> → <A> α` where `α` is a set of zero or more terms and tokens.
The term may have zero or more other rules which do not start with that term,
`<A> → β` where `β` is a set of zero or more terms and tokens which do not start with `<A>`.

If the rule is only the term by itself, `<A> → <A>` (i.e. `α` is empty or only contains prompts),
then simply remove that rule. It is an unproductive rule.

Construct a new term, `A'0`, and initialize it with the rule `<A'0> → λ`.
(We use `A'0` instead of simply `A'` to allow for multiple term constructions
based on the same term, i.e. `A'0`, `A'1`, `A'2` and so on.)

For all rules with start with the term, `<A>`:

- Remove that rule from `<A>`
- Remove `<A>` from the start of the rule
- Add `<A'0>` to the end of the rule
- Then add the modified rule to `<A'0>`
- The result will be `<A'0> → α <A'0>`

For all rules which do not start with the term, `<A>`:

- Add `<A'0>` to the end of the rule
- The result will be `<A> → β <A'0>`

### Direct left-recursion elimination example

Given:

```plain
> <A> → <A> [a] | [b]
```

This grammar accepts `b`, `ba`, `baa`, and so on.
Or as shown as a regular expression: `ba*`.

There is a direct left-recursion in the rule `<A> → <A> [a]`.

- Create `<A'0>` and initialize it with `<A'0> → λ`
- Remove `<A> → <A> [a]` and add `<A'0> → [a] <A'0>`
- Add `<A'0>` to the end of `<A> → [b]`

The resulting grammar is

```plain
> <A> → [b] <A'0>
<A'0> → [a] <A'0> | λ
```

As can be seen, this still matches `ba*` but is now right-recursive.

## Eliminating indirect left-recursion

A set of terms, `<A>, <B>, ... <N>` are indirect left-recursive if there is a set of rules where
each rule starts with the next term in the sequence (`<A> → <B> α`, `<B> → <C> β` ...),
until the last term in the sequence which has a rule starting with the first term in the sequence,
`<N> → <A> γ`.

Performing direct left-recursion removal on `<A>` will not eliminate the indirect left-recursion,
because `<A>` may not even have direct left-recursion.

Select a start term (`<i>`). For the following terms (`<j>`) in the indirect left-recursive sequence
(e.g. `<A>` and `<B>`, `<A>` and `<C>`, ..., `<A>` and `<N>`),
substitute `<j>`'s rules into any rule starting with `<j>`.

The substitution in `<i>` and the following `<j>` is performed by doing:

- Foreach rule (`<i> → <j> α`) in `<i>` which start with `<j>`:
  - Remove `<i> → <j> α` from `<i>`
  - Foreach rule (`<j> → β`) in `<j>`, add a rule to `<i>` such that `<i> → β α`

For example, given `<A> → <B> α` and `<B> → <C> β`, the result will be `<A> → <C> β α`, `<B> → <C> β`.

It can be seen that by performing a single substitution the sequence has been reduced by `<j>`.
For example, if `<B>` was substituted into `<A>`, since `<B>` contains rules with `<C>`,
then after the substitution `<A>` has rules which start with `<C>`.
Therefore the sequence has gone from `<A>, <B>, <C>, ... <N>` to `<A>, <C>, ... <N>`.

Once the substitution has been performed on the whole sequence `<A>` will have rules from `<N>`,
meaning `<A>` will now have rules starting with `<A>`, a direct left-recursion.
That direct left-recursion can then be handled as described
in [Eliminating direct left-recursion](#eliminating-direct-left-recursion).

### Indirect left-recursion elimination example

Given:

```plain
> <A> → <B> [a] | [d]
<B> → <C> [b] | [e]
<C> → <A> [c] | [f]
```

This grammar accepts `d`, `ea`, `fba`, `dcba`, `eacba`, and so on.
Or as shown as a regular expression: `(d|ea|fba)(cba)*`.

The left-recursive sequence is `<A>`, `<B>`, then `<C>`.
First perform substitution in `<A>` and `<B>` to get:

```plain
> <A> → <C> [b] [a] | [e] [a] | [d]
<B> → <C> [b] | [e]
<C> → <A> [c] | [f]
```

Since `<B>` is now unreachable, for this specific language, it may be dropped.
This will not be true for all languages since in a larger language `<B>`
may be used in a rule else where. This is not part of removing indirect left-recursion.
We are doing it here just to help keep this example simpler to read.

The left-recursive sequence is now `<A>` then `<C>`.
Perform substitution in `<A>` and `<C>` to get:

```plain
> <A> → <A> [c] [b] [a] | [f] [b] [a] | [e] [a] | [d]
<C> → <A> [c] | [f]
```

Since `<C>` is now unreachable, it may be dropped.
At this point there is a direct left-recursion in `<A>`.
Remove the direct left-recursion gets the resulting grammar
with the indirect left-recursion completely removed:

```plain
> <A> → [f] [b] [a] <A'0> | [e] [a] <A'0> | [d] <A'0>
<A'0> → [c] [b] [a] <A'0> | λ
```

Since `<B>` is now unreachable, for this specific language, it may be dropped.
This will not be true for all languages since in a larger language `<B>`
may be used in a rule else where. This is not part of removing indirect left-recursion.
We are doing it here just to help keep this example simpler to read.

```plain
> <A> → [f] [b] [a] <A'0> | [e] [a] <A'0> | [d] <A'0>
<A'0> → [c] [b] [a] <A'0> | λ
```

This grammar still matches `(d|ea|fba)(cba)*` but is now right-recursive.
