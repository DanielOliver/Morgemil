namespace Morgemil.Models

[<RequireQualifiedAccess>]
type FloorGenerationStrategy = | OpenFloor

[<Record>]
type FloorGenerationParameter =
    { [<RecordId>]
      ID: FloorGenerationParameterID
      /// Default Tile
      DefaultTile: Tile
      ///Tiles used
      Tiles: Tile list
      ///Size generation
      SizeRange: Morgemil.Math.Rectangle
      ///Generation Strategy
      Strategy: FloorGenerationStrategy }

    interface Relational.IRow with
        [<System.Text.Json.Serialization.JsonIgnore>]
        member this.Key = this.ID.Key
