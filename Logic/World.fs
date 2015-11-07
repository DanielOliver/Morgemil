namespace Morgemil.Logic

open Morgemil.Core

type World(level, spatialComponents, resourceComponents, playerComponents, triggers, actions, entities) = 
  let _spatial = SpatialSystem(spatialComponents)
  let _resources = ComponentSystem<ResourceComponent>(resourceComponents, (fun resource -> resource.EntityId))
  let _players = ComponentSystem<PlayerComponent>(playerComponents, (fun player -> player.EntityId))
  let _triggers = TriggerSystem(triggers)
  let _actions = ActionSystem(actions, 0.0<GameTime>)
  let _level : Level = level
  let _entities = IdentityPool(set entities, EntityId.EntityId, fun t -> t.Value)
  member this.Spatial = _spatial
  member this.Level = _level
  member this.Resources = _resources
  member this.Players = _players
  member this.Triggers = _triggers
  member this.Actions = _actions
  
  member this.Entity entityId = 
    { new ComponentAggregator(entityId) with
        
        member this.Position 
          with get () = _spatial.Find(entityId)
          and set (value) = 
            match value with
            | Some(x) -> _spatial.AddOrReplace(entityId, x)
            | None -> _spatial.Remove entityId
        
        member this.Player 
          with get () = _players.Find(entityId)
          and set (value) = 
            match value with
            | Some(x) -> _players.AddOrReplace(entityId, x)
            | None -> _players.Remove entityId
        
        member this.Resource 
          with get () = _resources.Find(entityId)
          and set (value) = 
            match value with
            | Some(x) -> _resources.AddOrReplace(entityId, x)
            | None -> _resources.Remove entityId
        
        member this.Action 
          with get () = _actions.Find(entityId)
          and set (value) = 
            match value with
            | Some(x) -> _actions.AddOrReplace(entityId, x)
            | None -> _actions.Remove entityId }
  
  static member Empty = World(Level.Empty, Seq.empty, Seq.empty, Seq.empty, Seq.empty, Seq.empty, Seq.empty)
