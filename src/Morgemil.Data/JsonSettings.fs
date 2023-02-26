module Morgemil.Data.JsonSettings

open System.Text.Json.Serialization

let options =
    JsonFSharpOptions.Default()
        .WithUnionUnwrapFieldlessTags()
        .WithSkippableOptionFields()
        .WithUnionTagCaseInsensitive()
        .WithUnionExternalTag()
        .WithUnionUnwrapRecordCases()
        .ToJsonSerializerOptions()
do
    options.NumberHandling <- JsonNumberHandling.AllowReadingFromString
    options.PropertyNameCaseInsensitive <- true

