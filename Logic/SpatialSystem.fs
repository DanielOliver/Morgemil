namespace Morgemil.Logic

open Morgemil.Core

type SpatialSystem(initial) = 
  inherit ComponentSystem<PositionComponent>(initial, (fun position -> position.EntityId))
  member this.InRectangle(area : Rectangle) = 
    this.Components |> Set.filter (fun position -> area.Contains(position.Position))
  static member Empty = SpatialSystem(Set.empty)
