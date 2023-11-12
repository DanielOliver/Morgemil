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
    | AfterMove of EventAfterMove
    | RefusedMove of EventRefusedMoved
    // | MapChange of EventMapChange
    | MapChange
    | Pause of EventPause
    | TileFeatureChanged of EventTileFeatureChanged
    | Empty of int
    | EndResponse of int
