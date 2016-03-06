namespace Morgemil.Logic

open Morgemil.Core
open Morgemil.Logic.Extensions

type EntitySystem(initial : seq<Component>) = 
  
  let initialComponents = 
    initial
    |> Seq.groupBy (fun t -> t.Type)
    |> Seq.map (fun (k, v) -> 
         (k, 
          v
          |> Seq.map (fun m -> (m.EntityId, m))
          |> Map.ofSeq))
    |> Map.ofSeq
  
  let mutable _components : Map<ComponentType, Map<EntityId, Component>> = initialComponents
  let _added = new Event<Component>()
  let _removed = new Event<Component>()
  let _replaced = new Event<Component * Component>()
  let _entityDeath = new Event<EntityId>()
  let _identities = 
    IdentityPool(set (initial |> Seq.map (fun t -> t.EntityId.Value)), EntityId.EntityId, fun t -> t.Value)
  //Gets all components in a sequence
  member this.Components = _components |> Seq.collect (fun k -> k.Value |> Seq.map (fun v -> v.Value))
  
  ///Gets all components with this ComponentType.
  member this.ByType(componentType : ComponentType) = 
    match _components.TryFind(componentType) with
    | Some(x) -> x
    | None -> Map.empty
  
  ///A component has been added
  member this.ComponentAdded = _added.Publish
  
  ///A component has been removed and not replaced
  member this.ComponentRemoved = _removed.Publish
  
  ///A component has been replaced
  member this.ComponentReplaced = _replaced.Publish
  
  ///An entity has been freed from use. Fired in sequence at end of turn
  member this.EntityDeath = _entityDeath.Publish
  
  ///The list of entities currently reserved
  member this.Entities() = _identities.Items
  
  ///Reserves a new EntityID
  member this.Generate() = _identities.Generate()
  
  ///Returns component associated with an entity
  member this.Find(entityId : EntityId, componentType : ComponentType) = 
    match _components.TryFind(componentType) with
    | Some(x) -> x.TryFind(entityId)
    | _ -> None
  
  ///Adds a component to an entity
  member this.Add(item : Component) = 
    if _components.ContainsKey(item.Type) then 
      _components <- _components.Replace(item.Type, fun t -> t.Add(item.EntityId, item))
    else _components <- _components.Add(item.Type, Map.ofSeq [ item.EntityId, item ])
    _added.Trigger(item)
  
  ///Removes a specific component
  member this.Remove(item : Component) = 
    _components <- _components.Replace(item.Type, fun t -> t.Remove(item.EntityId))
    _removed.Trigger(item)
  
  ///Marks an entity for cleanup
  member this.Kill entityId = _identities.Kill entityId
  
  ///returns true if this entity is dead
  member this.IsDead entityId = _identities.IsDead entityId
  
  ///Removes all entities marked as dead. Doesn't fire Component Removed event.
  member this.Free() = 
    let dead = List.ofSeq (_identities.Free())
    dead |> Seq.iter (_entityDeath.Trigger)
    let rec remove (ids, components : Map<EntityId, Component>) = 
      match ids with
      | [] -> components
      | [ head ] -> 
        let item = components.Item(head)
        let result = components.Remove(head)
        _removed.Trigger(item)
        result
      | head :: tail -> remove(tail, components).Remove(head)
    _components <- _components |> Map.map (fun k v -> remove (dead, v))
  
  ///Replaces an entity with a new value
  member this.Replace(old_value : Component, new_value : Component) = 
    _components <- _components.Replace(old_value.Type, fun t -> t.Replace(old_value.EntityId, fun w -> new_value))
    _replaced.Trigger(old_value, new_value)
