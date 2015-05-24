﻿namespace Morgemil.Math

/// <summary>
/// Edge-inclusive. Tile-Aligned
/// </summary>
type Rectangle(position : Vector2i, size : Vector2i) =
  member this.Position = position
  member this.Size = size
  member this.MinCoord = position
  member this.MaxCoord = position + size - 1
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

  ///[y * area.Width + x]
  ///As if accessing an array element in a Rectangle size.
  member this.FlatCoord(pt : Vector2i) = (pt.Y - this.Top) * this.Width + (pt.X - this.Left)

  /// <summary>
  /// A seq of all contained local coordinates
  /// </summary>
  member this.LocalCoordinates =
    seq {
      for y in 0..(this.Height - 1) do
        for x in 0..(this.Width - 1) do
          yield Vector2i(x, y)
    }

  /// <summary>
  /// A seq of all contained global coordinates
  /// </summary>
  member this.Coordinates =
    seq {
      for y in (this.Top)..(this.Bottom) do
        for x in (this.Left)..(this.Right) do
          yield Vector2i(x, y)
    }

  ///Returns the minimum area to enclose both rectangles
  static member (+) (rec1 : Rectangle, rec2 : Rectangle) =
    let minPos = rec1.MinCoord.Minimum(rec2.MinCoord)
    let maxPos = rec1.MaxCoord.Maximum(rec2.MaxCoord)
    Rectangle(minPos, maxPos - minPos + 1)
