namespace Morgemil.Logic

open Morgemil.Core
open Morgemil.Logic.Extensions

type EntitySystem(intialEntities : seq<ComponentAggregator>) = 
  
  let mutable _entities = 
    [ for item in intialEntities -> (item.EntityId, item) ]
    |> Map.ofSeq
  
  let _added = new Event<ComponentAggregator>()
  let _removed = new Event<ComponentAggregator>()
  let _replaced = new Event<EntityChanges>()
  let _identities = IdentityPool(set (_entities |> Seq.map (fun t -> t.Key.Value)), EntityId.EntityId, fun t -> t.Value)
  member this.EntityAdded = _added.Publish
  member this.EntityRemoved = _removed.Publish
  member this.EntityReplaced = _replaced.Publish
  
  member this.Entities = 
    _entities
    |> Map.toSeq
    |> Seq.map (fun (k, v) -> v)
  
  member this.Generate() = this.Add ComponentAggregator.Empty
  member this.Item 
    with get (entityId : EntityId) = _entities.[entityId]
  
  member this.Find entityId = 
    match _entities.ContainsKey(entityId) with
    | true -> Some(_entities.[entityId])
    | _ -> None
  
  member this.Add(item : ComponentAggregator) = 
    let actual = 
      if EntityId.IsDefault(item.EntityId) then { item with EntityId = _identities.Generate() }
      else item
    _entities <- _entities.Add(item.EntityId, actual)
    _added.Trigger(actual)
    actual
  
  member this.Remove(item : ComponentAggregator) = 
    _entities <- _entities.Remove(item.EntityId)
    _removed.Trigger(item)
    _identities.Free(item.EntityId)
  
  member this.Replace(old_value : ComponentAggregator, new_value : ComponentAggregator) = 
    _entities <- _entities.Replace(old_value.EntityId, new_value)
    _replaced.Trigger(EntityChanges(old_value, new_value))
    new_value
  
  member this.Replace(entityId : EntityId, replacement) = 
    let old_value = this.[entityId]
    (old_value, this.Replace(old_value, replacement old_value))
