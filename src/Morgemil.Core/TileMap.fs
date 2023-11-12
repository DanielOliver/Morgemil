namespace Morgemil.Core

open Morgemil.Math
open Morgemil.Models

type TileMap(mapSize: Rectangle, defaultTile: Tile, ?chunkData: (Tile * TileFeature option) array) =
    let mutable defaultTile = defaultTile
    let mutable mapSize = mapSize
    let mutable _trackedRecordEvent = ignore

    let mutable chunk: (Tile * TileFeature option) array =
        chunkData |> defaultArg <| Array.create mapSize.Area (defaultTile, None)

    let translateOffsetToCoordinate (position: int) =
        Point.create (position % mapSize.Width, position / mapSize.Width)

    let getCoordinateOffset (point: Point) = (mapSize.Width * point.Y) + point.X

    let getTileInstanceOrDefault (key: Point) =
        if mapSize.Contains key then
            let offset = getCoordinateOffset key
            let (tile, tileFeature) = chunk.[offset]

            { TileInstance.ID = offset |> TileInstanceID
              Tile = tile
              TileFeature = tileFeature
              Position = key }
        else
            { TileInstance.ID = -1 |> TileInstanceID
              Tile = defaultTile
              TileFeature = None
              Position = key }

    interface Tracked.ITrackedEventHistory<TileMapData> with
        member this.HistoryCallback
            with get () = _trackedRecordEvent
            and set x = _trackedRecordEvent <- x

    interface Tracked.ITrackedEntity<TileMapData> with
        member this.Get = this.TileMapData

        member this.Set(newTileMapData) =
            let oldValue = this.TileMapData

            if oldValue <> newTileMapData then
                mapSize <- Rectangle(0, 0, newTileMapData.Size.X, newTileMapData.Size.Y)
                chunk <- Array.zip newTileMapData.Tiles newTileMapData.TileFeatures
                defaultTile <- newTileMapData.DefaultTile

                _trackedRecordEvent
                    { NewValue = newTileMapData
                      OldValue = oldValue }

        member this.Value
            with get () = this.TileMapData
            and set newTileMapData =

                let oldValue = this.TileMapData

                if oldValue <> newTileMapData then
                    mapSize <- Rectangle(0, 0, newTileMapData.Size.X, newTileMapData.Size.Y)
                    chunk <- Array.zip newTileMapData.Tiles newTileMapData.TileFeatures
                    defaultTile <- newTileMapData.DefaultTile

                    _trackedRecordEvent
                        { NewValue = newTileMapData
                          OldValue = oldValue }


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

    member this.GetCoordinateOffset(point: Point) : int = getCoordinateOffset point

    interface Relational.IReadonlyTable<TileInstance, int> with
        member this.Item
            with get offset =
                let key = translateOffsetToCoordinate offset
                getTileInstanceOrDefault key

        member this.Items = this.Tiles
        member this.TryGetRow(var0) = failwith "todo"


    interface Relational.IFixedTable<TileInstance, int> with
        member this.Update _ row =
            if mapSize.Contains row.Position then
                let offset = this.GetCoordinateOffset row.Position
                chunk.[offset] <- (row.Tile, row.TileFeature)
                ()

        member this.Item
            with get (offset: int): TileInstance =
                let key = translateOffsetToCoordinate offset
                getTileInstanceOrDefault key
            and set key row =
                if mapSize.Contains row.Position then
                    let offset = this.GetCoordinateOffset row.Position
                    chunk.[offset] <- (row.Tile, row.TileFeature)


    member this.Item
        with get (key: Point): TileInstance = getTileInstanceOrDefault key
        and set (key: Point) (tile: Tile, tileFeature: TileFeature option) =
            if mapSize.Contains key then
                chunk.[this.GetCoordinateOffset key] <- (tile, tileFeature)

    member this.Tile
        with get (key: Point): Tile =
            if mapSize.Contains key then
                let (tile, _) = chunk.[this.GetCoordinateOffset key]
                tile
            else
                defaultTile
        and set (key: Point) (tile: Tile) =
            if mapSize.Contains key then
                let (_, tileFeature) = chunk.[this.GetCoordinateOffset key]
                chunk.[this.GetCoordinateOffset key] <- (tile, tileFeature)

    member this.TileFeature
        with get (key: Point): TileFeature option =
            if mapSize.Contains key then
                let (_, tileFeature) = chunk.[this.GetCoordinateOffset key]
                tileFeature
            else
                None
        and set (key: Point) (tileFeature: TileFeature option) =
            if mapSize.Contains key then
                let (tile, _) = chunk.[this.GetCoordinateOffset key]
                chunk.[this.GetCoordinateOffset key] <- (tile, tileFeature)

    member this.Tiles: TileInstance seq =
        chunk
        |> Seq.mapi (fun index (u, v) ->
            { TileInstance.ID = index |> TileInstanceID
              Position = (translateOffsetToCoordinate index)
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
