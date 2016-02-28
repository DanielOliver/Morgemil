namespace Morgemil.Core


type Stairs = 
  { DungeonParameter : DungeonParameter
    Area : Morgemil.Math.Rectangle }


type TileModifier = 
  | Stairs of Stairs
  | Entrance of Location : Morgemil.Math.Rectangle
