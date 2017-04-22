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


module JsonLoad =
  let CastEnum<'t>(value:JsonValue) =
    System.Enum.Parse(typeof<'t>, value.AsString()) :?> 't

  let LoadIntegerArray(value: JsonValue) =
    value.AsArray() |> Array.map(fun t -> t.AsInteger())

  let LoadVector2i(values: JsonValue) =
    Morgemil.Math.Vector2i.create(values?x.AsInteger(), values?y.AsInteger())
    
  let LoadRectangle(values: JsonValue) =
    Morgemil.Math.Rectangle.create( LoadVector2i(values?position), LoadVector2i(values?position))
    
  let LoadTags(values: JsonValue): Map<TagType, Tag> = 
    values.Properties
    |> Seq.map(fun (name, json) -> 
      match TagType.TryParse(name, true) with
      | true, tagType ->
        match tagType with
        | TagType.PlayerOption -> tagType, Tag.PlayerOption
        | _ -> failwithf "Unknown tag (%s) of (%A)" name tagType
      | _ -> failwithf "Unknown tag (%s)" name)
    |> Map.ofSeq

  let LoadFloorGenerationParameters(values: JsonValue) = 
    values.AsArray()
    |> Seq.map(fun item ->
      { FloorGenerationParameter.ID = item?id.AsInteger()
        Tiles = LoadIntegerArray(item?tiles)
        SizeRange = LoadRectangle(item?sizerange)
        Tags = LoadTags(item?tags)
        Strategy = item?strategy |> CastEnum<FloorGenerationStrategy>
      }
    )
    |> Seq.toArray

  let LoadScenario(values: JsonValue, basePath: string) =
    { Scenario.BasePath = basePath
      Version = values?version.AsString()
      Date = values?date.AsDateTime()
      Name = values?name.AsString()
      Description = values?description.AsString()
    }

  let LoadRaces(values: JsonValue) =
    values.AsArray()
    |> Seq.map(fun item ->
      { Race.ID = item?id.AsInteger()
        Noun = item?noun.AsString()        
        Adjective = item?adjective.AsString()
        Description = item?description.AsString()
        Tags = LoadTags(item?tags)
        AvailableRacialModifiers = LoadIntegerArray(item?racialmodifiers)
      }
    ) 
    |> Seq.toArray
    
  let LoadRaceModifiers(values: JsonValue) = 
    values.AsArray()
    |> Seq.map(fun item -> 
      { RaceModifier.ID = item?id.AsInteger()
        Noun = item?noun.AsString()
        Adjective = item?adjective.AsString()
        Description = item?description.AsString()
        Tags = LoadTags(item?tags)
      }
    ) 
    |> Seq.toArray

  let LoadTiles(values: JsonValue) =
    values.AsArray()
    |> Seq.map(fun item ->
      { Tile.ID = item?id.AsInteger()
        TileType = item?tiletype |> CastEnum<TileType>
        Name = item?name.AsString()
        Description = item?description.AsString()
        BlocksMovement = item?blocksmovement.AsBoolean()
        BlocksSight = item?blockssight.AsBoolean()
        Tags = LoadTags(item?tags)
      }
    ) 
    |> Seq.toArray
    
  let LoadSubItem(values: JsonValue, itemType: ItemType) = 
    match itemType with
    | ItemType.Weapon ->
      { Weapon.BaseRange = values?baserange.AsInteger()
        RangeType = values?rangetype |> CastEnum<WeaponRangeType>
        HandCount = values?handcount.AsInteger()
        Weight = values?weight.AsDecimal()
      } |> SubItem.Weapon
    | ItemType.Wearable ->
      { Wearable.Weight = values?weight.AsDecimal()
        WearableType = values?wearabletype |> CastEnum<WearableType>
      } |> SubItem.Wearable
    | _ -> failwithf "Undefined Sub Item Type %A" itemType

    

  let LoadItems(values: JsonValue) =
    values.AsArray()
    |> Seq.map(fun item ->
      let itemType = item?itemtype |> CastEnum<ItemType>

      { Item.ID = item?id.AsInteger()
        Noun = item?noun.AsString()
        IsUnique = item?isunique.AsBoolean()
        ItemType = itemType
        Tags = LoadTags(item?tags)
        SubItem = LoadSubItem(item?subitem, itemType)
      }
    )
    |> Seq.toArray

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
    let readText fileName = (scenario.BasePath + fileName) |> System.IO.File.ReadAllText |> JsonValue.Parse

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
    

