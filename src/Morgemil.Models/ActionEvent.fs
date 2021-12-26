namespace Morgemil.Models

open Morgemil.Math
open Morgemil.Models

type EventAfterMove =
    { CharacterID: CharacterID
      OldPosition: Vector2i
      NewPosition: Vector2i }

type EventRefusedMoved =
    { CharacterID: CharacterID
      OldPosition: Vector2i
      RequestedPosition: Vector2i }

type EventPaused =
    { CharacterID: CharacterID }

type TileMapData =
    { Tiles: Tile array
      DefaultTile: Tile
      TileFeatures: TileFeature option array
      Size: Vector2i }

type EventMapChange =
    { Characters: Character array
      TileMapData: TileMapData }

type EventTileFeatureChanged =
    { Position: Vector2i
      OldTileFeature: TileFeature option
      NewTileFeature: TileFeature option }

[<RequireQualifiedAccess>]
type ActionEvent =
    | AfterMove of EventAfterMove
    | RefusedMove of EventRefusedMoved
    | MapChange of EventMapChange
    | Paused of EventPaused
    | TileFeatureChanged of EventTileFeatureChanged
    | Empty of int
    | EndResponse of int
