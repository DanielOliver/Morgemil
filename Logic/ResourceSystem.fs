namespace Morgemil.Logic

open Morgemil.Core

type ResourceSystem(entitySystem : EntitySystem) = 
  inherit EntityView<ResourceComponent>(entitySystem, ComponentType.Resouce, 
                                        (fun t -> 
                                        match t with
                                        | Component.Resource(x) -> Some(x)
                                        | _ -> None), Component.Resource)
