module Morgemil.Data.JsonSettings

open System.Text.Json.Serialization

let createOptions () =
    let options =
        JsonFSharpOptions
            .Default()
            .WithUnionUnwrapFieldlessTags()
            .WithSkippableOptionFields()
            .WithUnionTagCaseInsensitive()
            .WithUnionInternalTag()
            .WithUnionUnwrapRecordCases()
            .WithUnionTagCaseInsensitive()
            .WithUnionUnwrapSingleFieldCases()
            .WithUnionUnwrapSingleCaseUnions()
            .ToJsonSerializerOptions()

    options.NumberHandling <- JsonNumberHandling.AllowReadingFromString
    options.PropertyNameCaseInsensitive <- true
    options.PropertyNamingPolicy <- System.Text.Json.JsonNamingPolicy.SnakeCaseLower
    options

let options = createOptions ()
