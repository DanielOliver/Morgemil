namespace Morgemil.Logic

open Morgemil.Core
open Morgemil.Logic.Extensions

type GameState = 
  | AiTurn
  | HumanTurn

type Game(level : Level, entities : seq<Entity>) = 
  
  ///Hardcoded test player
  let player = 
    { Entity.Id = 5
      Type = EntityType.Person }
  
  let mutable _entities = 
    [ for ent in entities -> ent.Id, ent ]
    |> Map.ofSeq
  
  let mutable _positions = 
    Map.empty.Add(player.Id, 
                  { PositionComponent.Entity = player
                    Mobile = true
                    Position = Vector2i(5, 5) })
  
  let mutable _controllers = 
    Map.empty.Add(player.Id, 
                  { ControlComponent.Entity = player
                    HumanControlled = true })
  
  let mutable _globalTurnQueue = entities |> List.ofSeq
  let mutable _currentGameState = GameState.AiTurn
  
  let _handleRequest (emit : EventRequestEmit) request = 
    match request with
    | EventRequest.EntityMovement(req) -> //TODO: moveEntity        
      let oldPosition = _positions.[req.EntityId]
      let newPosition = oldPosition.Position + req.Direction
      //TODO: Check that this move is actually valid
      Some(EventResult.EntityMoved { Entity = oldPosition.Entity
                                     MovedFrom = oldPosition.Position
                                     MovedTo = newPosition })
    | _ -> None
  
  member this.Update() = 
    let nextEntity = _globalTurnQueue.Head
    let controller = _controllers.[nextEntity.Id]
    _currentGameState <- match controller.HumanControlled with
                         | true -> GameState.HumanTurn
                         | false -> GameState.AiTurn
    //TODO: process AI turn
    _currentGameState
  
  ///Returns whether the human had a valid request
  member this.HumanRequest() = 
    match _currentGameState with
    | GameState.HumanTurn -> 
      let (processedEvents, results) = TurnQueue.HandleMessages _handleRequest
      //TODO: Display results better than debugger writing
      results |> Seq.iter (fun res -> System.Console.WriteLine(res))
      true
    | GameState.AiTurn -> false
