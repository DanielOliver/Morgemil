namespace Morgemil.Math.Vector2

type Vector2i = 
  struct
    val public X : int
    val public Y : int
    
    new(x, y) = 
      { X = x
        Y = y }
    
    //Identity
    static member Identity = Vector2i(0, 1)
    //########## Operator overloads #####################################
    //Addition    
    static member (+) (vec1 : Vector2i, vec2 : Vector2i) = 
      Vector2i(vec1.X + vec2.X, vec1.Y + vec2.Y)
    static member (+) (vec1 : Vector2i, scalar) = Vector2i(vec1.X + scalar, vec1.Y + scalar)
    static member (+) (scalar : int, vec1 : Vector2i) = vec1 + scalar
    //Subtraction
    static member (-) (vec1 : Vector2i, vec2 : Vector2i) = 
      Vector2i(vec1.X - vec2.X, vec1.Y - vec2.Y)
    static member (-) (vec1 : Vector2i, scalar) = Vector2i(vec1.X - scalar, vec1.Y - scalar)
    static member (-) (scalar, vec1 : Vector2i) = Vector2i(scalar - vec1.X, scalar - vec1.Y)
    //Multiplication
    static member (*) (vec1 : Vector2i, vec2 : Vector2i) = 
      Vector2i(vec1.X * vec2.X, vec1.Y * vec2.Y)
    static member (*) (vec1 : Vector2i, scalar) = Vector2i(vec1.X * scalar, vec1.Y * scalar)
    static member (*) (scalar : int, vec1 : Vector2i) = vec1 * scalar
    //Division (Does not guard against divide by zero)
    static member (/) (vec1 : Vector2i, vec2 : Vector2i) = 
      Vector2i(vec1.X / vec2.X, vec1.Y / vec2.Y)
    static member (/) (vec1 : Vector2i, scalar) = Vector2i(vec1.X / scalar, vec1.Y * scalar)
    //########## Member methods #########################################
    //Distance
    member this.LengthSquared() = float ((this.X * this.X) + (this.Y + this.Y))
    member this.Length() = System.Math.Sqrt(this.LengthSquared())
  end
