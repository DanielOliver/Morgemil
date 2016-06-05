namespace Morgemil.Logic

open Morgemil.Core
open Morgemil.Logic.Extensions
open System.Collections


type EntitySystem(initial : seq<Component>) =

  let _components = new Generic.Dictionary<ComponentType, Generic.Dictionary< EntityId, Component>>()

  do //Populate the initial items
    initial
    |> Seq.groupBy(fun comp -> comp.Type)
    |> Seq.iter(fun (componentType, items) -> 
      _components.[componentType] <- new Generic.Dictionary<EntityId, Component>(items 
                                                                                 |> Seq.map(fun t -> t.EntityId, t) 
                                                                                 |> Map.ofSeq))
  
  let _added = new Event<Component>()
  let _removed = new Event<Component>()
  let _replaced = new Event<Component * Component>()
  let _entityDeath = new Event<EntityId>()
  let _identities = 
    IdentityPool(set (initial |> Seq.map (fun t -> t.EntityId.Value)), EntityId.EntityId, fun t -> t.Value)

  let getType(componentType : ComponentType) = 
    match _components.TryGetValue(componentType) with
    | true, items -> items
    | false, _ -> let next = new Generic.Dictionary<EntityId, Component>()
                  _components.Add(componentType, next)
                  next

  let findItem(entityId: EntityId, componentType: ComponentType) =
    match getType(componentType).TryGetValue(entityId) with
    | true, item -> Some(item)
    | false, _ -> None

                  
  //Gets all components in a sequence
  member this.Components = _components |> Seq.collect (fun k -> k.Value |> Seq.map (fun v -> v.Value))
  
  ///Gets all components with this ComponentType.
  member this.ByType(componentType : ComponentType) = getType(componentType)
  
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
  member this.Find(entityId : EntityId, componentType : ComponentType) = findItem(entityId, componentType)
  
  ///Adds a component to an entity
  member this.Add(item : Component) = 
    getType(item.Type).Add(item.EntityId, item)
    _added.Trigger(item)
  
  ///Removes a specific component
  member this.Remove(item : Component) = 
    if getType(item.Type).Remove(item.EntityId) then
      _removed.Trigger(item)
  
  ///Marks an entity for cleanup
  member this.Kill entityId = _identities.Kill entityId
  
  ///returns true if this entity is dead
  member this.IsDead entityId = _identities.IsDead entityId
  
  ///Removes all entities marked as dead. Doesn't fire Component Removed event.
  member this.Free() = 
    let dead = List.ofSeq (_identities.Free())

    dead 
    |> Seq.iter (_entityDeath.Trigger)
    
    _components 
    |> Seq.iter(fun comp -> dead 
                            |> List.iter(comp.Value.Remove >> ignore))
  
  ///Replaces an entity with a new value
  member this.Replace(old_value : Component, new_value : Component) = 
    getType(old_value.Type).[old_value.EntityId] <- new_value
    _replaced.Trigger(old_value, new_value)
    
  ///Replaces an entity with a new value
  member this.Replace(new_value : Component) = 
    match findItem(new_value.EntityId, new_value.Type) with
    | Some(x) -> this.Replace(x, new_value)
    | None -> failwithf "Failed to replace entity \"%A\" and type \"%A\"" new_value.EntityId new_value.Type
