namespace Morgemil.GameEngine

open Morgemil.Core

type ScreenGameState =
    | MainScreen
    | SelectingScenario
    | PlayingGame
    | MapGeneratorConsole of GameState: IGameStateMachine * InitialGameData: InitialGameData
