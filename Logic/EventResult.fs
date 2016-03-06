namespace Morgemil.Logic

open Morgemil.Core
open Morgemil.Math

type EventResultType = 
  | EntityMoved
  | EntityMovementRequested
  | EntityResourceChanged
  | Exit

type RequestedMovement = 
  { EntityId : EntityId
    Direction : Vector2i }
  member this.MovementMultiplier() = (this.Direction.Length) * 1.0<GameTime>

type ResultMoved = 
  { EntityId : EntityId
    MovedFrom : Vector2i
    MovedTo : Vector2i }

type ResultResourceChanged = 
  { EntityId : EntityId
    OldValue : ResourceComponent
    NewValue : ResourceComponent }
  member this.StaminaChange = this.NewValue.Stamina - this.OldValue.Stamina

///This represents the results of an action
type EventResult = 
  | EntityMoved of ResultMoved
  | EntityMovementRequested of RequestedMovement
  | EntityResourceChanged of ResultResourceChanged
  | Exit
  
  member this.Type = 
    match this with
    | EventResult.EntityMoved(_) -> EventResultType.EntityMoved
    | EventResult.EntityMovementRequested(_) -> EventResultType.EntityMovementRequested
    | EventResult.EntityResourceChanged(_) -> EventResultType.EntityResourceChanged
    | _ -> EventResultType.Exit
  
type TurnStep = List<EventResult>
