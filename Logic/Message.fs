namespace Morgemil.Logic

///Discriminated union of all possible messages created
type Message = 
  | EntitySummoning
  | EntityFatality

///The callback to emit a message.
type MessageEmit = Message -> unit
