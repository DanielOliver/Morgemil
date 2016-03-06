namespace Morgemil.Logic

open Morgemil.Core

type PlayerSystem(entitySystem : EntitySystem) = 
  inherit EntityView<PlayerComponent>(entitySystem, ComponentType.Player, 
                                      (fun t -> 
                                      match t with
                                      | Component.Player(x) -> Some(x)
                                      | _ -> None), Component.Player)
