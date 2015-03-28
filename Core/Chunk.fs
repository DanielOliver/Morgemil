namespace Morgemil.Map

/// <summary>
/// A 2d Chunk of tiles. Used in the overworld and not in dungeons.
/// </summary>
/// <param name="tiles">2d array [row,column]</>
type Chunk(area : Morgemil.Math.Rectangle, tiles : TileDefinition array) = 
  member this.Area = area
  member this.Tiles = tiles
  member this.Tile x y = 5
