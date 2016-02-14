namespace Morgemil.Logic

open Morgemil.Core

type Game(level : Level) = 
  let _world = World(level)
  
  let _handleRequest request = 
    TurnBuilder () { 
      match request with
      | EventResult.EntityMovementRequested(req) -> 
        let positionComponent = _world.Positions.[req.EntityId]
        let newPosition = positionComponent.Position + req.Direction
        //If able to move, move
        if not (_world.Level.Tile(newPosition).BlocksMovement) && positionComponent.Mobile then 
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
          _world.Actions.Act(req.EntityId, 1.0<GameTime>)
        //else dont move
        else _world.Actions.Act(req.EntityId, 0.0<GameTime>)
      | _ -> ()
    }
  
  member this.HumanRequest(request : RequestedMovement) = 
    let nextAction = _world.Actions.Next()
    let results = TurnQueue.HandleMessages _handleRequest (EventResult.EntityMovementRequested request)
    results |> Seq.iter (fun res -> printfn "%A" res)
    _world.Entities.Free()
    printfn ""
    true
