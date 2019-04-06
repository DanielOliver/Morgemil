namespace Morgemil.Core

open Morgemil.Models

/// The steps and state of a game that's being built.
[<RequireQualifiedAccess>]
type GameBuilderState =
    | WaitingForInput
    | LoadingScenarioData
    | WaitingForPlayers
    | BuildingMap
    | AddingCharacters
    | GameBuilt of IGameStateMachine

/// The interface to interact with a game being built.
type GameBuilder =
    /// The current state of the builder
    abstract member State: GameBuilderState with get
    /// List all players connected
    abstract member ListPlayers: PlayerID list
    /// False if still waiting for player interaction
    abstract member Building: bool with get

