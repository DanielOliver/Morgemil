namespace Morgemil.Logic

open Morgemil.Core

type Game(level : Level) = 
  let _world = World(level)
  
  let _handleRequest request = 
    TurnBuilder () { 
      match request with
      | EventResult.EntityMovementRequested(req) -> ()
      //        let entity = _world.Entities.[req.EntityId]
      //        let position = entity.Position.Value
      //        let newPosition = position.Position + req.Direction
      //        if not (_world.Level.Tile(newPosition).BlocksMovement) && position.Mobile then
      //          yield Message.PositionChange
      //                  (_world.Spatial.Replace(req.EntityId, fun _ -> { position with Position = newPosition }))
      //          yield Message.ResourceChange
      //                  (_world.Resources.Replace(req.EntityId, fun old -> { old with Stamina = old.Stamina - 1.0<Stamina> }))
      //          _world.Actions.Act(req.EntityId, 1.0<GameTime>)
      //        else _world.Actions.Act(req.EntityId, 0.0<GameTime>)
      | _ -> ()
    }
  
  member this.HumanRequest(request : RequestedMovement) = 
    //    let nextEntity = _world.Actions.StepToNext()
    //    let results = TurnQueue.HandleMessages _handleRequest (EventResult.EntityMovementRequested request)
    //TODO: Display results through gui
    //    printfn ""
    //    printfn "Current Entity %A" nextEntity.EntityId
    //    printfn "Current time %f" _world.Actions.CurrentTime
    //    results |> Seq.iter (fun res -> printfn "%A" res)
    //    let currentEntity() = _world.Entity(_world.Actions.Next.EntityId)
    //    while not (currentEntity().Player.Value.IsHumanControlled) do
    //      let ne = _world.Actions.StepToNext()
    //      printfn ""
    //      printfn "###########  BEGIN AI TURN"
    //      printfn "Current time %f" _world.Actions.CurrentTime
    //      printfn "%A" ne
    //      _world.Actions.Act(ne.EntityId, 0.8<GameTime>)
    //      printfn "###########  END AI TURN"
    printfn ""
    true
