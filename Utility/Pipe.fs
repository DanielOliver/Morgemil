namespace Morgemil.Utility

///Each of the types of messages that a Pipe's Agent may receive.
type Message<'a> = 
    | Message of 'a
    | End of AsyncReplyChannel<unit>
    | Done of AsyncReplyChannel<unit>
    | Collect of AsyncReplyChannel<'a list>

///An actor to process messages sequentially
type Agent<'a>(messageLoop) =
    let mailbox =
        MailboxProcessor<Message<'a>>.Start(messageLoop)
    ///Tells this agent a message
    member this.Tell msg = mailbox.Post <| Message msg
    ///Kills this agent
    member this.Die() = mailbox.PostAndReply End
    ///Indicates the previous agent is done sending messages.
    member this.Done() = mailbox.PostAndReply Done
    ///Used to return results from the end of an agent's processing
    member this.Collect() = mailbox.PostAndReply Collect
    
    interface System.IDisposable with
        member this.Dispose() = 
            this.Die()

///A Pipe is a function that feeds all of its data to the next Agent in the Pipe.
[<RequireQualifiedAccess>]
type Pipe<'a> =
    Pipe of (Agent<'a> -> unit)

///A series of operators to help create a sequential "Pipe" of data in the same way an actor pipe would.
module Pipe =

    let private mapAgent (nextAgent: Agent<_>) map =
        new Agent<_>(fun inbox->
            let rec messageLoop() = async{
                let! msg = inbox.Receive()
                match msg with
                | Message(x) ->
                    let mappedItem = map x
                    nextAgent.Tell mappedItem
                    return! messageLoop()
                | End(replyChannel) ->
                    replyChannel.Reply()
                | Done(replyChannel) ->
                    nextAgent.Done()
                    replyChannel.Reply()
                    return! messageLoop()
                | Collect(replyChannel) ->
                    replyChannel.Reply []
                    return! messageLoop()
            }
            messageLoop() 
        )

    let private mapAgentAsync (nextAgent: Agent<_>) map =
        new Agent<_>(fun inbox->
            let rec messageLoop() = async{
                let! msg = inbox.Receive()
                match msg with
                | Message(x) ->
                    let! mappedItem = map x
                    nextAgent.Tell mappedItem
                    return! messageLoop()
                | End(replyChannel) ->
                    replyChannel.Reply()
                | Done(replyChannel) ->
                    nextAgent.Done()
                    replyChannel.Reply()
                    return! messageLoop()
                | Collect(replyChannel) ->
                    replyChannel.Reply []
                    return! messageLoop()
            }
            messageLoop() 
        )

    let private filterAgent (nextAgent: Agent<_>) condition =
        new Agent<_>(fun inbox->
            let rec messageLoop() = async{
                let! msg = inbox.Receive()
                match msg with
                | Message(x) ->
                    let isAcceptable = condition x
                    if isAcceptable then
                        nextAgent.Tell x
                    return! messageLoop()
                | End(replyChannel) ->
                    replyChannel.Reply()
                | Done(replyChannel) ->
                    nextAgent.Done()
                    replyChannel.Reply()
                    return! messageLoop()                
                | Collect(replyChannel) ->
                    replyChannel.Reply []
                    return! messageLoop()
            }
            messageLoop() 
        )

    let private filterAgentAsync (nextAgent: Agent<_>) condition =
        new Agent<_>(fun inbox->
            let rec messageLoop() = async{
                let! msg = inbox.Receive()
                match msg with
                | Message(x) ->
                    let! isAcceptable = condition x
                    if isAcceptable then
                        nextAgent.Tell x
                    return! messageLoop()
                | End(replyChannel) ->
                    replyChannel.Reply()
                | Done(replyChannel) ->
                    nextAgent.Done()
                    replyChannel.Reply()
                    return! messageLoop()
                | Collect(replyChannel) ->
                    replyChannel.Reply []
                    return! messageLoop()
            }
            messageLoop() 
        )
                
    let private iterAgent action =
        new Agent<_>(fun inbox->
            let rec messageLoop() = async{
                let! msg = inbox.Receive()
                match msg with
                | Message(x) ->
                    action x
                    return! messageLoop()
                | End(replyChannel) ->
                    replyChannel.Reply()
                | Done(replyChannel) ->
                    replyChannel.Reply()
                    return! messageLoop()
                | Collect(replyChannel) ->
                    replyChannel.Reply []
                    return! messageLoop()
            }
            messageLoop()
        )
                
    let private iterAgentAsync action =
        new Agent<_>(fun inbox->
            let rec messageLoop() = async{
                let! msg = inbox.Receive()
                match msg with
                | Message(x) ->
                    do! action x
                    return! messageLoop()
                | End(replyChannel) ->
                    replyChannel.Reply()
                | Done(replyChannel) ->
                    replyChannel.Reply()
                    return! messageLoop()
                | Collect(replyChannel) ->
                    replyChannel.Reply []
                    return! messageLoop()
            }
            messageLoop() 
        )
              
    let private collectAgent() =
        new Agent<_>(fun inbox->
            let rec messageLoop(current, isCached) = async{
                let! msg = inbox.Receive()
                match msg with
                | Message(next) ->                    
                    return! messageLoop( next :: current, isCached)
                | End(replyChannel) ->
                    replyChannel.Reply()
                | Done(replyChannel) ->
                    replyChannel.Reply()
                    return! messageLoop(current |> List.rev, isCached)
                | Collect(replyChannel) ->
                    replyChannel.Reply(current)
                    return! messageLoop(current, isCached)
            }
            messageLoop([],false)
        )
        
    ///Creates a pipe from a sequence of items.
    let From (items: seq<'a>): Pipe<'a> =
        Pipe.Pipe(
            fun nextAgent -> 
                use agent = mapAgent nextAgent id
                items |> Seq.iter (agent.Tell)
                agent.Done()
        )
        
    ///Maps items in the Pipe to a new item.
    let Map (map: 'a -> 'b) (pipe: Pipe<_>): Pipe<_> =
        Pipe.Pipe(
            fun nextAgent ->
                use agent = mapAgent nextAgent map
                let (Pipe.Pipe previousPipeCall) = pipe
                previousPipeCall agent
        )
        
    ///Maps items in the Pipe to a new item asynchronously.
    let MapAsync (map: 'a -> Async<'b>) (pipe: Pipe<_>): Pipe<_> =
        Pipe.Pipe(
            fun nextAgent ->
                use agent = mapAgentAsync nextAgent map
                let (Pipe.Pipe previousPipeCall) = pipe
                previousPipeCall agent
        )
                  
    ///Filter items in the Pipe.
    let Filter (condition: 'a -> bool) (pipe: Pipe<_>): Pipe<_> =
        Pipe.Pipe(
            fun nextAgent ->
                use agent = filterAgent nextAgent condition
                let (Pipe.Pipe previousPipeCall) = pipe
                previousPipeCall agent
        )

    ///Filter items in the Pipe asynchronously.
    let FilterAsync (condition: 'a -> Async<bool>) (pipe: Pipe<_>): Pipe<_> =
        Pipe.Pipe(
            fun nextAgent ->
                use agent = filterAgentAsync nextAgent condition
                let (Pipe.Pipe previousPipeCall) = pipe
                previousPipeCall agent
        )

    ///Iterate over all items in the Pipe.
    let Iter (action: 'a -> unit) (pipe: Pipe<'a>) =
        use agent = iterAgent action
        let (Pipe.Pipe previousPipeCall) = pipe
        previousPipeCall agent
        
    ///Iterate over all items in the Pipe asynchronously.
    let IterAsync (action: 'a -> Async<unit>) (pipe: Pipe<'a>) =
        use agent = iterAgentAsync action
        let (Pipe.Pipe previousPipeCall) = pipe
        previousPipeCall agent

    ///Collects all results of Pipe into a list.
    let Collect (pipe: Pipe<'a>) =
        use agent = collectAgent()
        let (Pipe.Pipe previousPipeCall) = pipe
        previousPipeCall agent
        agent.Collect()
    
    ///Only evaluates Pipe once, lazily.
    let Cache (pipe: Pipe<'a>) =
        let (Pipe.Pipe previousPipeCall) = pipe
        let items = lazy (
            use agent = collectAgent()
            previousPipeCall (agent)
            agent.Collect()
        )
        Pipe.Pipe(
            fun nextAgent ->
                items.Force()
                |> Seq.iter nextAgent.Tell
                nextAgent.Done()
        )
