namespace Morgemil.Map.OverWorld

open Morgemil.Map
open Morgemil.Math

/// <summary>
/// This is the OverWorld. A series of 2d chunks created randomly (eventually).
/// Each dungeon will be accessible from this world.
/// </summary>
/// <param name="size">The size of this overworld</param>
/// <param name="chunkSize">The size of each chunk. Also determines alignment</param>
/// <param name="generator">Takes this world as an argument and returns a chunk</param>
type OverWorldInfo(seed : int, size : Vector2i, generator : OverWorldInfo * Rectangle -> Chunk) = 
  member this.Seed = seed
  member this.Area = Rectangle(Vector2i(0, 0), size)
  member this.ChunkSize = Vector2i(16, 16)
  
  /// <summary>
  /// Returns the chunkId for a given location.
  /// The ChunkId * ChunkSize is the chunk's Position.
  /// </summary>
  /// <param name="position">The chunk-level coordinates</param>
  member this.ChunkId(loc : Vector2i) = loc / this.ChunkSize
  
  /// <summary>
  /// Generates a chunk that contains the given position
  /// </summary>
  member this.GenerateChunk(position : Vector2i) = 
    generator (this, Rectangle(this.ChunkId(position) * this.ChunkSize, this.ChunkSize))
