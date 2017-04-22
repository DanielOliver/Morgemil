[<EntryPoint>]
let main argv = 
  let loader = Morgemil.Utility.DataLoader @"C:\Users\Daniel\Development\github\Morgemil\data"
  let scenarios = loader.LoadScenarios()
  
  for scenario in scenarios do
    printfn ""
    printfn "%A" scenario

    let scenarioData = loader.LoadScenario scenario
    printfn ""
    printfn "%A" scenarioData


    let playableRaces = scenarioData.Races |> Seq.filter Morgemil.Utility.Generic.IsPlayerOption
    printfn ""
    printfn "Playable Races %A" playableRaces
    
    let playableModifiers = scenarioData.Races |> Seq.filter Morgemil.Utility.Generic.IsPlayerOption
    printfn ""
    printfn "Playable Races %A" playableRaces

  System.Console.ReadLine() |> ignore
  0 // return an integer exit code
