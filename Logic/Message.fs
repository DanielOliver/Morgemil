namespace Morgemil.Logic

open Morgemil.Core

type RequestMovement = 
  { EntityId : int
    Direction : Vector2i }

type RequestEntityResource = 
  { EntityId : int
    ResourceChange : double }

///This represents a "request" for an action.
type EventRequest = 
  | EntitySummoning
  | EntityFatality
  | EntityMovement of RequestMovement
  | EntityResourceChange of RequestEntityResource

type ResultMoved = 
  { Entity : Entity
    MovedFrom : Vector2i
    MovedTo : Vector2i }

type ResultResourceChange = 
  { Entity : Entity
    ResourceChanged : double
    OldValue : double
    NewValue : double }

///This represents the results of a request for an action
type EventResult = 
  | EntitySummoned
  | EntityDeath
  | EntityMoved of ResultMoved
  | EntityResourceChange of ResultResourceChange
