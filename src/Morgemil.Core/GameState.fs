namespace Morgemil.Core

open Morgemil.Models
open Morgemil.Models.Relational

[<RequireQualifiedAccess>]
type StepItem =
    | Character of Character TableEvent
    
type Step =
    {   Event: ActionEvent
        Updates: StepItem list
    }

[<RequireQualifiedAccess>]
type GameStateType =
    | Processing
    | Results
    | WaitingForInput

[<RequireQualifiedAccess>]
type GameState =
    | Processing
    | Results of Steps: Step list * AcknowledgeCallback: (unit -> unit)
    | WaitingForInput of InputCallback: (ActionRequest -> unit)

    member this.GameStateType =
        match this with
        | GameState.Processing -> GameStateType.Processing
        | GameState.Results _ -> GameStateType.Results
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
    abstract member CurrentState: GameState with get
