module Morgemil.Utility.JsonLoad


open FSharp.Data
open FSharp.Data.JsonExtensions
open Morgemil.Models
open Microsoft.Xna.Framework
open Morgemil.Utility.JsonHelper

let JsonAsVector2i(value: JsonValue) =
    json value {
        let! x = "x",JsonAsInteger
        let! y = "y",JsonAsInteger
        return Morgemil.Math.Vector2i.create(x, y)
    }
    
let JsonAsRectangle(value: JsonValue) =
    json value {
        let! position = Optional("position",JsonAsVector2i)
        let! size = "size",JsonAsVector2i
        return 
            match position with
            | Some x -> Morgemil.Math.Rectangle.create(x,size)
            | None -> Morgemil.Math.Rectangle.create(size)
    }

let JsonAsChar (value: JsonValue) =
    value 
    |> JsonAsString 
    |> Result.map(fun text ->
        if text.StartsWith("0x") then char(System.Convert.ToInt32(text, 16))
        else text.[0]
    )
  
let JsonAsColor(value: JsonValue) =
    let error = Error (JsonError.UnexpectedType("color", value))
    match value with
    | JsonValue.String text -> 
        let text = text.Trim()
        match text.Length with
        | 8 | 10 ->
            let convert v = System.Convert.ToInt32(text.[v..(v+1)], 16)
            let r = convert 2
            let g = convert 4
            let b = convert 6
            let a = if text.Length = 10 then convert 8
                    else 255
            Ok (Color(r, g, b, a))
        | _ -> error
    | JsonValue.Record _ -> 
        json value {  
            let! r = "r",JsonAsInteger
            let! g = "g",JsonAsInteger
            let! b = "b",JsonAsInteger
            let! a = Optional("a",JsonAsInteger)
            return Color(r , g,b,defaultArg a 255)
        }
    | _ -> error

let JsonAsTileRepresentation value =
    json value {
        let! representation = "char",JsonAsChar
        let! foreground = Optional("foreground",JsonAsColor)
        let! background = Optional("background",JsonAsColor)
        return {   
            TileRepresentation.AnsiCharacter = representation
            BackGroundColor = background
            ForegroundColor = foreground
        }
    }

let JsonAsTag name value =
    match TagType.TryParse(name, true) with
    | true, tagType ->
        match tagType with
        | TagType.PlayerOption -> Ok (tagType, Tag.PlayerOption)
        | _ -> Error (JsonError.UnexpectedType(name, value))
    | _ -> Error (JsonError.UnexpectedType(name, value))

let JsonAsTagsMap(values: JsonValue): Result<Map<TagType, Tag>,JsonError> = 
    values
    |> JsonAsProperties(fun (name, value) -> JsonAsTag name value)
    |> Result.map (Map.ofArray)

let JsonAsFloorGenerationParameters(value: JsonValue) = 
    value
    |> JsonAsArray(fun value -> 
        json value {
            let! id = "id",JsonAsInteger
            let! tileIds = "tiles",(JsonAsArray JsonAsInteger)
            let! sizeRange = "sizerange",JsonAsRectangle
            let! strategy = "strategy",JsonAsEnum<FloorGenerationStrategy>
            let! tagsMap = "tags",JsonAsTagsMap
            return {
                FloorGenerationParameter.ID = id
                Tiles = tileIds
                SizeRange = sizeRange
                Strategy = strategy
                Tags = tagsMap
            }
        }
    )

let JsonAsScenario (basePath: string) (value: JsonValue) =
    json value {
        let! version = "version",JsonAsString
        let! date = "date",JsonAsDateTime
        let! name = "name",JsonAsString
        let! description = "description",JsonAsString
        return {
            Scenario.BasePath = basePath
            Version = version
            Name = name
            Date = date
            Description = description
        }
    }

let JsonAsRaceModifierLinks(value: JsonValue) =
    value
    |> JsonAsArrayI(fun index value ->
        json value {
            let! raceModifierID = Optional("racemodifierid",JsonAsInteger)
            let! raceID = "raceid",JsonAsInteger
            let! ratio = "ratio",JsonAsInteger
            return {
                RaceModifierLink.ID = index
                RaceModifierID = raceModifierID
                RaceID = raceID
                Ratio = ratio
            }
        }
    )
      
let JsonAsRaces(value: JsonValue) =
    value
    |> JsonAsArray(fun value ->
        json value {
            let! raceID = "id",JsonAsInteger
            let! noun = "noun",JsonAsString
            let! adjective = "adjective",JsonAsString
            let! description = "description",JsonAsString
            let! tags = "tags",JsonAsTagsMap
            return {
                Race.ID = raceID
                Noun = noun
                Adjective = adjective
                Description = description
                Tags = tags
            }
        }
    )
    
let JsonAsRaceModifiers(value: JsonValue) = 
    value
    |> JsonAsArray(fun value ->
        json value {
            let! raceModifierID = "id",JsonAsInteger
            let! noun = "noun",JsonAsString
            let! adjective = "adjective",JsonAsString
            let! description = "description",JsonAsString
            let! tags = "tags",JsonAsTagsMap
            return {
                RaceModifier.ID = raceModifierID
                Noun = noun
                Adjective = adjective
                Description = description
                Tags = tags
            }
        }
    )

let JsonAsTiles(value: JsonValue) =
  value
  |>JsonAsArray(fun item ->
    json item {
        let! id = "id",JsonAsInteger
        let! tileType = "tiletype",JsonAsEnum<TileType>
        let! name = "name",JsonAsString
        let! description = "description",JsonAsString
        let! blocksMovement = Optional("blocksmovement",JsonAsBoolean)
        let! blocksSight = Optional("blockssight",JsonAsBoolean)
        let! tileRepresentation = "representation",JsonAsTileRepresentation
        let! tagsMap = "tags",JsonAsTagsMap
        return {
            Tile.ID = id
            TileType = tileType
            Name = name
            Description = description
            BlocksMovement = defaultArg blocksMovement false
            BlocksSight = defaultArg blocksSight false
            Tags = tagsMap
            Representation = tileRepresentation
        }
    }
  )

  
let JsonAsSubItem (itemType: ItemType) (value: JsonValue) = 
    match itemType with
    | ItemType.Weapon -> 
        json value {
            let! baseRange = "baserange",JsonAsInteger
            let! rangeType = "rangetype",JsonAsEnum<WeaponRangeType>
            let! handCount = "handcount",JsonAsInteger
            let! weight = "weight",JsonAsDecimal
            return {
                Weapon.BaseRange = baseRange
                RangeType = rangeType
                HandCount = handCount
                Weight = weight
            } |> SubItem.Weapon
        }
    | ItemType.Wearable -> 
        json value {
            let! wearableType = "wearabletype",JsonAsEnum<WearableType>
            let! weight = "weight",JsonAsDecimal
            return {
                Wearable.Weight = weight
                WearableType = wearableType
            } |> SubItem.Wearable
        }
    | ItemType.Consumable -> 
        json value {
            let! uses = "uses",JsonAsInteger
            return {
                Consumable.Uses = uses
            } |> SubItem.Consumable
        }
    | _ -> Error (JsonError.UnexpectedType("subitem", value))

let JsonAsItems (value: JsonValue) =
    value
    |> JsonAsArray(fun item ->
        json value {
            let! itemID = "id",JsonAsInteger
            let! noun = "noun",JsonAsString
            let! isUnique = "isunique",JsonAsBoolean
            let! itemType = "itemtype",JsonAsEnum<ItemType>
            let! subItem = "subitem",(JsonAsSubItem itemType)
            let! tags = "tags",JsonAsTagsMap
            return {
                Item.ID = itemID
                Noun = noun
                IsUnique = isUnique
                ItemType = itemType
                SubItem = subItem
                Tags = tags
            }
        }
    )
    