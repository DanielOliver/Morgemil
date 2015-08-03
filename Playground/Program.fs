let Instruct() = printfn "(E)ast (N)orth (W)est (S)outh (Q)uit"

let Prompt() = 
  let response = System.Console.ReadLine()
  match System.Char.ToLower(response.[0]) with
  | 'e' -> Some(Morgemil.Game.MoveEast)
  | 'n' -> Some(Morgemil.Game.MoveNorth)
  | 'w' -> Some(Morgemil.Game.MoveWest)
  | 's' -> Some(Morgemil.Game.MoveSouth)
  | _ -> None

let rec Continue (depth : int) (walkabout : Morgemil.Test.Walkabout) = 
  System.Console.WriteLine(walkabout.Player.Position)
  match Prompt() with
  | None -> ()
  | Some(act) -> 
    let filename = "map_test" + depth.ToString("0000") + ".bmp"
    let dungeonDraw = Morgemil.Test.DungeonVisualizer.Visualize walkabout.Dungeon
    Morgemil.Test.DungeonVisualizer.DrawPlayer walkabout.Player dungeonDraw
    dungeonDraw.Save(filename)
    Continue (depth + 1) (walkabout.Act act)

[<EntryPoint>]
let main argv = 
  let createdBspDungeon = 
    Morgemil.Map.DungeonGeneration.Generate { Type = Morgemil.Map.DungeonGenerationType.BSP
                                              Depth = 1
                                              RngSeed = 656556 }
  
  let walkAbout = 
    Morgemil.Test.Walkabout(createdBspDungeon, 
                            { Id = 5
                              Race = Morgemil.Game.Race.Lookup.[0]
                              Position = Morgemil.Math.Vector2i(5, 5) })
  
  Instruct()
  Continue 0 walkAbout
  //  let filename2 = "map_test2.bmp"
  //  let dungeonDraw2 = Morgemil.Test.DungeonVisualizer.Visualize(createdBspDungeon)
  //  dungeonDraw2.Save(filename2)
  //System.Console.ReadKey() |> ignore
  0 // return an integer exit code
