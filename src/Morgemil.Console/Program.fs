open System
open Argu
open Morgemil.Data

[<CliPrefix(CliPrefix.DoubleDash)>]
type CLIArguments =
    | ValidateRawGameData of workingDirectory: string
    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | ValidateRawGameData _ -> "specify a working directory."

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
            results.GetResults ValidateRawGameData
            |> List.map(fun path -> path, JsonReader.ReadGameFiles path)
            |> List.iter(fun (path, rawGameData) ->
                printfn "Path: %s" (System.IO.Path.GetFullPath path)
                printfn "Tiles: %s" (rawGameData.Tiles |> function | Ok x -> sprintf "%i tiles" x.Length | Error err -> "Errors")
                printfn "Summary: %s" (if rawGameData.Successful then "Valid" else String.Empty)
                rawGameData.Errors |> List.iter (printfn "Error: %s")
                )
    
        with e ->
            printfn "%s" e.Message
    

    0 // return an integer exit code
