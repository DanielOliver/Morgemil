module Morgemil.Map.Tiles

let DungeonFloor = 
  { Id = 1
    Name = "Floor"
    Description = "Dungeon floors are often trapped"
    BlocksMovement = false
    BlocksSight = false
    TileType = TileType.Land }

let DungeonWall = 
  { Id = 2
    Name = "Wall"
    Description = "Dungeon halls are long and misleading"
    BlocksMovement = true
    BlocksSight = true
    TileType = TileType.Land }

let DungeonCorridor = 
  { Id = 3
    Name = "Corridor"
    Description = "Dungeon halls are long and misleading"
    BlocksMovement = false
    BlocksSight = false
    TileType = TileType.Land }
