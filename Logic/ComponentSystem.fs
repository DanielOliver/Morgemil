namespace Morgemil.Logic

open Morgemil.Core

type ComponentSystem<'T when 'T : comparison>(initialComponents : Set<'T>, getId : 'T -> EntityId) = 
  let mutable _components : Set<'T> = initialComponents
  let _added = new Event<'T>()
  let _removed = new Event<'T>()
  let _replaced = new Event<'T * 'T>()
  let _matches entityId (item : 'T) = (getId (item) = entityId)
  new(getId) = ComponentSystem(Set.empty, getId)
  member this.Components = _components
  member this.ComponentRemoved = _removed.Publish
  member this.ComponentAdded = _added.Publish
  member this.ComponentReplaced = _replaced.Publish
  member this.Find entityId = _components |> Set.filter (_matches entityId)
  abstract Add : 'T -> unit
  abstract Remove : 'T -> unit
  abstract Remove : EntityId -> unit
  abstract Replace : 'T -> 'T -> unit
  
  override this.Add item = 
    _components <- _components.Add item
    _added.Trigger(item)
  
  override this.Remove item = 
    _components <- _components.Remove item
    _removed.Trigger(item)
  
  override this.Remove entityId = 
    let (to_remove, to_keep) = Set.partition (_matches entityId) _components
    _components <- to_keep
    to_remove |> Seq.iter _removed.Trigger
  
  override this.Replace (old_value : 'T) new_value = 
    this.Remove old_value
    this.Add new_value
    _replaced.Trigger(old_value, new_value)
