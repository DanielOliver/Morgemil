module Program

open Morgemil.Core
open Morgemil.Logic

[<EntryPoint>]
let main argv = 
  let player = 
    { Entity.Id = EntityId 3
      Type = EntityType.Person }
  
  let position = 
    { PositionComponent.EntityId = player.Id
      Position = Vector2i(5, 5)
      Mobile = true }
  
  let controller = 
    { PlayerComponent.EntityId = player.Id
      IsHumanControlled = true }
  
  let resource = 
    { ResourceComponent.EntityId = player.Id
      Stamina = 50.0<Stamina> }
  
  let level = 
    Morgemil.Core.DungeonGeneration.Generate({ DungeonParameter.Depth = 1
                                               RngSeed = 5
                                               Type = DungeonGenerationType.Square })
  
  let game = 
    Morgemil.Logic.Game(level, [| player |], [| position |], 
                        [| controller
                           { PlayerComponent.EntityId = EntityId 0
                             IsHumanControlled = false } |], [| resource |])
  
  let rec _continue() = 
    let key = System.Console.ReadKey()
    
    let direction = 
      match key.Key with
      | System.ConsoleKey.W -> Vector2i(-1, 0)
      | System.ConsoleKey.E -> Vector2i(1, 0)
      | System.ConsoleKey.N -> Vector2i(0, -1)
      | System.ConsoleKey.S -> Vector2i(0, 1)
      | _ -> Vector2i()
    game.HumanRequest({ RequestedMovement.EntityId = player.Id
                        Direction = direction })
    |> ignore
    _continue()
  
  _continue()
  0
