namespace Morgemil.Models

open Morgemil.Math

[<RequireQualifiedAccess>]
type TowerBacktrackBehavior =
    /// If this tower remains explored, do not bother making the player backtrack by hand, just skip to overworld directly.
    /// If player returns to tower, transport them to last level explored.
    | SkipToOverworld
    /// The player is stuck, doomed I tell you!
    | DoNotAllow
    /// This tower is effectively a maze, generate random rooms even though the player may have already beat the level number.
    | ExploreUnknown

[<RequireQualifiedAccess>]
type TowerOverworldConnection =
    /// Only meant to be used for a self-contained scenario
    | NoExit

[<Record>]
type Tower =
    { [<RecordId>]
      ID: TowerID
      Name: string
      LevelRangeInclusive: Vector2i
      BacktrackBehavior: TowerBacktrackBehavior
      OverworldConnection: TowerOverworldConnection
      DefaultFloorGenerationParameters: FloorGenerationParameterID }

    interface Relational.IRow with
        [<System.Text.Json.Serialization.JsonIgnore>]
        member this.Key = this.ID.Key
