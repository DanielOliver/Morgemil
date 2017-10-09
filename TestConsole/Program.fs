
[<EntryPoint>]
let main argv = 
    let loader = Morgemil.Utility.DataLoader @"C:\Users\Daniel\Development\Morgemil\data"
    let scenarios = loader.LoadScenarios()
  
    for scenario in scenarios do
        printfn ""
        match scenario with
        | Ok x -> 
            printfn "%A" x
            printfn ""
            match loader.LoadScenario x with
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
