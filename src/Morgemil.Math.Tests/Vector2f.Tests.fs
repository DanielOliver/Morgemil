module Morgemil.Math.Tests.Vector2f


open Xunit
open Morgemil.Math

let vec1 = Vector2f.create(5.0, -5.0)
let vec2 = Vector2f.create(-4.0, 2.0)

[<Fact>]
let ``Test Vector2f Addition``() = 
  Assert.Equal(Vector2f.create(1.0, -3.0), vec1 + vec2)
  Assert.Equal(Vector2f.create(7.0, -3.0), vec1 + 2.0)
  Assert.Equal(Vector2f.create(7.0, -3.0), 2.0 + vec1)
  
[<Fact>]
let ``Test Vector2f Subtraction``() = 
  Assert.Equal(Vector2f.create(9.0, -7.0), vec1 - vec2)
  Assert.Equal(Vector2f.create(3.0, -7.0), vec1 - 2.0)
  Assert.Equal(Vector2f.create(-3.0, 7.0), 2.0 - vec1)
  
[<Fact>]
let ``Test Vector2f Multiplication``() = 
  Assert.Equal(Vector2f.create(-20.0, -10.0), vec1 * vec2)
  Assert.Equal(Vector2f.create(10.0, -10.0), vec1 * 2.0)
  Assert.Equal(Vector2f.create(10.0, -10.0), 2.0 * vec1)
  
[<Fact>]
let ``Test Vector2f Division``() = 
  Assert.Equal(Vector2f.create(-1.25, -2.5), vec1 / vec2)
  Assert.Equal(Vector2f.create(2.5, -2.5),vec1 / 2.0)
  
[<Fact>]
let ``Test Vector2f Methods``() = 
  Assert.Equal(50.0, vec1.LengthSquared, 1)
  Assert.Equal(7.07, vec1.Length, 1)
  Assert.Equal(25.0, vec1.Area, 1)
  Assert.Equal(Vector2f.create(-4.0, -5.0), vec1.Minimum vec2)
  Assert.Equal(Vector2f.create(5.0, 2.0), vec1.Maximum vec2)
