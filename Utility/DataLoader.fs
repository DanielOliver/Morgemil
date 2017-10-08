namespace Morgemil.Utility

open FSharp.Data
open Morgemil.Models
open JsonLoad



module Data = 
    let ConvertTextToJsonValue text fileName =
        let failure = sprintf "Failed to parse JSON from file \"%s\"" fileName
        match JsonValue.TryParse text with
        | Some jsonValue -> 
            jsonValue
            |> Ok
        | None -> Error failure

    let ReadFileToText fileName = 
        try
            Result.Ok (System.IO.File.ReadAllText fileName)
        with
        _ as _ex -> 
            Result.Error(sprintf "Failed to read from file \"%s\"" fileName)


    [<RequireQualifiedAccess>]
    type DataFile = 
        | Races of JsonHelper.JsonResult<Race[]>
        | Items of JsonHelper.JsonResult<Item[]>
        | RaceModifiers of JsonHelper.JsonResult<RaceModifier[]>
        | RaceModifierLinks of JsonHelper.JsonResult<RaceModifierLink[]>
        | FloorGenerations of JsonHelper.JsonResult<FloorGenerationParameter[]>
        | Tiles of JsonHelper.JsonResult<Tile[]>

    let RaceFile = "races.json"
    let RaceModifierFile = "racemodifiers.json"
    let TileFile = "tiles.json"
    let ItemFile = "items.json"
    let FloorGenerationFile = "floorgeneration.json"
    let RaceModifierLinkFile = "racemodifierlinks.json"

    let FilesToLoad =
        [|  RaceFile, JsonAsRaces >> DataFile.Races
            RaceModifierFile, JsonAsRaceModifiers >> DataFile.RaceModifiers
            TileFile, JsonAsTiles >> DataFile.Tiles
            ItemFile, JsonAsItems >> DataFile.Items
            FloorGenerationFile, JsonAsFloorGenerationParameters >> DataFile.FloorGenerations
            RaceModifierLinkFile, JsonAsRaceModifierLinks >> DataFile.RaceModifierLinks
        |]



type DataLoader(baseGamePath: string) =
    let _basePath = System.IO.DirectoryInfo(baseGamePath).FullName

    member this.LoadScenarios() =
        let files = System.IO.DirectoryInfo(_basePath).GetDirectories() |> Array.collect(fun t -> t.GetFiles("game.json"))
        if files.Length = 0 then
          Result<Scenario,string>.Error(sprintf "No scenarios to load from \"%s\"" _basePath) |> List.singleton
        else
            files
            |> Pipe.From
            |> Pipe.Map(fun gameFileInfo ->
                let directoryName = gameFileInfo.DirectoryName
                let fileName = gameFileInfo.FullName

                Data.ReadFileToText fileName
                |> Result.map(fun text -> directoryName, fileName, text)
            )
            |> Pipe.Map(
                Result.bind(fun (directoryName, fileName, text) ->
                    let failure = sprintf "Failed to parse JSON from file \"%s\"" fileName

                    Data.ConvertTextToJsonValue text fileName
                    |> Result.bind (JsonAsScenario directoryName
                                    >> Result.mapError(fun _ -> failure))
                )
            )
            |> Pipe.Collect
    
    member this.LoadScenario(scenario: Scenario) =
        Data.FilesToLoad
        |> Pipe.From
        |> Pipe.Map(fun (fileName, jsonValueToDataFile) ->
            let fullFileName = System.IO.Path.Combine(scenario.BasePath, fileName)
            Data.ReadFileToText fullFileName
            |> Result.map(fun text -> text, fullFileName, jsonValueToDataFile)
        )
        |> Pipe.Map(
                Result.bind(fun (text, fileName, jsonValueToDataFile) ->
                Data.ConvertTextToJsonValue text fileName 
                |> Result.map jsonValueToDataFile)
            )
        |> Pipe.Collect

        