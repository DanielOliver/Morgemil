namespace Morgemil.Logic

open Morgemil.Core

type PositionSystem(world : World) as this = 
  inherit EntityView<PositionComponent>(world.Entities, ComponentType.Position, 
                                        (fun t -> 
                                        match t with
                                        | Component.Position(x) -> Some(x)
                                        | _ -> None), Component.Position)
  let remove(c: PositionComponent) = world.Level.Entity(c.Position) <- None
  let add(c: PositionComponent) = world.Level.Entity(c.Position) <- Some(c.EntityId)
  let replace(oldC, newC) =
    if oldC.Position <> newC.Position then remove(oldC)
                                           add(newC)

  do this.HandleAdd(add)
     this.HandleRemove(remove)
     this.HandleReplace(replace) 