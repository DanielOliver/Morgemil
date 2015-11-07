namespace Morgemil.Logic

open Morgemil.Core

type ActionSystem(initial) as this = 
  inherit ComponentSystem<ActionComponent>(initial, (fun action -> action.EntityId))
  member this.Next() = this.Components |> Seq.minBy (fun x -> x.TimeOfNextAction)
  member this.Step(time: float<GameTime>) = () //TODO: fix
  static member Empty = ActionSystem(Seq.empty)
