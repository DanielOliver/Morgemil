namespace Morgemil.Core

type World = 
  { Level : Level
    Entities : Map<int, Entity>
    Positions : Map<int, PositionComponent>
    Controllers : Map<int, ControlComponent>
    Resources : Map<int, ResourceComponent> }
