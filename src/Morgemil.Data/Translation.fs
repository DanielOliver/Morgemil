module Morgemil.Data.Translation

open System
open Morgemil.Data
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
        AnsiCharacter = tileRepresentation.AnsiCharacter
        ForegroundColor = tileRepresentation.ForegroundColor |> Option.map ColorToDto |> Option.defaultValue (ZeroColorDto())
        BackGroundColor = tileRepresentation.BackGroundColor |> Option.map ColorToDto |> Option.defaultValue (ZeroColorDto())
    }
    
let TileRepresentationFromDto (tileRepresentation: DTO.TileRepresentation): TileRepresentation =
    {
        AnsiCharacter = tileRepresentation.AnsiCharacter
        ForegroundColor = tileRepresentation.ForegroundColor |> ColorOptionFromDto
        BackGroundColor = tileRepresentation.BackGroundColor |> ColorOptionFromDto
    }