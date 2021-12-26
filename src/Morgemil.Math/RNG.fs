module Morgemil.Math.RNG

open MathNet.Numerics.Random

type WeightedRatio<'t> = { ID: int; Weight: int }

/// <summary>
/// Type alias for a System.Random implementation
/// </summary>
type DefaultRNG = MersenneTwister

let SeedRNG (seed: int) = DefaultRNG seed

/// <summary>
/// Given an RNG and likelihood, returns the success
/// </summary>
/// <param name="chance">[0.0, 1.0]</param>
let Probability (rng: DefaultRNG) chance =
    match chance with
    | _ when chance <= 0m -> false
    | _ when chance >= 1m -> true
    | _ -> (rng.NextDecimal() <= chance)

///Inclusive range
let Range (rng: DefaultRNG) min max = rng.Next(min, max + 1)

let ChooseRatio (rng: DefaultRNG) (ratios: WeightedRatio<_> list) =
    if ratios.Length = 1 then
        ratios.Head.ID
    else
        let weights = ratios |> Seq.sumBy (fun t -> t.Weight)
        let choice = Range rng 0 (weights - 1)

        let rec next current remaining =
            match remaining with
            | head :: tail ->
                if current + head.Weight > choice then
                    head.ID
                else
                    next (current + head.Weight) tail
            | [] -> failwithf "Failure to ratio"

        next 0 ratios

///Edge-inclusive range
let RandomPoint (rng: DefaultRNG) (area: Rectangle) =
    Vector2i.create (Range rng area.Left area.Right, Range rng area.Top area.Bottom)

///Edge-inclusive (0,0) to vec. Assume positive
let RandomVector (rng: DefaultRNG) (vec: Vector2i) =
    Vector2i.create (Range rng 0 vec.X, Range rng 0 vec.Y)
