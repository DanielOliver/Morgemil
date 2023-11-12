namespace Morgemil.Core

open Morgemil.Models
open Morgemil.Models.Relational
open Morgemil.Models.Tracked

[<RequireQualifiedAccess>]
type StepItem =
    | Character of Character TableEvent
    | CharacterAttributes of CharacterAttributes TableEvent
    | GameContext of GameContext TrackedEvent
    | CompleteMapChange of TileMapData TrackedEvent
    | TileInstance of TileInstance TableEvent

type Step =
    { Event: ActionEvent
      Updates: StepItem list }

[<RequireQualifiedAccess>]
type GameStateWaitingType =
    | WaitingForInput
    | WaitingForAI
    | WaitingForEngine

[<RequireQualifiedAccess>]
type GameStateType =
    | Processing
    | WaitingForInput
//    | WaitingForAI

[<RequireQualifiedAccess>]
type GameState =
    | Processing
    | Results of Steps: Step list * AcknowledgeCallback: (unit -> unit)
    | WaitingForInput of InputCallback: (ActionRequest -> unit)

    member this.GameStateType =
        match this with
        | GameState.Processing -> GameStateType.Processing
        | GameState.Results _ -> GameStateType.Processing
        | GameState.WaitingForInput _ -> GameStateType.WaitingForInput

[<RequireQualifiedAccess>]
type GameStateRequest =
    | Input of Input: ActionRequest
    | QueryState of AsyncReplyChannel<GameState>
    | SetResults of Steps: Step list
    | Kill
    | Acknowledge

type IGameStateMachine =
    /// Stops the game engine
    abstract member Stop: unit -> unit
    /// Gets the current state of the game loop
    abstract member CurrentState: GameState
    /// Gets the raw scenario data
    abstract member ScenarioData: ScenarioData
