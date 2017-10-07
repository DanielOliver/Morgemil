[<EntryPoint>]
let main argv = 
  let loader = Morgemil.Utility.DataLoader @"C:\Users\Daniel\Development\Morgemil\data"
  let scenarios = loader.LoadScenarios()
  
  for scenario in scenarios do
    printfn ""
    printfn "%A" scenario
    printfn ""
    match scenario with
    | Ok x -> 
        let scenarioData = loader.LoadScenario x
        printfn "%A" scenarioData
    | Error err -> 
        printfn "%A" err

  System.Console.ReadLine() |> ignore
  0 // return an integer exit code
