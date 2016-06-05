namespace Morgemil.Logic

///The callback to handle a request and results.
type EventMessageHandler = EventResult * EventResult list -> TurnStep

///Receives and processes events that make up a turn.
///Example: The action of moving onto a tile with a trap causes a new message chain starting with the trap's activation.
module TurnQueue = 
  let HandleMessages (handler : EventMessageHandler) (initialRequest : EventResult) : TurnStep = 
    let mutable _processedEvents : EventResult list = list.Empty
    let _eventQueue = new System.Collections.Generic.Queue<EventResult list>()
    
    let rec _handle() = 
      match _eventQueue.Count with
      | 0 -> ()
      | _ -> 
        let request = _eventQueue.Dequeue()
        //Assume that this list is never empty
        match request with
        | head :: tail -> _processedEvents <- head :: _processedEvents
                          handler (head, tail) |> Seq.iter (fun t -> _eventQueue.Enqueue(t :: request))
                          _handle()
        | _ -> failwith "TurnQueue.HandleMessages expected items"
    _eventQueue.Enqueue([ initialRequest ])
    _handle()
    _processedEvents
