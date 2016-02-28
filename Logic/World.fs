namespace Morgemil.Logic

open Morgemil.Core

type World(level, components, currentTime) = 
  let _currentTime : float<GameTime> = currentTime
  let _level : Level = level
  let _entities = EntitySystem(components)
  let _actions = ActionSystem(_entities)
  let _positions = PositionSystem(_entities)
  let _resources = ResourceSystem(_entities)
  member val CurrentTime : float<GameTime> = currentTime with get, set
  member this.Level = _level
  member this.Entities = _entities
  member this.Actions = _actions
  member this.Positions = _positions
  member this.Resources = _resources
  static member Empty = World(Level.Empty, Seq.empty, 0.0<GameTime>)
