module Morgemil.Math.Tests.Vector2i


open Xunit
open Morgemil.Math

let vec1 = Vector2i.create (5, -5)
let vec2 = Vector2i.create (-4, 2)


[<Fact>]
let ``Test Vector2i Addition`` () =
    Assert.Equal(Vector2i.create (1, -3), vec1 + vec2)
    Assert.Equal(Vector2i.create (7, -3), vec1 + 2)
    Assert.Equal(Vector2i.create (7, -3), 2 + vec1)

[<Fact>]
let ``Test Vector2i Subtraction`` () =
    Assert.Equal(Vector2i.create (9, -7), vec1 - vec2)
    Assert.Equal(Vector2i.create (3, -7), vec1 - 2)
    Assert.Equal(Vector2i.create (-3, 7), 2 - vec1)

[<Fact>]
let ``Test Vector2i Multiplication`` () =
    Assert.Equal(Vector2i.create (-20, -10), vec1 * vec2)
    Assert.Equal(Vector2i.create (10, -10), vec1 * 2)
    Assert.Equal(Vector2i.create (10, -10), 2 * vec1)

[<Fact>]
let ``Test Vector2i Division`` () =
    Assert.Equal(Vector2i.create (-1, -2), vec1 / vec2)
    Assert.Equal(Vector2i.create (2, -2), vec1 / 2)

[<Fact>]
let ``Test Vector2i Methods`` () =
    Assert.Equal(50.0, vec1.LengthSquared, 1)
    Assert.Equal(7.07, vec1.Length, 1)
    Assert.Equal(25, vec1.Area)
    Assert.Equal(Vector2i.create (-4, -5), vec1.Minimum vec2)
    Assert.Equal(Vector2i.create (5, 2), vec1.Maximum vec2)
