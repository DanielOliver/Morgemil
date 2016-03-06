namespace Morgemil.Logic

open Morgemil.Core

type ActionSystem(entitySystem : EntitySystem) = 
  
  inherit EntityView<ActionComponent>(entitySystem, ComponentType.Action, 
                                      (fun t -> 
                                      match t with
                                      | Component.Action(x) -> Some(x)
                                      | _ -> None), Component.Action)
  
  ///The next entity to act. Will repeat if not replaced. Throws error if none found
  member this.Next() = 
    base.Transformed()
    |> Seq.sortBy (fun t -> t.TimeOfNextAction)
    |> Seq.head
  
  member this.Act(entityId, time : decimal<GameTime>, currentTime : decimal<GameTime>) = 
    let current = this.Find(entityId)
    match current with
    | Some(old) -> 
      this.[entityId] <- { old with TimeOfNextAction = old.TimeOfNextAction + time
                                    TimeOfRequest = old.TimeOfNextAction }
    | None -> 
      this.[entityId] <- { EntityId = entityId
                           TimeOfNextAction = currentTime + time
                           TimeOfRequest = currentTime }
