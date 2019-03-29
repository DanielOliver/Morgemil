module Morgemil.Math.Tests.Circle

open Xunit
open Morgemil.Math

[<Fact>]
let ``Test Circle Properties``() =
    let size0Circle = Circle.create(Vector2i.Zero, 0)
    Assert.Equal(1, size0Circle.Points |> Seq.length)

    let size1Circle = Circle.create(Vector2i.Zero, 1)
    Assert.Equal(9, size1Circle.Points |> Seq.length)
    
    let size2Circle = Circle.create(Vector2i.Zero, 2)
    Assert.Equal(21, size2Circle.Points |> Seq.length)
    
    let size3Circle = Circle.create(Vector2i.Zero, 3)
    Assert.Equal(37, size3Circle.Points |> Seq.length)
