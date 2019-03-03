namespace Morgemil.Models

[<RequireQualifiedAccess>]
type FloorGenerationStrategy =
  | OpenFloor

type FloorGenerationParameter =
  { ID : FloorGenerationParameterID
    /// Default Tile
    DefaultTile : Tile
    ///Tiles used
    Tiles : Tile []
    ///Size generation
    SizeRange : Morgemil.Math.Rectangle
    ///Generation Strategy
    Strategy : FloorGenerationStrategy
  }


  interface Relational.IRow with
        member this.Key = this.ID.Key
