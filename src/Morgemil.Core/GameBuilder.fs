namespace Morgemil.Core

open Morgemil.Models

/// The steps and state of a game that's being built.
[<RequireQualifiedAccess>]
type GameBuilderState =
    | WaitingForPlayers of CancelPlayerReadyStatusCallback: (unit -> unit) * PlayerList: PlayerID list * CurrentPlayerID: PlayerID
    | WaitingForCurrentPlayer of AddCurrentPlayer: (Character -> unit) * PlayerList: PlayerID list
    | GameBuilt of GameEngine: IGameStateMachine * CurrentPlayerID: PlayerID
    | LoadingGameProgress of State: string

/// The interface to interact with a game being built.
type GameBuilder =
    /// The current state of the builder
    abstract member State: GameBuilderState with get

