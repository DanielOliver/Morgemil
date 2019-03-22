namespace Morgemil.Core

open Morgemil.Math
open Morgemil.Models

type TileMap(mapSize: Rectangle, defaultTile: Tile) =
    let chunk: (Tile * TileFeature option) array = Array.create mapSize.Area (defaultTile, None)

    member this.MapSize = mapSize
    member this.GetCoordinateOffset (point: Vector2i) =
        (mapSize.Width * point.Y) + point.X 

    member this.Item
        with get (key: Vector2i) =
            if mapSize.Contains key then chunk.[ this.GetCoordinateOffset key ]
            else (defaultTile, None)
        and set (key: Vector2i) (tile: Tile, tileFeature: TileFeature option) =
            if mapSize.Contains key then chunk.[ this.GetCoordinateOffset key ] <- (tile, tileFeature)
        
    member this.Tile
        with get (key: Vector2i) =
            if mapSize.Contains key then 
                let (tile, _) = chunk.[ this.GetCoordinateOffset key ]
                tile
            else defaultTile
        and set (key: Vector2i) (tile: Tile) =
            if mapSize.Contains key then 
                let (_, tileFeature) = chunk.[ this.GetCoordinateOffset key ]
                chunk.[ this.GetCoordinateOffset key ] <- (tile, tileFeature)

    member this.TileFeature
        with get (key: Vector2i) =
            if mapSize.Contains key then 
                let (_, tileFeature) = chunk.[ this.GetCoordinateOffset key ]
                tileFeature
            else None
        and set (key: Vector2i) (tileFeature: TileFeature option) =
            if mapSize.Contains key then 
                let (tile, _) = chunk.[ this.GetCoordinateOffset key ]
                chunk.[ this.GetCoordinateOffset key ] <- (tile, tileFeature)

    member this.Tiles = 
        chunk
        |> Seq.zip mapSize.Coordinates
        |> Seq.map( fun (t, (u, v)) -> t, u, v )

module TileMap =

    let blocksMovement (tile: Tile, tileFeature: TileFeature option) =
        tile.BlocksMovement || (tileFeature.IsSome && tileFeature.Value.BlocksMovement)
        
    let blocksSight (tile: Tile, tileFeature: TileFeature option) =
        tile.BlocksSight || (tileFeature.IsSome && tileFeature.Value.BlocksSight)

    let isExitPoint (tile: Tile, tileFeature: TileFeature option) =
        (tileFeature.IsSome && tileFeature.Value.ExitPoint)
