namespace Morgemil.Utility

open FSharp.Data
open FSharp.Data.JsonExtensions
open Morgemil.Models

type ScenarioData =
  { Races: Race []
    Tiles: Tile []
    Items: Item []
    RaceModifiers: RaceModifier []
    FloorGenerationParameters: FloorGenerationParameter []
  }

type DataLoader(baseGamePath: string) =
  let _basePath = System.IO.DirectoryInfo(baseGamePath).FullName


  member this.LoadScenarios() =
    let files = System.IO.DirectoryInfo(_basePath).GetDirectories() |> Array.collect(fun t -> t.GetFiles("game.json"))
    if files.Length = 0 then
      failwithf "No scenarios to load from %s" _basePath
    files
    |> Seq.map(fun gameFileInfo -> 
      let fileContents = System.IO.File.ReadAllText gameFileInfo.FullName
      let json = JsonValue.Parse(fileContents)
      JsonLoad.LoadScenario(json, gameFileInfo.DirectoryName)
      )
    |> Seq.toList
    
  member this.LoadScenario(scenario: Scenario) =
    let readText fileName = 
      try (scenario.BasePath + fileName) |> System.IO.File.ReadAllText |> JsonValue.Parse
      with | ex -> failwithf "Failed to load JSON from (%s)" fileName

    let racesData = readText "/races.json"
    let racemodifiersData = readText "/racemodifiers.json"
    let tilesData = readText "/tiles.json"
    let itemsData = readText "/items.json"
    let floorgenerationData = readText "/floorgeneration.json"

    { ScenarioData.Races = racesData |> JsonLoad.LoadRaces
      Tiles =  tilesData |> JsonLoad.LoadTiles
      Items = itemsData |> JsonLoad.LoadItems
      RaceModifiers = racemodifiersData |> JsonLoad.LoadRaceModifiers
      FloorGenerationParameters = floorgenerationData |> JsonLoad.LoadFloorGenerationParameters
    }
