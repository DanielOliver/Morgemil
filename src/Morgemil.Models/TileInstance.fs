namespace Morgemil.Models

open Morgemil.Math
open Morgemil.Models

[<Record>]
type TileInstance =
    { [<RecordId>]
      ID: TileInstanceID
      Tile: Tile
      TileFeature: TileFeature option
      Position: Vector2i }

    interface Relational.IRow with
        member this.Key = this.ID.Key
