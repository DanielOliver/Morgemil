namespace Morgemil.Core

open Morgemil.Core
open Morgemil.Models
open Morgemil.Math

type GameServerLocalhost(loadScenarioData: (ScenarioData -> unit) -> unit) =
    let mutable currentPlayerID: PlayerID option = None
    let mutable chosenAncestryID: AncestryID option = None
    let mutable chosenScenarioName: string option = None
    let mutable loadedScenarioData: ScenarioData option = None

    let mutable lastKnownState = GameServerState.LoadingGameProgress "Loading"

    let buildGameState (callback: IGameStateMachine * InitialGameData -> unit) =
        async {

            let scenarioData = loadedScenarioData.Value

            let rng = RNG.SeedRNG(500)

            let (tileMap, mapGenerationResults) =
                FloorGenerator.Create
                    (scenarioData.FloorGenerationParameters.Items |> Seq.head)
                    (scenarioData.TileFeatures)
                    rng

            let createTileMapFromData (data: TileMapData) =
                let result =
                    TileMap(
                        Rectangle.WithPositionAndSize(Point(0, 0), data.Size),
                        data.DefaultTile,
                        Array.zip data.Tiles data.TileFeatures
                    )

                result

            let tileMap = createTileMapFromData tileMap.TileMapData

            let timeTable = TimeTable()

            let characterTable = CharacterTable(timeTable)
            let characterAttributesTable = CharacterAttributesTable()

            let gameContext =
                { GameContext.CurrentTimeTick = 0L<TimeTick>
                  Floor = 1L<Floor> }

            let character1 =
                { Character.ID = Table.GenerateKey characterTable
                  // Ancestry = Table.GetRowByKey scenarioData.Ancestries chosenAncestryID.Value
                  // Heritage = []
                  NextTick = 0L<TimeTick>
                  NextAction = Character.DefaultPlayerTickActions.Head
                  TickActions = Character.DefaultPlayerTickActions
                  Position = mapGenerationResults.EntranceCoordinate
                  PlayerID = currentPlayerID.Value |> Some
                  Floor = gameContext.Floor
                // Tags = Map.empty
                }

            let character1Attributes =
                { CharacterAttributes.ID = character1.ID
                  Ancestry = Table.GetRowByKey scenarioData.Ancestries chosenAncestryID.Value
                  Heritage = []
                  Tags = Map.empty }

            Table.AddRow characterTable character1
            Table.AddRow characterAttributesTable character1Attributes

            for i in [ 2 .. (RNG.Range rng 4 6) ] do

                let npc1 =
                    { Character.ID = Table.GenerateKey characterTable
                      // Ancestry = scenarioData.Ancestries.Items |> Seq.last
                      // Heritage = []
                      NextTick = 0L<TimeTick>
                      NextAction = Character.DefaultTickActions.Head
                      TickActions = Character.DefaultTickActions
                      Position = mapGenerationResults.EntranceCoordinate + Point.create (i, i)
                      PlayerID = None
                      Floor = gameContext.Floor
                    // Tags = Map.empty
                    }

                let npc1Attributes =
                    { CharacterAttributes.ID = npc1.ID
                      Ancestry = scenarioData.Ancestries.Items |> Seq.last
                      Heritage = []
                      Tags = Map.empty }

                Table.AddRow characterTable npc1
                Table.AddRow characterAttributesTable npc1Attributes

            let gameLoop =
                Loop(
                    { StaticLoopContext.ScenarioData = scenarioData
                      Scenario =
                        { Scenario.Date = System.DateTime.MinValue
                          Description = "Empty Scenario"
                          Name = "Empty Scenario"
                          Version = "0.0.0"
                          BasePath = "empty" }
                      RNG = rng },
                    { LoopContext.Characters = characterTable
                      CharacterAttributes = characterAttributesTable
                      TileMap = tileMap
                      TimeTable = timeTable
                      GameContext = TrackedEntity gameContext }
                )

            let gameState =
                SimpleGameStateMachine(
                    gameLoop.ProcessRequest,
                    (fun () -> gameLoop.WaitingType),
                    scenarioData,
                    (fun () -> gameLoop.NextMove)
                )
                :> IGameStateMachine

            let initialGameData =
                { InitialGameData.Characters = characterTable.ByTicks |> Seq.toArray
                  CharacterAttributes = characterAttributesTable |> Table.Items |> Seq.toArray
                  TileMap = tileMap
                  CurrentPlayerID = currentPlayerID.Value
                  Scenario =
                    { Scenario.Date = System.DateTime.MinValue
                      Description = "Empty Scenario is the best kind of scenario"
                      Name = "Basic Crawl"
                      Version = "0.0.0"
                      BasePath = "empty" }
                  GameContext = gameContext }

            callback (gameState, initialGameData)
        }
        |> Async.Start

    let loopWorkAgent =
        MailboxProcessor<GameServerInternalStateRequest>.Start(fun inbox ->
            let rec loop (previousState: GameServerState) =
                lastKnownState <- previousState

                async {
                    let! message = inbox.Receive()

                    match message with
                    | GameServerInternalStateRequest.RequestWorkflow workflow ->
                        match previousState with
                        | GameServerState.GameBuilt(gameState, _) ->
                            // Cleans up any outstanding game instances.
                            gameState.Stop()
                        | _ -> ()

                        do! loop (GameServerState.SelectScenario([ "Main Scenario" ]))

                    | GameServerInternalStateRequest.QueryState replyChannel ->
                        replyChannel.Reply previousState
                        do! loop previousState

                    | GameServerInternalStateRequest.AddPlayer ancestryID ->
                        currentPlayerID <- PlayerID 1L |> Some
                        chosenAncestryID <- Some ancestryID
                        buildGameState (GameServerInternalStateRequest.SetGameData >> inbox.Post)
                        do! loop (GameServerState.LoadingGameProgress "Creating Game")

                    | GameServerInternalStateRequest.SelectScenario scenarioName ->
                        chosenScenarioName <- Some scenarioName

                        loadScenarioData (GameServerInternalStateRequest.SetScenarioData >> inbox.Post)

                        do! loop (GameServerState.LoadingGameProgress "Loading Scenario Data")

                    | GameServerInternalStateRequest.SetScenarioData scenarioData ->
                        loadedScenarioData <- Some scenarioData

                        do! loop GameServerState.WaitingForCurrentPlayer
                    | GameServerInternalStateRequest.SetGameData(gameState, initialGameData) ->
                        do! loop (GameServerState.GameBuilt(gameState, initialGameData))

                }

            loop (GameServerState.SelectScenario([ "Main Scenario" ])))

    do
        // Primes the agent loop.
        loopWorkAgent.PostAndReply((fun replyChannel -> GameServerInternalStateRequest.QueryState replyChannel), 5000)
        |> ignore

    interface IGameServer with
        /// Gets the current state of the game loop
        member this.CurrentState: GameServerState = lastKnownState

        member this.Request(request: GameServerRequest) =
            match request with
            | GameServerRequest.SelectScenario scenarioId ->
                loopWorkAgent.Post(GameServerInternalStateRequest.SelectScenario scenarioId)
            | GameServerRequest.AddCurrentPlayer ancestryID ->
                loopWorkAgent.Post(GameServerInternalStateRequest.AddPlayer ancestryID)
            | GameServerRequest.Workflow workflow ->
                loopWorkAgent.Post(GameServerInternalStateRequest.RequestWorkflow workflow)
