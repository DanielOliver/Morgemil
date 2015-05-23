module Morgemil.Test.RNGTest

open Morgemil.Math
open NUnit.Framework

[<Test>]
let ``RNG Probability test``() =
  let seed = 150
  let rng = RNG.SeedRNG seed
  Assert.IsFalse(RNG.Probability rng 0m)
  Assert.True(RNG.Probability rng 1m)
