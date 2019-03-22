namespace Morgemil.Models

open Morgemil.Math

[<RequireQualifiedAccess>]
type ActionRequest =
    | Move of CharacterID: CharacterID * Direction: Vector2i
    | GoToNextLevel of CharacterID: CharacterID
