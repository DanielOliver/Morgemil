namespace Morgemil.Models

[<Record>]
type TileFeature =
    { [<RecordId>]
      ID: TileFeatureID
      /// A short name. eg: "Stairs Down"
      Name: string
      ///A long description. eg: "Take these stairs down to the next level."
      Description: string
      ///If true, this Tile Feature ALWAYS blocks ALL movement by ANYTHING.
      BlocksMovement: bool
      ///If true, this Tile Feature ALWAYS blocks ALL Line-Of-Sight of ANYTHING.
      BlocksSight: bool
      ///What this tile Feature looks like.
      Representation: TileRepresentation
      ///The tiles that this feature is valid to exist on.
      PossibleTiles: Tile list
      ///True if this tile is an exit point.  Usually stairs down to the next level.
      ExitPoint: bool
      ///True if this tile is an entry point.  Usually stairs up to the previous level.
      EntryPoint: bool }


    interface Relational.IRow with
        [<System.Text.Json.Serialization.JsonIgnore>]
        member this.Key = this.ID.Key
