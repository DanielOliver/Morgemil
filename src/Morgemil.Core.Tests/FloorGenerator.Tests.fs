module Morgemil.Core.Tests.FloorGenerator


open Xunit
open Morgemil.Core
open Morgemil.Models
open Morgemil.Math

let defaultTile: Tile =
    { ID = TileID 3L
      Name = "Dungeon Wall"
      TileType = TileType.Solid
      Description = "Dungeon floors are rock, paved cobblestone, and very slipper when bloody."
      BlocksMovement = true
      BlocksSight = true
      Representation =
        { AnsiCharacter = '#'
          ForegroundColor = Some <| Color.From(200, 200, 200)
          BackGroundColor = Some <| Color.From(0, 0, 0) } }

let tile2 =
    { defaultTile with
        Name = "Dungeon Floor"
        BlocksMovement = false
        BlocksSight = false
        ID = TileID 4L }

let floorParameters: FloorGenerationParameter =
    { Strategy = FloorGenerationStrategy.OpenFloor
      Tiles = [ defaultTile; tile2 ]
      SizeRange = SadRogue.Primitives.Rectangle(10, 10, 15, 15)
      DefaultTile = defaultTile
      ID = FloorGenerationParameterID 5L }

let stairTileFeature: TileFeature =
    { ID = TileFeatureID 2L
      Name = "Stairs down"
      Description = "Stairs down"
      BlocksMovement = false
      BlocksSight = false
      Representation =
        { AnsiCharacter = char (242)
          ForegroundColor = Some <| Color.From(30, 30, 255)
          BackGroundColor = Some <| Color.From(0, 240, 0, 50) }
      PossibleTiles = [ tile2 ]
      ExitPoint = true
      EntryPoint = false }

let startingPointFeature: TileFeature =
    { ID = TileFeatureID 1L
      Name = "Starting point"
      Description = "Starting point"
      BlocksMovement = false
      BlocksSight = false
      Representation =
        { AnsiCharacter = '@'
          ForegroundColor = Some <| Color.From(0)
          BackGroundColor = None }
      PossibleTiles = [ tile2 ]
      ExitPoint = false
      EntryPoint = true }

[<Fact>]
let FloorGeneratorTests () =
    let rng = RNG.SeedRNG(50)

    let tileFeatureTable =
        Morgemil.Core.TileFeatureTable([ stairTileFeature; startingPointFeature ])

    let (tileMap, results) = FloorGenerator.Create floorParameters tileFeatureTable rng

    Assert.Equal((17, 15) |> Rectangle.create, tileMap.MapSize)
    Assert.Equal(defaultTile, tileMap.Tile Point.Zero)
    Assert.Equal(tile2, tileMap.Tile(Point.create (1, 1)))
    Assert.Equal(defaultTile, tileMap.Tile tileMap.MapSize.MaxExtent)
    Assert.Equal(tile2, tileMap.Tile(tileMap.MapSize.MaxExtent.Translate(-1, -1)))
    Assert.Equal(defaultTile, tileMap.Tile tileMap.MapSize.MinExtent)
    Assert.Equal(Point.create (1, 1), results.EntranceCoordinate)
