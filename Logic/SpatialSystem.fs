namespace Morgemil.Logic

type SpatialSystem() = 
  let mutable _components : Set<Morgemil.Core.PositionComponent> = Set.empty
