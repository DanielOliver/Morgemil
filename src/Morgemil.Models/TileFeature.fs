namespace Morgemil.Models


type TileFeature =
    {   ID: TileFeatureID
        /// A short name. eg: "Stairs Down"
        Name : string
        ///A long description. eg: "Take these stairs down to the next level."
        Description : string
        ///If true, this Tile Feature ALWAYS blocks ALL movement by ANYTHING.
        BlocksMovement : bool
        ///If true, this Tile Feature ALWAYS blocks ALL Line-Of-Sight of ANYTHING.
        BlocksSight : bool
        ///What this tile Feature looks like.
        Representation: TileRepresentation
        ///The tiles that this feature is valid to exist on.
        PossibleTiles: Tile list }

  
    interface Relational.IRow with
        member this.Key = this.ID.Key