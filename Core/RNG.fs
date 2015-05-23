module Morgemil.Math.RNG

open MathNet.Numerics.Random

/// <summary>
/// Type alias for a System.Random implementation
/// </summary>
type DefaultRNG = MersenneTwister

let SeedRNG(seed : int) = DefaultRNG seed

/// <summary>
/// Given an RNG and likelihood, returns the success
/// </summary>
/// <param name="chance">[0.0, 1.0]</param>
let Probability (rng : DefaultRNG) chance =
  match chance with
  | 0m -> false
  | 1m -> true
  | _ -> (rng.NextDecimal() >= chance)
