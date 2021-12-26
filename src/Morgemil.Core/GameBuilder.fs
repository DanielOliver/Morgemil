namespace Morgemil.Core

open Morgemil.Models


[<RequireQualifiedAccess>]
type GameBuilderStateType =
    | WaitingForCurrentPlayer
    | GameBuilt
    | LoadingGameProgress
    | LoadedScenarioData
    | SelectScenario

/// The initial game data returned
type InitialGameData =
    { TileMap: TileMap
      Characters: Character []
      CurrentPlayerID: PlayerID
      GameContext: GameContext }

/// The steps and state of a game that's being built.
[<RequireQualifiedAccess>]
type GameBuilderState =
    | WaitingForCurrentPlayer of AddCurrentPlayer: (RaceID -> unit)
    | GameBuilt of GameEngine: IGameStateMachine * InitialGameData: InitialGameData
    | LoadingGameProgress of State: string
    | LoadedScenarioData of ScenarioData: ScenarioData
    | SelectScenario of Scenarios: string list * ChooseScenario: (string -> unit)

    member this.GameBuilderStateType =
        match this with
        | GameBuilderState.WaitingForCurrentPlayer _ -> GameBuilderStateType.WaitingForCurrentPlayer
        | GameBuilderState.GameBuilt _ -> GameBuilderStateType.GameBuilt
        | GameBuilderState.LoadingGameProgress _ -> GameBuilderStateType.LoadingGameProgress
        | GameBuilderState.LoadedScenarioData _ -> GameBuilderStateType.LoadedScenarioData
        | GameBuilderState.SelectScenario _ -> GameBuilderStateType.SelectScenario

/// The steps and state of a game that's being built.
[<RequireQualifiedAccess>]
type GameBuilderStateRequest =
    | QueryState of AsyncReplyChannel<GameBuilderState>
    | AddPlayer of RaceID: RaceID
    | SelectScenario of ScenarioName: string
    | SetScenarioData of ScenarioData: ScenarioData
    | SetGameData of GameEngine: IGameStateMachine * InitialGameData: InitialGameData

/// The interface to interact with a game being built.
type IGameBuilder =
    /// The current state of the builder
    abstract member CurrentState : GameBuilderState
