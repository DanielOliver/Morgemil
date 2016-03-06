namespace Morgemil.Logic

open Morgemil.Core
open Morgemil.Math

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

type TurnStep = List<EventResult>
