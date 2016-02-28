namespace Morgemil.Logic

open Morgemil.Core

type Game(world : World, callback : unit -> EventResult) = 
  let _world = world
  
  let _handleRequest request = 
    TurnBuilder () { 
      match request with
      | EventResult.EntityMovementRequested(req) -> 
        let positionComponent = _world.Positions.[req.EntityId]
        let newPosition = positionComponent.Position + req.Direction
        let movementCost = 1.0<GameTime>
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
          _world.Actions.Act(req.EntityId, movementCost, _world.CurrentTime)
        //else dont move
        else _world.Actions.Act(req.EntityId, movementCost, _world.CurrentTime)
      | _ -> ()
    }
  
  member this.Loop() = 
    let nextAction = _world.Actions.Next()
    let action = callback()
    let results = TurnQueue.HandleMessages _handleRequest action
    results |> Seq.iter (fun res -> printfn "%A" res)
    _world.Entities.Free()
    printfn ""
    this.Loop()
