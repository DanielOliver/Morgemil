module Morgemil.Utility.JsonLoad


open FSharp.Data
open FSharp.Data.JsonExtensions
open Morgemil.Models
open Microsoft.Xna.Framework


let CastEnum<'t>(value:JsonValue) =
  System.Enum.Parse(typeof<'t>, value.AsString()) :?> 't

let LoadIntegerArray(value: JsonValue) =
  value.AsArray() |> Array.map(fun t -> t.AsInteger())

let LoadVector2i(values: JsonValue) =
  Morgemil.Math.Vector2i.create(values?x.AsInteger(), values?y.AsInteger())
    
let LoadRectangle(values: JsonValue) =
  Morgemil.Math.Rectangle.create( LoadVector2i(values?position), LoadVector2i(values?position))
    
let LoadChar(values: JsonValue) =
  let text = values.AsString()
  if text.Length > 1 then
    match System.Int32.TryParse(text) with
    | true, textInt -> char textInt
    | false, _ -> failwithf "Failed to convert %s to a character representation" text
  else text.[0]
  
let LoadColor(values: JsonValue) =
  if values.Properties.Length = 0 then
    let text = values.InnerText().Trim()
    let convert v = System.Convert.ToInt32(text.[v..(v+1)], 16)
    let r = convert 2
    let g = convert 4
    let b = convert 6
    let a = if text.Length = 10 then convert 8
            else 255
    Some (Color(r, g, b, a))
  else
    Some (Color( values?r.AsInteger(), values?g.AsInteger(), values?b.AsInteger(), values?a.AsInteger()))

let LoadTileRepresentation(values: JsonValue) =
  { TileRepresentation.AnsiCharacter = LoadChar(values?char)
    ForegroundColor = (values.TryGetProperty "foreground") |> Option.bind(LoadColor)
    BackGroundColor = (values.TryGetProperty "background") |> Option.bind(LoadColor)
  }

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

let LoadFloorGenerationParameters(values: JsonValue, tiles: Tile []) = 
  values.AsArray()
  |> Seq.map(fun item ->
    { FloorGenerationParameter.ID = item?id.AsInteger()
      Tiles = LoadIntegerArray(item?tiles) |> Array.map(fun t -> tiles.[t])
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

let LoadRaceModifierLinks(values: JsonValue, races: Race[], raceModifiers: RaceModifier[]) =
  values.AsArray()
  |> Seq.mapi(fun index item ->
    { RaceModifierLink.ID = index
      RaceModifier = item.TryGetProperty("racemodifierid") |> Option.map(JsonExtensions.AsInteger >> (fun t -> raceModifiers.[t]))
      Race = races.[item?raceid.AsInteger()]
      Ratio = item?ratio.AsInteger()
    }
  )
  |> Seq.sortBy(fun t -> t.ID)
  |> Seq.toArray
  
let LoadRaces(values: JsonValue) =
  values.AsArray()
  |> Seq.map(fun item ->
    { Race.ID = item?id.AsInteger()
      Noun = item?noun.AsString()        
      Adjective = item?adjective.AsString()
      Description = item?description.AsString()
      Tags = LoadTags(item?tags)
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
      Representation = LoadTileRepresentation(item?representation)
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
