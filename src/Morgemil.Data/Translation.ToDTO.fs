module Morgemil.Data.Translation.ToDTO

open System
open Morgemil.Data.DTO
open Morgemil.Models
open Morgemil.Math
open Morgemil.Data

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


///DTO to TileRepresentation
let TileRepresentationToDto (tileRepresentation: TileRepresentation): DTO.TileRepresentation =
    {
        AnsiCharacter = (int)(Char.GetNumericValue tileRepresentation.AnsiCharacter)
        ForegroundColor = tileRepresentation.ForegroundColor |> Option.map ColorToDto |> Option.defaultValue (ZeroColorDto())
        BackGroundColor = tileRepresentation.BackGroundColor |> Option.map ColorToDto |> Option.defaultValue (ZeroColorDto())
    }


