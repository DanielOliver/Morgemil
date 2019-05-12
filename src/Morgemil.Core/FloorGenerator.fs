module Morgemil.Core.FloorGenerator

open Morgemil.Math
open Morgemil.Models
open Morgemil.Models.Relational

type Results = {
    EntranceCoordinate: Vector2i
    Parameters: FloorGenerationParameter
}

let Create (parameters: FloorGenerationParameter) (tileFeatures: IReadonlyTable<TileFeature, TileFeatureID>) (rng: RNG.DefaultRNG) =
    let floorSize = Rectangle.create (RNG.RandomPoint rng parameters.SizeRange)
    let tileMap: TileMap = TileMap(floorSize, parameters.DefaultTile)
    let subFloorSize = floorSize.Expand(-1)
    let mutable entraceCoordinate = Vector2i.Zero
    
    match parameters.Strategy with
    | FloorGenerationStrategy.OpenFloor ->
        let openFloorTile = parameters.Tiles |> Seq.tryFind(fun t -> not(t.BlocksMovement))
        match openFloorTile with
        | Some(tile1: Tile) ->
            subFloorSize.Coordinates 
            |> Seq.iter(fun (vec2: Vector2i) -> tileMap.Tile(vec2) <- tile1 )
            entraceCoordinate <- subFloorSize.MinCoord
            let exitCoordinate  = subFloorSize.MaxCoord
            let entrancePointFeature = tileFeatures |> TileFeatureTable.GetFeaturesForTile tile1.ID |> Seq.find(fun t -> t.EntryPoint)
            let exitPointFeature = tileFeatures |> TileFeatureTable.GetFeaturesForTile tile1.ID |> Seq.find(fun t -> t.ExitPoint)
            
            tileMap.TileFeature(entraceCoordinate) <- Some entrancePointFeature
            tileMap.TileFeature(exitCoordinate) <- Some exitPointFeature

        | None -> ()

    let results: Results = {
        EntranceCoordinate = entraceCoordinate
        Parameters = parameters
    }

    tileMap, results
