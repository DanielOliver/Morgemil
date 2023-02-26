module Morgemil.Math.Tests.Rectangle

open Xunit
open Morgemil.Math

let rect1 = Rectangle.create (Vector2i.create (-5, 5), Vector2i.create (10, 4))

[<Fact>]
let ``Test Rectangle Properties`` () =
    Assert.Equal(Vector2i.create (-5, 5), rect1.MinCoord)
    Assert.Equal(Vector2i.create (4, 8), rect1.MaxCoord)
    Assert.Equal(-5, rect1.Left)
    Assert.Equal(5, rect1.Top)
    Assert.Equal(4, rect1.Right)
    Assert.Equal(8, rect1.Bottom)
    Assert.Equal(40, rect1.Area)

[<Fact>]
let ``Test Rectangle Contains`` () =
    Assert.True(rect1.Contains(Vector2i.create (-4, 6)))
    Assert.False(rect1.Contains(Vector2i.create (5, 8)))
    Assert.False(rect1.Contains(rect1.MinCoord - Vector2i.Identity))

[<Fact>]
let ``Test Rectangle IsOnEdge`` () =
    Assert.True(rect1.IsOnEdge(Vector2i.create (-5, 5)))
    Assert.False(rect1.IsOnEdge(Vector2i.create (-4, 6)))

[<Fact>]
let ``Test Rectangle Expand`` () =
    Assert.Equal(Rectangle.create (-6, 4, 12, 6), rect1.Expand 1)
    Assert.Equal(Rectangle.create (-6, 4, 12, 6), rect1.Expand(Vector2i.create (1)))

[<Fact>]
let ``Test Rectangle Intersect`` () =
    Assert.True(rect1.Intersects(Rectangle.create (-6, 4, 12, 6)))
    Assert.False(rect1.Intersects(Rectangle.create (5, 8, 1, 1)))
    Assert.True(rect1.Intersects(Rectangle.create (-5, 5, 1, 1)))
