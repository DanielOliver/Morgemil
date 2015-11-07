namespace Morgemil.Logic

open Morgemil.Core

type ActionSystem(initial, initialTime) as this = 
  inherit ComponentSystem<ActionComponent>(initial, (fun action -> action.EntityId))
  let mutable _currentTime : float<GameTime> = initialTime
  let _next() = this.Components |> Seq.minBy (fun x -> x.TimeOfNextAction)
  let _stepTo (time : float<GameTime>) = _currentTime <- time
  member this.CurrentTime = _currentTime
  
  member this.StepToNext() = 
    let next = _next()
    _stepTo (next.TimeOfNextAction)
    this.Remove(next)
    next
  
  member this.Act(entityId, timeUntil) = 
    this.Add({ ActionComponent.EntityId = entityId
               TimeOfNextAction = timeUntil + _currentTime })
  
  static member Empty = ActionSystem(Seq.empty, 0.0<GameTime>)
