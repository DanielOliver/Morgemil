module Morgemil.Map.Tiles

let DungeonFloor =
  TileDefinition(1, "Floor", "Dungeon floors are often trapped", false, false, TileType.Land)
let DungeonWall =
  TileDefinition
    (2, "Wall", "Dungeon walls are built from the prisoner's bones", true, true, TileType.Land)
