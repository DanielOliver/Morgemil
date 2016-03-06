namespace Morgemil.Logic

open Morgemil.Core

type PlayerSystem(world : World) = 
  inherit EntityView<PlayerComponent>(world.Entities, ComponentType.Player, 
                                      (fun t -> 
                                      match t with
                                      | Component.Player(x) -> Some(x)
                                      | _ -> None), Component.Player)
