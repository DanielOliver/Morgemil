namespace Morgemil.Logic

open Morgemil.Core

type World(level, triggers, entities) =
  let _level : Level = level
  let _entities = EntitySystem(entities)
  let _triggers = TriggerSystem(triggers)

  member this.Level = _level
  member this.Entities = _entities
  member this.Triggers = _triggers

  static member Empty = World(Level.Empty, Seq.empty, Seq.empty)