module Morgemil.Core.Tests.TileMap

open Xunit
open Morgemil.Core
open Morgemil.Models
open Morgemil.Math

let defaultTile: Tile =
    { ID = TileID 1L
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

let fromTileInstance (t: TileInstance) = t.Position, t.Tile, t.TileFeature

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
      PossibleTiles = [ defaultTile; tile2 ]
      EntryPoint = false
      ExitPoint = true }

[<Fact>]
let TileMapTests () =
    let mapSize = Rectangle(Vector2i(0, 0), Vector2i(10, 10))
    let tileMap = TileMap(mapSize, defaultTile)

    Assert.Equal(mapSize, tileMap.MapSize)
    tileMap.Tile(Point.Zero) <- tile2

    Assert.Equal(false, (tileMap.Tile Point.Zero).BlocksMovement)

    Assert.Equal(true, (tileMap.Tile(Point.Zero - Point.Identity)).BlocksMovement)

    tileMap.Tile(Point.Zero - Point.Identity) <- tile2

    Assert.Equal(true, (tileMap.Tile(Point.Zero - Point.Identity)).BlocksMovement)

    let (coord1, firstTile, tileFeature) =
        tileMap.Tiles |> Seq.map fromTileInstance |> Seq.head

    Assert.Equal(Point.Zero, coord1)
    Assert.Equal(tile2, firstTile)
    Assert.Equal(None, tileFeature)

    tileMap.Item(Point.Zero - Point.Identity) <- (tile2, tileFeature)

    let (_, tile1, feature1) =
        tileMap.Item(Point.Zero - Point.Identity) |> fromTileInstance

    Assert.Equal(defaultTile, tile1)
    Assert.True(feature1.IsNone)

    let (coord2, secondTile, tileFeature) =
        tileMap.Tiles |> Seq.skip (1) |> Seq.head |> fromTileInstance

    Assert.Equal(Point.create (1, 0), coord2)
    Assert.Equal(defaultTile, secondTile)
    Assert.Equal(None, tileFeature)

    Assert.Equal(mapSize.Area, tileMap.Tiles |> Seq.length)
