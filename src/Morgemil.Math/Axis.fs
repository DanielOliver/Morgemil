namespace Morgemil.Math

[<RequireQualifiedAccess>]
type Axis =
    | Horizontal
    | Vertical
    ///Choose the opposite Axis
    member this.Opposite =
        match this with
        | Axis.Vertical -> Axis.Horizontal
        | Axis.Horizontal -> Axis.Vertical
