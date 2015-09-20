namespace Morgemil.Core

type World = 
  { Level : Level
    Entities : Map<int, Entity>
    Positions : Map<int, PositionComponent>
    Controllers : Map<int, ControllerComponent>
    Resources : Map<int, ResourceComponent> }
  member this.Empty = 
    { Level = Level.Empty
      Entities = Map.empty
      Positions = Map.empty
      Controllers = Map.empty
      Resources = Map.empty }
