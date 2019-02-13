// Learn more about F# at http://fsharp.org

open System

let Init() = 
    SadConsole.Game.Instance.Components.Add(new SadConsole.Game.FPSCounterComponent(SadConsole.Game.Instance))
    SadConsole.Game.Instance.Window.Title <- "Morgemil";

[<EntryPoint>]
let main argv =
    printfn "Starting Morgemil"
    SadConsole.Game.Create(80, 25);
    SadConsole.Game.OnInitialize <- new Action(Init)
    SadConsole.Game.Instance.Run();
    SadConsole.Game.Instance.Dispose();
    0 // return an integer exit code


