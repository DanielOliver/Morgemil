module Morgemil.Core.Tests.TileMap

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

[<Fact>]
let TileMapTests() =
    let mapSize = Rectangle.create(Vector2i.create(10))
    let tileMap = TileMap(mapSize, defaultTile)
    
    Assert.Equal(mapSize, tileMap.MapSize)
    tileMap.[Vector2i.Zero] <- tile2

    Assert.Equal(false, tileMap.[Vector2i.Zero].BlocksMovement)
    Assert.Equal(true, tileMap.[Vector2i.Zero - Vector2i.Identity].BlocksMovement)

    tileMap.[Vector2i.Zero - Vector2i.Identity] <- tile2
    Assert.Equal(true, tileMap.[Vector2i.Zero - Vector2i.Identity].BlocksMovement)

    let (coord1, firstTile) = tileMap.Tiles |> Seq.head
    Assert.Equal(Vector2i.Zero, coord1)
    Assert.Equal(tile2, firstTile)
        
    let (coord2, secondTile) = tileMap.Tiles |> Seq.skip(1) |> Seq.head
    Assert.Equal(Vector2i.create(1, 0), coord2)
    Assert.Equal(defaultTile, secondTile)

    Assert.Equal(mapSize.Area, tileMap.Tiles |> Seq.length)
