# PetiteParserCSharp

Petite Parser is a simple CLR(1) parsing tool written in C#
which can be configured to read different languages and structures.

This can be used to read complex data files, interpret scripts,
or even translate a file into another (basic compile).

This library contains a parser and a tokenizer. The parser is a tool for
reading a complicated noncontextual language, such as a programming language.
The tokenizer is part of the parser but can be used by itself for
breaking up text for something like text coloring.

## Installing

- Clone this repo locally
- Install any version of Visual Studio 2019 or later
- Open solution with Visual Studio
- Build the library *.dll as needed for your project

## Creating a language definition

- See [PetiteParser Documentation](./PetiteParser/PetiteParser/Documentation/README.md)

## Run unit-tests

- Open solution with Visual Studio
- Use the unit-testing tool to run tests

## Run the calculator example

- Open solution with Visual Studio
- Run the Calculator Example
- See [Calculator Documentation](./PetiteParser/CalculatorExample/Calculator/README.md)
- See [Calculator Language File](./PetiteParser/CalculatorExample/Calculator/Calculator.lang)

## Other Versions

- [PetiteParserDart](https://github.com/Grant-Nelson/PetiteParserDart#petiteparserdart) (Incomplete)

## To Do

- Need to add serialization to make reloading computed parser languages faster.
- Need to add a simple way to add errors into the parse table via the language
  definition so that the language can add suggestions for failures or indication
  of unsupported or future features of the language.
- Need to write a tutorial for how the parser language works and how to use prompts.
- Need to add a way to add predef matchers.
