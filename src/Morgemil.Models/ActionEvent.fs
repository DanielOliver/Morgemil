namespace Morgemil.Models

open Morgemil.Math
open Morgemil.Models

type EventAfterMove =
    {
        CharacterID: CharacterID
        OldPosition: Vector2i
        NewPosition: Vector2i
    }


type EventRefusedMoved =
    {
        CharacterID: CharacterID
        OldPosition: Vector2i
        RequestedPosition: Vector2i
    }


[<RequireQualifiedAccess>]
type ActionEvent =
    | AfterMove of EventAfterMove
    | RefusedMove of EventRefusedMoved

