namespace Morgemil.Logic

///The callback to handle a request and results.
type EventMessageHandler = EventResult -> TurnStep

///Receives and processes events that make up a turn.
///Example: The action of moving onto a tile with a trap causes a new message chain starting with the trap's activation.
module TurnQueue = 
  let HandleMessages (handler : EventMessageHandler) (initialRequest : EventResult) : TurnStep = 
    let _processedEvents = new System.Collections.Generic.List<EventResult>()
    let _eventQueue = new System.Collections.Generic.Queue<EventResult>()
    
    let rec _handle() = 
      match _eventQueue.Count with
      | 0 -> ()
      | _ -> 
        let request = _eventQueue.Dequeue()
        request |> _processedEvents.Add
        request
        |> handler
        |> Seq.iter _eventQueue.Enqueue
        _handle()
    _eventQueue.Enqueue(initialRequest)
    _handle()
    (List.ofSeq _processedEvents)
