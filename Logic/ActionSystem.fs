namespace Morgemil.Logic

open Morgemil.Core

type ActionSystem(initialTime: float<GameTime>, entities: EntitySystem) =
  member val CurrentTime = initialTime with get, set
  member this.Next = entities.Entities |> Seq.choose(fun t -> t.Action) |> Seq.sortBy (fun t -> t.TimeOfNextAction) |> Seq.head