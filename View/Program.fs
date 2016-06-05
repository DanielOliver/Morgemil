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
  let entityAI = EntityId(2)
  
  let components = 
    [ Component.Position({ PositionComponent.EntityId = entityFive
                           Position = Vector2i(5, 5)
                           Mobile = true })
      Component.Position({ PositionComponent.EntityId = entityAI
                           Position = Vector2i(5, 6)
                           Mobile = true })
      Component.Action({ ActionComponent.EntityId = entityFive
                         TimeOfNextAction = 1.0M<GameTime>
                         TimeOfRequest = 0.0M<GameTime> })
      Component.Action({ ActionComponent.EntityId = entityAI
                         TimeOfNextAction = 1.1M<GameTime>
                         TimeOfRequest = 0.1M<GameTime> })
      Component.Player({ PlayerComponent.EntityId = entityFive
                         IsHumanControlled = true })
      Component.Resource({ ResourceComponent.EntityId = entityFive
                           ResourceComponent.Stamina = 50.0<Stamina> }) ]
  
  let worldOne = Morgemil.Logic.World(level, components, 1.0M<GameTime>)
  
  let rec getAction() = 
    match Console.ReadKey().Key with
    | ConsoleKey.W -> 
      EventResult.EntityMovementRequested { RequestedMovement.EntityId = entityFive
                                            Direction = Vector2i(-1, 0) }
    | ConsoleKey.X -> 
      EventResult.EntityMovementRequested { RequestedMovement.EntityId = entityFive
                                            Direction = Vector2i(1, 0) }
    | ConsoleKey.D -> 
      EventResult.EntityMovementRequested { RequestedMovement.EntityId = entityFive
                                            Direction = Vector2i(0, 1) }
    | ConsoleKey.A -> 
      EventResult.EntityMovementRequested { RequestedMovement.EntityId = entityFive
                                            Direction = Vector2i(0, -1) }

    | ConsoleKey.Q -> 
      EventResult.EntityMovementRequested { RequestedMovement.EntityId = entityFive
                                            Direction = Vector2i(-1, -1) }
    | ConsoleKey.E -> 
      EventResult.EntityMovementRequested { RequestedMovement.EntityId = entityFive
                                            Direction = Vector2i(-1, 1) }
    | ConsoleKey.Z -> 
      EventResult.EntityMovementRequested { RequestedMovement.EntityId = entityFive
                                            Direction = Vector2i(1, -1) }
    | ConsoleKey.C -> 
      EventResult.EntityMovementRequested { RequestedMovement.EntityId = entityFive
                                            Direction = Vector2i(1, 1) }

    | ConsoleKey.Escape -> EventResult.Exit
    | _ -> getAction()
  
  let game = Morgemil.Logic.Game(worldOne, getAction)
  game.Loop()
  0
