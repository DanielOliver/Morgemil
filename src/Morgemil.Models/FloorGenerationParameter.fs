namespace Morgemil.Models

open Newtonsoft.Json

[<RequireQualifiedAccess>]
type FloorGenerationStrategy =
  | OpenFloor

[<Record>]
type FloorGenerationParameter =
  { [<RecordId>] ID : FloorGenerationParameterID
    /// Default Tile
    DefaultTile : Tile
    ///Tiles used
    Tiles : Tile list
    ///Size generation
    SizeRange : Morgemil.Math.Rectangle
    ///Generation Strategy
    Strategy : FloorGenerationStrategy
  }


  interface Relational.IRow with
        [<JsonIgnore()>]
        member this.Key = this.ID.Key
