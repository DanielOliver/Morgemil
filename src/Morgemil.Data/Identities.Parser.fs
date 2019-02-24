module Morgemil.Data.Identities.Parser
open FSharp.Data
open Morgemil.Data
open Morgemil.Models


let tryParseTileFeatureID(value: JsonValue) =
    value
    |> Helper.tryParseLongPropertyWith "tileFeatureID" TileFeatureID
    
let tryParseRaceID(value: JsonValue) =
    value
    |> Helper.tryParseLongPropertyWith "raceID" RaceID

let tryParseRaceModifierID(value: JsonValue) =
    value
    |> Helper.tryParseLongPropertyWith "raceModifierID" RaceModifierID

let tryParseRaceModifierLinkID(value: JsonValue) =
    value
    |> Helper.tryParseLongPropertyWith "raceModifierLinkID" RaceModifierLinkID

let tryParseTileID(value: JsonValue) =
    value
    |> Helper.tryParseLongPropertyWith "tileID" TileID

let tryParseItemID(value: JsonValue) =
    value
    |> Helper.tryParseLongPropertyWith "itemID" ItemID

let tryParseFloorGenerationParameterID(value: JsonValue) =
    value
    |> Helper.tryParseLongPropertyWith "floorGenerationParameterID" FloorGenerationParameterID
