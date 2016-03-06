namespace Morgemil.Logic

open Morgemil.Core
open Morgemil.Math

type Game(world : World, callback : unit -> EventResult) = 
  let _world = world
  let _actions = ActionSystem(world)
  let _positions = PositionSystem(world)
  let _resources = ResourceSystem(world)
  let _players = PlayerSystem(world)
  //Completely Fake AI
  let mutable one = false
  
  let _artificalPlayer (action : ActionComponent) = 
    one <- not (one)
    EventResult.EntityMovementRequested({ RequestedMovement.EntityId = action.EntityId
                                          RequestedMovement.Direction = 
                                            Vector2i(if one then 1
                                                     else -1) })
  
  let _requestedMovement (request : RequestedMovement) = 
    TurnBuilder () { 
      let positionComponent = _positions.[request.EntityId]
      let newPosition = positionComponent.Position + request.Direction
      //If able to move, move
      if not (_world.Level.Tile(newPosition).BlocksMovement) && positionComponent.Mobile then 
        let movementcost = System.Math.Round(decimal (request.Direction.Length), 2) * 1M<GameTime>
        let newPositionComponent = { positionComponent with Position = newPosition }
        _positions.[request.EntityId] <- newPositionComponent
        yield Message.PositionChange(positionComponent, newPositionComponent)
        //Spend resources if possible
        match _resources.Find(request.EntityId) with
        | Some(oldResources) -> 
          let newResources = { oldResources with Stamina = oldResources.Stamina - 1.0<Stamina> }
          _resources.[request.EntityId] <- newResources
          yield Message.ResourceChange(oldResources, newResources)
        | None -> ()
        _actions.Act(request.EntityId, movementcost)
      //else dont move
      else _actions.Act(request.EntityId, 0.0M<GameTime>)
    }
  
  let _handleRequest (request, history : EventResult list) = 
    TurnBuilder () { 
      match request with
      | EventResult.EntityMovementRequested(req) -> yield _requestedMovement (req)
      | _ -> ()
    }
  
  member this.Loop() = 
    let nextAction = _actions.Next()
    _world.CurrentTime <- nextAction.TimeOfNextAction
    let action = 
      match _players.Find(nextAction.EntityId) with
      | Some(x) -> 
        match x.IsHumanControlled with
        | true -> callback()
        | _ -> _artificalPlayer nextAction
      | _ -> _artificalPlayer nextAction
    match action with
    | EventResult.Exit -> ()
    | _ -> 
      let results = TurnQueue.HandleMessages _handleRequest action
      results |> Seq.iter (fun res -> printfn "%A" res)
      printfn "time %f" _world.CurrentTime
      _world.Entities.Free()
      printfn ""
      this.Loop()
