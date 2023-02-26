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
            .ToJsonSerializerOptions()

    options.NumberHandling <- JsonNumberHandling.AllowReadingFromString
    options.PropertyNameCaseInsensitive <- true
    options

let options = createOptions ()
