open System
open Morgemil.Core
open Morgemil.Data
open Morgemil.GameEngine
open SadRogue.Primitives
open SadConsole

let StartMainStateMachine () =
    let mainGameState =
        GameServerLocalhost(DataLoader.LoadScenarioData, EventRecorder.fromCurrentDirectory ()) :> IGameServer

    let Init gamehost =

        Settings.WindowTitle <- "Morgemil"
        Settings.AllowWindowResize <- true
        Settings.ResizeMode <- Settings.WindowResizeOptions.None
        Settings.WindowMinimumSize <- Point(500, 400)
        let container = ScreenContainer(ScreenGameState.SelectingScenario, mainGameState)
        Game.Instance.Screen <- container
        Game.Instance.DestroyDefaultStartingConsole()
        Game.Instance.MonoGameInstance.Window.Title <- "Morgemil"
        Game.Instance.MonoGameInstance.WindowResized.Add(fun _ -> container.Reposition())

    Game.Create(60, 30, "Cheepicus12.font", EventHandler<GameHost>(fun _ -> Init))
    // SadConsole.Game.Instance.Started.Add(Init)
    Game.Instance.Run()
    Game.Instance.Dispose()

[<EntryPoint>]
let main argv =
    printfn "Starting Morgemil"
    StartMainStateMachine()
    0 // return an integer exit code
