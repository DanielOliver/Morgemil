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
  static member Empty = World(Level.Empty, Seq.empty, Seq.empty, Seq.empty, Seq.empty, Seq.empty, Seq.empty)
