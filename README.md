# PetiteParserCSharp

Petite Parser is an CLR(1) parsing tool written in C#
which can be configured to read different languages and structures.

This can be used to read complex data files, interpret scripts,
or even translate a file into another (basic compile).

This library contains a parser and a tokenizer. The parser is a tool for
reading a complicated non-contextual language, such as a programming language.
The tokenizer is part of the parser but can be used by itself for
breaking up text for features like simple text coloring.

## Installing

- Clone this repo locally
- Install any current version of Visual Studio
- Open solution with Visual Studio
- Build the library *.dll as needed for your project

## Creating a language definition

- See [PetiteParser Documentation](./PetiteParser/PetiteParser/Documentation/README.md)

## Run unit-tests

- Open solution with Visual Studio
- Use the unit-testing tool to run tests

## Run examples

- Open solution with Visual Studio
- Run the ExampleRunner project
- One example important example is the Calculator:
  - See [Calculator Documentation](./PetiteParser/Examples/Calculator/README.md)
  - See [Calculator Language File](./PetiteParser/Examples/Calculator/Calculator.lang)
