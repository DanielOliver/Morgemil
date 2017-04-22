namespace Morgemil.Utility

open Morgemil.Models
open FSharp.Data
open FSharp.Data.JsonExtensions

type ScenarioData =
  { Races: Race []
    Tiles: Tile []
    Items: Item []
  }


type DataLoader(baseGamePath: string) =
  let _basePath = System.IO.DirectoryInfo(baseGamePath).FullName

  let _loadScenario(values: JsonValue, basePath: string) =
    { Morgemil.Models.Scenario.BasePath = basePath
      Version = values?version.AsString()
      Date = values?date.AsDateTime()
      Name = values?name.AsString()
      Description = values?description.AsString()
    }

  let _loadTags(values: JsonValue): Map<TagType, Tag> = 
    Map.empty

  let _loadRaces(values: JsonValue) =
    values.AsArray()
    |> Seq.map(fun item ->
      { Race.ID = item?id.AsInteger()
        Noun = item?isunique.AsString()        
        Adjective = item?adjective.AsString()
        Description = item?description.AsString()
        Tags = _loadTags(item?tags)
      }
    )
    |> Seq.toArray

  member this.LoadScenarios() =
    let files = System.IO.DirectoryInfo(_basePath).GetDirectories() |> Array.collect(fun t -> t.GetFiles("game.json"))
    if files.Length = 0 then
      failwithf "No scenarios to load from %s" _basePath
    files
    |> Seq.map(fun gameFileInfo -> 
      let fileContents = System.IO.File.ReadAllText gameFileInfo.FullName
      let json = JsonValue.Parse(fileContents)
      _loadScenario(json, gameFileInfo.DirectoryName)
      )
    |> Seq.toList

    
  member this.LoadScenario(scenario: Scenario) =

    let readText fileName = (scenario.BasePath + fileName) |> System.IO.File.ReadAllText

    let racesData = readText "/races.json"
    let tilesData = readText "/tiles.json"
    let itemsData = readText "/items.json"

    { ScenarioData.Races = racesData |> JsonValue.Parse |> _loadRaces
      Tiles = [||]
      Items = [||]
    }
    

