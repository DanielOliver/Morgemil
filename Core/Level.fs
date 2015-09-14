namespace Morgemil.Core

open Morgemil.Core

/// <summary>
/// A 2d Level.
/// </summary>
/// <param name="area">edge-inclusive area of tiles</param>
/// <param name="tiles">2d array [row,column]</param>
type Level = 
  { ///[0,0] (MaxX,MaxY)
    Area : Rectangle
    Tiles : Tile array
    TileModifiers : List<TileModifier>
    Depth: int }
  
  /// <summary>
  /// Global coordinates. Zero-based indices relative to this.Area.Position
  ///</summary>
  member this.Tile(vec2 : Vector2i) = this.Tiles.[this.Area.FlatCoord(vec2)]
  
  ///Seq<Math.Vector2i * TileDefinition>
  member this.TileCoordinates = Seq.map (fun coord -> coord, (this.Tile coord)) this.Area.Coordinates
