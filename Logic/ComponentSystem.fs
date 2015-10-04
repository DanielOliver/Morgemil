namespace Morgemil.Logic

open Morgemil.Core

type ComponentSystem<'T when 'T : comparison>(initialComponents : Set<'T>, getId : 'T -> EntityId) = 
  
  let mutable _components : Map<EntityId, 'T> = 
    [ for item in initialComponents -> (getId item), item ]
    |> Map.ofSeq
  
  let _added = new Event<'T>()
  let _removed = new Event<'T>()
  let _replaced = new Event<'T * 'T>()
  let _matches entityId (item : 'T) = (getId (item) = entityId)
  new(getId) = ComponentSystem(Set.empty, getId)
  member this.Components = _components |> Seq.map (fun t -> t.Value)
  member this.ComponentRemoved = _removed.Publish
  member this.ComponentAdded = _added.Publish
  member this.ComponentReplaced = _replaced.Publish
  
  member this.Find entityId = 
    match _components.ContainsKey(entityId) with
    | true -> Some(_components.[entityId])
    | _ -> None
  
  member this.Item 
    with get (entityId : EntityId) = 
      match this.Find entityId with
      | Some(x) -> x
      | _ -> invalidOp "ComponentSystem.Item"
  
  abstract Add : 'T -> unit
  abstract Remove : 'T -> unit
  abstract Remove : EntityId -> unit
  abstract Replace : 'T * 'T -> 'T
  abstract Replace : EntityId * ('T -> 'T) -> 'T * 'T
  
  override this.Add item = 
    _components <- _components.Add(getId item, item)
    _added.Trigger(item)
  
  override this.Remove item = 
    _components <- _components.Remove(getId item)
    _removed.Trigger(item)
  
  override this.Remove entityId = this.Remove _components.[entityId]
  
  override this.Replace(old_value : 'T, new_value) = 
    _components <- _components.Remove(getId old_value).Add(getId new_value, new_value)
    _replaced.Trigger(old_value, new_value)
    new_value
  
  override this.Replace(entityId : EntityId, replacement) = 
    let old_value = this.[entityId]
    (old_value, this.Replace(old_value, replacement old_value))
