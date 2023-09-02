namespace Morgemil.Models

open Morgemil.Models

[<Record>]
type TileInstance =
    { [<RecordId>]
      ID: TileInstanceID
      Tile: Tile
      TileFeature: TileFeature option
      Position: Morgemil.Math.Point }

    interface Relational.IRow with
        member this.Key = this.ID.Key
