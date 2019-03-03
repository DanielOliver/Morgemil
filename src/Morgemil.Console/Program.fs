open System
open Argu
open Morgemil.Data
open Morgemil.Data

[<CliPrefix(CliPrefix.DoubleDash)>]
type CLIArguments =
    | ValidateGameData of workingDirectory: string
    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | ValidateGameData _ -> "specify a working directory."

[<EntryPoint>]
let main argv =
    
    let errorHandler = ProcessExiter(colorizer = function ErrorCode.HelpText -> None | _ -> Some System.ConsoleColor.Red)
    let parser = ArgumentParser.Create<CLIArguments>(programName = "Morgemil.Console.exe", errorHandler = errorHandler)

    let results = parser.ParseCommandLine(inputs = argv, raiseOnUsage = true)
    
    let allResults = results.GetAllResults()
    if allResults |> Seq.isEmpty then
        parser.PrintUsage() |> System.Console.Write
        
    else
        try
            results.GetResults ValidateGameData
            |> List.map(fun path -> path, JsonReader.ReadGameFiles path)
            |> List.map(fun (path, rawGameDataPhase0) -> path, Validator.ValidateDtos rawGameDataPhase0)
            |> List.iter(fun (path, rawGameDataPhase1) ->                  
                  Newtonsoft.Json.JsonConvert.SerializeObject(rawGameDataPhase1, Newtonsoft.Json.Formatting.Indented)
                  |> System.Console.Write
                  System.Console.WriteLine()
                )
    
        with e ->
            printfn "%s" e.Message
    

    0 // return an integer exit code
