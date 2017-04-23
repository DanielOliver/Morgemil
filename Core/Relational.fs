namespace Morgemil.Core 

open Morgemil

type Relational(scenarioData: Models.ScenarioData, rng: Math.RNG.DefaultRNG) =
  let _raceModifiers = 
    scenarioData.RaceModifierLinks 
    |> Seq.groupBy(fun t -> t.Race.ID)
    |> Seq.map(fun (raceId, modifiers) -> raceId, modifiers |> Seq.toList)
    |> Map.ofSeq
  
  member this.Data = scenarioData
  member this.GetRaceModifiers raceId = _raceModifiers.[raceId]


