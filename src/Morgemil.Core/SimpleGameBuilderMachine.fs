namespace Morgemil.Core

open Morgemil.Core
open Morgemil.Models
open Morgemil.Math

type SimpleGameBuilderMachine(loadScenarioData: (ScenarioData -> unit) -> unit) =
    let mutable currentPlayerID: PlayerID option = None
    let mutable chosenAncestryID: AncestryID option = None
    let mutable chosenScenarioName: string option = None
    let mutable loadedScenarioData: ScenarioData option = None

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

            let gameContext =
                { GameContext.CurrentTimeTick = 0L<TimeTick>
                  Floor = 1L<Floor> }

            let character1 =
                { Character.ID = Table.GenerateKey characterTable
                  Ancestry = Table.GetRowByKey scenarioData.Ancestries chosenAncestryID.Value
                  Heritage = []
                  NextTick = 0L<TimeTick>
                  NextAction = Character.DefaultPlayerTickActions.Head
                  TickActions = Character.DefaultPlayerTickActions
                  Position = mapGenerationResults.EntranceCoordinate
                  PlayerID = currentPlayerID.Value |> Some
                  Floor = gameContext.Floor
                  Tags = Map.empty }

            Table.AddRow characterTable character1

            for i in [ 2..6 ] do

                let npc1 =
                    { Character.ID = Table.GenerateKey characterTable
                      Ancestry = scenarioData.Ancestries.Items |> Seq.last
                      Heritage = []
                      NextTick = 0L<TimeTick>
                      NextAction = Character.DefaultTickActions.Head
                      TickActions = Character.DefaultTickActions
                      Position = mapGenerationResults.EntranceCoordinate + Point.create (i, i)
                      PlayerID = None
                      Floor = gameContext.Floor
                      Tags = Map.empty }

                Table.AddRow characterTable npc1

            let gameLoop =
                Loop(
                    { StaticLoopContext.ScenarioData = scenarioData
                      RNG = rng },
                    { LoopContext.Characters = characterTable
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
                  TileMap = tileMap
                  CurrentPlayerID = currentPlayerID.Value
                  GameContext = gameContext }

            callback (gameState, initialGameData)
        }
        |> Async.Start



    let loopWorkAgent =
        MailboxProcessor<GameBuilderStateRequest>.Start(fun inbox ->
            let rec loop (previousState: GameBuilderState) =
                async {
                    let! message = inbox.Receive()

                    match message with
                    | GameBuilderStateRequest.QueryState replyChannel ->
                        replyChannel.Reply previousState
                        do! loop previousState

                    | GameBuilderStateRequest.AddPlayer ancestryID ->
                        currentPlayerID <- PlayerID 1L |> Some
                        chosenAncestryID <- Some ancestryID
                        buildGameState (GameBuilderStateRequest.SetGameData >> inbox.Post)
                        do! loop (GameBuilderState.LoadingGameProgress "Creating Game")

                    | GameBuilderStateRequest.SelectScenario scenarioName ->
                        chosenScenarioName <- Some scenarioName

                        loadScenarioData (GameBuilderStateRequest.SetScenarioData >> inbox.Post)

                        do! loop (GameBuilderState.LoadingGameProgress "Loading Scenario Data")

                    | GameBuilderStateRequest.SetScenarioData scenarioData ->
                        loadedScenarioData <- Some scenarioData

                        do!
                            loop (
                                (GameBuilderStateRequest.AddPlayer >> inbox.Post)
                                |> GameBuilderState.WaitingForCurrentPlayer
                            )
                    | GameBuilderStateRequest.SetGameData(gameState, initialGameData) ->
                        do! loop (GameBuilderState.GameBuilt(gameState, initialGameData))

                }

            loop (
                GameBuilderState.SelectScenario(
                    [ "Main Scenario" ],
                    (GameBuilderStateRequest.SelectScenario >> inbox.Post)
                )
            ))

    interface IGameBuilder with
        /// Gets the current state of the game loop
        member this.CurrentState: GameBuilderState =
            loopWorkAgent.PostAndReply((fun replyChannel -> GameBuilderStateRequest.QueryState replyChannel), 5000)
