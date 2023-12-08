module Morgemil.Core.FloorGenerator

open Morgemil.Math
open Morgemil.Models
open Morgemil.Models.Relational
open SadRogue.Primitives

type Results =
    { EntranceCoordinate: Point
      Parameters: FloorGenerationParameter }

let Create
    (parameters: FloorGenerationParameter)
    (tileFeatures: IReadonlyTable<TileFeature, TileFeatureID>)
    (rng: RNG.DefaultRNG)
    : TileMap * Results =
    let floorSize = Rectangle(Point(0, 0), RNG.RandomPoint rng parameters.SizeRange)

    let tileMap: TileMap = TileMap(floorSize, parameters.DefaultTile)

    let subFloorSize = floorSize.Expand(-1, -1)
    let mutable entraceCoordinate = Point.Zero

    match parameters.Strategy with
    | FloorGenerationStrategy.OpenFloor ->
        let openFloorTile =
            parameters.Tiles |> Seq.tryFind (fun t -> not t.BlocksMovement)

        match openFloorTile with
        | Some(tile1: Tile) ->
            subFloorSize.Positions()
            |> Seq.iter (fun (vec2: Point) -> tileMap.Tile vec2 <- tile1)

            entraceCoordinate <- subFloorSize.MinExtent
            let exitCoordinate = subFloorSize.MaxExtent

            let entrancePointFeature =
                tileFeatures
                |> TileFeatureTable.GetFeaturesForTile tile1.ID
                |> Seq.find (_.EntryPoint)

            let exitPointFeature =
                tileFeatures
                |> TileFeatureTable.GetFeaturesForTile tile1.ID
                |> Seq.find (_.ExitPoint)

            tileMap.TileFeature entraceCoordinate <- Some entrancePointFeature
            tileMap.TileFeature exitCoordinate <- Some exitPointFeature

        | None -> ()

    let results: Results =
        { EntranceCoordinate = entraceCoordinate
          Parameters = parameters }

    tileMap, results
