module Morgemil.Test.Vector2iTest

open Morgemil.Math
open NUnit.Framework

[<Test>]
let ``Vector2i add zero Vector2i``() = 
  let identity = Vector2i(5, 5)
  Assert.AreEqual(Vector2i() + identity, identity)
  Assert.AreEqual(identity + Vector2i(), identity)

[<Test>]
let ``Vector2i subtract zero Vector2i``() = 
  let identity = Vector2i(5, 5)
  Assert.AreEqual(identity - Vector2i(), identity)
  Assert.AreNotEqual(Vector2i() - identity, identity)

[<Test>]
let ``Vector2i add zero scalar``() = 
  let identity = Vector2i(5, 5)
  Assert.AreEqual(0 + identity, identity)
  Assert.AreEqual(identity + 0, identity)

[<Test>]
let ``Vector2i subtract zero scalar``() = 
  let identity = Vector2i(5, 5)
  Assert.AreEqual(identity - 0, identity)
  Assert.AreNotEqual(0 - identity, identity)
