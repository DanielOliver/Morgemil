namespace Morgemil.Utility

open Morgemil.Models
open FSharp.Data
open FSharp.Data.JsonExtensions

type ScenarioData =
  { Races: Race []
    Tiles: Tile []
    Items: Item []
  }


module JsonLoad =
  let CastEnum<'t>(value) =
    System.Enum.Parse(typeof<'t>, value) :?> 't

  let LoadScenario(values: JsonValue, basePath: string) =
    { Morgemil.Models.Scenario.BasePath = basePath
      Version = values?version.AsString()
      Date = values?date.AsDateTime()
      Name = values?name.AsString()
      Description = values?description.AsString()
    }

  let LoadTags(values: JsonValue): Map<TagType, Tag> = 
    values.Properties
    |> Seq.map(fun (name, json) -> 
      match name.ToLower() with
      | "playeroption" ->  
        TagType.PlayerOption, Tag.PlayerOption
      | _ -> failwithf "Unknown tag %s" name)
    |> Map.ofSeq

  let LoadRaces(values: JsonValue) =
    values.AsArray()
    |> Seq.map(fun item ->
      { Race.ID = item?id.AsInteger()
        Noun = item?noun.AsString()        
        Adjective = item?adjective.AsString()
        Description = item?description.AsString()
        Tags = LoadTags(item?tags)
        AvailableRacialModifiers = item?racialmodifiers.AsArray() |> Array.map(fun t -> t.AsInteger())
      }
    )
    |> Seq.toArray
    
  let LoadTiles(values: JsonValue) =
    values.AsArray()
    |> Seq.map(fun item ->
      { Tile.ID = item?id.AsInteger()
        TileType = item?tiletype.AsString() |> CastEnum<TileType>
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
        RangeType = values?rangetype.AsString() |> CastEnum<WeaponRangeType>
        HandCount = values?handcount.AsInteger()
        Weight = values?weight.AsDecimal()
      } |> SubItem.Weapon
    | ItemType.Wearable ->
      { Wearable.Weight = values?weight.AsDecimal()
        WearableType = values?wearabletype.AsString() |> CastEnum<WearableType>
      } |> SubItem.Wearable
    | _ -> failwithf "Undefined Sub Item Type %A" itemType

    

  let LoadItems(values: JsonValue) =
    values.AsArray()
    |> Seq.map(fun item ->
      let itemType = item?itemtype.AsString() |> CastEnum<ItemType>

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
    let tilesData = readText "/tiles.json"
    let itemsData = readText "/items.json"

    { ScenarioData.Races = racesData |> JsonLoad.LoadRaces
      Tiles =  tilesData |> JsonLoad.LoadTiles
      Items = itemsData |> JsonLoad.LoadItems
    }
    

