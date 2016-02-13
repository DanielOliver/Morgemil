namespace Morgemil.Logic

open Morgemil.Core

type World(level) = 
  let _level : Level = level
  let _entities = EntitySystem()
  let _actions = ActionSystem(0.0<GameTime>, _entities)
  let _positions = PositionSystem(_entities)
  member this.Level = _level
  member this.Entities = _entities
  member this.Actions = _actions
  member this.Positions = _positions
  static member Empty = World(Level.Empty)
