namespace Morgemil.Models

open Morgemil.Math


type ActionRequestMove =
    { CharacterID: CharacterID
      Direction: Vector2i }

[<RequireQualifiedAccess>]
type ActionRequest =
    | Move of ActionRequestMove
    | GoToNextLevel of CharacterID: CharacterID
    | Pause of CharacterID: CharacterID