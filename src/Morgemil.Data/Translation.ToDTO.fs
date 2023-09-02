module Morgemil.Data.Translation.ToDTO

open System
open Morgemil.Data.DTO
open Morgemil.Models
open Morgemil.Math
open Morgemil.Data

let private ZeroColorDto () : DTO.Color =
    { A = Byte.MinValue
      B = Byte.MinValue
      G = Byte.MinValue
      R = Byte.MinValue }

let ColorToDto (color: Color) : DTO.Color =
    { A = color.A
      B = color.B
      G = color.G
      R = color.R }

let ColorOptionToDto (color: Color option) : DTO.Color =
    match color with
    | Some x -> ColorToDto x
    | None ->
        { A = Byte.MinValue
          B = Byte.MinValue
          G = Byte.MinValue
          R = Byte.MinValue }

let Vector2iToDto (vec: Point) : DTO.Vector2i =
    { DTO.Vector2i.X = vec.X
      DTO.Vector2i.Y = vec.Y }

let RectangleToDto (rectangle: Rectangle) : DTO.Rectangle =
    { DTO.Rectangle.X = rectangle.X
      DTO.Rectangle.Y = rectangle.Y
      DTO.Rectangle.W = rectangle.Width
      DTO.Rectangle.H = rectangle.Height }

let TileRepresentationToDto (tileRepresentation: TileRepresentation) : DTO.TileRepresentation =
    { AnsiCharacter = (int) (Char.GetNumericValue tileRepresentation.AnsiCharacter)
      ForegroundColor =
        tileRepresentation.ForegroundColor
        |> Option.map ColorToDto
        |> Option.defaultValue (ZeroColorDto())
      BackGroundColor =
        tileRepresentation.BackGroundColor
        |> Option.map ColorToDto
        |> Option.defaultValue (ZeroColorDto()) }

let TileToDto (tile: Tile) : DTO.Tile =
    { Description = tile.Description
      Name = tile.Name
      BlocksMovement = tile.BlocksMovement
      BlocksSight = tile.BlocksSight
      ID = tile.ID.Key
      Representation = tile.Representation |> TileRepresentationToDto
      TileType = tile.TileType }
