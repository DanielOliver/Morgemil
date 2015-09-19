namespace Morgemil.Logic

open Morgemil.Core

type RequestMovement = 
  { EntityId : int
    Direction : Vector2i }

///This represents a "request" for an action.
type EventRequest = 
  | EntitySummoning
  | EntityFatality
  | EntityMovement of RequestMovement

type ResultMoved = 
  { Entity : Entity
    MovedFrom : Vector2i
    MovedTo : Vector2i }

///This represents the results of a request for an action
type EventResult = 
  | EntitySummoned
  | EntityDeath
  | EntityMoved of ResultMoved

///Emits a new Request
type EventRequestEmit = EventRequest -> unit

///The callback to handle a request and results.
type EventMessageHandler = EventRequestEmit -> EventRequest -> EventResult option
