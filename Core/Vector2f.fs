namespace Morgemil.Math

/// <summary>
/// Use this in combination with <see cref="Morgemil.Math.Vector2f">Matrix2f</see> for floating point positions.
/// </summary>
type Vector2f = 
  struct
    val public X : float
    val public Y : float
    
    new(x, y) = 
      { X = x
        Y = y }
    
    //Identity
    static member Identity = Vector2f(0.0, 1.0)
    //########## Operator overloads #####################################
    //Addition    
    static member (+) (vec1 : Vector2f, vec2 : Vector2f) = 
      Vector2f(vec1.X + vec2.X, vec1.Y + vec2.Y)
    static member (+) (vec1 : Vector2f, scalar) = Vector2f(vec1.X + scalar, vec1.Y + scalar)
    static member (+) (scalar : float, vec1 : Vector2f) = vec1 + scalar
    //Subtraction
    static member (-) (vec1 : Vector2f, vec2 : Vector2f) = 
      Vector2f(vec1.X - vec2.X, vec1.Y - vec2.Y)
    static member (-) (vec1 : Vector2f, scalar) = Vector2f(vec1.X - scalar, vec1.Y - scalar)
    static member (-) (scalar, vec1 : Vector2f) = Vector2f(scalar - vec1.X, scalar - vec1.Y)
    //Multiplication
    static member (*) (vec1 : Vector2f, vec2 : Vector2f) = 
      Vector2f(vec1.X * vec2.X, vec1.Y * vec2.Y)
    static member (*) (vec1 : Vector2f, scalar) = Vector2f(vec1.X * scalar, vec1.Y * scalar)
    static member (*) (scalar : float, vec1 : Vector2f) = vec1 * scalar
    //Division (Does not guard against divide by zero)
    static member (/) (vec1 : Vector2f, vec2 : Vector2f) = 
      Vector2f(vec1.X / vec2.X, vec1.Y / vec2.Y)
    static member (/) (vec1 : Vector2f, scalar) = Vector2f(vec1.X / scalar, vec1.Y * scalar)
    //########## Member methods #########################################
    //Distance
    member this.LengthSquared() = (this.X * this.X) + (this.Y + this.Y)
    member this.Length() = System.Math.Sqrt(this.LengthSquared())
    //Normalization
    member this.Normalize() = 
      let length = this.Length()
      Vector2f(this.X / length, this.Y / length)
  end
