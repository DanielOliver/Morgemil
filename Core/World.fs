namespace Morgemil.Core

type World = 
  { Level : Level
    Entities : Map<int, Entity>
    Positions : Map<int, PositionComponent>
    Players : Map<int, PlayerComponent>
    Resources : Map<int, ResourceComponent> }
  member this.Empty = 
    { Level = Level.Empty
      Entities = Map.empty
      Positions = Map.empty
      Players = Map.empty
      Resources = Map.empty }
