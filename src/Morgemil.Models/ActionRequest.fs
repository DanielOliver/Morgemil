namespace Morgemil.Models



type ActionRequestMove =
    { EntityID: EntityID
      Direction: SadRogue.Primitives.Point }

[<RequireQualifiedAccess>]
type ActionRequest =
    | Move of ActionRequestMove
    | GoToNextLevel of EntityID: EntityID
    | Pause of EntityID: EntityID
    | Engine
