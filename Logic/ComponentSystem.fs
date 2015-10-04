namespace Morgemil.Logic

open Morgemil.Core

[<AbstractClass>]
type ComponentSystem<'T>(convertFrom : Component -> 'T, convertTo : 'T -> Component, add : 'T -> unit, remove : 'T -> unit) = 
  let _components = new System.Collections.Generic.Dictionary<EntityId, Component>()
  
  member this.Add item = 
    let comp = convertTo item
    _components.Add(comp.EntityId, comp)
    add item
  
  member this.Remove item = 
    let comp = convertTo item
    _components.Remove(comp.EntityId) |> ignore
    remove item
  
  member this.Replace entityId (replacement : 'T -> 'T) = 
    let old_val = _components.[entityId] |> convertFrom
    remove old_val
    let new_val = old_val |> replacement
    _components.[entityId] <- (convertTo new_val)
    add new_val
