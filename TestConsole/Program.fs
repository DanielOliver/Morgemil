[<EntryPoint>]
let main argv = 
  let loader = Morgemil.Utility.DataLoader @"C:\Users\Daniel\Development\Morgemil\data"
  let scenarios = loader.LoadScenarios()
  
  for scenario in scenarios do
    printfn ""
    printfn "%A" scenario

    //let scenarioData = loader.LoadScenario scenario
    //printfn ""
    //printfn "%A" scenarioData

  System.Console.ReadLine() |> ignore
  0 // return an integer exit code
