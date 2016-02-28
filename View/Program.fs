module Morgemil.View.Program

open Morgemil.Core
open Morgemil.Logic
open Morgemil.Math
open System

[<EntryPoint>]
let main argv = 
  //  GameWindow.Start()
  let parameters = 
    { DungeonParameter.Depth = 5
      RngSeed = 50
      Type = DungeonGenerationType.Square }
  
  let level = Morgemil.Core.DungeonGeneration.Generate(parameters)
  let entityFive = EntityId(5)
  
  let components = 
    [ Component.Position({ PositionComponent.EntityId = entityFive
                           Position = Vector2i.From(5, 5)
                           Mobile = true })
      Component.Action({ ActionComponent.EntityId = entityFive
                         TimeOfNextAction = 1.0<GameTime>
                         TimeOfRequest = 0.0<GameTime> }) ]
  
  let worldOne = Morgemil.Logic.World(level, components, 1.0<GameTime>)
  Morgemil.Utility.WorldLoader.SaveWorld("output.txt", worldOne)
  let worldTwo = Morgemil.Utility.WorldLoader.OpenWorld("output.txt")
  
  let rec getAction() = 
    match Console.ReadKey().Key with
    | ConsoleKey.W -> 
      EventResult.EntityMovementRequested { RequestedMovement.EntityId = entityFive
                                            Direction = Vector2i.From(-1, 0) }
    | ConsoleKey.E -> 
      EventResult.EntityMovementRequested { RequestedMovement.EntityId = entityFive
                                            Direction = Vector2i.From(1, 0) }
    | ConsoleKey.S -> 
      EventResult.EntityMovementRequested { RequestedMovement.EntityId = entityFive
                                            Direction = Vector2i.From(0, 1) }
    | ConsoleKey.N -> 
      EventResult.EntityMovementRequested { RequestedMovement.EntityId = entityFive
                                            Direction = Vector2i.From(0, -1) }
    | _ -> getAction()
  
  let game = Morgemil.Logic.Game(worldTwo, getAction)
  game.Loop()
  0
