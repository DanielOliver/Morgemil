namespace Morgemil.Map

open Morgemil.Math

/// <summary>
/// The existence of a mutable structure is necessary as a dungeon requires multiple passes to generate
/// </summary>
type DungeonGeneration(maxSize : Rectangle) =
  let internal_map = Array.create maxSize.Area (TileDefinition.Default)
  member this.SetValue (tile : TileDefinition) (pos : Vector2i) =
    internal_map.[maxSize.FlatCoord pos] <- tile
