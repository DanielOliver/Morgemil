namespace Morgemil.Core

open Morgemil.Core
open Morgemil.Models
open Morgemil.Math

type SimpleGameBuilderMachine(loadScenarioData: (ScenarioData -> unit) -> unit) =
    let mutable currentPlayerID: PlayerID option = None
    let mutable chosenRaceID: RaceID option = None
    let mutable chosenScenarioName: string option = None
    let mutable loadedScenarioData: ScenarioData option = None

    let buildGameState (callback: IGameStateMachine * InitialGameData -> unit) =
        async {

            let scenarioData = loadedScenarioData.Value

            let rng = RNG.SeedRNG(50)

            let (tileMap, mapGenerationResults) =
                FloorGenerator.Create
                    (scenarioData.FloorGenerationParameters.Items
                     |> Seq.head)
                    (scenarioData.TileFeatures)
                    rng

            let createTileMapFromData (data: TileMapData) =
                let result =
                    TileMap(Rectangle.create data.Size, data.DefaultTile, Array.zip data.Tiles data.TileFeatures)

                result

            let tileMap =
                createTileMapFromData tileMap.TileMapData

            let characterTable = CharacterTable()

            let character1 =
                { Character.ID = Table.GenerateKey characterTable
                  Race = Table.GetRowByKey scenarioData.Races chosenRaceID.Value
                  RaceModifier = None
                  NextTick = 0L<TimeTick>
                  Position = mapGenerationResults.EntranceCoordinate
                  PlayerID = currentPlayerID.Value |> Some }

            Table.AddRow characterTable character1

            let gameContext =
                { GameContext.CurrentTimeTick = 0L<TimeTick> }

            let gameLoop =
                Loop(characterTable, tileMap, scenarioData, rng, TrackedEntity gameContext)

            let gameState =
                SimpleGameStateMachine(gameLoop.ProcessRequest, (fun () -> gameLoop.WaitingType), scenarioData) :> IGameStateMachine

            let initialGameData =
                { InitialGameData.Characters = [| character1 |]
                  TileMap = tileMap
                  CurrentPlayerID = currentPlayerID.Value
                  GameContext = gameContext }

            callback (gameState, initialGameData)
        }
        |> Async.Start



    let loopWorkAgent =
        MailboxProcessor<GameBuilderStateRequest>.Start
            (fun inbox ->
                let rec loop (previousState: GameBuilderState) =
                    async {
                        let! message = inbox.Receive()

                        match message with
                        | GameBuilderStateRequest.QueryState replyChannel ->
                            replyChannel.Reply previousState
                            do! loop previousState

                        | GameBuilderStateRequest.AddPlayer raceID ->
                            currentPlayerID <- PlayerID 1L |> Some
                            chosenRaceID <- Some raceID
                            buildGameState (GameBuilderStateRequest.SetGameData >> inbox.Post)
                            do! loop (GameBuilderState.LoadingGameProgress "Creating Game")

                        | GameBuilderStateRequest.SelectScenario scenarioName ->
                            chosenScenarioName <- Some scenarioName

                            loadScenarioData (
                                GameBuilderStateRequest.SetScenarioData
                                >> inbox.Post
                            )

                            do! loop (GameBuilderState.LoadingGameProgress "Loading Scenario Data")

                        | GameBuilderStateRequest.SetScenarioData scenarioData ->
                            loadedScenarioData <- Some scenarioData

                            do!
                                loop (
                                    (GameBuilderStateRequest.AddPlayer >> inbox.Post)
                                    |> GameBuilderState.WaitingForCurrentPlayer
                                )
                        | GameBuilderStateRequest.SetGameData (gameState, initialGameData) ->
                            do! loop (GameBuilderState.GameBuilt(gameState, initialGameData))

                    }

                loop (
                    GameBuilderState.SelectScenario(
                        [ "Main Scenario" ],
                        (GameBuilderStateRequest.SelectScenario
                         >> inbox.Post)
                    )
                ))

    interface IGameBuilder with
        /// Gets the current state of the game loop
        member this.CurrentState: GameBuilderState =
            loopWorkAgent.PostAndReply((fun replyChannel -> GameBuilderStateRequest.QueryState replyChannel), 5000)
