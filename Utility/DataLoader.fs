namespace Morgemil.Utility

open FSharp.Data
open Morgemil.Models
open JsonLoad



module Data =
    let ConvertTextToJsonValue fileName text =
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
            
    let RaceFile = "races.json"
    let RaceModifierFile = "racemodifiers.json"
    let TileFile = "tiles.json"
    let ItemFile = "items.json"
    let FloorGenerationFile = "floorgeneration.json"
    let RaceModifierLinkFile = "racemodifierlinks.json"
    
open Data
open SuccessBuilder
open JsonHelper


type RawScenarioData =
    {   Races: Race[]
        Items: Item[]
        RaceModifiers: RaceModifier[]
        RaceModifierLinks: RaceModifierLink[]
        FloorGenerationsParameters: FloorGenerationParameter[]
        Tiles: Tile[]
    }

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

                    Data.ConvertTextToJsonValue fileName text
                    |> Result.bind (JsonAsScenario directoryName
                                    >> Result.mapError(fun _ -> failure))
                )
            )
            |> Pipe.Collect
    
    member this.LoadScenario(scenario: Scenario) =
        let loadData fileName (jsonAsData: JsonValue -> JsonResult<_>) =
            System.IO.Path.Combine(scenario.BasePath, fileName)
            |> Data.ReadFileToText
            |> Result.bind (Data.ConvertTextToJsonValue fileName)
            |> Result.bind (jsonAsData >> Result.mapError(fun t -> t.ToString()))
        
        success {
            let! races = loadData Data.RaceFile JsonAsRaces
            let! tiles = loadData Data.TileFile JsonAsTiles
            let! raceModifiers = loadData Data.RaceModifierFile JsonAsRaceModifiers
            let! raceModifierLinks = loadData Data.RaceModifierLinkFile JsonAsRaceModifierLinks
            let! floorGenerationsParameters = loadData Data.FloorGenerationFile JsonAsFloorGenerationParameters
            let! items = loadData Data.ItemFile JsonAsItems
            
            return {
                RawScenarioData.Items = items
                Tiles = tiles
                RaceModifiers = raceModifiers
                RaceModifierLinks = raceModifierLinks
                FloorGenerationsParameters = floorGenerationsParameters
                Races = races
            }
        }





        