open Morgemil.Utility

[<EntryPoint>]
let main argv = 
    let scenarios = DataLoader.LoadScenarios @"C:\Users\Daniel\Development\Morgemil\data"
  
    for scenario in scenarios do
        printfn ""
        match scenario with
        | Ok x -> 
            printfn "%A" x
            printfn ""
            match DataLoader.LoadScenario x with
            | Ok scenarioData ->
                printfn "%A" scenarioData
                printfn ""
            | Error err ->
                printfn "%A" err
                printfn ""
        | Error err -> 
            printfn "%A" err

    System.Console.ReadLine() |> ignore
    0 // return an integer exit code
