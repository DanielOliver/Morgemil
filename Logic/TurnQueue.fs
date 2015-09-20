namespace Morgemil.Logic

///The callback to handle a request and results.
type EventMessageHandler = EventRequest -> TurnStep

///Receives and processes events that make up a turn.
///Example: The action of moving onto a tile with a trap causes a new message chain starting with the trap's activation.
module TurnQueue = 
  let HandleMessages (handler : EventMessageHandler) (initialRequest : EventRequest) = 
    let _processedEvents = new System.Collections.Generic.List<EventRequest>()
    let _eventQueue = new System.Collections.Generic.Queue<EventRequest>()
    let _eventResults = new System.Collections.Generic.List<EventResult>()
    let _emit message = _eventQueue.Enqueue(message)
    
    let rec _handle() = 
      match _eventQueue.Count with
      | 0 -> ()
      | _ -> 
        let request = _eventQueue.Dequeue()
        request |> _processedEvents.Add
        let step = request |> handler
        step.Results |> _eventResults.AddRange
        step.Requests |> Seq.iter _eventQueue.Enqueue
        _handle()
    _eventQueue.Enqueue(initialRequest)
    _handle()
    (List.ofSeq _processedEvents, List.ofSeq _eventResults) //The processed messages are for development purposes. The eventResults matter though
