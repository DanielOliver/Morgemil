namespace Morgemil.Logic

open Morgemil.Core
open Morgemil.Math

type Game(world : World, callback : unit -> EventResult) = 
  let _world = world
  
  let _artificalPlayer (action : ActionComponent) = 
    EventResult.EntityMovementRequested({ RequestedMovement.EntityId = action.EntityId
                                          RequestedMovement.Direction = Vector2i(1) })
  
  let _handleRequest request = 
    TurnBuilder () { 
      match request with
      | EventResult.EntityMovementRequested(req) -> 
        let positionComponent = _world.Positions.[req.EntityId]
        let newPosition = positionComponent.Position + req.Direction
        //If able to move, move
        if not (_world.Level.Tile(newPosition).BlocksMovement) && positionComponent.Mobile then 
          let movementcost = System.Math.Round(decimal (req.Direction.Length), 2) * 1M<GameTime>
          let newPositionComponent = { positionComponent with Position = newPosition }
          _world.Positions.[req.EntityId] <- newPositionComponent
          yield Message.PositionChange(positionComponent, newPositionComponent)
          //Spend resources if possible
          match _world.Resources.Find(req.EntityId) with
          | Some(oldResources) -> 
            let newResources = { oldResources with Stamina = oldResources.Stamina - 1.0<Stamina> }
            _world.Resources.[req.EntityId] <- newResources
            yield Message.ResourceChange(oldResources, newResources)
          | None -> ()
          _world.Actions.Act(req.EntityId, movementcost, _world.CurrentTime)
        //else dont move
        else _world.Actions.Act(req.EntityId, 0.0M<GameTime>, _world.CurrentTime)
      | _ -> ()
    }
  
  member this.Loop() = 
    let nextAction = _world.Actions.Next()
    _world.CurrentTime <- nextAction.TimeOfNextAction
    let action = 
      match _world.Players.Find(nextAction.EntityId) with
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
