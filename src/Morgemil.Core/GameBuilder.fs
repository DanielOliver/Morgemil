namespace Morgemil.Core

open Morgemil.Models

/// The steps and state of a game that's being built.
[<RequireQualifiedAccess>]
type GameBuilderState =
    | WaitingForCurrentPlayer of AddCurrentPlayer: (RaceID -> unit)
    | GameBuilt of GameEngine: IGameStateMachine * CurrentPlayerID: PlayerID
    | LoadingGameProgress of State: string

/// The steps and state of a game that's being built.
[<RequireQualifiedAccess>]
type GameBuilderStateRequest =
    | QueryState of AsyncReplyChannel<GameBuilderState>
    | AddPlayer of PlayerID: PlayerID

/// The interface to interact with a game being built.
type GameBuilder =
    /// The current state of the builder
    abstract member State: GameBuilderState with get

