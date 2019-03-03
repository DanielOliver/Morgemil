module Morgemil.Data.Translation

open System
open Morgemil.Core
open Morgemil.Data.DTO
open Morgemil.Models
open Morgemil.Math

let private ZeroColorDto(): DTO.Color =
    {
        A = Byte.MinValue
        B = Byte.MinValue
        G = Byte.MinValue
        R = Byte.MinValue
    }

let ColorToDto (color: Color): DTO.Color =
    {
        A = color.A
        B = color.B
        G = color.G
        R = color.R
    }

let ColorFromDto (color: DTO.Color): Color =
    {
        A = color.A
        B = color.B
        G = color.G
        R = color.R
    }
    
let ColorOptionFromDto (color: DTO.Color): Color option =
    if color.A = Byte.MinValue then
        None
    else
        Some <|
        {
            A = color.A
            B = color.B
            G = color.G
            R = color.R
        }

let TileRepresentationToDto (tileRepresentation: TileRepresentation): DTO.TileRepresentation =
    {
        AnsiCharacter = (int)(Char.GetNumericValue tileRepresentation.AnsiCharacter)
        ForegroundColor = tileRepresentation.ForegroundColor |> Option.map ColorToDto |> Option.defaultValue (ZeroColorDto())
        BackGroundColor = tileRepresentation.BackGroundColor |> Option.map ColorToDto |> Option.defaultValue (ZeroColorDto())
    }
    
let TileRepresentationFromDto (tileRepresentation: DTO.TileRepresentation): TileRepresentation =
    {
        AnsiCharacter = (char)tileRepresentation.AnsiCharacter
        ForegroundColor = tileRepresentation.ForegroundColor |> ColorOptionFromDto
        BackGroundColor = tileRepresentation.BackGroundColor |> ColorOptionFromDto
    }
    
let TileFromDto (tile: DTO.Tile): Tile =
    let tileType  =
        match tile.TileType with
        | DTO.TileType.Void -> TileType.Void
        | DTO.TileType.Ground -> TileType.Ground
        | DTO.TileType.Solid -> TileType.Solid
        | _ -> TileType.Ground
    {
        ID = TileID tile.ID
        Name = tile.Name
        Description = tile.Description
        Representation = TileRepresentationFromDto tile.Representation
        BlocksSight = tile.BlocksSight
        BlocksMovement = tile.BlocksMovement
        TileType = tileType
    }
    
let RaceFromDto (getRaceModifierByID: RaceModifierID -> RaceModifier) (race: DTO.Race) : Race =
    {
        Race.ID = RaceID race.ID
        Noun = race.Noun
        Adjective = race.Adjective
        Description = race.Description
        PossibleRaceModifiers = race.PossibleRaceModifiers |> List.map (RaceModifierID >> getRaceModifierByID)
    }
    
let RaceModifierFromDto (raceModifier: DTO.RaceModifier) : RaceModifier =
    {
        RaceModifier.ID = RaceModifierID raceModifier.ID
        Noun = raceModifier.Noun
        Adjective = raceModifier.Adjective
        Description = raceModifier.Description
    }    


let TranslateFromDtosToPhase2 (dtos: RawDtoPhase0): RawDtoPhase2 =
    let tiles = dtos.Tiles.Item |> Seq.map (TileFromDto) |> Table.CreateReadonlyTable (fun (t: TileID) -> t.Key)
    let raceModifiers = dtos.RaceModifiers.Item |> Seq.map (RaceModifierFromDto) |> Table.CreateReadonlyTable (fun (t: RaceModifierID) -> t.Key)
    let races = dtos.Races.Item |> Seq.map (RaceFromDto (fun t -> raceModifiers.Item(t))) |> Table.CreateReadonlyTable (fun (t: RaceID) -> t.Key)
    
    {
        RawDtoPhase2.Tiles = tiles.Items |> Seq.toArray
        RaceModifiers = raceModifiers.Items |> Seq.toArray
        Races = races.Items |> Seq.toArray
    }

let TranslateFromDtosToScenario (dtos: RawDtoPhase0): ScenarioData =
    let tiles = dtos.Tiles.Item |> Seq.map (TileFromDto) |> Table.CreateReadonlyTable (fun (t: TileID) -> t.Key)
    let raceModifiers = dtos.RaceModifiers.Item |> Seq.map (RaceModifierFromDto) |> Table.CreateReadonlyTable (fun (t: RaceModifierID) -> t.Key)
    let races = dtos.Races.Item |> Seq.map (RaceFromDto (fun t -> raceModifiers.Item(t))) |> Table.CreateReadonlyTable (fun (t: RaceID) -> t.Key)
    
    {
        ScenarioData.Tiles = tiles
        RaceModifiers = raceModifiers
        Races = races
    }
    