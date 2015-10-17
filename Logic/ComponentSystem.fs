namespace Morgemil.Logic

open Morgemil.Core
open Morgemil.Logic.Extensions

type ComponentSystem<'T when 'T : comparison>(initialComponents : seq<'T>, getId : 'T -> EntityId) = 
  
  let mutable _components : Map<EntityId, 'T> = 
    [ for item in initialComponents -> (getId item), item ]
    |> Map.ofSeq
  
  let _added = new Event<'T>()
  let _removed = new Event<'T>()
  let _replaced = new Event<'T * 'T>()
  let _matches entityId (item : 'T) = (getId (item) = entityId)
  new(getId) = ComponentSystem(Seq.empty, getId)
  member this.Components = _components |> Seq.map (fun t -> t.Value)
  member this.ComponentRemoved = _removed.Publish
  member this.ComponentAdded = _added.Publish
  member this.ComponentReplaced = _replaced.Publish
  
  member this.Find entityId = 
    match _components.ContainsKey(entityId) with
    | true -> Some(_components.[entityId])
    | _ -> None
  
  member this.Item 
    with get (entityId : EntityId) = _components.[entityId]
  
  member this.Add item = 
    _components <- _components.Add(getId item, item)
    _added.Trigger(item)
  
  member this.Remove item = 
    _components <- _components.Remove(getId item)
    _removed.Trigger(item)
  
  member this.Remove entityId = this.Remove _components.[entityId]
  
  member this.Replace(old_value : 'T, new_value : 'T) = 
    _components <- _components.Replace(getId old_value, new_value)
    _replaced.Trigger(old_value, new_value)
    new_value
  
  member this.Replace(entityId : EntityId, replacement) = 
    let old_value = this.[entityId]
    (old_value, this.Replace(old_value, replacement old_value))
  
  member this.Copy() = ComponentSystem(this.Components, getId)
