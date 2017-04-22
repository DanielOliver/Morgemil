namespace Morgemil.Models

[<RequireQualifiedAccess>]
type TagType =
  | PlayerOption = 1

[<RequireQualifiedAccess>]
type Tag =
  | PlayerOption

  member this.TagType =
    match this with
    | PlayerOption -> TagType.PlayerOption

