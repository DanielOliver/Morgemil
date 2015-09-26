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
    ResourceChanged : double
    OldValue : double
    NewValue : double }

///This represents the results of an action
type EventResult = 
  | EntityMoved of ResultMoved
  | EntityMovementRequested of RequestedMovement
  | EntityResourceChanged of ResultResourceChanged
