namespace Morgemil.Logic

///Receives and processes events that make up a turn.
///Example: The action of moving onto a tile with a trap causes a new message chain starting with the trap's activation.
type TurnQueue() = 
  let _queue = new System.Collections.Generic.Queue<Message>()
  member this.Emit(message : Message) : unit = _queue.Enqueue(message)
  member this.Pop() = _queue.Dequeue
  member this.Empty() = _queue.Count = 0
