namespace Morgemil.Logic

open Morgemil.Core

///Convenience functions over a view
type EntityView<'U>(entitySystem : EntitySystem, componentType : ComponentType, fromComponent : Component -> 'U option, toComponent : 'U -> Component) = 
  
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
