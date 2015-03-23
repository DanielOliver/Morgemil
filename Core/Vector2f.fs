module Math.Vector2

type Vector2f = 
  struct
    val public X : float
    val public Y : float
    
    new(x, y) = 
      { X = x
        Y = y }
    
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
  end
