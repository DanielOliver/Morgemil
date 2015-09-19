namespace Morgemil.Logic

///Receives and processes events that make up a turn.
///Example: The action of moving onto a tile with a trap causes a new message chain starting with the trap's activation.
module TurnQueue = 
  let HandleMessages(handler : EventMessageHandler) = 
    let _processedEvents = new System.Collections.Generic.List<EventMessage>()
    let _eventQueue = new System.Collections.Generic.Queue<EventMessage>()
    let _eventResults = new System.Collections.Generic.List<EventResult>()
    let _emit (message : EventMessage) = _eventQueue.Enqueue(message)
    let one : EventMessageHandler = fun emit msg -> EventResult.EntitySummoned
    
    let rec _handle() = 
      match _eventQueue.Count with
      | 0 -> ()
      | _ -> 
        let next = _eventQueue.Dequeue()
        next
        |> (handler _emit)
        |> _eventResults.Add
        next |> _processedEvents.Add
        _handle()
    _handle()
    (List.ofSeq _processedEvents, List.ofSeq _eventResults)
