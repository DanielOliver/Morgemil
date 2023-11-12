namespace Morgemil.Core

open Morgemil.Core
open Morgemil.Models

type SimpleGameStateMachine
    (
        gameLoop: ActionRequest -> Step list,
        waitingType: unit -> GameStateWaitingType,
        scenarioData: ScenarioData,
        nextMove: unit -> ActionRequest,
        eventRecorder: EventRecorder
    ) =

    let loopWorkAgent =
        MailboxProcessor<GameStateRequest>.Start(fun inbox ->
            let processRequest (actionRequest: ActionRequest) (callback: Step list -> unit) =
                async {
                    eventRecorder.RecordActionRequest actionRequest
                    let results = gameLoop actionRequest
                    eventRecorder.RecordSteps results
                    callback results
                }
                |> Async.Start

            let inputFunc = (GameStateRequest.Input >> inbox.Post)
            let mutable resultQ: Step list = []

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
                            resultQ <- List.concat [ resultQ; results ]

                        match waitingType () with
                        | GameStateWaitingType.WaitingForInput -> currentState <- GameState.WaitingForInput inputFunc
                        | GameStateWaitingType.WaitingForEngine ->
                            processRequest ActionRequest.Engine (GameStateRequest.SetResults >> inbox.Post)
                        | GameStateWaitingType.WaitingForAI ->
                            processRequest (nextMove ()) (GameStateRequest.SetResults >> inbox.Post)
                            currentState <- GameState.Processing
                    | GameStateRequest.QueryState replyChannel ->
                        if not resultQ.IsEmpty then
                            (resultQ, (fun () -> inbox.Post(GameStateRequest.Acknowledge resultQ.Length)))
                            |> GameState.Results
                            |> replyChannel.Reply
                        else
                            currentState |> replyChannel.Reply
                    | GameStateRequest.Kill -> ()
                    | GameStateRequest.Acknowledge count -> resultQ <- resultQ |> List.skip count

                    match message with
                    | GameStateRequest.Kill -> ()
                    | _ -> do! loop currentState
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
                loopWorkAgent.PostAndReply((fun replyChannel -> GameStateRequest.QueryState replyChannel), 20)
            with _ ->
                GameState.Processing

        /// Get Scenario Data
        member this.ScenarioData = scenarioData
