namespace Morgemil.Models

[<RequireQualifiedAccess>]
type TagType =
  | One = 1

[<RequireQualifiedAccess>]
type Tag =
  | One of Tags.One

  member this.TagType =
    match this with
    | One _ -> TagType.One

