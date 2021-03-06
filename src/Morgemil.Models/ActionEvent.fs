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

type TileMapData =
    {
        Tiles: Tile array
        DefaultTile: Tile
        TileFeatures: TileFeature option array
        Size: Vector2i
    }

type EventMapChange =
    {
        Characters: Character array
        TileMapData: TileMapData
    }

[<RequireQualifiedAccess>]
type ActionEvent =
    | AfterMove of EventAfterMove
    | RefusedMove of EventRefusedMoved
    | MapChange of EventMapChange
    | Empty of int
