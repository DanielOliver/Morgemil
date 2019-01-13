namespace Morgemil.Core

open Morgemil.Math
open Morgemil.Models

type TileMap(mapSize: Rectangle, defaultTile: Tile) =
    let chunk: Tile[] = Array.create mapSize.Area defaultTile

    member this.MapSize = mapSize
    member this.GetCoordinateOffset (point: Vector2i) =
        (mapSize.Width * point.Y) + point.X

    member this.Item
        with get (key: Vector2i) =
            if mapSize.Contains key then chunk.[ this.GetCoordinateOffset key ]
            else defaultTile
        and set (key: Vector2i) (tile: Tile) =
            if mapSize.Contains key then chunk.[ this.GetCoordinateOffset key ] <- tile

    member this.Tiles = 
        chunk
        |> Seq.zip mapSize.Coordinates

