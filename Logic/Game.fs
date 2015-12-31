namespace Morgemil.Logic

open Morgemil.Core

type Game(level : Level, entities : seq<ComponentAggregator>) =
  let _world =
    World(level, Seq.empty, entities)

  let _handleTrigger (event : EventResult) (trigger : Trigger) =
    TriggerBuilder (trigger) {
      match event, trigger with
      | EventResult.EntityMoved(req), Trigger.Empty(x, y, z) -> return TriggerStatus.Done
      | _ -> ()
    }

  let _handleRequest request =
    TurnBuilder () {
      yield _world.Triggers.Handle(_handleTrigger request)
      match request with
      | EventResult.EntityMovementRequested(req) ->
        let entity = _world.Entities.[req.EntityId]
//        let position = entity.Position.Value
//        let newPosition = position.Position + req.Direction
//        if not (_world.Level.Tile(newPosition).BlocksMovement) && position.Mobile then
//          yield Message.PositionChange
//                  (_world.Spatial.Replace(req.EntityId, fun _ -> { position with Position = newPosition }))
//          yield Message.ResourceChange
//                  (_world.Resources.Replace(req.EntityId, fun old -> { old with Stamina = old.Stamina - 1.0<Stamina> }))
//          _world.Actions.Act(req.EntityId, 1.0<GameTime>)
//        else _world.Actions.Act(req.EntityId, 0.0<GameTime>)
        ()
      | _ -> ()
    }

  //TODO: REMOVE TRIGGER TEST CODE
  do _world.Triggers.Add(fun t -> Trigger.Empty(_world.Entities.Generate().EntityId, { EmptyTrigger.Name = "" }, t)) |> ignore
//
//  do
//    _world.Players.Components
//    |> Seq.mapi (fun i t ->
//         { ActionComponent.EntityId = t.EntityId
//           TimeOfRequest = 0.0<GameTime>
//           TimeOfNextAction = 1.0<GameTime> / ((float i) + 1.0) })
//    |> Seq.iter (_world.Actions.Add)

  ///Humans can only move units right now
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