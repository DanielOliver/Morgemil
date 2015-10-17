﻿namespace Morgemil.Logic

open Morgemil.Core

type World(level, spatialComponents, resourceComponents, playerComponents) = 
  let _spatial = SpatialSystem(spatialComponents)
  let _resources = ComponentSystem<ResourceComponent>(resourceComponents, (fun resource -> resource.EntityId))
  let _players = ComponentSystem<PlayerComponent>(playerComponents, (fun player -> player.EntityId))
  let _level : Level = level
  member this.Spatial = _spatial
  member this.Level = _level
  member this.Resources = _resources
  member this.Players = _players
  member this.Copy() = World(level, _spatial.Components, _resources.Components, _players.Components)
  static member Empty = World(Level.Empty, Seq.empty, Seq.empty, Seq.empty)
