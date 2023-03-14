module Morgemil.Data.Validator

open System.Text.Json
open System.Text.Json.Nodes
open Morgemil.Core
open Morgemil.Data.DTO
open Morgemil.Data.Translation
open Morgemil.Models.Relational

/// Expects the value to be an unique occurence
let private ExpectedUnique<'T>
    (currentItem: 'T)
    (property: 'T -> _)
    (propertyName: string)
    (items: 'T DtoValidResult list)
    : string option =
    let currentItemProperty = property currentItem

    items
    |> Seq.tryFind (fun x -> property x.Object = currentItemProperty)
    |> Option.map (fun duplicate -> sprintf "Expected Unique %s: %A" propertyName currentItemProperty)

///The enumeration value should be defined
let inline private DefinedEnum< ^T> (value: ^T) : string option =
    if System.Enum.IsDefined(typeof< ^T>, value) then
        None
    else
        sprintf "Value %A is not defined for enum %s" value typeof<'T>.Name |> Some

let private DefinedMorTag (title: string) (value: JsonNode) : string option =
    try
        let tag = FromDTO.ParseMorTag title value

        match tag with
        | Morgemil.Models.MorTags.Custom ->
            if value.ToString() |> System.String.IsNullOrWhiteSpace then
                None
            else
                Some(sprintf "%s: Values on custom tags is not accepted for validation purposes." title)
        | _ -> None
    with :? JsonException as ex ->
        Some(sprintf "Tag %s failed. %A" title ex)

let private ExpectDefinedMorTags (values: Map<string, JsonNode>) : string option list =
    values |> Seq.map (fun kv -> DefinedMorTag kv.Key kv.Value) |> Seq.toList


let private ExpectDefinedMorTagsOption (values: Map<string, JsonNode> option) : string option list =
    values |> Option.map (ExpectDefinedMorTags) |> Option.defaultValue List.empty

///Checks if an id exists in a readonly table.
let private ExistsInTable (id: int64) (propertyName: string) (table: IReadonlyTable<_, int64>) : string option =
    match table.TryGetRow id with
    | Some _ -> None
    | None -> sprintf "%s with ID %i doesn't exist" propertyName id |> Some

///Checks if an id exists in a readonly table if the id isn't null.
let private ExistsInTableIfValue
    (id: System.Nullable<int64>)
    (propertyName: string)
    (table: IReadonlyTable<_, int64>)
    : string option =
    match id.HasValue with
    | false -> None
    | true ->
        match table.TryGetRow id.Value with
        | Some _ -> None
        | None -> sprintf "%s with ID %i doesn't exist" propertyName id.Value |> Some


///Checks if a list of values satisfy a condition
let private AllSatisfyCondition<'T> (values: 'T list) (description: string) (condition: 'T -> bool) : string option =
    values
    |> Seq.filter (condition >> not)
    |> Seq.map (fun t -> t.ToString())
    |> (fun t -> System.String.Join(",", t))
    |> function
        | x when x.Length = 0 -> None
        | x -> sprintf "Values [ %s ] doesn't satisfy condition: %s" x description |> Some

///Checks if a list of ids exists in a readonly table.
let private AllExistsInTable
    (ids: int64 list)
    (propertyName: string)
    (table: IReadonlyTable<_, int64>)
    : string option =
    ids
    |> List.filter (table.TryGetRow >> Option.isNone)
    |> (fun t -> System.String.Join(",", t))
    |> function
        | x when x.Length = 0 -> None
        | x -> sprintf "%s with IDs [ %s ] doesn't exist" propertyName x |> Some

///Checks if a list of ids exists in a readonly table if the id isn't null.
let private AllExistsInTableIfValue
    (ids: System.Nullable<int64> list)
    (propertyName: string)
    (table: IReadonlyTable<_, int64>)
    : string option =
    ids
    |> List.filter (function
        | x when not x.HasValue -> false
        | x -> x.Value |> table.TryGetRow |> Option.isNone)
    |> (fun t -> System.String.Join(",", t))
    |> function
        | x when x.Length = 0 -> None
        | x -> sprintf "%s with IDs [ %s ] doesn't exist" propertyName x |> Some

///Checks that each item fulfills a list of conditions
let private ValidateItems<'T> (getItemErrors: List<DtoValidResult<'T>> -> 'T -> string option list) (items: 'T[]) =
    items
    |> Seq.fold
        (fun (acc: List<DtoValidResult<'T>>) (element: 'T) ->
            let errors = getItemErrors acc element |> List.choose id

            { DtoValidResult.Object = element
              Errors = errors
              Success = errors |> List.isEmpty }
            :: acc)
        []
    |> Seq.rev
    |> Seq.toArray


///Validates game data with a set of conditions, and then returns the results and a readonly table
let private ValidateGameDataWithTable<'T when 'T :> IRow>
    (getItemErrors: List<DtoValidResult<'T>> -> 'T -> string option list)
    (item: DtoValidResult<'T[]>)
    : DtoValidResult<DtoValidResult<'T>[]> * IReadonlyTable<'T, int64> =
    let table = item.Object |> Table.CreateReadonlyTable id

    let validatedItems = item.Object |> ValidateItems getItemErrors

    { Object = validatedItems
      Success = validatedItems |> Seq.forall (fun x -> x.Success)
      Errors = [] },
    table

/// Validate Tiles
let private ValidateDtoTiles
    (item: DtoValidResult<Tile[]>)
    : DtoValidResult<DtoValidResult<Tile>[]> * IReadonlyTable<Tile, int64> =
    item
    |> ValidateGameDataWithTable(fun acc element ->
        [ ExpectedUnique element (fun x -> x.ID) "TileID" acc
          //            DefinedEnum element.TileType
          ])

/// Validate Tile Features
let private ValidateDtoTileFeatures
    (item: DtoValidResult<TileFeature[]>)
    (tileTable: IReadonlyTable<Tile, int64>)
    : DtoValidResult<DtoValidResult<TileFeature>[]> * IReadonlyTable<TileFeature, int64> =
    item
    |> ValidateGameDataWithTable(fun acc element ->
        [ ExpectedUnique element (fun x -> x.ID) "TileFeatureID" acc
          tileTable |> AllExistsInTable element.PossibleTiles "Tiles" ])

/// Validate Ancestries
let private ValidateDtoAncestries
    (item: DtoValidResult<Ancestry[]>)
    : DtoValidResult<DtoValidResult<Ancestry>[]> * IReadonlyTable<Ancestry, int64> =
    item
    |> ValidateGameDataWithTable(fun acc element ->
        element.Tags
        |> ExpectDefinedMorTagsOption
        |> List.append [ ExpectedUnique element (fun x -> x.ID) "AncestryID" acc ])

/// Validate Heritages
let private ValidateDtoHeritages
    (item: DtoValidResult<Heritage[]>)
    (ancestryTable: IReadonlyTable<Ancestry, int64>)
    : DtoValidResult<DtoValidResult<Heritage>[]> * IReadonlyTable<Heritage, int64> =
    item
    |> ValidateGameDataWithTable(fun acc element ->
        element.Tags
        |> ExpectDefinedMorTagsOption
        |> List.append [ ExpectedUnique element (fun x -> x.ID) "HeritageID" acc ])

/// Validate Monster Generation Parameters
let private ValidateDtoMonsterGenerationParameters
    (item: DtoValidResult<MonsterGenerationParameter[]>)
    : DtoValidResult<DtoValidResult<MonsterGenerationParameter>[]> * IReadonlyTable<MonsterGenerationParameter, int64> =
    item
    |> ValidateGameDataWithTable(fun acc element ->
        [ AllSatisfyCondition
              (element.GenerationRatios |> List.map (fun t -> t.Ratio))
              "Ratios should be positive values"
              (Option.map (fun v -> v >= 1) >> Option.defaultValue true)
          AllSatisfyCondition
              (element.GenerationRatios |> List.map (fun t -> t.Min))
              "Min should be positive values if defined"
              (Option.map (fun v -> v >= 0) >> Option.defaultValue true)
          AllSatisfyCondition
              (element.GenerationRatios |> List.map (fun t -> t.Max))
              "Max should be positive values if defined"
              (Option.map (fun v -> v >= 1) >> Option.defaultValue true) ])

/// Validate Items
let private ValidateDtoItems
    (item: DtoValidResult<Item[]>)
    : DtoValidResult<DtoValidResult<Item>[]> * IReadonlyTable<Item, int64> =
    item
    |> ValidateGameDataWithTable(fun acc element ->
        let itemTypeError =
            match element.ItemType, element.Weapon, element.Wearable, element.Consumable with
            | Morgemil.Models.ItemType.Weapon, Some _, _, _ -> None
            | Morgemil.Models.ItemType.Wearable, _, Some _, _ -> None
            | Morgemil.Models.ItemType.Consumable, _, _, Some _ -> None
            | _ -> sprintf "Expected ItemType %A to have associated info" element.ItemType |> Some

        [ ExpectedUnique element (fun x -> x.ID) "ItemID" acc; itemTypeError ])

/// Validate Floor Generation Parameters
let private ValidateDtoFloorGenerationParameters
    (item: DtoValidResult<FloorGenerationParameter[]>)
    (tileTable: IReadonlyTable<Tile, int64>)
    : DtoValidResult<DtoValidResult<FloorGenerationParameter>[]> * IReadonlyTable<FloorGenerationParameter, int64> =
    item
    |> ValidateGameDataWithTable(fun acc element ->
        [ ExpectedUnique element (fun x -> x.ID) "FloorGenerationParameterID" acc
          tileTable |> AllExistsInTable element.Tiles "Tiles"
          tileTable |> ExistsInTable element.DefaultTile "DefaultTile" ])

/// Validate Aspects
let private ValidateDtoAspects
    (aspect: DtoValidResult<Aspect[]>)
    : DtoValidResult<DtoValidResult<Aspect>[]> * IReadonlyTable<Aspect, int64> =
    aspect
    |> ValidateGameDataWithTable(fun acc element -> [ ExpectedUnique element (fun x -> x.ID) "AspectID" acc ])

/// Validate Towers
let private ValidateDtoTowers
    (tower: DtoValidResult<Tower[]>)
    (floorGenerationParameterTable: IReadonlyTable<FloorGenerationParameter, int64>)
    : DtoValidResult<DtoValidResult<Tower>[]> * IReadonlyTable<Tower, int64> =
    tower
    |> ValidateGameDataWithTable(fun acc element ->
        let levelRangeError =
            match element.LevelRangeInclusive with
            | r when r.X <= 0 || r.Y <= 0 || r.Y > 100 || r.X > 100 ->
                Some "Level X & Y must be between 1 and 100 inclusive."
            | r when r.X > r.Y -> Some "Level X must be equal to or less than Y."
            | _ -> None

        [ ExpectedUnique element (fun x -> x.ID) "TowerID" acc
          levelRangeError
          floorGenerationParameterTable
          |> ExistsInTable element.DefaultFloorGenerationParameters "DefaultFloorGenerationParameters" ])

/// Tie together all validation routines
let ValidateDtos (phase0: RawDtoPhase0) : RawDtoPhase1 =
    let (tileResults, tileTable) = ValidateDtoTiles phase0.Tiles

    let (tileFeatureResults, tileFeatureTable) =
        ValidateDtoTileFeatures phase0.TileFeatures tileTable

    let (ancestryResults, ancestryTable) = ValidateDtoAncestries phase0.Ancestries

    let (heritageResults, heritageTable) =
        ValidateDtoHeritages phase0.Heritages ancestryTable

    let (monsterGenerationParameterResults, monsterGenerationParametersLinkTable) =
        ValidateDtoMonsterGenerationParameters phase0.MonsterGenerationParameters

    let (itemResults, itemTable) = ValidateDtoItems phase0.Items

    let (floorGenerationParameterResults, floorGenerationParametersLinkTable) =
        ValidateDtoFloorGenerationParameters phase0.FloorGenerationParameters tileTable

    let (aspectResults, aspectTable) = ValidateDtoAspects phase0.Aspects

    let (towerResults, towerTable) =
        ValidateDtoTowers phase0.Towers floorGenerationParametersLinkTable

    { RawDtoPhase1.Tiles = tileResults
      TileFeatures = tileFeatureResults
      Ancestries = ancestryResults
      Heritages = heritageResults
      MonsterGenerationParameters = monsterGenerationParameterResults
      Items = itemResults
      FloorGenerationParameters = floorGenerationParameterResults
      Aspects = aspectResults
      Towers = towerResults }
