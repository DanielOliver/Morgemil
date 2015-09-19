namespace Morgemil.Logic

///Discriminated union of all possible messages created
type EventMessage = 
  | EntitySummoning
  | EntityFatality
  | EntityMovement

///The results of an event
type EventResult = 
  | EntitySummoned
  | EntityDeath
  | EntityMoved

///Emits a new message
type EventMessageEmit = EventMessage -> unit

///The callback to handle a message.
type EventMessageHandler = EventMessageEmit -> EventMessage -> EventResult
