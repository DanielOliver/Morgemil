module Morgemil.Test.Vector2iTest

open Morgemil.Math
open NUnit.Framework

//Addition
[<Test>]
let ``Vector2i add zero``() =
  let identity = Vector2i(5)
  Assert.AreEqual(Vector2i() + identity, identity)
  Assert.AreEqual(identity + Vector2i(), identity)
  Assert.AreEqual(0 + identity, identity)
  Assert.AreEqual(identity + 0, identity)

//Subtraction
[<Test>]
let ``Vector2i subtract zero ``() =
  let identity = Vector2i(5)
  Assert.AreEqual(identity - Vector2i(), identity)
  Assert.AreNotEqual(Vector2i() - identity, identity)
  Assert.AreEqual(identity - 0, identity)
  Assert.AreNotEqual(0 - identity, identity)

//Multiplication
[<Test>]
let ``Vector2i add/multiply Vector2i``() =
  let identity = Vector2i(5)
  Assert.AreEqual(identity + identity, 2 * identity)

//Division
[<Test>]
let ``Vector2i divide Vector2i``() =
  let identity = Vector2i(5)
  let half = Vector2i(2)
  Assert.AreEqual(identity / half, half)
