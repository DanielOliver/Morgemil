namespace Morgemil.Logic

open Morgemil.Core

///Convenience functions over a view
type EntityView<'U>(entitySystem : EntitySystem, componentType : ComponentType, fromComponent : Component -> 'U option, toComponent : 'U -> Component) = 
  
  member this.HandleAdd(add : 'U -> unit) = 
    entitySystem.ComponentAdded.Add(fromComponent >> (fun t -> 
                                    match t with
                                    | Some(x) -> add (x)
                                    | _ -> ()))
  
  member this.HandleReplace(replace : 'U * 'U -> unit) = 
    entitySystem.ComponentReplaced.Add(fun (oldC, newC) -> 
      if oldC.Type = componentType then replace ((fromComponent (oldC)).Value, (fromComponent (newC)).Value))
  
  member this.HandleRemove(remove : 'U -> unit) = 
    entitySystem.ComponentAdded.Add(fromComponent >> (fun t -> 
                                    match t with
                                    | Some(x) -> remove (x)
                                    | _ -> ()))
  
  ///Returns raw map access
  member this.Components() = entitySystem.ByType(componentType)
  
  ///Transforms all components to encapsulated type
  member this.Transformed() = entitySystem.ByType(componentType) |> Seq.choose (fun v -> fromComponent (v.Value))
  
  ///Returns option on item
  member this.Find(entityId) = 
    match entitySystem.Find(entityId, componentType) with
    | Some(x) -> fromComponent (x)
    | None -> None
  
  ///Throws an exception if not found. Upserts if setting
  member this.Item 
    with get (entityId) = fromComponent(entitySystem.Find(entityId, componentType).Value).Value
    and set entityId value = 
      let comp = toComponent value
      match entitySystem.Find(comp.EntityId, componentType) with
      | Some(x) -> entitySystem.Replace(x, comp)
      | None -> entitySystem.Add(comp)
