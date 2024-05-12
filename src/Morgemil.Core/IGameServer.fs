namespace Morgemil.Core

open Morgemil.Models


[<RequireQualifiedAccess>]
type GameServerStateType =
    | WaitingForCurrentPlayer
    | GameBuilt
    | LoadingGameProgress
    | LoadedScenarioData
    | SelectScenario

/// The initial game data returned
type InitialGameData =
    { TileMap: TileMap
      Characters: Character[]
      CharacterAttributes: CharacterAttributes[]
      CurrentPlayerID: PlayerID
      Scenario: Scenario
      Entities: Entity[]
      GameContext: GameContext }

    member this.ToLoopContext() : LoopContext =
        let timeTable = TimeTable()
        let gameContext = TrackedEntity(this.GameContext, StepItem.GameContext)

        let loopContext =
            { LoopContext.Characters = CharacterTable(timeTable)
              CharacterAttributes = CharacterAttributesTable()
              Entities = EntityTable(timeTable)
              TimeTable = timeTable
              TileMap = this.TileMap
              GameContext = gameContext }

        for character in this.Characters do
            Table.AddRow loopContext.Characters character

        for entity in this.Entities do
            Table.AddRow loopContext.Entities entity

        for characterAttributes in this.CharacterAttributes do
            Table.AddRow loopContext.CharacterAttributes characterAttributes

        loopContext

[<RequireQualifiedAccess>]
type GameServerWorkflow = | ScenarioSelection

[<RequireQualifiedAccess>]
type GameServerRequest =
    | AddCurrentPlayer of AncestryID: AncestryID
    | SelectScenario of ScenarioID: string
    | Workflow of Workflow: GameServerWorkflow

/// The steps and state of a game that's being built.
[<RequireQualifiedAccess>]
type GameServerState =
    | WaitingForCurrentPlayer
    | GameBuilt of GameEngine: IGameStateMachine * InitialGameData: InitialGameData
    | LoadingGameProgress of State: string
    | LoadedScenarioData of ScenarioData: ScenarioData
    | SelectScenario of Scenarios: string list

    member this.GameServerStateType =
        match this with
        | GameServerState.WaitingForCurrentPlayer -> GameServerStateType.WaitingForCurrentPlayer
        | GameServerState.GameBuilt _ -> GameServerStateType.GameBuilt
        | GameServerState.LoadingGameProgress _ -> GameServerStateType.LoadingGameProgress
        | GameServerState.LoadedScenarioData _ -> GameServerStateType.LoadedScenarioData
        | GameServerState.SelectScenario _ -> GameServerStateType.SelectScenario

/// The steps and state of a game that's being built.
[<RequireQualifiedAccess>]
type GameServerInternalStateRequest =
    | QueryState of AsyncReplyChannel<GameServerState>
    | AddPlayer of AncestryID: AncestryID
    | SelectScenario of ScenarioName: string
    | SetScenarioData of ScenarioData: ScenarioData
    | SetGameData of GameEngine: IGameStateMachine * InitialGameData: InitialGameData
    | RequestWorkflow of GameServerWorkflow

/// The interface to interact with a game being built.
type IGameServer =
    /// The current state of the builder
    abstract member CurrentState: GameServerState
    abstract member Request: GameServerRequest -> unit
