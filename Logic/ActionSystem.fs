namespace Morgemil.Logic

open Morgemil.Core

type ActionSystem(initialTime : float<GameTime>, entitySystem : EntitySystem) = 
  
  inherit EntityView<ActionComponent>(entitySystem, ComponentType.Action, 
                                      (fun t -> 
                                      match t with
                                      | Component.Action(x) -> Some(x)
                                      | _ -> None), Component.Action)
  
  ///The current time
  member val CurrentTime = initialTime with get, set
  
  ///The next component to act. Will repeat if not replaced
  member this.Next() = 
    base.Transformed()
    |> Seq.sortBy (fun t -> t.TimeOfNextAction)
    |> Seq.head
