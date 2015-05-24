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
  | _ when chance <= 0m -> false
  | _ when chance >= 1m -> true
  | _ -> (rng.NextDecimal() <= chance)

///Inclusive range
let Range (rng : DefaultRNG) min max = rng.Next(min, max + 1)
