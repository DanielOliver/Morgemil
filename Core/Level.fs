namespace Morgemil.Core

open Microsoft.FSharp.Collections
open Morgemil.Core
open Morgemil.Math

/// <summary>
/// A 2d Level.
/// </summary>
/// <param name="area">edge-inclusive area of tiles</param>
/// <param name="tiles">2d array [row,column]</param>
type Level(tiles : Tile [,], depth : int) = 
  let _size = Vector2i.create(Array2D.length1 tiles, Array2D.length2 tiles)
  let _entities : EntityId option [,] = Array2D.zeroCreate _size.X _size.Y
  let _tileModifiers : TileModifier option [,] = Array2D.zeroCreate _size.X _size.Y
  member this.Area = Rectangle.create(Vector2i.Zero, _size)
  member this.Tiles = tiles
  member this.Depth = depth
  
  /// <summary>
  /// Zero-based indices relative to this.Area.Position
  ///</summary>
  member this.Tile 
    with get (index : Vector2i) = tiles.[index.X, index.Y]
    and set (index : Vector2i) value = tiles.[index.X, index.Y] <- value
  
  /// <summary>
  /// Zero-based indices relative to this.Area.Position
  ///</summary>
  member this.Entity 
    with get (index : Vector2i) = _entities.[index.X, index.Y]
    and set (index : Vector2i) value = _entities.[index.X, index.Y] <- value
  
  /// <summary>
  /// Zero-based indices relative to this.Area.Position
  ///</summary>
  member this.TileModifier
    with get (index : Vector2i) = _tileModifiers.[index.X, index.Y]
    and set (index : Vector2i) value = _tileModifiers.[index.X, index.Y] <- value
  
  /// <summary>
  /// Zero-based indices relative to this.Area.Position
  ///</summary>
  member this.BlocksMovement
    with get (index : Vector2i) = 
      if this.Tile(index).BlocksMovement then true
      else 
        match this.TileModifier(index) with
        | Some(modifier) -> modifier.BlocksMovement
        | _ -> false
  
  /// <summary>
  /// Zero-based indices relative to this.Area.Position
  ///</summary>
  member this.BlocksSight 
    with get (index : Vector2i) = 
      if this.Tile(index).BlocksSight then true
      else 
        match this.TileModifier(index) with
        | Some(modifier) -> modifier.BlocksSight
        | _ -> false
