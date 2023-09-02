open System
open Morgemil.Core
open Morgemil.Data
open Morgemil.GameEngine

let StartMainStateMachine () =
    let mainGameState =
        SimpleGameBuilderMachine(DataLoader.LoadScenarioData) :> IGameBuilder

    let Init () =
        SadConsole.Settings.WindowTitle <- "Morgemil"

        SadConsole.Game.Instance.Screen <- ScreenContainer(ScreenGameState.SelectingScenario, mainGameState)

        SadConsole.Game.Instance.DestroyDefaultStartingConsole()

    SadConsole.Game.Create(80, 40, "Cheepicus12.font")
    SadConsole.Game.Instance.OnStart <- new Action(Init)
    SadConsole.Game.Instance.Run()
    SadConsole.Game.Instance.Dispose()

[<EntryPoint>]
let main argv =
    printfn "Starting Morgemil"
    StartMainStateMachine()
    0 // return an integer exit code
