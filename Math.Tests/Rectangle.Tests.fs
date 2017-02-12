module Morgemil.Math.Rectangle.Tests

open NUnit.Framework
open Morgemil.Math

let rect1 = Rectangle(Vector2i(-5, 5), Vector2i(10, 4))

[<Test>]
let ``Test Rectangle Properties``() =
  Assert.AreEqual(Vector2i(-5,5), rect1.MinCoord)
  Assert.AreEqual(Vector2i(4,8), rect1.MaxCoord)
  Assert.AreEqual(-5, rect1.Left)
  Assert.AreEqual(5, rect1.Top)
  Assert.AreEqual(4, rect1.Right)
  Assert.AreEqual(8, rect1.Bottom)

[<Test>]
let ``Test Rectangle Contains``() =
  Assert.IsTrue(rect1.Contains(Vector2i(-4, 6)))
  Assert.IsFalse(rect1.Contains(Vector2i(5, 8)))
  
[<Test>]
let ``Test Rectangle IsOnEdge``() =
  Assert.IsTrue(rect1.IsOnEdge(Vector2i(-5, 5)))
  Assert.IsFalse(rect1.IsOnEdge(Vector2i(-4, 6)))
  
[<Test>]
let ``Test Rectangle Expand``() =
  Assert.AreEqual(Rectangle(-6, 4, 12, 6), rect1.Expand 1)
  Assert.AreEqual(Rectangle(-6, 4, 12, 6), rect1.Expand(Vector2i(1)))
  
[<Test>]
let ``Test Rectangle Intersect``() =
  Assert.IsTrue(rect1.Intersects( Rectangle(-6, 4, 12, 6)))
  Assert.IsFalse(rect1.Intersects( Rectangle(5, 8, 1, 1)))
  Assert.IsTrue(rect1.Intersects( Rectangle(-5, 5, 1, 1)))


