namespace Morgemil.Models

[<RequireQualifiedAccess>]
type TagType =
  | PlayerOption


[<RequireQualifiedAccess>]
type Tag =
  | PlayerOption

  member this.TagType =
    match this with
    | PlayerOption -> TagType.PlayerOption

