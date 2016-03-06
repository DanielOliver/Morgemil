namespace Morgemil.Core

open Morgemil.Math

type DungeonGenerationType = 
  | Square
  | BSP

type DungeonParameter = 
  { Type : DungeonGenerationType
    Depth : int
    RngSeed : int }
