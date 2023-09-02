namespace Morgemil.Core

open Morgemil.Core
open Morgemil.Models

type SimpleGameStateMachine
    (
        gameLoop: ActionRequest -> Step list,
        waitingType: unit -> GameStateWaitingType,
        scenarioData: ScenarioData,
        nextMove: unit -> ActionRequest
    ) =

    let loopWorkAgent =
        MailboxProcessor<GameStateRequest>.Start(fun inbox ->
            let processRequest (actionRequest: ActionRequest) (callback: Step list -> unit) =
                async {
                    let results = gameLoop actionRequest
                    callback results
                }
                |> Async.Start

            let inputFunc = (GameStateRequest.Input >> inbox.Post)

            let resultQ = System.Collections.Generic.Queue<Step list>()

            let rec loop (previousState: GameState) =
                async {
                    let mutable currentState: GameState = previousState
                    let! message = inbox.Receive()

                    match message with
                    | GameStateRequest.Input request ->
                        match currentState with
                        | GameState.WaitingForInput _ ->
                            processRequest request (GameStateRequest.SetResults >> inbox.Post)
                            currentState <- GameState.Processing
                        | GameState.Processing
                        | GameState.Results _ -> ()
                    | GameStateRequest.SetResults results ->
                        if not results.IsEmpty then
                            resultQ.Enqueue(results)

                        match waitingType () with
                        | GameStateWaitingType.WaitingForInput -> currentState <- GameState.WaitingForInput inputFunc
                        | GameStateWaitingType.WaitingForEngine ->
                            processRequest (ActionRequest.Engine) (GameStateRequest.SetResults >> inbox.Post)
                        | GameStateWaitingType.WaitingForAI ->
                            processRequest (nextMove ()) (GameStateRequest.SetResults >> inbox.Post)
                            currentState <- GameState.Processing
                    | GameStateRequest.QueryState replyChannel ->
                        if resultQ.Count > 0 then
                            (resultQ.Peek(), (fun () -> inbox.Post GameStateRequest.Acknowledge))
                            |> GameState.Results
                            |> replyChannel.Reply
                        else
                            currentState |> replyChannel.Reply
                    | GameStateRequest.Kill -> ()
                    | GameStateRequest.Acknowledge ->
                        if resultQ.Count > 0 then
                            resultQ.Dequeue() |> ignore

                    match message with
                    | GameStateRequest.Kill -> ()
                    | _ -> do! loop (currentState)
                }

            loop (GameState.WaitingForInput inputFunc))

    do loopWorkAgent.Post(GameStateRequest.SetResults [])

    interface IGameStateMachine with
        /// Stops
        member this.Stop() =
            GameStateRequest.Kill |> loopWorkAgent.Post

        /// Gets the current state of the game loop
        member this.CurrentState: GameState =
            try
                loopWorkAgent.PostAndReply((fun replyChannel -> GameStateRequest.QueryState replyChannel), 500)
            with _ ->
                GameState.Processing

        /// Get Scenario Data
        member this.ScenarioData = scenarioData
