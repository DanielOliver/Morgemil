module Morgemil.Data.Translation.FromDTO

open System
open System.Collections.Generic
open System.Text.Json
open System.Text.Json.Nodes
open Microsoft.FSharp.Core
open Microsoft.FSharp.Reflection
open Morgemil.Core
open Morgemil.Data.DTO
open Morgemil.Models
open Morgemil.Math
open Morgemil.Data

///DTO to Color
let ColorFromDto (color: DTO.Color) : Color =
    Color(color.R, color.G, color.B, color.A)

let NullToOption (t: Nullable<_>) =
    if t.HasValue then Some t.Value else None

let private MorTagCases =
    FSharpType.GetUnionCases(typeof<Morgemil.Models.MorTags>)
    |> Array.map (fun x -> x.Name, x)
    |> Map.ofArray

let ParseMorTag (title: string) (value: JsonNode) : Morgemil.Models.MorTags =
    MorTagCases
    |> Map.tryFind title
    |> Option.map (fun t ->
        match t.GetFields().Length with
        | 0 ->
            JsonValue
                .Create(title)
                .Deserialize<Morgemil.Models.MorTags>(JsonSettings.options)
        | _ ->
            let clonedValueHack = value.Deserialize<JsonNode>(JsonSettings.options)
            let unionObject = JsonObject([| KeyValuePair(title, clonedValueHack) |])
            unionObject.Deserialize<Morgemil.Models.MorTags>(JsonSettings.options))
    |> Option.defaultValue Morgemil.Models.MorTags.Custom

///DTO to Vector2i
let Vector2iFromDto (vec: DTO.Vector2i) : Point = Point.create (vec.X, vec.Y)

///DTO to Rectangle
let RectangleFromDto (rectangle: DTO.Rectangle) : Rectangle =
    Rectangle(rectangle.X, rectangle.Y, rectangle.W, rectangle.H)

///DTO to Color Option
let rec ColorOptionFromDto (color: DTO.Color) : Color option =
    if color.A = Byte.MinValue then
        None
    else
        Some(ColorFromDto color)

///DTO to TileRepresentation
let TileRepresentationFromDto (tileRepresentation: DTO.TileRepresentation) : TileRepresentation =
    { AnsiCharacter = (char) tileRepresentation.AnsiCharacter
      ForegroundColor = tileRepresentation.ForegroundColor |> ColorOptionFromDto
      BackGroundColor = tileRepresentation.BackGroundColor |> ColorOptionFromDto }

///DTO to Tile
let TileFromDto (tile: DTO.Tile) : Tile =
    { ID = TileID tile.ID
      Name = tile.Name
      Description = tile.Description
      Representation = TileRepresentationFromDto tile.Representation
      BlocksSight = tile.BlocksSight
      BlocksMovement = tile.BlocksMovement
      TileType = tile.TileType }

///DTO to TileFeature
let TileFeatureFromDto (getTilebyID: TileID -> Tile) (tileFeature: DTO.TileFeature) : TileFeature =
    { ID = TileFeatureID tileFeature.ID
      Name = tileFeature.Name
      Description = tileFeature.Description
      Representation = TileRepresentationFromDto tileFeature.Representation
      BlocksSight = tileFeature.BlocksSight
      BlocksMovement = tileFeature.BlocksMovement
      PossibleTiles = tileFeature.PossibleTiles |> List.map (TileID >> getTilebyID)
      ExitPoint = tileFeature.ExitPoint
      EntryPoint = tileFeature.EntryPoint }

///DTO to Ancestry
let AncestorFromDto (ancestry: DTO.Ancestry) : Ancestry =
    { Ancestry.ID = AncestryID ancestry.ID
      Noun = ancestry.Noun
      Adjective = ancestry.Adjective
      Description = ancestry.Description
      Tags =
        ancestry.Tags
        |> Option.map (Map.map ParseMorTag)
        |> Option.defaultValue Map.empty
      RequireTags = ancestry.RequireTags |> Option.defaultValue Map.empty }

///DTO to Heritage
let HeritageFromDto (getAncestryByID: AncestryID -> Ancestry) (heritage: DTO.Heritage) : Heritage =
    { Heritage.ID = HeritageID heritage.ID
      Noun = heritage.Noun
      Adjective = heritage.Adjective
      Description = heritage.Description
      Tags =
        heritage.Tags
        |> Option.map (Map.map ParseMorTag)
        |> Option.defaultValue Map.empty
      RequireTags = heritage.RequireTags |> Option.defaultValue Map.empty }

///DTO to Item
let ItemFromDto (item: DTO.Item) : Item =
    { Item.ID = ItemID item.ID
      Noun = item.Noun
      IsUnique = item.IsUnique
      SubItem =
        match item.ItemType with
        | ItemType.Weapon ->
            { Weapon.Weight = item.Weapon.Value.Weight * 1M<Weight>
              Weapon.BaseRange = item.Weapon.Value.BaseRange * 1<TileDistance>
              Weapon.HandCount = item.Weapon.Value.HandCount * 1<HandSlot>
              Weapon.RangeType = item.Weapon.Value.RangeType }
            |> SubItem.Weapon
        | ItemType.Wearable ->
            { Wearable.Weight = item.Wearable.Value.Weight * 1M<Weight>
              Wearable.WearableType = item.Wearable.Value.WearableType }
            |> SubItem.Wearable
        | ItemType.Consumable
        | _ -> { Consumable.Uses = item.Consumable.Value.Uses } |> SubItem.Consumable }

///DTO to MonsterGenerationParameter
let MonsterGenerationParameterFromDto
    (monsterGenerationParameter: DTO.MonsterGenerationParameter)
    : MonsterGenerationParameter =
    { MonsterGenerationParameter.ID = MonsterGenerationParameterID monsterGenerationParameter.ID
      GenerationRatios =
        monsterGenerationParameter.GenerationRatios
        |> List.map (fun t ->
            { GenerationRatio.Ratio = t.Ratio
              GenerationRatio.Max = t.Max
              GenerationRatio.Min = t.Min
              Tags = t.Tags |> set }) }

///DTO to FloorGenerationParameter
let FloorGenerationParameterFromDto
    (getTileByID: TileID -> Tile)
    (floorGenerationParameter: DTO.FloorGenerationParameter)
    : FloorGenerationParameter =
    { FloorGenerationParameter.ID = FloorGenerationParameterID floorGenerationParameter.ID
      DefaultTile = floorGenerationParameter.DefaultTile |> TileID |> getTileByID
      Tiles = floorGenerationParameter.Tiles |> Seq.map (TileID >> getTileByID) |> Seq.toList
      SizeRange = floorGenerationParameter.SizeRange |> RectangleFromDto
      Strategy = floorGenerationParameter.Strategy }

///DTO to Aspect
let AspectFromDto (aspect: DTO.Aspect) : Aspect =
    { Aspect.ID = AspectID aspect.ID
      Noun = aspect.Noun
      Adjective = aspect.Adjective
      Description = aspect.Description }

///DTO to Tower
let TowerFromDto (tower: DTO.Tower) : Tower =
    { Tower.ID = TowerID tower.ID
      Name = tower.Name
      LevelRangeInclusive = tower.LevelRangeInclusive |> Vector2iFromDto
      BacktrackBehavior = tower.BacktrackBehavior
      OverworldConnection = tower.OverworldConnection
      DefaultFloorGenerationParameters = FloorGenerationParameterID tower.DefaultFloorGenerationParameters }

///DTO to Phase2
let TranslateFromDtosToPhase2 (dtos: RawDtoPhase0) : RawDtoPhase2 =
    let tiles =
        dtos.Tiles.Object
        |> Seq.map (TileFromDto)
        |> Table.CreateReadonlyTable(fun (t: TileID) -> t.Key)

    let ancestries =
        dtos.Ancestries.Object
        |> Seq.map (AncestorFromDto)
        |> Table.CreateReadonlyTable(fun (t: AncestryID) -> t.Key)

    let heritages =
        dtos.Heritages.Object
        |> Seq.map (HeritageFromDto(fun t -> ancestries.Item(t)))
        |> Table.CreateReadonlyTable(fun (t: HeritageID) -> t.Key)

    let items =
        dtos.Items.Object
        |> Seq.map (ItemFromDto)
        |> Table.CreateReadonlyTable(fun (t: ItemID) -> t.Key)

    let tileFeatures =
        dtos.TileFeatures.Object
        |> Seq.map (TileFeatureFromDto(fun t -> tiles.Item(t)))
        |> Table.CreateReadonlyTable(fun (t: TileFeatureID) -> t.Key)

    let monsterGenerationParameters =
        dtos.MonsterGenerationParameters.Object
        |> Seq.map (MonsterGenerationParameterFromDto)
        |> Table.CreateReadonlyTable(fun (t: MonsterGenerationParameterID) -> t.Key)

    let floorGenerationParameters =
        dtos.FloorGenerationParameters.Object
        |> Seq.map (FloorGenerationParameterFromDto(fun t -> tiles.Item(t)))
        |> Table.CreateReadonlyTable(fun (t: FloorGenerationParameterID) -> t.Key)

    let aspects =
        dtos.Aspects.Object
        |> Seq.map (AspectFromDto)
        |> Table.CreateReadonlyTable(fun (t: AspectID) -> t.Key)

    let towers =
        dtos.Towers.Object
        |> Seq.map TowerFromDto
        |> Table.CreateReadonlyTable(fun (t: TowerID) -> t.Key)

    { RawDtoPhase2.Tiles = tiles.Items |> Seq.toArray
      Heritages = heritages.Items |> Seq.toArray
      Ancestries = ancestries.Items |> Seq.toArray
      Items = items.Items |> Seq.toArray
      MonsterGenerationParameters = monsterGenerationParameters.Items |> Seq.toArray
      TileFeatures = tileFeatures.Items |> Seq.toArray
      FloorGenerationParameters = floorGenerationParameters.Items |> Seq.toArray
      Aspects = aspects.Items |> Seq.toArray
      Towers = towers.Items |> Seq.toArray }

///DTO to Scenario
let TranslateFromDtosToScenario (dtos: RawDtoPhase0) : ScenarioData =
    let tiles =
        dtos.Tiles.Object
        |> Seq.map (TileFromDto)
        |> Table.CreateReadonlyTable(fun (t: TileID) -> t.Key)

    let ancestries =
        dtos.Ancestries.Object
        |> Seq.map (AncestorFromDto)
        |> Table.CreateReadonlyTable(fun (t: AncestryID) -> t.Key)

    let heritages =
        dtos.Heritages.Object
        |> Seq.map (HeritageFromDto(fun t -> ancestries.Item(t)))
        |> Table.CreateReadonlyTable(fun (t: HeritageID) -> t.Key)

    let items =
        dtos.Items.Object
        |> Seq.map (ItemFromDto)
        |> Table.CreateReadonlyTable(fun (t: ItemID) -> t.Key)

    let monsterGenerationParameters =
        dtos.MonsterGenerationParameters.Object
        |> Seq.map (MonsterGenerationParameterFromDto)
        |> Table.CreateReadonlyTable(fun (t: MonsterGenerationParameterID) -> t.Key)

    let tileFeatures =
        dtos.TileFeatures.Object
        |> Seq.map (TileFeatureFromDto(fun t -> tiles.Item(t)))
        |> Table.CreateReadonlyTable(fun (t: TileFeatureID) -> t.Key)

    let floorGenerationParameters =
        dtos.FloorGenerationParameters.Object
        |> Seq.map (FloorGenerationParameterFromDto(fun t -> tiles.Item(t)))
        |> Table.CreateReadonlyTable(fun (t: FloorGenerationParameterID) -> t.Key)

    let aspects =
        dtos.Aspects.Object
        |> Seq.map AspectFromDto
        |> Table.CreateReadonlyTable(fun (t: AspectID) -> t.Key)

    let towers =
        dtos.Towers.Object
        |> Seq.map TowerFromDto
        |> Table.CreateReadonlyTable(fun (t: TowerID) -> t.Key)

    { ScenarioData.Tiles = tiles
      Heritages = heritages
      Ancestries = ancestries
      Items = items
      MonsterGenerationParameters = monsterGenerationParameters
      TileFeatures = tileFeatures
      FloorGenerationParameters = floorGenerationParameters
      Aspects = aspects
      Towers = towers }
