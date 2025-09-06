module Morgemil.Data.JsonSettings

open System.Text.Json.Serialization

let createOptions () =
    let options =
        JsonFSharpOptions
            .Default()
            .WithUnionUnwrapFieldlessTags()
            .WithSkippableOptionFields()
            .WithUnionTagCaseInsensitive()
            .WithUnionExternalTag()
            .WithUnionUnwrapRecordCases()
            .WithUnionUnwrapSingleFieldCases()
            //default includes .WithUnionUnwrapSingleCaseUnions()
            .ToJsonSerializerOptions()

    options.NumberHandling <- JsonNumberHandling.AllowReadingFromString
    options.PropertyNameCaseInsensitive <- true
    options

let options = createOptions ()
