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

///Edge-inclusive range
let RandomPoint (rng : DefaultRNG) (area : Rectangle) = 
  Vector2i.create(Range rng area.Left area.Right, Range rng area.Top area.Bottom)

///Edge-inclusive (0,0) to vec. Assume positive
let RandomVector (rng : DefaultRNG) (vec : Vector2i) = Vector2i.create(Range rng 0 vec.X, Range rng 0 vec.Y)
