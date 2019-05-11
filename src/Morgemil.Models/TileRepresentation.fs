namespace Morgemil.Models

open Morgemil.Math

[<RecordSerialization>]
type TileRepresentation =
  { AnsiCharacter: char
    ForegroundColor: Color option
    BackGroundColor: Color option
  }

