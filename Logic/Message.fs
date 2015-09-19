namespace Morgemil.Logic

///This represents a "request" for an action.
type EventRequest = 
  | EntitySummoning
  | EntityFatality
  | EntityMovement

///This represents the results of a request for an action
type EventResult = 
  | EntitySummoned
  | EntityDeath
  | EntityMoved

///Emits a new Request
type EventRequestEmit = EventRequest -> unit

///The callback to handle a request and results.
type EventMessageHandler = EventRequestEmit -> EventRequest -> EventResult
