namespace Morgemil.Models



type ActionRequestMove =
    { CharacterID: CharacterID
      Direction: SadRogue.Primitives.Point }

[<RequireQualifiedAccess>]
type ActionRequest =
    | Move of ActionRequestMove
    | GoToNextLevel of CharacterID: CharacterID
    | Pause of CharacterID: CharacterID
    | Engine
