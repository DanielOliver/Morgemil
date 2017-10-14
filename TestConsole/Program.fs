open Morgemil.Utility
open Morgemil.Utility.SuccessBuilder

[<EntryPoint>]
let main argv = 
    let scenarios = DataLoader.LoadScenarios @"C:\Users\Daniel\Development\Morgemil\data"
  
    for scenario in scenarios do
        printfn ""

        success {
            let! scenario = scenario
            let! rawScenarioData = DataLoader.LoadScenario scenario |> Result.mapError(fun err -> sprintf "Scenario has error: %s" err)
            let! validScenarioData = DataLoader.ValidateRawScenarioData rawScenarioData
            return validScenarioData
        }
        |> printfn "%A"
        

    System.Console.ReadLine() |> ignore
    0 // return an integer exit code
