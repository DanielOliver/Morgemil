module Morgemil.Data.JsonReader

open Morgemil.Data.DTO
open System.IO

let ReadJsonFile<'T> (fileName: string) : DtoValidResult<'T[]> =
    if not <| File.Exists(fileName) then
        { DtoValidResult.Errors = [ $"File \"%s{fileName}\" doesn't exist " ]
          Object = [||]
          Success = false }
    else
        try
            let fileContents = File.ReadAllText fileName

            let jsonContents =
                System.Text.Json.JsonSerializer.Deserialize<'T[]>(fileContents, JsonSettings.options)

            { DtoValidResult.Errors = List.empty
              Object = jsonContents
              Success = true }
        with
        | :? System.Text.Json.JsonException as ex ->
            { DtoValidResult.Errors = [ $"File \"%s{fileName}\" doesn't contains valid Json. %A{ex}" ]
              Object = [||]
              Success = false }
        | :? IOException as ex ->
            { DtoValidResult.Errors = [ $"File \"%s{fileName}\" was unable to be read" ]
              Object = [||]
              Success = false }

let ReadGameFiles (basePath: string) : RawDtoPhase0 =
    let combinePaths fileName =
        Path.Combine(Path.GetFullPath(basePath), fileName)

    { RawDtoPhase0.Tiles = ReadJsonFile <| combinePaths "tiles.json"
      TileFeatures = ReadJsonFile <| combinePaths "tilefeatures.json"
      Ancestries = ReadJsonFile <| combinePaths "ancestries.json"
      Heritages = ReadJsonFile <| combinePaths "heritages.json"
      MonsterGenerationParameters = ReadJsonFile <| combinePaths "monstergenerationparameters.json"
      Items = ReadJsonFile <| combinePaths "items.json"
      FloorGenerationParameters = ReadJsonFile <| combinePaths "floorgeneration.json"
      Aspects = ReadJsonFile <| combinePaths "aspects.json"
      Towers = ReadJsonFile <| combinePaths "towers.json" }
