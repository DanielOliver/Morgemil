open Morgemil.Game
open Morgemil.Map
open Morgemil.Math
open Morgemil.Test

let Instruct() = printfn "(E)ast (N)orth (W)est (S)outh (Q)uit"

let Prompt() = 
  let response = System.Console.ReadLine()
  match System.Char.ToLower(response.[0]) with
  | 'e' -> Some(Actions.MoveEast)
  | 'n' -> Some(Actions.MoveNorth)
  | 'w' -> Some(Actions.MoveWest)
  | 's' -> Some(Actions.MoveSouth)
  | _ -> None

let rec Continue (depth : int) (walkabout : Walkabout) = 
  System.Console.WriteLine(walkabout.Player.Position)
  match Prompt() with
  | None -> ()
  | Some(act) -> 
    let filename = "map_test" + depth.ToString("0000") + ".bmp"
    let dungeonDraw = DungeonVisualizer.Visualize walkabout.Dungeon
    DungeonVisualizer.DrawPlayer walkabout.Player dungeonDraw
    dungeonDraw.Save(filename)
    Continue (depth + 1) (walkabout.Act act)

[<EntryPoint>]
let main argv = 
  let createdBspDungeon = 
    DungeonGeneration.Generate { Type = DungeonGenerationType.BSP
                                 Depth = 1
                                 RngSeed = 656556 }
  
  //Assumes there is at least one entrance. Takes the first one
  let entrance = 
    (TileModifier.Entrance(Rectangle(Vector2i(5, 5), Vector2i(1))) :: createdBspDungeon.TileModifiers)
    |> List.choose (function 
         | TileModifier.Entrance(location) -> Some(location)
         | _ -> None)
    |> List.rev
    |> List.head
  
  //Replace the player
  let walkAbout = 
    Walkabout(createdBspDungeon, 
              { Id = 5
                Race = Race.Lookup.[0]
                Position = entrance.Position })
  
  Instruct()
  Continue 0 walkAbout
  //  let filename2 = "map_test2.bmp"
  //  let dungeonDraw2 = Morgemil.Test.DungeonVisualizer.Visualize(createdBspDungeon)
  //  dungeonDraw2.Save(filename2)
  //System.Console.ReadKey() |> ignore
  0 // return an integer exit code
