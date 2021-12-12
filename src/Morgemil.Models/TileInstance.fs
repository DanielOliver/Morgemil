namespace Morgemil.Models

open Morgemil.Math
open Morgemil.Models
open Newtonsoft.Json

[<Record>]
type TileInstance =
    {   [<RecordId>] ID: TileInstanceID
        Tile: Tile
        TileFeature: TileFeature option
        Position: Vector2i
    }

    interface Relational.IRow with
        [<JsonIgnore()>]
        member this.Key = this.ID.Key
