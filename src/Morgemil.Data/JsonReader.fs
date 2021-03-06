module Morgemil.Data.JsonReader
open Morgemil.Data.DTO
open Newtonsoft.Json
open System.IO


let ReadJsonFile<'T> (fileName: string): DtoValidResult<'T[]> =
    if not <| File.Exists(fileName) then
        {
            DtoValidResult.Errors = [ (sprintf "File \"%s\" doesn't exist " fileName) ]
            Object = [||]
            Success = false
        }
    else
        try
            let fileContents = File.ReadAllText fileName

            let enumUnionCaseConvertor = new Morgemil.Data.Convertors.EnumUnionConvertor()
            let jsonContents = Newtonsoft.Json.JsonConvert.DeserializeObject<'T[]>(fileContents, enumUnionCaseConvertor)
            {
                DtoValidResult.Errors = List.empty
                Object = jsonContents
                Success = true
            }
        with
        | :? JsonException as ex ->
            {
                DtoValidResult.Errors = [ (sprintf "File \"%s\" doesn't contains valid Json" fileName) ]
                Object = [||]
                Success = false
            }
        | :? IOException as ex ->
            {
                DtoValidResult.Errors = [ (sprintf "File \"%s\" was unable to be read" fileName) ]
                Object = [||]
                Success = false
            }

let ReadGameFiles (basePath: string): RawDtoPhase0 =
    let combinePaths fileName = System.IO.Path.Combine(System.IO.Path.GetFullPath(basePath), fileName)
    {
        RawDtoPhase0.Tiles = ReadJsonFile <| combinePaths "tiles.json"
        TileFeatures = ReadJsonFile <| combinePaths "tilefeatures.json"
        Races = ReadJsonFile <| combinePaths "races.json"
        RaceModifiers = ReadJsonFile <| combinePaths "racemodifiers.json"
        MonsterGenerationParameters = ReadJsonFile <| combinePaths "monstergenerationparameters.json"
        Items = ReadJsonFile <| combinePaths "items.json"
        FloorGenerationParameters = ReadJsonFile <| combinePaths "floorgeneration.json"
    }