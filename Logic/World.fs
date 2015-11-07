namespace Morgemil.Logic

open Morgemil.Core

type World(level, spatialComponents, resourceComponents, playerComponents, triggers, entities) = 
  let _spatial = SpatialSystem(spatialComponents)
  let _resources = ComponentSystem<ResourceComponent>(resourceComponents, (fun resource -> resource.EntityId))
  let _players = ComponentSystem<PlayerComponent>(playerComponents, (fun player -> player.EntityId))
  let _triggers = TriggerSystem(triggers)
  let _level : Level = level
  let _entities = IdentityPool(set entities, EntityId.EntityId, fun t -> t.Value)
  member this.Spatial = _spatial
  member this.Level = _level
  member this.Resources = _resources
  member this.Players = _players
  member this.Triggers = _triggers
  member this.Copy() = 
    World(level, _spatial.Components, _resources.Components, _players.Components, _triggers.Items, _entities.Items)
  static member Empty = World(Level.Empty, Seq.empty, Seq.empty, Seq.empty, Seq.empty, Seq.empty)
