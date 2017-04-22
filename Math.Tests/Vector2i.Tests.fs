module Morgemil.Math.Vector2i.create.Tests


open NUnit.Framework
open Morgemil.Math

let vec1 = Vector2i.create(5, -5)
let vec2 = Vector2i.create(-4, 2)


[<Test>]
let ``Test Vector2i.create Addition``() = 
  Assert.AreEqual(Vector2i.create(1, -3), vec1 + vec2)
  Assert.AreEqual(Vector2i.create(7, -3), vec1 + 2)
  Assert.AreEqual(Vector2i.create(7, -3), 2 + vec1)
  
[<Test>]
let ``Test Vector2i.create Subtraction``() = 
  Assert.AreEqual(Vector2i.create(9, -7), vec1 - vec2)
  Assert.AreEqual(Vector2i.create(3, -7), vec1 - 2)
  Assert.AreEqual(Vector2i.create(-3, 7), 2 - vec1)
  
[<Test>]
let ``Test Vector2i.create Multiplication``() = 
  Assert.AreEqual(Vector2i.create(-20, -10), vec1 * vec2)
  Assert.AreEqual(Vector2i.create(10, -10), vec1 * 2)
  Assert.AreEqual(Vector2i.create(10, -10), 2 * vec1)
  
[<Test>]
let ``Test Vector2i.create Division``() = 
  Assert.AreEqual(Vector2i.create(-1, -2), vec1 / vec2)
  Assert.AreEqual(Vector2i.create(2, -2), vec1 / 2)
  
[<Test>]
let ``Test Vector2i.create Methods``() = 
  Assert.AreEqual(50.0, vec1.LengthSquared, 0.0001)
  Assert.AreEqual(7.07, vec1.Length, 0.01)
  Assert.AreEqual(25, vec1.Area)
  Assert.AreEqual(Vector2i.create(-4, -5), vec1.Minimum vec2)
  Assert.AreEqual(Vector2i.create(5, 2), vec1.Maximum vec2)


