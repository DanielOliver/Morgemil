namespace Morgemil.Logic

open Morgemil.Core

type RequestedMovement = 
  { EntityId : EntityId
    Direction : Vector2i }

type ResultMoved = 
  { EntityId : EntityId
    MovedFrom : Vector2i
    MovedTo : Vector2i }

type ResultResourceChanged = 
  { EntityId : EntityId
    OldValue : double
    NewValue : double }
  member this.ResourceChanged = this.OldValue - this.NewValue

///This represents the results of an action
type EventResult = 
  | EntityMoved of ResultMoved
  | EntityMovementRequested of RequestedMovement
  | EntityResourceChanged of ResultResourceChanged

type TurnStep = List<EventResult>
