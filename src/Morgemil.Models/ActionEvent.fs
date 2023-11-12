namespace Morgemil.Models

open Morgemil.Math
open Morgemil.Models

type EventAfterMove =
    { CharacterID: CharacterID
      OldPosition: Point
      NewPosition: Point }

type EventRefusedMoved =
    { CharacterID: CharacterID
      OldPosition: Point
      RequestedPosition: Point }

type EventPause = { CharacterID: CharacterID }

type TileMapData =
    { Tiles: Tile array
      DefaultTile: Tile
      TileFeatures: TileFeature option array
      Size: Point }

type EventMapChange =
    { Characters: Character array
      TileMapData: TileMapData }

type EventTileFeatureChanged =
    { Position: Point
      OldTileFeature: TileFeature option
      NewTileFeature: TileFeature option }

[<RequireQualifiedAccess>]
type ActionEvent =
    | ActionArchetype of ActionArchetype
    | AfterMove of EventAfterMove
    | RefusedMove of EventRefusedMoved
    | MapChange
    | Pause of EventPause
    | TileFeatureChanged of EventTileFeatureChanged
    /// Only purpose is to fix rare cases where SOME response is needed. Probably a code smell if used.
    | Empty of int
    /// Only purpose is to capture any outstanding events. Has no meaning on its own.
    | EndResponse of int
