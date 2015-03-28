namespace Morgemil.Math

/// <summary>
/// Edge-inclusive. Tile-Aligned
/// </summary>
type Rectangle(position : Vector2i, size : Vector2i) = 
  member this.Position = position
  member this.Size = size
  member this.X = position.X
  member this.Y = position.Y
  member this.Width = size.X
  member this.Height = size.Y
  member this.Area = this.Width * this.Height
  member this.Contains(pt : Vector2i) = 
    let diff_pt = pt - position
    diff_pt.X < size.X && diff_pt.Y < size.Y
