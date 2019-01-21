namespace Morgemil.Models

[<RequireQualifiedAccess>]
type TagType =
  | PlayerOption = 1

[<RequireQualifiedAccess>]
type Tag =
  | PlayerOption of IsPlayer: bool

  member this.TagType =
    match this with
    | PlayerOption _ -> TagType.PlayerOption

