namespace Morgemil.Map

open Morgemil.Math.Vector2

type Chunk(area : Morgemil.Math.Shapes.Box) = 
  member this.Box = area
  member this.Tiles = Array.init area.Area (fun x -> TileDefinition.Default)
