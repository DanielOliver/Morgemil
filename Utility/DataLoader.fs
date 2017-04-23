﻿namespace Morgemil.Utility

open FSharp.Data
open FSharp.Data.JsonExtensions
open Morgemil.Models

type DataLoader(baseGamePath: string) =
  let _basePath = System.IO.DirectoryInfo(baseGamePath).FullName

  member this.ValidateScenarioData(scenarioData: ScenarioData) =
    
    for index in [0..scenarioData.FloorGenerationParameters.Length-1] do
      if scenarioData.FloorGenerationParameters.[index].ID <> index then
        failwithf "Scenario FloorGenerationParameters not indexed correctly"

    for index in [0..scenarioData.Items.Length-1] do
      if scenarioData.Items.[index].ID <> index then
        failwithf "Scenario Items not indexed correctly"
        
    for index in [0..scenarioData.RaceModifiers.Length-1] do
      if scenarioData.RaceModifiers.[index].ID <> index then
        failwithf "Scenario RaceModifiers not indexed correctly"
        
    for index in [0..scenarioData.Races.Length-1] do
      let item = scenarioData.Races.[index]
      if item.ID <> index then
        failwithf "Scenario Races not indexed correctly"
        
    for index in [0..scenarioData.Tiles.Length-1] do
      if scenarioData.Tiles.[index].ID <> index then
        failwithf "Scenario Tiles not indexed correctly"


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
      with | ex -> failwithf "Failed to load JSON from (%s) with error (%A)" fileName ex

    let racesData = readText "/races.json"
    let racemodifiersData = readText "/racemodifiers.json"
    let tilesData = readText "/tiles.json"
    let itemsData = readText "/items.json"
    let floorgenerationData = readText "/floorgeneration.json"
    let racemodifierlinksData = readText "/racemodifierlinks.json"
    

    let races = racesData |> JsonLoad.LoadRaces
    let tiles = tilesData |> JsonLoad.LoadTiles
    let items = itemsData |> JsonLoad.LoadItems
    let raceModifiers = racemodifiersData |> JsonLoad.LoadRaceModifiers
    let floorGenerationParameters = JsonLoad.LoadFloorGenerationParameters(floorgenerationData, tiles)
    let raceModifierLinks = JsonLoad.LoadRaceModifierLinks( racemodifierlinksData, races, raceModifiers)

    let result = 
      { ScenarioData.Races = races
        Tiles =  tiles
        Items = items
        RaceModifiers = raceModifiers
        FloorGenerationParameters = floorGenerationParameters
        RaceModifierLinks = raceModifierLinks
        Scenario = scenario
      }
    this.ValidateScenarioData result
    result