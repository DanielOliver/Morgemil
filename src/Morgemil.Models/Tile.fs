namespace Morgemil.Models

[<RequireQualifiedAccess>]
type TileType = 
  | Void 
  | Solid
  | Ground

type Tile = 
  { ID : TileID
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
    ///What this tile looks like.
    Representation: TileRepresentation}

  
  interface Relational.IRow with
      member this.Key = this.ID.Key