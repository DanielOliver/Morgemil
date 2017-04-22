module Morgemil.Utility.JsonLoad


open FSharp.Data
open FSharp.Data.JsonExtensions
open Morgemil.Models


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
  |> Seq.sortBy(fun t -> t.ID)
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
    let raceModifiers = 
      item?racialmodifiers.AsArray() 
      |> Array.map (fun item ->
        { RaceModifierRatio.RaceModifierID = item.TryGetProperty("id") |> Option.map(JsonExtensions.AsInteger)
          Ratio = item?ratio.AsInteger()
        }
      )

    raceModifiers |> Array.sortInPlaceBy(fun t -> t.RaceModifierID)

    { Race.ID = item?id.AsInteger()
      Noun = item?noun.AsString()        
      Adjective = item?adjective.AsString()
      Description = item?description.AsString()
      Tags = LoadTags(item?tags)
      RaceModifiers = raceModifiers
    }
  )
  |> Seq.sortBy(fun t -> t.ID) 
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
  |> Seq.sortBy(fun t -> t.ID)
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
  |> Seq.sortBy(fun t -> t.ID)
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
  |> Seq.sortBy(fun t -> t.ID)
  |> Seq.toArray
