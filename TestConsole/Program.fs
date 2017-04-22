[<EntryPoint>]
let main argv = 
  let loader = Morgemil.Utility.DataLoader @"C:\Users\Daniel\Development\github\Morgemil\data"
  let scenarios = loader.LoadScenarios()

  scenarios |> Seq.iter (printf "%A")

  System.Console.ReadLine() |> ignore
  0 // return an integer exit code
