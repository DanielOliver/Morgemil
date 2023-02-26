namespace Morgemil.Core

open Morgemil.Math
open Morgemil.Models

type TileMap(mapSize: Rectangle, defaultTile: Tile, ?chunkData: (Tile * TileFeature option) array) =
    let chunk: (Tile * TileFeature option) array =
        chunkData |> defaultArg <| Array.create mapSize.Area (defaultTile, None)

    let translateOffsetToCoordinate (position: int) =
        Vector2i.create (position % mapSize.Width, position / mapSize.Width)

    member this.EntryPoints =
        chunk
        |> Seq.mapi (fun index (_, t) ->
            if t.IsSome && t.Value.EntryPoint then
                Some <| translateOffsetToCoordinate index
            else
                None)
        |> Seq.choose id

    member this.TileMapData: TileMapData =
        { TileMapData.Size = mapSize.Size
          TileFeatures = chunk |> Array.map (fun (_, t) -> t) |> Array.copy
          Tiles = chunk |> Array.map (fun (t, _) -> t) |> Array.copy
          DefaultTile = defaultTile }

    member this.DefaultTile: Tile = defaultTile
    member this.MapSize: Rectangle = mapSize
    member this.GetCoordinateOffset(point: Vector2i) : int = (mapSize.Width * point.Y) + point.X

    member this.Item
        with get (key: Vector2i): TileInstance =
            let offset = this.GetCoordinateOffset key

            if mapSize.Contains key then
                let (tile, tileFeature) = chunk.[offset]

                { TileInstance.ID = offset |> int64 |> TileInstanceID
                  Tile = tile
                  TileFeature = tileFeature
                  Position = key }
            else
                { TileInstance.ID = -1L |> TileInstanceID
                  Tile = defaultTile
                  TileFeature = None
                  Position = key }
        and set (key: Vector2i) (tile: Tile, tileFeature: TileFeature option) =
            if mapSize.Contains key then
                chunk.[this.GetCoordinateOffset key] <- (tile, tileFeature)

    member this.Tile
        with get (key: Vector2i): Tile =
            if mapSize.Contains key then
                let (tile, _) = chunk.[this.GetCoordinateOffset key]
                tile
            else
                defaultTile
        and set (key: Vector2i) (tile: Tile) =
            if mapSize.Contains key then
                let (_, tileFeature) = chunk.[this.GetCoordinateOffset key]
                chunk.[this.GetCoordinateOffset key] <- (tile, tileFeature)

    member this.TileFeature
        with get (key: Vector2i): TileFeature option =
            if mapSize.Contains key then
                let (_, tileFeature) = chunk.[this.GetCoordinateOffset key]
                tileFeature
            else
                None
        and set (key: Vector2i) (tileFeature: TileFeature option) =
            if mapSize.Contains key then
                let (tile, _) = chunk.[this.GetCoordinateOffset key]
                chunk.[this.GetCoordinateOffset key] <- (tile, tileFeature)

    member this.Tiles: TileInstance seq =
        chunk
        |> Seq.zip mapSize.Coordinates
        |> Seq.mapi (fun index (t, (u, v)) ->
            { TileInstance.ID = index |> int64 |> TileInstanceID
              Position = t
              TileInstance.Tile = u
              TileFeature = v })

module TileMap =

    let blocksMovement (tileInstance: TileInstance) =
        tileInstance.Tile.BlocksMovement
        || (tileInstance.TileFeature.IsSome && tileInstance.TileFeature.Value.BlocksMovement)

    let blocksSight (tileInstance: TileInstance) =
        tileInstance.Tile.BlocksSight
        || (tileInstance.TileFeature.IsSome && tileInstance.TileFeature.Value.BlocksSight)

    let isExitPoint (tileInstance: TileInstance) =
        (tileInstance.TileFeature.IsSome && tileInstance.TileFeature.Value.ExitPoint)

    let isEntryPoint (tileInstance: TileInstance) =
        (tileInstance.TileFeature.IsSome && tileInstance.TileFeature.Value.EntryPoint)
