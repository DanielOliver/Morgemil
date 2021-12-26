namespace Morgemil.Math

[<Struct>]
type Circle =
    { Center: Vector2i
      Radius: int }

    static member create(center, radius) =
        { Circle.Center = center
          Radius = radius }

    //Identity
    static member Identity = Circle.create (Vector2i.Identity, 1)
    static member Zero = Circle.create (Vector2i.Zero, 0)

    member this.Points: Vector2i seq =
        let radiusSq = this.Radius * this.Radius
        let center = this.Center

        Rectangle
            .create(
                this.Center - this.Radius,
                Vector2i.create (this.Radius * 2 + 1)
            )
            .Coordinates
        |> Seq.filter
            (fun t ->
                (int) ((t - center).LengthSquared - 0.5)
                <= radiusSq)
