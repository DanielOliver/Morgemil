module Morgemil.Data.Translation

open System
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
    
let TileFromDto (tile: DTO.Tile): Result<Tile, string> =
    match tile.TileType with
    | DTO.TileType.Void -> TileType.Void |> Ok
    | DTO.TileType.Ground -> TileType.Ground |> Ok
    | DTO.TileType.Solid -> TileType.Solid |> Ok
    | _ as x -> sprintf "Unrecognized TileType: %A for TileID: %i" tile.TileType tile.ID |> Error
    |> Result.map(fun tileType ->
        {
            ID = TileID tile.ID
            Name = tile.Name
            Description = tile.Description
            Representation = TileRepresentationFromDto tile.Representation
            BlocksSight = tile.BlocksSight
            BlocksMovement = tile.BlocksMovement
            TileType = tileType
        }
    )
    