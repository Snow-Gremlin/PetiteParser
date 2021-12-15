# Petite Parser Calculator

The calculator uses the petite parser to create a simple mathematical language.

- [Examples](#examples)
- [Literals](#literals)
  - [Implicit Conversions](#implicit-conversions)
- [Constants](#constants)
- [Functions](#functions)
  - [Explicit Casts](#explicit-casts)
  - [Formatting](#formatting)
  - [Trigonometry](#trigonometry)
  - [Logarithms](#logarithms)
  - [Basic Math](#basic-math)
- [Operators](#operators)
  - [Unary Operators](#unary-operators)
  - [Binary Operators](#binary-operators)
  - [Comparing Operators](#comparing-operators)
  - [Order of Operators](#order-of-operators)

## Examples

| Input                          | Result                    |
|--------------------------------|---------------------------|
| `10 * 4 + 6`                   | `46`                      |
| `10 * (-4 + 6)**2.0`           | `40.0`                    |
| `cos(1.5*pi)`                  | `-1.8369701987210297e-16` |
| `min(4, 8, 15, 16, 23, 42)`    | `4`                       |
| `0x00FF & 0xAAAA`              | `170`                     |
| `hex(0x00FF & 0xAAAA)`         | `0xAA`                    |
| `int(string(12) + string(34))` | `1234`                    |
| `x := 4; y := x + 2; x*y`      | `24`                      |
| `1 + 1; 2 + 2; 3 + 3;`         | `2, 4, 6`                 |
| `upper(sub("Hello", 1, 3))`    | `EL`                      |

## Literals

- **Binary** numbers are made up of `0` and `1`'s followed by a `b`. For example `1011b`.
- **Octal** numbers are made up of `0` to `7`'s followed by a `o`. For example `137o`.
- **Decimal** numbers are made up of `0` to `9`'s, optionally followed by a `d`. For example `42`.
- **Hexadecimal** numbers are made up of `0` to `9` and `a` to `f`'s preceded by a `0x`. For example `0x00FF`.
- **Boolean** is either `true` and `false`.
- **Real** numbers are decimals numbers with either a decimal point or exponent in it.
  For example `0.01`, `12e-3`, and `1.1e2`.
- **String** literals are quoted letters. It can have escaped characters for quotations (`\"`),
  newlines (`\n`), tabs (`\t`), ASCII (`\x0A`) with two hex digits, and Unicode (`\u000A`)
  with four hex digits. For example `""`, `"abc"`, `"\n"`, and `"\x0A"`.

### Implicit Conversions

- Booleans can be implicitly converted to and integer or real as 0 and 1.
- Integers can be implicitly converted into reals.

## Constants

These are the built-in constants. Additional constants may be added as needed.

- `pi`: This is a real with the value for pi.
- `e`: This is a real with the value for e.
- `true`: This is a Boolean for true.
- `false`: This is a Boolean for false.

## Functions

These are the built-in functions. Additional functions may be added as needed.

### Explicit Casts

- `bool`: Converts the value to Boolean, e.g `bool(1)`.
- `int`: Converts the value to integer, e.g `int(123)`.
- `real`: Converts the value to real, e.g `real(123)`.
- `string`: Converts the value to string, e.g. `string(123)`.
  If the value is an integer or real, the result is as decimal number string.

### Formatting

- `bin`: Formats an integer as a binary number string.
- `oct`: Formats an integer as an octal number string.
- `hex`: Formats an integer as a hexadecimal number string.
- `sub`: Gets the substring of a string given an integer start and stop, e.g. `sub("hello", 2, 4)`.
- `upper`: Gets the upper case of a string.
- `lower`: Gets the lower case of a string.
- `len`: Returns the length of a string.
- `padLeft`: Pads the string on the left side with an optional string
          until the string's length is equal to a specified length,
          e.g. `padLeft("hello", 3)` and `padLeft("hello", 3, "-")`.
          If not specified, the string will be padded with spaces.
- `padRight`: Pads the string on the right side with an optional string
          until the string's length is equal to a specified length,
          e.g. `padRight("hello", 3)` and `padRight("hello", 3, "-")`.
          If not specified, the string will be padded with spaces.
- `trim`: Trims all whitespace from the left and right of a string.
- `trimLeft`: Trims all whitespace from the left of a string.
- `trimRight`: Trims all whitespace from the right of a string.
- `join`: Joins zero or more strings with the first parameter as the separator,
          e.g. `join(" ", "hello", "world", "again")`.

### Trigonometry

- `sin`: Works on one number to get the sine.
- `cos`: Works on one number to get the cosine.
- `tan`: Works on one number to get the tangent.
- `acos`: Works on one number to get the arc cosine.
- `asin`: Works on one number to get the arc sine.
- `atan`: Works on one number to get the arc tangent.
- `atan2`: Works on two numbers to get the arc tangent given `y` and `x` as `atan(y/x)`.

### Logarithms

- `log`: Works on two numbers to get the log given `a` and `b` as `log(a)/log(b)`.
- `log2`: Works on one number to get the log base 2.
- `log10`: Works on one number to get the log base 10.
- `ln`: Works on one number to get the natural log.

### Basic Math

- `abs`: Works on one number to get the absolute value, e.g. `abs(5)`.
- `ceil`: Works on one real to get the ceiling (rounded up) value. Returns integers unchanged.
- `floor`: Works on one real to get the floor (rounded down) value. Returns integers unchanged.
- `round`: Works on one real to round the value. Returns integers unchanged.
- `sqrt`: Works on one number to get the square root.
- `rand`: Takes no arguments and will return a random real number between 0 and 1.
- `avg`: Works on one or more numbers to get the average of all the numbers.
    If all the numbers are integers then the result will be an integer, e.g. `avg(4.5, 3.3, 12.0)`.
- `max`: Works on one or more numbers to get the maximum of all the numbers.
    If all the numbers are integers then the result will be an integer, e.g. `max(4.5, 3.3, 12.0)`.
- `min`: Works on one or more numbers to get the minimum of all the numbers.
    If all the numbers are integers then the result will be an integer, e.g. `max(4.5, 3.3, 12.0)`.
- `sum`: Works on one or more numbers to get the summation of all the numbers.
    If all the numbers are integers then the result will be an integer, e.g. `sum(4.5, 3.3, 12.0)`.

## Operators

These are the operators to use for mathematics. Mathematical expressions can be separated by `;`,
e.g. `5*2; 1+2`
Parentheses, `(` and `)`, can be used to perform part of the equation first, e.g. `4 * (2 + 3)`.

### Unary Operators

- `+`: As an unary it has no effect on a number because it simply visually asserts the sign, e.g. `+4`.
- `-`: As an unary for a number it will negate the number, e.g. `-4`.
- `~`: This gets bitwise NOT the value of an integer, e.g. `~10`.
- `!`: This gets the NOT the a Boolean value.

### Binary Operators

- `+`: This will add them together two numbers, e.g. `2+4`. If both numbers are integers then an integer is returned.
    This can also be used between two strings to concatenate them, e.g. `"ab" + "cd"`.
    If used between two Booleans it will OR them.
- `-`: This will subtract the number right from the number left, e.g. `45-11`. If both numbers are integers then an integer
    is returned. If used between two Booleans it will imply (`!a|b`) them.
- `*`: This will multiplying two numbers together. If both numbers are integers then an integer is returned.
- `**`: This gets the power of the left raised to the right. If both numbers are integers then an integer is returned.
- `/`: This divides the left number from the right number. If both numbers are integers then a truncated integer is returned.
- `&`: This performs a bitwise ANDing of two integers or two Booleans.
- `|`: This performs a bitwise ORing of two integers or two Booleans.
- `^`: This performs a bitwise XORing of two integers or two Booleans.
- `:=`: This assigns a value to a variable, e.g. `x := 5; y := x + 2`.
  When a variable is assigned it is removed from the stack so will not be outputted.

### Comparing Operators

- `==`: This checks the equality of two values and returns a Boolean with the result.
    The values are compared if they are the same kind or can be implicitly cast to the same kind, otherwise false is returned.
- `!=`: This checks the inequality of two values and returns a Boolean with the result.
    The values are compared if they are the same kind or can be implicitly cast to the same kind, otherwise true is returned.
- `>`: This checks if the left number is greater than the right number.
- `>=`: This checks if the left number is greater than or equal to the right number.
- `<`: This checks if the left number is less than the right number.
- `<=`: This checks if the left number is less than or equal to the right number.

### Order of Operators

This is the order of operations so that `2 * 3 + 4` and `4 + 3 * 2` will be multiplied first
then added resulting in `10` for both and not `14` unless parentheses are used, e.g. `2 * (3 + 4)`.
These are in order of highest to lowest priority. When values have the same priority they will
be executed right to left.

- `:=`
- `|`
- `&`
- `==`, `!=`, `<`, `<=`, `>`, `>=`
- `+` (binary), `-` (binary)
- `*`, `\`
- `()`, `^`, `**`, `-` (unary), `+` (unary), `!`, `~`
