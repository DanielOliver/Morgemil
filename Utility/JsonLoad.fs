module Morgemil.Utility.JsonLoad


open FSharp.Data
open FSharp.Data.JsonExtensions
open Morgemil.Models
open Microsoft.Xna.Framework
open Morgemil.Utility.JsonHelper

let LoadVector2i(value: JsonValue) =
    json value {
        let! x = "x",AsInteger
        let! y = "y",AsInteger
        return Morgemil.Math.Vector2i.create(x, y)
    }
    
let LoadRectangle(value: JsonValue) =
    json value {
        let! position = OptionalResult("position",LoadVector2i)
        let! size = "size",LoadVector2i
        return 
            match position with
            | Some x -> Morgemil.Math.Rectangle.create(x,size)
            | None -> Morgemil.Math.Rectangle.create(size)
    }

let AsChar (value: JsonValue) =
    value 
    |> AsString 
    |> Option.map(fun text ->
        if text.StartsWith("0x") then char(System.Convert.ToInt32(text, 16))
        else text.[0]
    )
  
let AsColor(value: JsonValue) =
    match value with
    | JsonValue.String text -> 
        let text = text.Trim()
        let convert v = System.Convert.ToInt32(text.[v..(v+1)], 16)
        let r = convert 2
        let g = convert 4
        let b = convert 6
        let a = if text.Length = 10 then convert 8
                else 255
        Some (Color(r, g, b, a))
    | JsonValue.Record _ -> 
        json value {  
            let! r = "r",AsInteger
            let! g = "g",AsInteger
            let! b = "b",AsInteger
            let! a = Optional("a",AsInteger) 
            return Color(r , g,b,defaultArg a 255)
        } |> function
                | Ok x -> Some x
                | Error _ -> None
    | _ -> None

let AsTileRepresentation value =
    json value {
        let! representation = "char",AsChar
        let! foreground = Optional("foreground",AsColor)
        let! background = Optional("background",AsColor)
        return {   
            TileRepresentation.AnsiCharacter = representation
            BackGroundColor = background
            ForegroundColor = foreground
        }
    }

let AsTag (name, value) =
    match TagType.TryParse(name, true) with
    | true, tagType ->
        match tagType with
        | TagType.PlayerOption -> Ok (tagType, Tag.PlayerOption)
        | _ -> Error (JsonError.UnexpectedType(name, value))
    | _ -> Error (JsonError.UnexpectedType(name, value))

let AsTagsMap(values: JsonValue): Result<Map<TagType, Tag>,JsonError> = 
    values
    |> PropertiesAsType AsTag
    |> (fun t -> match t with
        | Ok x -> x |> Map.ofArray |> Ok
        | Error err -> Error err)

let LoadFloorGenerationParameters(value: JsonValue) = 
    value
    |> ArrayAsType(fun value -> 
        json value {
            let! id = "id",AsInteger
            let! tileIds = "tiles",(ArrayAsType AsInteger)
            let! sizeRange = "sizerange",LoadRectangle
            let! strategy = "strategy",AsEnum<FloorGenerationStrategy>
            let! tagsMap = "tags",AsTagsMap
            return {
                FloorGenerationParameter.ID = id
                Tiles = tileIds
                SizeRange = sizeRange
                Strategy = strategy
                Tags = tagsMap
            }
        }
    )

let LoadScenario (basePath: string) (value: JsonValue) =
    json value {
        let! version = "version",AsString
        let! date = "date",AsDateTime
        let! name = "name",AsString
        let! description = "description",AsString
        return {
            Scenario.BasePath = basePath
            Version = version
            Name = name
            Date = date
            Description = description
        }
    }

let LoadRaceModifierLinks(value: JsonValue) =
  value
  |> ArrayAsType( )
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
    json item {
        let! id = "id",AsInteger
        let! tileType = "tiletype",AsEnum<TileType>
        let! name = "name",AsString
        let! description = "description",AsString
        let! blocksMovement = "blocksmovement",AsBoolean
        let! blocksSight = "blockssight",AsBoolean
        let! tileRepresentation = "representation",AsTileRepresentation
        let! tagsMap = "tags",AsTagsMap

        return {
            Tile.ID = id
            TileType = tileType
            Name = name
            Description = description
            BlocksMovement = blocksMovement
            BlocksSight = blocksSight
            Tags = tagsMap
            Representation = tileRepresentation
        }
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
