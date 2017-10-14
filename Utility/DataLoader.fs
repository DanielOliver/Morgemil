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
        Scenario: Scenario
    }

module DataLoader =

    let AsResult errorMsg truth = if truth then Ok() else Error errorMsg

    let private IsIdentityColumnUnique(getID: _ -> int) items =
        (items |> Seq.length) = (items |> Seq.distinctBy getID |> Seq.length)
        
    let private ValidateRaceModifierLinks (links: RaceModifierLink[]) (races: Map<int,Race>) (modifiers: Map<int,RaceModifier>) =
        links 
        |>  Array.forall(fun item ->
            races.ContainsKey item.RaceID
            && (match item.RaceModifierID with | Some id -> modifiers.ContainsKey id | None -> true)
        )
        |> function | true -> Ok() | false -> Error "RaceModifierLinks contain invalid Links."

    let private ValidateFloorGenerationTiles (floorGenerationParameters: FloorGenerationParameter[]) (tiles: Map<int, Tile>) =
        floorGenerationParameters
        |> Array.forall(fun item ->
            item.Tiles
            |> Array.forall(tiles.ContainsKey)
        )
        |> function | true -> Ok() | false -> Error "FloorGenerationParameters contain invalid TileIDs."

    let ValidateRawScenarioData(scenario: RawScenarioData) =
        let isIDUnique name getID = IsIdentityColumnUnique getID >> AsResult(sprintf "%s do not have Unique Identity Column." name)

        success {
            do! scenario.Races |> isIDUnique "Races" (fun t -> t.ID)
            do! scenario.Items |> isIDUnique "Items" (fun t -> t.ID)
            do! scenario.Tiles |> isIDUnique "Tiles" (fun t -> t.ID)
            do! scenario.FloorGenerationsParameters |> isIDUnique "FloorGenerationsParameters" (fun t -> t.ID)
            do! scenario.RaceModifiers |> isIDUnique "RaceModifiers" (fun t -> t.ID)
            do! scenario.RaceModifierLinks |> isIDUnique "RaceModifierLinks" (fun t -> t.ID)
            
            let itemMap = scenario.Items |> Seq.map(fun t -> t.ID, t) |> Map.ofSeq
            let raceMap = scenario.Races |> Seq.map(fun t -> t.ID, t) |> Map.ofSeq
            let tileMap = scenario.Tiles |> Seq.map(fun t -> t.ID, t) |> Map.ofSeq
            let floorGenerationsParameterMap = scenario.FloorGenerationsParameters |> Seq.map(fun t -> t.ID, t) |> Map.ofSeq
            let raceModifierMap = scenario.RaceModifiers |> Seq.map(fun t -> t.ID, t) |> Map.ofSeq
            let raceModifierLinkMap = scenario.RaceModifierLinks |> Seq.map(fun t -> t.ID, t) |> Map.ofSeq

            do! ValidateRaceModifierLinks scenario.RaceModifierLinks raceMap raceModifierMap
            do! ValidateFloorGenerationTiles scenario.FloorGenerationsParameters tileMap
            
            return {
                ScenarioData.Scenario = scenario.Scenario
                ScenarioData.FloorGenerationParameters = floorGenerationsParameterMap
                ScenarioData.Items = itemMap
                ScenarioData.Tiles = tileMap
                ScenarioData.RaceModifierLinks = raceModifierLinkMap
                ScenarioData.RaceModifiers = raceModifierMap
                ScenarioData.Races = raceMap
            }
        }


    let LoadScenarios(baseGamePath: string) =
        let _basePath = System.IO.DirectoryInfo(baseGamePath).FullName
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
    
    let LoadScenario(scenario: Scenario) =
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
                Scenario = scenario
            }
        }





        