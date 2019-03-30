# Morgemil

A dungeon-crawler, single-player RPG framework and game.

[![Build Status](https://dev.azure.com/morgemil/Game/_apis/build/status/DanielOliver.Morgemil?branchName=master)](https://dev.azure.com/morgemil/Game/_build/latest?definitionId=1&branchName=master)

[![codecov](https://codecov.io/gh/DanielOliver/Morgemil/branch/master/graph/badge.svg)](https://codecov.io/gh/DanielOliver/Morgemil)

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
* Morgemil.Math
    * A custom math library dedicated to ease of use and composability.
* Morgemil.Models
    * These are the rich domain models that the game is built on top of.
* Morgemil.Utility
    * A catch-all namespace.


