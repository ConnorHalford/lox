# Lox

This repository implements two interpreters for the Lox language. The language and these interpreters come from the book [_Crafting Interpreters_ by Robert Nystrom](https://www.craftinginterpreters.com/), freely available online in its entirety. The Lox language and these interpreters are licensed under MIT by Robert Nystrom, so these particular implementations are also provided under MIT.

The first interpreter in the book is named `jlox` because it is written in Java. The equivalent in this repo is the `TreeWalkInterpreter` project written in C#, and is more-or-less a direct translation from the book's Java code to C#. It is a tree-walk interpreter which parses and executes Lox scripts at runtime; not very fast but usefully simple for understanding language concepts. It is built in stages across chapters 4-13.

The second interpreter in the book in named `clox` because it is written in C. The equivalent in this repo is the `BytecodeVM` project, also in C, which is more-or-less the direct code from the book. It compiles Lox scripts into bytecode and then runs that on a virtual machine; more complex to create but significantly faster at runtime and much closer to how modern scripting languages operate. It is built in stages across chapters 14-30.

Other than converting jlox to C# I made basically no changes. If you want to learn more check out the book yourself or look at the [official implementation](https://github.com/munificent/craftinginterpreters).