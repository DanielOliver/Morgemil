namespace Morgemil.Models

[<RequireQualifiedAccess>]
type TileType = 
  | Void = 0
  | Solid = 1
  | Ground = 2

type Tile = 
  { ID : int
    ///The general specification of this tile
    TileType: TileType
    /// A short name. eg: "Lush Grass"
    Name : string
    ///A long description. eg: "Beware the burning sand. Scorpions and asps make their home here."
    Description : string
    ///If true, this tile ALWAYS blocks ALL movement by ANYTHING.
    BlocksMovement : bool
    ///If true, this tile ALWAYS blocks ALL Line-Of-Sight of ANYTHING.
    BlocksSight : bool
    ///A list of Tags that this Tile possesses, along with the Value(s)
    Tags : Map<TagType, Tag> }
