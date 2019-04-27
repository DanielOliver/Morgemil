namespace Morgemil.Core

open Morgemil.Models

type SimpleGameStateMachine(gameLoop: ActionRequest -> Step list) =
 
    let loopWorkAgent = MailboxProcessor<GameStateRequest>.Start(fun inbox ->
        let processRequest (actionRequest: ActionRequest) (callback: Step list -> unit) =
            async {
                let results = gameLoop actionRequest
                callback results
            }
            |> Async.Start
            
        let inputFunc = (GameStateRequest.Input >> inbox.Post)

        let rec loop(previousState: GameState) =
            async {
                let mutable currentState: GameState = previousState
                let! message = inbox.Receive();
                match message with
                | GameStateRequest.Input request ->
                    match currentState with
                    | GameState.WaitingForInput _ ->
                        processRequest request (GameStateRequest.SetResults >> inbox.Post)
                        currentState <- GameState.Processing
                    | GameState.Processing
                    | GameState.Results _ ->
                        ()
                | GameStateRequest.SetResults results ->
                    currentState <- GameState.Results (results, (fun () -> inbox.Post GameStateRequest.Acknowledge))
                | GameStateRequest.QueryState replyChannel ->
                    replyChannel.Reply currentState
                | GameStateRequest.Kill ->
                    ()
                | GameStateRequest.Acknowledge ->
                    match currentState with
                    | GameState.Results _ ->
                        currentState <- GameState.WaitingForInput inputFunc
                    | GameState.Processing
                    | GameState.WaitingForInput _ ->
                        ()
                        
                match message with
                | GameStateRequest.Kill ->
                    ()
                | _ ->
                    do! loop(currentState)
            }
        loop(GameState.WaitingForInput inputFunc)
        )

    interface IGameStateMachine with
        /// Stops
        member this.Stop() =
            GameStateRequest.Kill
            |> loopWorkAgent.Post
        /// Gets the current state of the game loop
        member this.CurrentState
            with get(): GameState =
                loopWorkAgent.PostAndReply((fun replyChannel ->
                    GameStateRequest.QueryState replyChannel
                    ), 5000)


