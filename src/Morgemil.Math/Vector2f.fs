namespace Morgemil.Math

open System

/// <summary>
/// Use this in combination with <see cref="Morgemil.Math.Matrix2f">Matrix2f</see> for floating point positions.
/// </summary>
[<Struct>]
type Vector2f =
    { X: float
      Y: float }

    static member create(x, y) = { Vector2f.X = x; Vector2f.Y = y }

    static member create(scalar) = { Vector2f.X = scalar; Y = scalar }

    //Identity
    static member Identity = Vector2f.create (0.0, 1.0)
    //########## Operator overloads #####################################
    //Addition
    static member (+)(vec1: Vector2f, vec2: Vector2f) =
        Vector2f.create (vec1.X + vec2.X, vec1.Y + vec2.Y)

    static member (+)(vec1: Vector2f, scalar) =
        Vector2f.create (vec1.X + scalar, vec1.Y + scalar)

    static member (+)(scalar: float, vec1: Vector2f) = vec1 + scalar
    //Subtraction
    static member (-)(vec1: Vector2f, vec2: Vector2f) =
        Vector2f.create (vec1.X - vec2.X, vec1.Y - vec2.Y)

    static member (-)(vec1: Vector2f, scalar) =
        Vector2f.create (vec1.X - scalar, vec1.Y - scalar)

    static member (-)(scalar, vec1: Vector2f) =
        Vector2f.create (scalar - vec1.X, scalar - vec1.Y)
    //Multiplication
    static member (*)(vec1: Vector2f, vec2: Vector2f) =
        Vector2f.create (vec1.X * vec2.X, vec1.Y * vec2.Y)

    static member (*)(vec1: Vector2f, scalar) =
        Vector2f.create (vec1.X * scalar, vec1.Y * scalar)

    static member (*)(scalar: float, vec1: Vector2f) = vec1 * scalar
    //Division (Does not guard against divide by zero)
    static member (/)(vec1: Vector2f, vec2: Vector2f) =
        Vector2f.create (vec1.X / vec2.X, vec1.Y / vec2.Y)

    static member (/)(vec1: Vector2f, scalar) =
        Vector2f.create (vec1.X / scalar, vec1.Y / scalar)
    //########## Member methods #########################################
    //Distance
    member this.LengthSquared = (this.X * this.X) + (this.Y * this.Y)

    member this.Length =
        match this.LengthSquared with
        | 0.0 -> 0.0
        | x -> System.Math.Sqrt(x)

    ///The area as though this were a rectangle size
    member this.Area = Math.Abs(this.X * this.Y)

    ///Minimum (x,y) of both elements
    member this.Minimum(vec1: Vector2f) =
        Vector2f.create (Math.Min(this.X, vec1.X), Math.Min(this.Y, vec1.Y))

    ///Maximum (x,y) of both elements
    member this.Maximum(vec1: Vector2f) =
        Vector2f.create (Math.Max(this.X, vec1.X), Math.Max(this.Y, vec1.Y))

    //Normalization
    member this.Normalize() =
        let length = this.Length
        Vector2f.create (this.X / length, this.Y / length)

    member this.IsSame(vec2: Vector2f) =
        Math.Abs(this.X - vec2.X) <= 0.00001 && Math.Abs(this.Y - vec2.Y) <= 0.00001
