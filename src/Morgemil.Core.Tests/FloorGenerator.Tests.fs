module Morgemil.Core.Tests.FloorGenerator


open Xunit
open Morgemil.Core
open Morgemil.Models
open Morgemil.Math

let defaultTile: Tile = {
        ID = 1
        Name = "Dungeon Wall"
        TileType = TileType.Solid
        Description = "Dungeon floors are rock, paved cobblestone, and very slipper when bloody."
        BlocksMovement = true
        BlocksSight = true
        Tags = Map.empty
        Representation = {
            AnsiCharacter = '#'
            ForegroundColor = Some <| Color.From(200, 200, 200)
            BackGroundColor = Some <| Color.From(0, 0, 0)
        }
    }

let tile2 = {
    defaultTile with
        Name = "Dungeon Floor"
        BlocksMovement = false
        BlocksSight = false
        ID = 4
}

let floorParameters: FloorGenerationParameter = {
    Strategy = FloorGenerationStrategy.OpenFloor
    Tiles = [|
        defaultTile
        tile2
    |]
    SizeRange = Rectangle.create(10, 10, 15, 15)
    Tags = Map.empty
    DefaultTile = defaultTile
    ID = 5
}

[<Fact>]
let FloorGeneratorTests() =
    let rng = RNG.SeedRNG(50)
    let (tileMap, results) = FloorGenerator.Create floorParameters rng

    Assert.Equal(Vector2i.create(16, 14) |> Rectangle.create, tileMap.MapSize)
    Assert.Equal(defaultTile, tileMap.[ Vector2i.Zero ])
    Assert.Equal(tile2, tileMap.[ Vector2i.Zero + Vector2i.create(1) ])
    Assert.Equal(defaultTile, tileMap.[ tileMap.MapSize.MaxCoord ])
    Assert.Equal(tile2, tileMap.[ tileMap.MapSize.MaxCoord - Vector2i.create(1) ])
    Assert.Equal(defaultTile, tileMap.[ tileMap.MapSize.MinCoord ])
    Assert.Equal(Vector2i.create(1), results.EntranceCoordinate)
