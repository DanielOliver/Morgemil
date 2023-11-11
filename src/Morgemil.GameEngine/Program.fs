open System
open Morgemil.Core
open Morgemil.Data
open Morgemil.GameEngine
open SadRogue.Primitives
open SadConsole

let StartMainStateMachine () =
    let mainGameState = GameServerLocalhost(DataLoader.LoadScenarioData) :> IGameServer

    let Init (gamehost) =

        SadConsole.Settings.WindowTitle <- "Morgemil"
        SadConsole.Settings.AllowWindowResize <- true
        SadConsole.Settings.ResizeMode <- SadConsole.Settings.WindowResizeOptions.None
        SadConsole.Settings.WindowMinimumSize <- Point(500, 400)
        let container = ScreenContainer(ScreenGameState.SelectingScenario, mainGameState)
        SadConsole.Game.Instance.Screen <- container
        SadConsole.Game.Instance.DestroyDefaultStartingConsole()
        SadConsole.Game.Instance.MonoGameInstance.Window.Title <- "Morgemil"
        SadConsole.Game.Instance.MonoGameInstance.WindowResized.Add(fun _ -> container.Reposition())

    SadConsole.Game.Create(60, 30, "Cheepicus12.font", EventHandler<GameHost>(fun _ -> Init))
    // SadConsole.Game.Instance.Started.Add(Init)
    SadConsole.Game.Instance.Run()
    SadConsole.Game.Instance.Dispose()

[<EntryPoint>]
let main argv =
    printfn "Starting Morgemil"
    StartMainStateMachine()
    0 // return an integer exit code
