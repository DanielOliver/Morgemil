namespace Morgemil.Core

open Morgemil.Math

type TileModifier = 
  | Entrance of Vector2i
  | Stairs of DungeonParameter
  | ClosedDoor
  | OpenDoor
  
  member this.BlocksMovement = 
    match this with
    | ClosedDoor -> true
    | _ -> false
  
  member this.BlocksSight = 
    match this with
    | ClosedDoor -> true
    | _ -> false
