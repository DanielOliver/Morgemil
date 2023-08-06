open System
open Morgemil.Core
open Morgemil.Data
open Morgemil.GameEngine
open Morgemil.Models

let StartMainStateMachine () =
    let mainGameState =
        new SimpleGameBuilderMachine(DataLoader.LoadScenarioData) :> IGameBuilder

    let mutable gameHasRun = true

    while gameHasRun do
        match mainGameState.CurrentState with
        | GameBuilderState.GameBuilt(gameState, initialGameData) ->
            gameHasRun <- false

            let Init () =
                SadConsole.Settings.WindowTitle <- "Morgemil"
                SadConsole.Game.Instance.Screen <-
                    ScreenContainer(ScreenGameState.MapGeneratorConsole(gameState, initialGameData))

            SadConsole.Game.Create(80, 40, "Cheepicus12.font")
            SadConsole.Game.Instance.OnStart <- new Action(Init)
            SadConsole.Game.Instance.Run()
            SadConsole.Game.Instance.Dispose()
        | GameBuilderState.SelectScenario(scenarios, callback) ->
            printfn "Scenarios: "

            scenarios
            |> Seq.iteri (fun index scenarioName -> printfn "%-5i | %s" index scenarioName)

            printfn "Choose Scenario: "
            Console.ReadLine() |> callback
        | GameBuilderState.LoadedScenarioData _ -> failwith "one"
        | GameBuilderState.WaitingForCurrentPlayer addCurrentPlayer -> addCurrentPlayer (AncestryID 1L)
        | GameBuilderState.LoadingGameProgress status -> printfn "Status %s" status

    ()

[<EntryPoint>]
let main argv =
    printfn "Starting Morgemil"
    StartMainStateMachine()
    0 // return an integer exit code
