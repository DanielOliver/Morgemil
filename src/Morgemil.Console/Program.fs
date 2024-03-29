open Argu
open Morgemil.Data
open Morgemil.Data.Translation

[<CliPrefix(CliPrefix.DoubleDash)>]
type CLIArguments =
    | [<AltCommandLine("-d"); Unique>] WorkingDirectory of workingDirectory: string
    | [<Unique>] GameDataRead
    | [<Unique>] GameDataValidate
    | [<Unique>] GameDataFinal

    interface IArgParserTemplate with
        member s.Usage =
            match s with
            | GameDataRead _ -> "read game data from a directory"
            | GameDataValidate _ -> "validate game data while still in raw format"
            | GameDataFinal _ -> "create final game data to be output"
            | WorkingDirectory _ -> "specify a working directory"

[<EntryPoint>]
let main argv =

    let errorHandler =
        ProcessExiter(
            colorizer =
                function
                | ErrorCode.HelpText -> None
                | _ -> Some System.ConsoleColor.Red
        )

    let parser =
        ArgumentParser.Create<CLIArguments>(programName = "Morgemil.Console.exe", errorHandler = errorHandler)

    let results = parser.ParseCommandLine(inputs = argv, raiseOnUsage = true)

    let allResults = results.GetAllResults()

    if allResults |> Seq.isEmpty then
        parser.PrintUsage() |> System.Console.Write

    else
        try
            match results.TryGetResult WorkingDirectory with
            | Some path ->
                let rawGameDataPhase0 =
                    Lazy<DTO.RawDtoPhase0>(fun () -> JsonReader.ReadGameFiles path)

                if results.Contains GameDataValidate then
                    let rawGameDataPhase1 = Validator.ValidateDtos rawGameDataPhase0.Value

                    let options = JsonSettings.createOptions ()
                    options.WriteIndented <- true

                    System.Text.Json.JsonSerializer.Serialize(rawGameDataPhase1, options)
                    |> System.Console.Write

                if results.Contains GameDataRead then
                    let rawGameDataPhase0 = Validator.ValidateDtos rawGameDataPhase0.Value

                    let options = JsonSettings.createOptions ()
                    options.WriteIndented <- true

                    System.Text.Json.JsonSerializer.Serialize(rawGameDataPhase0, options)
                    |> System.Console.Write

                if results.Contains GameDataFinal then
                    let rawGameDataPhase2 = FromDTO.TranslateFromDtosToPhase2 rawGameDataPhase0.Value

                    let options = JsonSettings.createOptions ()
                    options.WriteIndented <- true

                    System.Text.Json.JsonSerializer.Serialize(rawGameDataPhase2, options)
                    |> System.Console.Write

                System.Console.WriteLine()
            | None -> ()
        with e ->
            printfn $"%s{e.Message}"

    0 // return an integer exit code
