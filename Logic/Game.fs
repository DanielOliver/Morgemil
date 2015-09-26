namespace Morgemil.Logic

open Morgemil.Core
open Morgemil.Logic.Extensions

type Game(level : Level, entities : seq<Entity>, positions : seq<PositionComponent>, players : seq<PlayerComponent>, resources : seq<ResourceComponent>) = 
  
  let mutable _entities = 
    [ for ent in entities -> ent.Id, ent ]
    |> Map.ofSeq
  
  let mutable _positions = 
    [ for pos in positions -> pos.Entity.Id, pos ]
    |> Map.ofSeq
  
  let mutable _players = 
    [ for cont in players -> cont.Entity.Id, cont ]
    |> Map.ofSeq
  
  let mutable _resources = 
    [ for res in resources -> res.Entity.Id, res ]
    |> Map.ofSeq
  
  //TODO: fix
  let mutable _globalTurnQueue = entities |> List.ofSeq
  
  let _handleRequest request = 
    TurnBuilder () { 
      match request with
      | EventResult.EntityMovementRequested(req) -> //TODO: moveEntity        
        let oldPosition = _positions.[req.EntityId]
        let newPositionVec = oldPosition.Position + req.Direction
        //TODO: Check that this move is actually valid
        _positions <- _positions.Replace(req.EntityId, fun old -> { old with Position = newPositionVec })
        //Movement takes one resource. More for testing purposes. 
        let oldResource = _resources.[req.EntityId]
        let newResourceAmount = oldResource.ResourceAmount - 1.0
        _resources <- _resources.Replace(req.EntityId, fun old -> { old with ResourceAmount = newResourceAmount })
        yield EventResult.EntityResourceChanged { Entity = _entities.[req.EntityId]
                                                  OldValue = oldResource.ResourceAmount
                                                  NewValue = newResourceAmount
                                                  ResourceChanged = oldResource.ResourceAmount - newResourceAmount }
        yield EventResult.EntityMoved { Entity = oldPosition.Entity
                                        MovedFrom = oldPosition.Position
                                        MovedTo = newPositionVec }
      | _ -> ()
    }
  
  member this.Update() = 
    let nextEntity = _globalTurnQueue.Head //TODO: Actually have more than one entity (the player)
    let player = _players.[nextEntity.Id]
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
