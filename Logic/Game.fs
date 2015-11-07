namespace Morgemil.Logic

open Morgemil.Core

type Game(level : Level, entities : seq<Entity>, positions : seq<PositionComponent>, players : seq<PlayerComponent>, resources : seq<ResourceComponent>) = 
  let _world = 
    World(level, Set.ofSeq (positions), Set.ofSeq (resources), Set.ofSeq (players), Seq.empty, Seq.empty, Seq.empty)
  
  let _handleTrigger (event : EventResult) (trigger : Trigger) = 
    TriggerBuilder (trigger) { 
      match event, trigger with
      | EventResult.EntityMoved(req), Trigger.Empty(x, y, z) -> 
        yield Message.ResourceChange
                (_world.Resources.Replace
                   (req.EntityId, fun old -> { old with ResourceAmount = old.ResourceAmount - 1.0 }))
        return TriggerStatus.Done
      | _ -> ()
    }
  
  let _handleRequest request = 
    TurnBuilder () { 
      yield _world.Triggers.Handle(_handleTrigger request)
      match request with
      | EventResult.EntityMovementRequested(req) -> 
        yield Message.PositionChange
                (_world.Spatial.Replace(req.EntityId, fun old -> { old with Position = old.Position + req.Direction }))
        yield Message.ResourceChange
                (_world.Resources.Replace
                   (req.EntityId, fun old -> { old with ResourceAmount = old.ResourceAmount - 1.0 }))
      | _ -> ()
    }
  
  //TODO: REMOVE TRIGGER TEST CODE
  do _world.Triggers.Add(fun t -> Trigger.Empty(EntityId 5, { EmptyTrigger.Name = "" }, t)) |> ignore
  
  do 
    _world.Players.Components
    |> Seq.map (fun t -> 
         { ActionComponent.EntityId = t.EntityId
           TimeOfNextAction = 1.0<GameTime> })
    |> Seq.iter (_world.Actions.Add)
  
  ///Humans can only move units right now
  member this.HumanRequest(request : RequestedMovement) = 
    let nextEntity = _world.Actions.StepToNext()
    let results = TurnQueue.HandleMessages _handleRequest (EventResult.EntityMovementRequested request)
    _world.Actions.Act(nextEntity.EntityId, 1.0<GameTime>)
    //TODO: Display results through gui
    printfn ""
    printfn "Current time %f" _world.Actions.CurrentTime
    results |> Seq.iter (fun res -> printfn "%A" res)
    true
