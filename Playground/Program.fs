[<EntryPoint>]
let main argv =
  let createdSampleDungeon = Morgemil.Test.SimpleDungeon.DungeonGenerator

  let ChunkRowToString y (chunk : Morgemil.Map.Chunk) =
    { chunk.Area.Left..chunk.Area.Right }
    |> Seq.map (fun x -> Morgemil.Math.Vector2i(x, y))
    |> Seq.map (chunk.Tile)
    |> Seq.map (fun tile -> tile.ID.ToString())
    |> Seq.reduce (+)

  let ChunkToString(chunk : Morgemil.Map.Chunk) =
    { chunk.Area.Top..chunk.Area.Bottom } |> Seq.map (fun y -> ChunkRowToString y chunk)

  let CombineChunks =
    createdSampleDungeon
    |> Seq.map (ChunkToString)
    |> Seq.concat

  let filename = "map_test.txt"
  System.IO.File.WriteAllLines(filename, CombineChunks)
  System.Console.ReadKey() |> ignore
  0 // return an integer exit code
