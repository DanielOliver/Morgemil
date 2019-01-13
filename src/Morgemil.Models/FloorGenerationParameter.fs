namespace Morgemil.Models

[<RequireQualifiedAccess>]
type FloorGenerationStrategy = 
  | OpenFloor = 0

type FloorGenerationParameter =
  { ID: int
    /// Default Tile
    DefaultTile: Tile
    ///Tiles used
    Tiles: Tile []
    ///Size generation
    SizeRange: Morgemil.Math.Rectangle
    ///Generation Strategy
    Strategy: FloorGenerationStrategy
    ///A list of Tags that this floor generation parameter possesses, along with the Value(s)
    Tags : Map<TagType, Tag>
  }

