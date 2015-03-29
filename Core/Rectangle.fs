namespace Morgemil.Math

/// <summary>
/// Edge-inclusive. Tile-Aligned
/// </summary>
type Rectangle(position : Vector2i, size : Vector2i) = 
  member this.Position = position
  member this.Size = size
  member this.PosX = position.X
  member this.PosY = position.Y
  member this.Width = size.X
  member this.Height = size.Y
  member this.Area = this.Width * this.Height
  member this.Left = position.X
  member this.Top = position.Y
  member this.Right = position.X + size.X - 1
  member this.Bottom = position.Y + size.Y - 1
  override this.ToString() = "(" + this.Position.ToString() + "," + this.Size.ToString() + ")"
  
  member this.Contains(pt : Vector2i) = 
    let diff_pt = pt - position
    diff_pt.X < size.X && diff_pt.Y < size.Y
  
  /// <summary>
  /// A seq of all Vector2i contained within this Rectangle.
  /// </summary>
  member this.Coordinates = 
    seq { 
      for y in 0..(this.Height - 1) do
        for x in 0..(this.Width - 1) do
          yield Vector2i(this.PosX + x, this.PosY + y)
    }
