[<EntryPoint>]
let main argv = 
  let loader = Morgemil.Utility.DataLoader @"C:\Users\Daniel\Development\github\Morgemil\data"
  let scenarios = loader.LoadScenarios()
  
  for scenario in scenarios do
    printfn "%A" scenario

    let scenarioData = loader.LoadScenario scenario
    printfn "%A" scenarioData


  System.Console.ReadLine() |> ignore
  0 // return an integer exit code
