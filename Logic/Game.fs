namespace Morgemil.Logic

open Morgemil.Core
open Morgemil.Math

type Game(world : World, callback : EntityId -> EntityAction) = 
  let _world = world
  let _actions = ActionSystem(world)
  let _positions = PositionSystem(world)
  let _resources = ResourceSystem(world)
  let _players = PlayerSystem(world)
  //Completely Fake AI
  let mutable one = false
  
  let _artificalPlayer (action : ActionComponent) = 
    one <- not (one)
    EntityAction.Movement(Vector2i(if one then 1 else -1))
  
  let _goDownStairs() = ()

  let _requestedMovement (request : RequestedMovement) = 
    TurnBuilder () { 
      let positionComponent = _positions.[request.EntityId]
      let newPosition = positionComponent.Position + request.Direction
      let movementcost = System.Math.Round(decimal (request.Direction.Length), 2) * 1M<GameTime>
      //If able to move, move
      if not (_world.Level.BlocksMovement(newPosition)) && positionComponent.Mobile then 
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
      else _actions.Act(request.EntityId, movementcost)
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
    
    let currentEntity = nextAction.EntityId

    let action, IsHumanControlled  = 
      match _players.Find(currentEntity) with
      | Some(x) when x.IsHumanControlled -> callback currentEntity, true
      | _ -> _artificalPlayer nextAction, false

    match action with
    | EntityAction.Exit -> ()
    | EntityAction.DownStairs -> _goDownStairs() 
    | _ -> 
      let request = EventResult.FromEntityAction currentEntity action
      let results = TurnQueue.HandleMessages _handleRequest request
      
      if IsHumanControlled then
        printfn "EntityID %A" nextAction.EntityId
        printfn "Current time %f       Time since last %f" _world.CurrentTime nextAction.Duration
        results |> Seq.iter (fun res -> printfn "%A" res)
        printfn ""

      _world.Entities.Free()
      this.Loop()
