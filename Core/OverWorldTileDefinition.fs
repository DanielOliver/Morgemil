namespace Morgemil.Map.OverWorld

/// <summary>
/// A <see cref="Morgemil.Map.TileDefinition">TileDefinition</see> is a subset of this
/// overworld tile.
/// </summary>
type OverWorldTileDefinition(definition : Morgemil.Map.TileDefinition) =
  member this.Definition = definition
