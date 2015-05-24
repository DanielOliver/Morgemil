[<EntryPoint>]
let main argv =
  let createdSampleDungeon = Morgemil.Test.SimpleDungeon.DungeonGenerator
  let createdBspDungeon = Morgemil.Test.BspGenerator.DungeonGenerator
  //  let ChunkRowToString y (chunk : Morgemil.Map.Chunk) =
  //    { chunk.Area.Left..chunk.Area.Right }
  //    |> Seq.map (fun x -> Morgemil.Math.Vector2i(x, y))
  //    |> Seq.map (chunk.Tile)
  //    |> Seq.map (fun tile -> tile.ID.ToString())
  //    |> Seq.reduce (+)
  //
  //  let ChunkToString(chunk : Morgemil.Map.Chunk) =
  //    { chunk.Area.Top..chunk.Area.Bottom } |> Seq.map (fun y -> ChunkRowToString y chunk)
  //
  //  let CombineChunks =
  //    createdSampleDungeon
  //    |> Seq.map (ChunkToString)
  //    |> Seq.concat
  //
  //  let filename = "map_test.txt"
  //  System.IO.File.WriteAllLines(filename, CombineChunks)
  let filename = "map_test.bmp"
  let dungeonDraw = Morgemil.Test.DungeonVisualizer.Visualize(createdSampleDungeon)
  dungeonDraw.Save(filename)
  let filename2 = "map_test2.bmp"
  let dungeonDraw2 = Morgemil.Test.DungeonVisualizer.Visualize(createdBspDungeon)
  dungeonDraw2.Save(filename2)
  //System.Console.ReadKey() |> ignore
  0 // return an integer exit code
