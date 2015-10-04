namespace Morgemil.Logic

open Morgemil.Core

type ComponentSystem(initialComponents : seq<Component>) = 
  let mutable _components : Set<Component> = Set.empty
  let _added = new Event<Component>()
  let _removed = new Event<Component>()
  let _replaced = new Event<Component * Component>()
  let _matches entityId (item : Component) = (item.EntityId = entityId)
  new() = ComponentSystem(Seq.empty)
  member this.ComponentRemoved = _removed.Publish
  member this.ComponentAdded = _added.Publish
  member this.ComponentReplaced = _replaced.Publish
  member this.Find entityId = _components |> Set.filter (_matches entityId)
  
  member this.Add item = 
    _components <- _components.Add item
    _added.Trigger(item)
  
  member this.Remove item = 
    _components <- _components.Remove item
    _removed.Trigger(item)
  
  member this.Remove entityId = 
    let (to_remove, to_keep) = Set.partition (_matches entityId) _components
    _components <- to_keep
    to_remove |> Seq.iter _removed.Trigger
  
  member this.Replace (old_value : Component) new_value = 
    this.Remove old_value
    this.Add new_value
    _replaced.Trigger(old_value, new_value)
