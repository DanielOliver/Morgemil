module Morgemil.Core.FloorGenerator

open Morgemil.Math
open Morgemil.Models

type Results = {
    EntranceCoordinate: Vector2i
}

let Create (parameters: FloorGenerationParameter) (rng: RNG.DefaultRNG) =
    let floorSize = Rectangle.create (RNG.RandomPoint rng parameters.SizeRange)
    let tileMap = TileMap(floorSize, parameters.DefaultTile)
    let subFloorSize = floorSize.Expand(-1)
    let mutable entraceCoordinate = Vector2i.Zero
    
    match parameters.Strategy with
    | FloorGenerationStrategy.OpenFloor ->
        let openFloorTile = parameters.Tiles |> Seq.tryFind(fun t -> not(t.BlocksMovement))
        match openFloorTile with
        | Some(tile) ->
            subFloorSize.Coordinates 
            |> Seq.iter(fun t -> tileMap.[t] <- tile )
            entraceCoordinate <- subFloorSize.MinCoord
        | None -> ()
    | _ -> ()

    let results: Results = {
        EntranceCoordinate = entraceCoordinate
    }

    tileMap, results
