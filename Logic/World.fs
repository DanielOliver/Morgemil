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
  
  member this.Free(entityId : EntityId) = 
    _spatial.Remove entityId |> ignore
    _resources.Remove entityId |> ignore
    _players.Remove entityId |> ignore
    _triggers.Remove entityId |> ignore
    _actions.Remove entityId |> ignore
    _entities.Free entityId
  
  member this.Reserve() = _entities.Generate()
  
  member this.Entity entityId = 
    { new ComponentAggregator(entityId) with
        member this.Position = _spatial.Find entityId
        member this.Player = _players.Find entityId
        member this.Resource = _resources.Find entityId
        member this.Action = _actions.Find entityId
        member this.Triggers = _triggers.Find entityId }
  
  static member Empty = World(Level.Empty, Seq.empty, Seq.empty, Seq.empty, Seq.empty, Seq.empty, Seq.empty)
