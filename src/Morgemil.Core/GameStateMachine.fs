namespace Morgemil.Core

open Morgemil.Models
open System.Threading.Tasks

[<RequireQualifiedAccess>]
type GameState =
    | Processing
    | Results of ActionEvent seq
    | WaitingForInput

[<RequireQualifiedAccess>]
type GameStateRequest =
    | Input of ActionRequest
    | QueryState of AsyncReplyChannel<GameState>
    | SetResults of ActionEvent seq
    | Kill
    | Acknowledge

type GameStateMachine(gameLoop: ActionRequest -> ActionEvent seq) =
    let mutable gameState: GameState = GameState.WaitingForInput
    let mutable continueLoop: bool = true

    let loopWorkAgent = MailboxProcessor<GameStateRequest>.Start(fun inbox ->
        let processRequest (actionRequest: ActionRequest) (callback: ActionEvent seq -> unit) =
            async {
                let results = gameLoop actionRequest
                callback results
            }
            |> Async.Start

        let rec loop() =
            async {
                let! message = inbox.Receive();
                match message with
                | GameStateRequest.Input request ->
                    System.Diagnostics.Debugger.Break()

                    match gameState with
                    | GameState.WaitingForInput ->
                        processRequest request (GameStateRequest.SetResults >> inbox.Post)
                        gameState <- GameState.Processing
                    | GameState.Processing
                    | GameState.Results _ ->
                        ()
                | GameStateRequest.SetResults results ->
                    gameState <- GameState.Results results
                | GameStateRequest.QueryState replyChannel ->
                    replyChannel.Reply gameState
                | GameStateRequest.Kill ->
                    ()
                | GameStateRequest.Acknowledge ->
                    match gameState with
                    | GameState.Results _ ->
                        gameState <- GameState.WaitingForInput
                    | GameState.Processing
                    | GameState.WaitingForInput ->
                        ()
                        
                match message with
                | GameStateRequest.Kill ->
                    ()
                | _ ->
                    do! loop()
            }
        loop()
        )

    /// Sends a poison pill.
    member this.Stop() =
        GameStateRequest.Kill
        |> loopWorkAgent.Post
    /// Gets the current state of the game loop
    member this.GetCurrentState(): GameState =
        loopWorkAgent.PostAndReply((fun replyChannel ->
            GameStateRequest.QueryState replyChannel
            ), 5000)
    /// Sends input
    member this.Input(request: ActionRequest) =
        request
        |> GameStateRequest.Input
        |> loopWorkAgent.Post

    /// Acknowledge results
    member this.Acknowledge() =
        GameStateRequest.Acknowledge
        |> loopWorkAgent.Post


