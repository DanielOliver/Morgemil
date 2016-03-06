namespace Morgemil.Logic

open Morgemil.Core

type World(level, components, currentTime) = 
  let _level : Level = level
  let _entities = EntitySystem(components)
  member val CurrentTime : decimal<GameTime> = currentTime with get, set
  member this.Level = _level
  member this.Entities = _entities
