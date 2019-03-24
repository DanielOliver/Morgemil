namespace Morgemil.Core

open Morgemil.Math
open Morgemil.Models

type TileMap(mapSize: Rectangle, defaultTile: Tile, ?chunkData: (Tile * TileFeature option) array) =
    let chunk: (Tile * TileFeature option) array =
        chunkData
        |> defaultArg
        <| Array.create mapSize.Area (defaultTile, None)

    let translateOffsetToCoordinate (position: int) =
        Vector2i.create(position % mapSize.Width, position / mapSize.Width)

    member this.EntryPoints =
        chunk
        |> Seq.mapi(fun index (_, t) ->
            if t.IsSome && t.Value.EntryPoint then
                Some <| translateOffsetToCoordinate index
            else None
        )
        |> Seq.choose id
    member this.TileMapData: TileMapData =
        {
            TileMapData.Size = mapSize.Size
            TileFeatures = chunk |> Array.map(fun (_, t) -> t) |> Array.copy
            Tiles = chunk |> Array.map(fun (t, _) -> t) |> Array.copy
            DefaultTile = defaultTile
        }
    member this.DefaultTile: Tile = defaultTile
    member this.MapSize: Rectangle = mapSize
    member this.GetCoordinateOffset (point: Vector2i): int =
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
        
    let isEntryPoint (tile: Tile, tileFeature: TileFeature option) =
        (tileFeature.IsSome && tileFeature.Value.EntryPoint)
