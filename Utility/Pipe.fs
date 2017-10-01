namespace Morgemil.Utility

///Each of the types of messages that a Pipe's Agent may receive.
type Message<'a, 'b> = 
    | Message of 'a
    | End of AsyncReplyChannel<unit>
    | Collect of AsyncReplyChannel<'a list>

///An alias for 
type Agent<'a, 'b> = MailboxProcessor<Message<'a, 'b>>

///A Pipe is a function that feeds all of its data to the next Agent in the Pipe.
[<RequireQualifiedAccess>]
type Pipe<'a, 'b> =
    Pipe of (Agent<'a, 'b> -> unit)

///A series of operators to help create a sequential "Pipe" of data in the same way an actor pipe would.
module Pipe =

    let private mapAgent (nextAgent: Agent<_,_>) map =
        MailboxProcessor.Start(fun inbox->
            let rec messageLoop() = async{
                let! msg = inbox.Receive()
                match msg with
                | Message(x) ->
                    let mappedItem = map x
                    nextAgent.Post (Message mappedItem)
                    return! messageLoop()
                | End(replyChannel) ->
                    nextAgent.PostAndReply(End)
                    replyChannel.Reply()
                | Collect(replyChannel) ->
                    replyChannel.Reply []
                    return! messageLoop()
            }
            messageLoop() 
        )

    let private mapAgentAsync (nextAgent: Agent<_,_>) map =
        MailboxProcessor.Start(fun inbox->
            let rec messageLoop() = async{
                let! msg = inbox.Receive()
                match msg with
                | Message(x) ->
                    let! mappedItem = map x
                    nextAgent.Post (Message mappedItem)
                    return! messageLoop()
                | End(replyChannel) ->
                    nextAgent.PostAndReply(End)
                    replyChannel.Reply()
                | Collect(replyChannel) ->
                    replyChannel.Reply []
                    return! messageLoop()
            }
            messageLoop() 
        )

    let private filterAgent (nextAgent: Agent<_,_>) condition =
        MailboxProcessor.Start(fun inbox->
            let rec messageLoop() = async{
                let! msg = inbox.Receive()
                match msg with
                | Message(x) ->
                    let isAcceptable = condition x
                    if isAcceptable then
                        nextAgent.Post msg
                    return! messageLoop()
                | End(replyChannel) ->
                    nextAgent.PostAndReply(End)
                    replyChannel.Reply()
                | Collect(replyChannel) ->
                    replyChannel.Reply []
                    return! messageLoop()
            }
            messageLoop() 
        )

    let private filterAgentAsync (nextAgent: Agent<_,_>) condition =
        MailboxProcessor.Start(fun inbox->
            let rec messageLoop() = async{
                let! msg = inbox.Receive()
                match msg with
                | Message(x) ->
                    let! isAcceptable = condition x
                    if isAcceptable then
                        nextAgent.Post msg
                    return! messageLoop()
                | End(replyChannel) ->
                    nextAgent.PostAndReply(End)
                    replyChannel.Reply()
                | Collect(replyChannel) ->
                    replyChannel.Reply []
                    return! messageLoop()
            }
            messageLoop() 
        )
                
    let private iterAgent action =
        MailboxProcessor.Start(fun inbox->
            let rec messageLoop() = async{
                let! msg = inbox.Receive()
                match msg with
                | Message(x) ->
                    action x
                    return! messageLoop()
                | End(replyChannel) ->
                    replyChannel.Reply()
                | Collect(replyChannel) ->
                    replyChannel.Reply []
                    return! messageLoop()
            }
            messageLoop() 
        )
                
    let private iterAgentAsync action =
        MailboxProcessor.Start(fun inbox->
            let rec messageLoop() = async{
                let! msg = inbox.Receive()
                match msg with
                | Message(x) ->
                    do! action x
                    return! messageLoop()
                | End(replyChannel) ->
                    replyChannel.Reply()
                | Collect(replyChannel) ->
                    replyChannel.Reply []
                    return! messageLoop()
            }
            messageLoop() 
        )
              
    let private collectAgent() =
        MailboxProcessor.Start(fun inbox->
            let rec messageLoop(current, isCached) = async{
                let! msg = inbox.Receive()
                match msg with
                | Message(next) ->                    
                    return! messageLoop( next :: current, isCached)
                | End(replyChannel) ->
                    replyChannel.Reply()
                    return! messageLoop(current, true)
                | Collect(replyChannel) ->
                    replyChannel.Reply(current |> List.rev)
            }
            messageLoop([],false)
        )
        
    ///Creates a pipe from a sequence of items.
    let From (items: seq<'a>): Pipe<'a, 'b> =
        Pipe.Pipe(
            fun nextAgent -> 
                let agent = mapAgent nextAgent id
                items |> Seq.iter (Message >> agent.Post)
                agent.PostAndReply(End) |> ignore
        )
        
    ///Maps items in the Pipe to a new item.
    let Map (map: 'a -> 'b) (pipe: Pipe<_,_>): Pipe<_,_> =
        Pipe.Pipe(
            fun nextAgent ->
                let agent = mapAgent nextAgent map
                let (Pipe.Pipe previousPipeCall) = pipe
                previousPipeCall agent
        )
        
    ///Maps items in the Pipe to a new item asynchronously.
    let MapAsync (map: 'a -> Async<'b>) (pipe: Pipe<_,_>): Pipe<_,_> =
        Pipe.Pipe(
            fun nextAgent ->
                let agent = mapAgentAsync nextAgent map
                let (Pipe.Pipe previousPipeCall) = pipe
                previousPipeCall agent
        )
                  
    ///Filter items in the Pipe.
    let Filter (condition: 'a -> bool) (pipe: Pipe<_,_>): Pipe<_,_> =
        Pipe.Pipe(
            fun nextAgent ->
                let agent = filterAgent nextAgent condition
                let (Pipe.Pipe previousPipeCall) = pipe
                previousPipeCall agent
        )

    ///Filter items in the Pipe asynchronously.
    let FilterAsync (condition: 'a -> Async<bool>) (pipe: Pipe<_,_>): Pipe<_,_> =
        Pipe.Pipe(
            fun nextAgent ->
                let agent = filterAgentAsync nextAgent condition
                let (Pipe.Pipe previousPipeCall) = pipe
                previousPipeCall agent
        )

    ///Iterate over all items in the Pipe.
    let Iter (action: 'a -> unit) (pipe: Pipe<'a,'b>) =
        let agent = iterAgent action
        let (Pipe.Pipe previousPipeCall) = pipe
        previousPipeCall agent
        agent.PostAndReply(End)
        
    ///Collects all results of Pipe into a list.
    let Collect (pipe: Pipe<'a,'b>) =
        let agent = collectAgent()
        let (Pipe.Pipe previousPipeCall) = pipe
        previousPipeCall agent
        agent.PostAndReply(Collect)
    
    ///Only evaluates Pipe once, lazily.
    let Cache (pipe: Pipe<'a,'b>) =
        let (Pipe.Pipe previousPipeCall) = pipe
        let agent = lazy(collectAgent())
        let items = lazy (
            previousPipeCall (agent.Force())
            agent.Force().PostAndReply(Message.Collect)
        )
        Pipe.Pipe(
            fun nextAgent ->
                items.Force()
                |> Seq.iter (Message >> nextAgent.Post)
        )
