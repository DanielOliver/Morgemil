namespace Morgemil.Math

type Axis =
  | Horizontal
  | Vertical
  ///Choose the opisite Axis
  member this.Opposite =
    match this with
    | Axis.Vertical -> Axis.Horizontal
    | Axis.Horizontal -> Axis.Vertical
