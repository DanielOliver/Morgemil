namespace Morgemil.Logic

open Morgemil.Core
open Morgemil.Logic.Extensions

type Game(level : Level, entities : seq<Entity>, positions : seq<PositionComponent>, players : seq<PlayerComponent>, resources : seq<ResourceComponent>) = 
  let _world = World(level, Set.ofSeq (positions), Set.ofSeq (resources), Set.ofSeq (players))
  //TODO: fix
  let mutable _globalTurnQueue = (entities |> Seq.toList).Head
  
  let _handleRequest request = 
    TurnBuilder () { 
      match request with
      | EventResult.EntityMovementRequested(req) -> 
        let old_position = _world.Spatial.[req.EntityId]
        //TODO: Check that this move is actually valid
        let new_position = 
          _world.Spatial.Replace(old_position, { old_position with Position = old_position.Position + req.Direction })
        yield Message.PositionChange(old_position, new_position)
        //Movement takes one resource. More for testing purposes. 
        let old_resource = _world.Resources.[req.EntityId]
        let new_resource = 
          _world.Resources.Replace
            (old_resource, { old_resource with ResourceAmount = old_resource.ResourceAmount - 1.0 })
        yield Message.ResourceChange(old_resource, new_resource)
      | _ -> ()
    }
  
  member this.Update() = 
    let nextEntity = _globalTurnQueue //TODO: Actually have more than one entity (the player)
    let player = _world.Players.[nextEntity.Id]
    match player.IsHumanControlled with
    | true -> ()
    | false -> () //TODO: process AI turn
  
  ///Humans can only move units right now
  member this.HumanRequest(request : RequestedMovement) = 
    let results = TurnQueue.HandleMessages _handleRequest (EventResult.EntityMovementRequested request)
    //TODO: Display results through gui
    printfn ""
    results |> Seq.iter (fun res -> printfn "%A" res)
    true
