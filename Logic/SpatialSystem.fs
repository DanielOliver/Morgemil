namespace Morgemil.Logic

open Morgemil.Core

type SpatialSystem(initial) = 
  inherit ComponentSystem<PositionComponent>(initial, (fun position -> position.EntityId))
  member this.InRectangle(area : Rectangle) = this.Components |> Seq.filter (fun value -> area.Contains(value.Position))
  static member Empty = SpatialSystem(Seq.empty)
