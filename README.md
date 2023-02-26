# Morgemil

A dungeon-crawler, single-player RPG framework and game.

[![Build](https://github.com/DanielOliver/Morgemil/actions/workflows/build.yml/badge.svg)](https://github.com/DanielOliver/Morgemil/actions/workflows/build.yml)
[![codecov](https://codecov.io/gh/DanielOliver/Morgemil/branch/nightly/graph/badge.svg)](https://codecov.io/gh/DanielOliver/Morgemil)

## Code Structure

Here is the list of projects and their purpose.

* Morgemil.Console
    * Command line tools to interact with game data outside the context of a complete graphical game engine.
* Morgemil.Core
    * The actual game logic and constructs necessary to be a game.
* Morgemil.Data
    * The game's responsibilities for reading from data stores, validating data from those stores, and if necessary, translating data from the stores.
* Morgemil.GameEngine
    * The graphical game engine lives in here, built on top of SadConsole engine.
* Morgemil.Generation
    * An experimental addition to enable code generation of boiler-plate. Especially relevant for purposes of serialization and deserialization of data.
* Morgemil.Math
    * A custom math library dedicated to ease of use and composability.
* Morgemil.Models
    * These are the rich domain models that the game is built on top of.
* Morgemil.Utility
    * A catch-all namespace.
* build
    * F# Fake build project.


## Coding Guidelines

These coding guidelines are not absolute. There are always cases not considered and cases where the guidelines should be set aside.

1. Consider using F# code type annotations to be specifically clear about expected purpose.
2. Every field on a F# record should be immutable (which is the default).
3. Every piece of randomness should use a provided RNG (Random Number Generator) in order for the game logic to be completely deterministic.
4. Every F# discriminated union should use the `RequireQualifiedAccess` attribute by default.
5. Use F# discriminated unions in place of .NET enums by default.
6. Be sparing with returning a `Tuple` as a result. If there are two or more fields, consider using a record with defined names.
7. Most F# data models shouldn't depend on any services or injected functions. These data models are subject to serialization and being sent over a network.
8. Casing
    * Pascal Casing for Types and the Type's Methods and Properties.
    * camel Casing for function parameters and privately scoped variables.
9. Be sparing in use of F# Computation Expressions.

## Tools used

* JetBrains Rider
* .NET 7.0

## Scratchpad notes

* [Interesting F# extensions to System.Text.Json](https://github.com/Tarmil/FSharp.SystemTextJson/)
