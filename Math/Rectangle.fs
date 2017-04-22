namespace Morgemil.Math

open System

/// Edge-inclusive. Axis-aligned bounding box (AABB)
type Rectangle = 
  struct
    val public Position : Vector2i
    val public Size : Vector2i
    
    new(position : Vector2i, size : Vector2i) = 
      { Position = position
        Size = size }    
    new(size : Vector2i) = Rectangle(Vector2i.Zero, size)
    new(x, y, width, height) = Rectangle(Vector2i.create(x, y), Vector2i.create(width, height))

    member this.MinCoord = this.Position
    member this.MaxCoord = this.Position + this.Size - 1
    //Area
    member this.Width = this.Size.X
    member this.Height = this.Size.Y
    member this.Area = this.Width * this.Height
    member this.IsEmpty = this.Size.X = 0 || this.Size.Y = 0
    //Edges
    member this.Left = this.Position.X
    member this.Top = this.Position.Y
    member this.Right = this.Position.X + this.Size.X - 1
    member this.Bottom = this.Position.Y + this.Size.Y - 1
    override this.ToString() = "(" + this.Position.ToString() + "," + this.Size.ToString() + ")"
    
    member this.Corners = 
      [| Vector2i.create(this.Left, this.Top)
         Vector2i.create(this.Right, this.Top)
         Vector2i.create(this.Left, this.Bottom)
         Vector2i.create(this.Right, this.Bottom) |]
    
    member this.Contains(pt : Vector2i) = 
      let diff_pt = pt - this.Position
      diff_pt.X < this.Size.X && diff_pt.Y < this.Size.Y
    
    member this.IsOnEdge(pt : Vector2i) = 
      (not this.IsEmpty) && (pt.X = this.Left || pt.X = this.Right || pt.Y = this.Top || pt.Y = this.Bottom)
    
    ///Expands in every direction
    member this.Expand(scalar) = Rectangle(this.Position - scalar, this.Size + (scalar * 2))
    
    ///Expands 
    member this.Expand(vec : Vector2i) = Rectangle(this.Position - vec, this.Size + (vec * 2))
    
    ///True if any overlap
    member this.Intersects(rect : Rectangle) = 
      not (this.Left > rect.Right || this.Right < rect.Left || this.Top > rect.Bottom || this.Bottom < rect.Top)
    
    ///[y * area.Width + x]
    ///As if accessing an array element in a Rectangle this.Size.
    member this.FlatCoord(pt : Vector2i) = (pt.Y - this.Top) * this.Width + (pt.X - this.Left)
    
    ///Gets a point contained by this rectangle closest to the given point
    member this.GetClosestInsidePoint(pt : Vector2i) = 
      if this.Contains(pt) then pt
      else 
        let x = Math.Min(Math.Max(pt.X, this.Left), this.Right)
        let y = Math.Min(Math.Max(pt.Y, this.Top), this.Bottom)
        Vector2i.create(x, y)
    
    /// <summary>
    /// A seq of all contained local coordinates
    /// </summary>
    member this.LocalCoordinates = 
      let height = this.Height - 1
      let width = this.Width - 1
      seq { 
        for y in 0..height do
          for x in 0..width do
            yield Vector2i.create(x, y)
      }
    
    /// <summary>
    /// A seq of all contained global coordinates
    /// </summary>
    member this.Coordinates = 
      let top = this.Top
      let bottom = this.Bottom
      let left = this.Left
      let right = this.Right
      seq { 
        for y in top..bottom do
          for x in left..right do
            yield Vector2i.create(x, y)
      }
    
    ///Returns the minimum area to enclose both rectangles (union)
    static member (+) (rec1 : Rectangle, rec2 : Rectangle) = 
      let minPos = rec1.MinCoord.Minimum(rec2.MinCoord)
      let maxPos = rec1.MaxCoord.Maximum(rec2.MaxCoord)
      Rectangle(minPos, maxPos - minPos + 1)
    
    ///Returns a rectangle which encloses both points
    static member Enclose (vec1 : Vector2i) (vec2 : Vector2i) = 
      let min = vec1.Minimum vec2
      let max = vec1.Maximum vec2
      Rectangle(min, max - min + 1)
  end
