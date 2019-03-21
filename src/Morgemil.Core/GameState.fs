namespace Morgemil.Core

open Morgemil.Models
open Morgemil.Models.Relational

[<RequireQualifiedAccess>]
type GameState =
    | Processing
    | Results of Character Step list
    | WaitingForInput

[<RequireQualifiedAccess>]
type GameStateRequest =
    | Input of ActionRequest
    | QueryState of AsyncReplyChannel<GameState>
    | SetResults of Character Step list
    | Kill
    | Acknowledge

type IGameStateMachine =
    /// Sends a poison pill.
    abstract member Stop: unit -> unit
    /// Gets the current state of the game loop
    abstract member CurrentState: GameState with get
    /// Sends input
    abstract member Input: ActionRequest -> unit
    /// Acknowledge results
    abstract member Acknowledge: unit -> unit
