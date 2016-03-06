namespace Morgemil.Logic

open Morgemil.Core

type ResourceSystem(world : World) = 
  inherit EntityView<ResourceComponent>(world.Entities, ComponentType.Resouce, 
                                        (fun t -> 
                                        match t with
                                        | Component.Resource(x) -> Some(x)
                                        | _ -> None), Component.Resource)
