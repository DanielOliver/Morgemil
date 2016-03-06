namespace Morgemil.Logic

open Morgemil.Core
open Morgemil.Math

type Game(world : World, callback : unit -> EventResult) = 
  let _world = world
  let _actions = ActionSystem(world)
  let _positions = PositionSystem(world)
  let _resources = ResourceSystem(world)
  let _players = PlayerSystem(world)
  let mutable one = false
  
  let _artificalPlayer (action : ActionComponent) = 
    one <- not (one)
    EventResult.EntityMovementRequested({ RequestedMovement.EntityId = action.EntityId
                                          RequestedMovement.Direction = 
                                            Vector2i(if one then 1
                                                     else -1) })
  
  let _handleRequest request = 
    TurnBuilder () { 
      match request with
      | EventResult.EntityMovementRequested(req) -> 
        let positionComponent = _positions.[req.EntityId]
        let newPosition = positionComponent.Position + req.Direction
        //If able to move, move
        if not (_world.Level.Tile(newPosition).BlocksMovement) && positionComponent.Mobile then 
          let movementcost = System.Math.Round(decimal (req.Direction.Length), 2) * 1M<GameTime>
          let newPositionComponent = { positionComponent with Position = newPosition }
          _positions.[req.EntityId] <- newPositionComponent
          yield Message.PositionChange(positionComponent, newPositionComponent)
          //Spend resources if possible
          match _resources.Find(req.EntityId) with
          | Some(oldResources) -> 
            let newResources = { oldResources with Stamina = oldResources.Stamina - 1.0<Stamina> }
            _resources.[req.EntityId] <- newResources
            yield Message.ResourceChange(oldResources, newResources)
          | None -> ()
          _actions.Act(req.EntityId, movementcost)
        //else dont move
        else _actions.Act(req.EntityId, 0.0M<GameTime>)
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
