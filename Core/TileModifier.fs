namespace Morgemil.Core

type TileModifier = 
  | Stairs of Stairs
  | Entrance of Location : Morgemil.Math.Rectangle
