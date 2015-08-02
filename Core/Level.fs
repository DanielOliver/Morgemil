namespace Morgemil.Map

/// <summary>
/// A 2d Level.
/// </summary>
/// <param name="area">edge-inclusive area of tiles</param>
/// <param name="tiles">2d array [row,column]</param>
type Level = 
  { Area : Morgemil.Math.Rectangle
    Tiles : TileDefinition array }
  
  /// <summary>
  /// Local coordinates. Zero-based indices: [y * area.Width + x]
  /// No check for valid coordinates
  /// </summary>
  member this.TileLocal(vec2 : Morgemil.Math.Vector2i) = this.Tiles.[vec2.Y * this.Area.Width + vec2.X]
  
  /// <summary>
  /// Global coordinates. Zero-based indices relative to this.Area.Position
  ///</summary>
  member this.Tile(vec2 : Morgemil.Math.Vector2i) = this.Tiles.[this.Area.FlatCoord(vec2)]
  
  /// <summary>
  /// True if every tile is TileDefinition.Default
  /// </summary>
  member this.IsEmpty = this.Tiles |> Array.forall (TileDefinition.IsDefault)
  
  ///Seq<Math.Vector2i * TileDefinition>
  member this.TileCoordinates = Seq.map (fun coord -> coord, (this.Tile coord)) this.Area.Coordinates
