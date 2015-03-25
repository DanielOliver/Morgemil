namespace Morgemil.Math.Shapes

open Morgemil.Math.Vector2

//Edge-inclusive
type Box(position : Vector2i, size : Vector2i) = 
  member this.Position = position
  member this.Size = size
  
  member this.Contains(pt : Vector2i) = 
    let diff_pt = pt - position
    diff_pt.X <= size.X && diff_pt.Y <= size.Y
  
  member this.Area = 
    let diff_pt = size - position + 1
    diff_pt.X * diff_pt.Y
