namespace Morgemil.Core

open Morgemil.Models

[<RequireQualifiedAccess>]
type GameState =
    | Processing
    | Results of ActionEvent seq
    | WaitingForInput

[<RequireQualifiedAccess>]
type GameStateRequest =
    | Input of ActionRequest
    | QueryState of AsyncReplyChannel<GameState>
    | SetResults of ActionEvent seq
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
