namespace Morgemil.Logic

///Receives and processes events that make up a turn.
///Example: The action of moving onto a tile with a trap causes a new message chain starting with the trap's activation.
module TurnQueue = 
  let HandleMessages(handler : EventMessageHandler) (initialRequest: EventRequest) = 
    let _processedEvents = new System.Collections.Generic.List<EventRequest>()
    let _eventQueue = new System.Collections.Generic.Queue<EventRequest>()
    let _eventResults = new System.Collections.Generic.List<EventResult>()
    let _emit message = _eventQueue.Enqueue(message)
    
    let rec _handle() = 
      match _eventQueue.Count with
      | 0 -> ()
      | _ -> 
        _eventQueue.Dequeue()
        |> (fun request -> 
        request |> _processedEvents.Add
        request |> handler _emit)
        |> (function 
        | Some(x) -> x |> _eventResults.Add
        | _ -> ())
        _handle()

    _eventQueue.Enqueue(initialRequest)
    _handle()
    (List.ofSeq _processedEvents, List.ofSeq _eventResults) //The processed messages are for development purposes. The eventResults matter though
