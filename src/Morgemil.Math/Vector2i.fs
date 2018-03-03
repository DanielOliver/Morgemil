namespace Morgemil.Math

open System

[<Struct>]
type Vector2i = 
  { X: int
    Y: int
  }

  static member create(x, y) =
    { Vector2i.X = x
      Y = y
    }

  static member create(scalar) =
    { Vector2i.X = scalar
      Y = scalar
    }
    
  //Identity
  static member Identity = Vector2i.create(0, 1)
  static member Zero = Vector2i.create(0)
  //########## Operator overloads #####################################
  //Addition
  static member (+) (vec1 : Vector2i, vec2 : Vector2i) = Vector2i.create(vec1.X + vec2.X, vec1.Y + vec2.Y)
  static member (+) (vec1 : Vector2i, scalar) = Vector2i.create(vec1.X + scalar, vec1.Y + scalar)
  static member (+) (scalar : int, vec1 : Vector2i) = vec1 + scalar
  //Subtraction
  static member (-) (vec1 : Vector2i, vec2 : Vector2i) = Vector2i.create(vec1.X - vec2.X, vec1.Y - vec2.Y)
  static member (-) (vec1 : Vector2i, scalar) = Vector2i.create(vec1.X - scalar, vec1.Y - scalar)
  static member (-) (scalar, vec1 : Vector2i) = Vector2i.create(scalar - vec1.X, scalar - vec1.Y)
  //Multiplication
  static member (*) (vec1 : Vector2i, vec2 : Vector2i) = Vector2i.create(vec1.X * vec2.X, vec1.Y * vec2.Y)
  static member (*) (vec1 : Vector2i, scalar) = Vector2i.create(vec1.X * scalar, vec1.Y * scalar)
  static member (*) (scalar : int, vec1 : Vector2i) = vec1 * scalar
  //Division (Does not guard against divide by zero)
  static member (/) (vec1 : Vector2i, vec2 : Vector2i) = Vector2i.create(vec1.X / vec2.X, vec1.Y / vec2.Y)
  static member (/) (vec1 : Vector2i, scalar) = Vector2i.create(vec1.X / scalar, vec1.Y / scalar)
  //########## Member methods #########################################
  //Distance
  member this.LengthSquared = float ((this.X * this.X) + (this.Y * this.Y))
    
  member this.Length = 
    match this.LengthSquared with
    | 0.0 -> 0.0
    | x -> Math.Sqrt(x)

  ///The area as though this were a rectangle size
  member this.Area = Math.Abs(this.X * this.Y)
    
  ///Minimum (x,y) of both elements
  member this.Minimum(vec1 : Vector2i) = Vector2i.create(Math.Min(this.X, vec1.X), Math.Min(this.Y, vec1.Y))
    
  ///Maximum (x,y) of both elements
  member this.Maximum(vec1 : Vector2i) = Vector2i.create(Math.Max(this.X, vec1.X), Math.Max(this.Y, vec1.Y))

