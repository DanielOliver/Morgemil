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
  
  /// <summary>
  /// A seq of all Vector2i contained within this Rectangle.
  /// </summary>
  member this.Coordinates = 
    seq { 
      for y in this.Y..(this.Y + this.Height - 1) do
        for x in this.X..(this.X + this.Width - 1) do
          yield Vector2i(x, y)
    }
