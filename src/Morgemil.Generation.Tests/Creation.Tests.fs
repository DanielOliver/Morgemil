module Morgemil.Generation.Tests.Creation_Tests

open Morgemil.Generation
open Xunit

let tileDtoCode =
    @"namespace Morgemil.Dto

type Tile
    { BlocksMovement: System.Boolean
      BlocksSight: System.Boolean
      Description: System.String
      ID: Morgemil.Models.TileID
      Name: System.String
      Representation: Morgemil.Models.TileRepresentation
      TileType: Morgemil.Models.TileType }
"

let ``Tile translation to DTO`` () =
    let analysis = Analysis.AnalyzeType typeof<Morgemil.Models.Tile>

    let translation =
        Creation.translateRecordToDto analysis Creation.RecordToDtoParamters.Default

    Assert.Equal<System.Object>(tileDtoCode, translation)
