namespace Morgemil.Logic

open Morgemil.Core
open Morgemil.Logic.Extensions

type GameState = 
  | AiTurn
  | HumanTurn

type Game(level : Level, entities : seq<Entity>, positions : seq<PositionComponent>, controllers : seq<ControlComponent>) = 
  
  let mutable _entities = 
    [ for ent in entities -> ent.Id, ent ]
    |> Map.ofSeq
  
  let mutable _positions = 
    [ for pos in positions -> pos.Entity.Id, pos ]
    |> Map.ofSeq
  
  let mutable _controllers = 
    [ for cont in controllers -> cont.Entity.Id, cont ]
    |> Map.ofSeq
  
  let mutable _resources = 
    [ for ent in entities -> 
        ent.Id, 
        { ResourceComponent.Entity = ent
          ResourceAmount = 50.0 } ] //TODO: fix. Start off with 50 resources because testing
    |> Map.ofSeq
  
  //TODO: fix
  let mutable _globalTurnQueue = entities |> List.ofSeq
  let mutable _currentGameState = GameState.AiTurn
  
  let _handleRequest (emit : EventRequestEmit) request = 
    match request with
    | EventRequest.EntityMovement(req) -> //TODO: moveEntity        
      let oldPosition = _positions.[req.EntityId]
      let newPosition = oldPosition.Position + req.Direction
      //TODO: Check that this move is actually valid
      _positions <- _positions.Replace(req.EntityId, fun old -> { old with Position = newPosition })
      //Movement takes one resource. More for testing purposes. 
      emit (EventRequest.EntityResourceChange { EntityId = oldPosition.Entity.Id
                                                ResourceChange = -1.0 })
      Some(EventResult.EntityMoved { Entity = oldPosition.Entity
                                     MovedFrom = oldPosition.Position
                                     MovedTo = newPosition })
    | EventRequest.EntityResourceChange(req) -> 
      let oldResource = _resources.[req.EntityId]
      let newResourceAmount = oldResource.ResourceAmount + req.ResourceChange
      _resources <- _resources.Replace(req.EntityId, fun old -> { old with ResourceAmount = newResourceAmount })
      Some(EventResult.EntityResourceChange { Entity = _entities.[req.EntityId]
                                              OldValue = oldResource.ResourceAmount
                                              NewValue = newResourceAmount
                                              ResourceChanged = req.ResourceChange })
    | _ -> None
  
  member this.Update() = 
    let nextEntity = _globalTurnQueue.Head
    let controller = _controllers.[nextEntity.Id]
    _currentGameState <- match controller.IsHumanControlled with
                         | true -> GameState.HumanTurn
                         | false -> GameState.AiTurn
    //TODO: process AI turn
    _currentGameState
  
  ///Returns whether the human had a valid request
  member this.HumanRequest(request : EventRequest) = 
    match _currentGameState with
    | GameState.HumanTurn -> 
      let (processedEvents, results) = TurnQueue.HandleMessages _handleRequest request
      //TODO: Display results through gui
      printfn ""
      results |> Seq.iter (fun res -> printfn "%A" res)
      true
    | GameState.AiTurn -> false
