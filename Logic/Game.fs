namespace Morgemil.Logic

open Morgemil.Core

type Game(level : Level, entities : seq<Entity>, positions : seq<PositionComponent>, players : seq<PlayerComponent>, resources : seq<ResourceComponent>) = 
  let _world = World(level, Set.ofSeq (positions), Set.ofSeq (resources), Set.ofSeq (players), Seq.empty, Seq.empty)
  //TODO: fix
  let mutable _globalTurnQueue = (entities |> Seq.toList).Head
  
  let _handleTrigger (event : EventResult) (trigger : Trigger) = 
    TriggerBuilder (trigger) { 
      match event, trigger with
      | EventResult.EntityMovementRequested(req), Trigger.Empty(x, y, z) -> 
        //        yield Message.PositionChange
        //                (_world.Spatial.Replace(req.EntityId, fun old -> { old with Position = old.Position + req.Direction }))
        yield Message.ResourceChange
                (_world.Resources.Replace
                   (req.EntityId, fun old -> { old with ResourceAmount = old.ResourceAmount - 1.0 }))
        return TriggerStatus.Continue
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
  
  member this.Update() = 
    let nextEntity = _globalTurnQueue //TODO: Actually have more than one entity (the player)
    let player = _world.Players.[nextEntity.Id]
    match player.IsHumanControlled with
    | true -> ()
    | false -> () //TODO: process AI turn
  
  ///Humans can only move units right now
  member this.HumanRequest(request : RequestedMovement) = 
    let results = TurnQueue.HandleMessages _handleRequest (EventResult.EntityMovementRequested request)
    //TODO: Display results through gui
    printfn ""
    results |> Seq.iter (fun res -> printfn "%A" res)
    true
