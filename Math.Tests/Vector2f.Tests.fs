module Morgemil.Math.Vector2f.Tests


open NUnit.Framework
open Morgemil.Math

let vec1 = Vector2f(5.0, -5.0)
let vec2 = Vector2f(-4.0, 2.0)

[<Test>]
let ``Test Vector2f Addition``() = 
  Assert.AreEqual(Vector2f(1.0, -3.0), vec1 + vec2)
  Assert.AreEqual(Vector2f(7.0, -3.0), vec1 + 2.0)
  Assert.AreEqual(Vector2f(7.0, -3.0), 2.0 + vec1)
  
[<Test>]
let ``Test Vector2f Subtraction``() = 
  Assert.AreEqual(Vector2f(9.0, -7.0), vec1 - vec2)
  Assert.AreEqual(Vector2f(3.0, -7.0), vec1 - 2.0)
  Assert.AreEqual(Vector2f(-3.0, 7.0), 2.0 - vec1)
  
[<Test>]
let ``Test Vector2f Multiplication``() = 
  Assert.AreEqual(Vector2f(-20.0, -10.0), vec1 * vec2)
  Assert.AreEqual(Vector2f(10.0, -10.0), vec1 * 2.0)
  Assert.AreEqual(Vector2f(10.0, -10.0), 2.0 * vec1)
  
[<Test>]
let ``Test Vector2f Division``() = 
  Assert.AreEqual(Vector2f(-1.0, -2.0), vec1 / vec2)
  Assert.AreEqual(Vector2f(2.5, -2.5), vec1 / 2.0)
  
[<Test>]
let ``Test Vector2f Methods``() = 
  Assert.AreEqual(50.0, vec1.LengthSquared, 0.0001)
  Assert.AreEqual(7.07, vec1.Length, 0.01)
  Assert.AreEqual(25, vec1.Area)
  Assert.AreEqual(Vector2f(-4.0, -5.0), vec1.Minimum vec2)
  Assert.AreEqual(Vector2f(5.0, 2.0), vec1.Maximum vec2)
