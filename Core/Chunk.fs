namespace Morgemil.Map

/// <summary>
/// A 2d Chunk of tiles. Used in the overworld and not in dungeons.
/// Typically generated from a world generator
/// </summary>
/// <param name="tiles">2d array [row,column]</param>
type Chunk(area : Morgemil.Math.Rectangle, tiles : TileDefinition array) =
  member this.Area = area
  member this.Tiles = tiles

  /// <summary>
  /// Local coordinates. Zero-based indices: [y * area.Width + x]
  /// No check for valid coordinates
  /// </summary>
  member this.TileLocal(vec2 : Morgemil.Math.Vector2i) = tiles.[vec2.Y * area.Width + vec2.X]

  /// <summary>
  /// Global coordinates. Zero-based indices relative to this.Area.Position
  ///</summary>
  member this.Tile(vec2 : Morgemil.Math.Vector2i) = (vec2 - area.Position) |> this.TileLocal
