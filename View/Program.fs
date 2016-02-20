module Morgemil.View.Program

open Morgemil.Core

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
                           Position = Vector2i(5, 5)
                           Mobile = false }) ]
  
  let worldOne = Morgemil.Logic.World(level, components)
  Morgemil.Utility.WorldLoader.SaveWorld("output.txt", worldOne)
  let worldTwo = Morgemil.Utility.WorldLoader.OpenWorld("output.txt")
  0
