namespace Morgemil.Core

open Morgemil.Models

type SimpleGameBuilderMachine(loadScenarioData: (ScenarioData -> unit) -> unit) =
    let mutable currentPlayerID: PlayerID option = None
    let mutable chosenRaceID: RaceID option = None
    let mutable chosenScenarioName: string option = None
    let mutable loadedScenarioData: ScenarioData option = None
    
    let loopWorkAgent = MailboxProcessor<GameBuilderStateRequest>.Start(fun inbox ->
        let rec loop(previousState: GameBuilderState) =
            async {
                let! message = inbox.Receive();
                match message with
                | GameBuilderStateRequest.QueryState replyChannel ->
                    replyChannel.Reply previousState
                    do! loop previousState
                    
                | GameBuilderStateRequest.AddPlayer raceID ->
                    currentPlayerID <- PlayerID 1L |> Some
                    chosenRaceID <- Some raceID
                    do! loop (GameBuilderState.LoadingGameProgress "Creating Game")
                    
                | GameBuilderStateRequest.SelectScenario scenarioName ->
                    chosenScenarioName <- Some scenarioName
                    loadScenarioData (GameBuilderStateRequest.SetScenarioData >> inbox.Post)
                    do! loop (GameBuilderState.LoadingGameProgress "Loading Scenario Data")
                    
                | GameBuilderStateRequest.SetScenarioData scenarioData ->
                    loadedScenarioData <- Some scenarioData
                    do! loop (
                         (GameBuilderStateRequest.AddPlayer >> inbox.Post)
                         |> GameBuilderState.WaitingForCurrentPlayer
                     )                    
            }
        loop(GameBuilderState.SelectScenario ([ "Main Scenario" ], (GameBuilderStateRequest.SelectScenario >> inbox.Post)))
        )
    interface IGameBuilder with
        /// Gets the current state of the game loop
        member this.CurrentState
            with get(): GameBuilderState =
                loopWorkAgent.PostAndReply((fun replyChannel ->
                    GameBuilderStateRequest.QueryState replyChannel
                    ), 5000)

