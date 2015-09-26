namespace Morgemil.Logic

open Morgemil.Core

type RequestedMovement = 
  { EntityId : int
    Direction : Vector2i }

type ResultMoved = 
  { Entity : Entity
    MovedFrom : Vector2i
    MovedTo : Vector2i }

type ResultResourceChanged = 
  { Entity : Entity
    ResourceChanged : double
    OldValue : double
    NewValue : double }

///This represents the results of an action
type EventResult = 
  | EntityMoved of ResultMoved
  | EntityMovementRequested of RequestedMovement
  | EntityResourceChanged of ResultResourceChanged
