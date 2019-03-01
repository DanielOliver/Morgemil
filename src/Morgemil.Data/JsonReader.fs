module Morgemil.Data.JsonReader
open Morgemil.Data.DTO
open System.IO


let ReadJsonFile<'T> (fileName: string): Result<'T[], string> =
    if not <| File.Exists(fileName) then
        Error "File doesn't exist"
    else
        let fileContents = File.ReadAllText fileName
        try
            let jsonContents = Newtonsoft.Json.JsonConvert.DeserializeObject<'T[]>(fileContents)
            jsonContents |> Ok
        with
        | ex -> "Error reading file" |> Error

let ReadGameFiles (basePath: string): RawDtoLists =
    let combinePaths fileName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(basePath), fileName)
    {
        Tiles = ReadJsonFile <| combinePaths "tiles.json"
    }
