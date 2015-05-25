[<EntryPoint>]
let main argv =
  //let createdBspDungeon = Morgemil.Test.BspGenerator.GenerateRoomDivides 5 Morgemil.Math.Vector2i(500, 340)
  let createdBspDungeon = [| Morgemil.Map.DungeonGeneration.Generate 56 |]
  let filename2 = "map_test2.bmp"
  let dungeonDraw2 = Morgemil.Test.DungeonVisualizer.Visualize(createdBspDungeon)
  dungeonDraw2.Save(filename2)
  //System.Console.ReadKey() |> ignore
  0 // return an integer exit code
