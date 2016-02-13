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
        if not (_world.Level.Tile(newPosition).BlocksMovement) && positionComponent.Mobile then 
          let newPositionComponent = { positionComponent with Position = newPosition }
          _world.Positions.[req.EntityId] <- newPositionComponent
          yield Message.PositionChange(positionComponent, newPositionComponent)
          //TODO: Spend resources?
          _world.Actions.Act(req.EntityId, 1.0<GameTime>)
        else _world.Actions.Act(req.EntityId, 0.0<GameTime>)
      | _ -> ()
    }
  
  member this.HumanRequest(request : RequestedMovement) = 
    let nextAction = _world.Actions.Next()
    let results = TurnQueue.HandleMessages _handleRequest (EventResult.EntityMovementRequested request)
    //TODO: Display results through gui
    //    printfn ""
    //    printfn "Current Entity %A" nextEntity.EntityId
    //    printfn "Current time %f" _world.Actions.CurrentTime
    results |> Seq.iter (fun res -> printfn "%A" res)
    //    let currentEntity() = _world.Entity(_world.Actions.Next.EntityId)
    //    while not (currentEntity().Player.Value.IsHumanControlled) do
    //      let ne = _world.Actions.StepToNext()
    //      printfn ""
    //      printfn "###########  BEGIN AI TURN"
    //      printfn "Current time %f" _world.Actions.CurrentTime
    //      printfn "%A" ne
    //      _world.Actions.Act(ne.EntityId, 0.8<GameTime>)
    //      printfn "###########  END AI TURN"
    _world.Actions.[nextAction.EntityId] <- { nextAction with TimeOfNextAction = 
                                                                nextAction.TimeOfNextAction + 1.0<GameTime>
                                                              TimeOfRequest = nextAction.TimeOfNextAction }
    printfn ""
    true
