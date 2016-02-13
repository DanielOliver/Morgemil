namespace Morgemil.Logic

open Morgemil.Core

type EntityView<'U>(entitySystem : EntitySystem, componentType : ComponentType, fromComponent : Component -> 'U option, toComponent : 'U -> Component) = 
  
  let mutable _entities = 
    (entitySystem.ByType componentType)
    |> Seq.map (fun t -> t.Key)
    |> Set
  
  do 
    entitySystem.ComponentAdded.Add(fun t -> 
      if t.Type = componentType then _entities <- _entities.Add(t.EntityId))
    entitySystem.ComponentRemoved.Add(fun t -> 
      if t.Type = componentType then _entities <- _entities.Remove(t.EntityId))
    entitySystem.EntityDeath.Add(fun t -> _entities <- _entities.Remove(t))
  
  member this.Components() = entitySystem.ByType(componentType)
  member this.Transformed() = entitySystem.ByType(componentType) |> Seq.choose (fun v -> fromComponent (v.Value))
  
  member this.Item 
    with get (entityId) = entitySystem.Find(entityId, componentType)
    and set entityId value = 
      let comp = toComponent value
      match entitySystem.Find(comp.EntityId, componentType) with
      | Some(x) -> entitySystem.Replace(x, comp)
      | None -> entitySystem.Add(comp)
