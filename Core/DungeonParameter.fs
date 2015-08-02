namespace Morgemil.Map

type DungeonGenerationType = 
  | Square
  | BSP

type DungeonParameter = 
  { Type : DungeonGenerationType
    Depth : int
    RngSeed : int }
