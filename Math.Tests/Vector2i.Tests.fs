module Morgemil.Math.Vector2i.Tests


open NUnit.Framework
open Morgemil.Math

let vec1 = Vector2i(5, -5)
let vec2 = Vector2i(-4, 2)


[<Test>]
let ``Test Vector2i Addition``() = 
  Assert.AreEqual(Vector2i(1, -3), vec1 + vec2)
  Assert.AreEqual(Vector2i(7, -3), vec1 + 2)
  Assert.AreEqual(Vector2i(7, -3), 2 + vec1)
  
[<Test>]
let ``Test Vector2i Subtraction``() = 
  Assert.AreEqual(Vector2i(9, -7), vec1 - vec2)
  Assert.AreEqual(Vector2i(3, -7), vec1 - 2)
  Assert.AreEqual(Vector2i(-3, 7), 2 - vec1)
  
[<Test>]
let ``Test Vector2i Multiplication``() = 
  Assert.AreEqual(Vector2i(-20, -10), vec1 * vec2)
  Assert.AreEqual(Vector2i(10, -10), vec1 * 2)
  Assert.AreEqual(Vector2i(10, -10), 2 * vec1)
  
[<Test>]
let ``Test Vector2i Division``() = 
  Assert.AreEqual(Vector2i(-1, -2), vec1 / vec2)
  Assert.AreEqual(Vector2i(2, -2), vec1 / 2)
  
[<Test>]
let ``Test Vector2i Methods``() = 
  Assert.AreEqual(50.0, vec1.LengthSquared, 0.0001)
  Assert.AreEqual(7.07, vec1.Length, 0.01)
  Assert.AreEqual(25, vec1.Area)
  Assert.AreEqual(Vector2i(-4, -5), vec1.Minimum vec2)
  Assert.AreEqual(Vector2i(5, 2), vec1.Maximum vec2)


