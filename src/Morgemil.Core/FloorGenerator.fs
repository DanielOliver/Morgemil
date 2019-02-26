module Morgemil.Core.FloorGenerator

open Morgemil.Math
open Morgemil.Models

type Results = {
    EntranceCoordinate: Vector2i
    Parameters: FloorGenerationParameter
}

let Create (parameters: FloorGenerationParameter) (rng: RNG.DefaultRNG) =
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
        | None -> ()

    let results: Results = {
        EntranceCoordinate = entraceCoordinate
        Parameters = parameters
    }

    tileMap, results
