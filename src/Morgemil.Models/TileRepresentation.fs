namespace Morgemil.Models

open Morgemil.Math

// [<Record>]
type TileRepresentation =
    { AnsiCharacter: char
      ForegroundColor: Color voption
      BackgroundColor: Color voption }
