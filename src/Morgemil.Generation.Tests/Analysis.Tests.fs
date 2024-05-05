module Tests

open Microsoft.FSharp.Core
open Morgemil.Models
open Xunit
open Morgemil.Generation.Analysis

// [<Fact>]
let ``TileRepresentation Analysis`` () =
    let tileGenerationType = typeof<TileRepresentation>
    let analysis = AnalyzeType tileGenerationType

    match analysis with
    | AstCollectedType.MorgemilRecord record ->
        let ansiCharacter =
            { AstRecordField.FieldName = "AnsiCharacter"
              Type = AstCollectedType.System typeof<char>
              MeasureBy = None }

        let foregroundColor =
            { AstRecordField.FieldName = "ForegroundColor"
              Type =
                AstCollectedType.Generic
                    { Type = AstCollectedType.System typeof<(SadRogue.Primitives.Color option)>
                      GenericParamaterTypes = [ AstCollectedType.System typeof<SadRogue.Primitives.Color> ] }
              MeasureBy = None }

        let backgroundColor =
            { foregroundColor with
                AstRecordField.FieldName = "BackgroundColor" }

        let expected =
            { AstRecordType.ActualType = tileGenerationType
              RecordIdField = None
              Fields =
                Map
                    [ "AnsiCharacter", ansiCharacter
                      "ForegroundColor", foregroundColor
                      "BackgroundColor", backgroundColor ] }

        Assert.Equal(ansiCharacter, record.Fields["AnsiCharacter"])
        Assert.Equal(backgroundColor, record.Fields["BackgroundColor"])
        Assert.Equal(foregroundColor, record.Fields["ForegroundColor"])
        Assert.Equal(expected, record)
    | _ -> Assert.Fail "Expected a record type"

    Assert.True(true)


let systemField<'t> name =
    { AstRecordField.FieldName = name
      Type = AstCollectedType.System typeof<'t>
      MeasureBy = None }

let morgemilField<'t> name =
    { AstRecordField.FieldName = name
      Type = AstCollectedType.MorgemilBase typeof<'t>
      MeasureBy = None }

[<Fact>]
let ``Tile Analysis`` () =
    let tileType = typeof<Tile>
    let analysis = AnalyzeType tileType

    match analysis with
    | AstCollectedType.MorgemilRecord record ->
        let nameField = systemField<string> "Name"
        let descriptionField = systemField<string> "Description"
        let blocksMovementField = systemField<bool> "BlocksMovement"
        let blocksSightField = systemField<bool> "BlocksSight"

        let representationField = morgemilField<TileRepresentation> "Representation"

        let idField =
            { AstRecordField.FieldName = "ID"
              Type =
                AstCollectedType.SingleCaseUnion
                    { AstSingleCaseUnion.UnionName = "TileID"
                      CaseName = "Item"
                      Type = AstCollectedType.System typeof<System.Int64>
                      ActualType = typeof<Morgemil.Models.TileID> }
              MeasureBy = None }

        let tileTypeField =
            { AstRecordField.FieldName = "TileType"
              Type = AstCollectedType.EnumUnion typeof<TileType>
              MeasureBy = None }

        let expected =
            { AstRecordType.ActualType = tileType
              RecordIdField = Some "ID"
              Fields =
                Map
                    [ "ID", idField
                      "Name", nameField
                      "TileType", tileTypeField
                      "Description", descriptionField
                      "BlocksMovement", blocksMovementField
                      "BlocksSight", blocksSightField
                      "Representation", representationField ] }

        Assert.Equal(expected, record)
    | _ -> Assert.Fail "Expected a record type"

    Assert.True(true)
