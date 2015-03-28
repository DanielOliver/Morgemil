namespace Morgemil.Map

/// <summary>
/// This is the OverWorld. A series of 2d chunks created randomly (eventually).
/// Each dungeon will be accessible from this world.
/// </summary>
/// <param name="size">The size of this overworld</param>
/// <param name="chunkSize">The size of each chunk. Also determines alignment</param>
/// <param name="generator">Takes this world as an argument and returns a chunk</param>
type OverWorld(seed : int, size : Morgemil.Math.Vector2i, chunkSize : Morgemil.Math.Vector2i, generator : OverWorld * Morgemil.Math.Rectangle -> Chunk) = 
  member this.Seed = seed
  member this.Area = Morgemil.Math.Rectangle(Morgemil.Math.Vector2i(0, 0), size)
  member this.ChunkSize = chunkSize
  /// <summary>
  /// Generates a chunk that contains the given position
  /// </summary>
  member this.GenerateChunk(position : Morgemil.Math.Vector2i) = 
    generator (this, Morgemil.Math.Rectangle((position / chunkSize) * chunkSize, chunkSize))
