namespace Morgemil.Models

[<Record>]
type Floor =
    {
        [<RecordId>]
        ID: FloorID
        Tower: Tower
        /// Level [1,n] of the tower.
        Level: int
    }

    interface Relational.IRow with
        [<System.Text.Json.Serialization.JsonIgnore>]
        member this.Key = this.ID.Key
