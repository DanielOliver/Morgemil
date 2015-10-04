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
      | EventResult.EntityMovementRequested(req) -> //TODO: moveEntity        
        let oldPosition = _world.Spatial.[req.EntityId]
        let newPositionVec = oldPosition.Position + req.Direction
        //TODO: Check that this move is actually valid
        _world.Spatial.Replace(oldPosition, { oldPosition with Position = newPositionVec })
        //Movement takes one resource. More for testing purposes. 
        let oldResource = _world.Resources.[req.EntityId]
        let newResourceAmount = oldResource.ResourceAmount - 1.0
        _world.Resources.Replace(oldResource, { oldResource with ResourceAmount = newResourceAmount })
        yield EventResult.EntityResourceChanged { EntityId = req.EntityId
                                                  OldValue = oldResource.ResourceAmount
                                                  NewValue = newResourceAmount
                                                  ResourceChanged = oldResource.ResourceAmount - newResourceAmount }
        yield EventResult.EntityMoved { EntityId = oldPosition.EntityId
                                        MovedFrom = oldPosition.Position
                                        MovedTo = newPositionVec }
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
