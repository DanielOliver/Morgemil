namespace Morgemil.Logic

open Morgemil.Core

type PositionSystem(entitySystem : EntitySystem) = 
  inherit EntityView<PositionComponent>(entitySystem, ComponentType.Position, 
                                        (fun t -> 
                                        match t with
                                        | Component.Position(x) -> Some(x)
                                        | _ -> None), Component.Position)
