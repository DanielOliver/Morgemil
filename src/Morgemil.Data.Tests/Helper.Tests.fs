module Morgemil.Data.Helper.Tests

open FSharp.Data
open Morgemil.Data.Helper
open Morgemil.Models
open Xunit

let isError = function | Result.Ok _ -> false | Result.Error _ -> true
let isOk = function | Result.Ok _ -> false | Result.Error _ -> true

[<Fact>]
let ``optionToResult Tests`` () =
    let x1 = optionToResult "message" None
    let expected1 = Result<obj, string>.Error "message" 
    Assert.Equal(expected1, x1)
    
    let x2 = optionToResult "message" (Some 5)
    let expected2 = Result<int, string>.Ok 5 
    Assert.Equal(expected2, x2)

[<Fact>]
let ``resultWithDefault Tests`` () =
    let x1 = resultWithDefault 200 (Result.Ok 255)
    let expected1 = 255
    Assert.Equal(expected1, x1)
    
    let x2 = resultWithDefault 200 (Result.Error "something")
    let expected2 = 200
    Assert.Equal(expected2, x2)

[<Fact>]
let ``expectedProperty Tests`` () =
    let x1 = expectedProperty "one" (Some 5)
    let expected1 = Result<int, string>.Ok 5
    Assert.Equal(expected1, x1)
    
    let x2 = expectedProperty "one" (None)
    Assert.True(isError x2)

[<Fact>]
let ``tryParseLongProperty Tests`` () =
    let x1 = tryParseLongProperty "one" (JsonValue.Parse "{ \"one\": 25 }")
    let expected1 = Result<int64, string>.Ok 25L
    Assert.Equal(expected1, x1)
    
    let x2 = tryParseLongProperty "one" (JsonValue.Parse "{ }")
    Assert.True(isError x2)
    
[<Fact>]
let ``tryParseIntProperty Tests`` () =
    let x1 = tryParseIntProperty "one" (JsonValue.Parse "{ \"one\": 25 }")
    let expected1 = Result<int, string>.Ok 25
    Assert.Equal(expected1, x1)
    
    let x2 = tryParseIntProperty "one" (JsonValue.Parse "{ }")
    Assert.True(isError x2)
    
    
[<Fact>]
let ``tryParseCharProperty Tests`` () =
    let x1 = tryParseCharProperty "one" (JsonValue.Parse "{ \"one\": \"hello\" }")
    let expected1 = Result<char, string>.Ok 'h'
    Assert.Equal(expected1, x1)
    
    let x2 = tryParseCharProperty "one" (JsonValue.Parse "{ }")
    Assert.True(isError x2)
    
    let x3 = tryParseCharProperty "one" (JsonValue.Parse "{ \"one\": \"\" }")
    Assert.True(isError x3)
    
    let x4 = tryParseCharProperty "one" (JsonValue.Parse "{ \"one\": 32 }")
    let expected4 = Result<char, string>.Ok ' '
    Assert.Equal(expected4, x4)
    
    let x5 = tryParseCharProperty "one" (JsonValue.Parse "{ \"one\": [] }")
    Assert.True(isError x5)
    
    let x6 = tryParseCharProperty "one" (JsonValue.Parse "{ \"two\": [] }")
    Assert.True(isError x6)
    

[<Fact>]
let ``tryParseStringProperty Tests`` () =
    let x1 = tryParseStringProperty "one" (JsonValue.Parse "{ \"one\": \"hello\" }")
    let expected1 = Result<string, string>.Ok "hello"
    Assert.Equal(expected1, x1)
    
    let x2 = tryParseStringProperty "one" (JsonValue.Parse "{ }")
    Assert.True(isError x2)

[<Fact>]
let ``tryParseLongPropertyWith Tests`` () =
    let x1 = tryParseLongPropertyWith "one" TileFeatureID (JsonValue.Parse "{ \"one\": 25 }")
    let expected1 = Result<TileFeatureID, string>.Ok (TileFeatureID 25L)
    Assert.Equal(expected1, x1)
    
    let x2 = tryParseLongPropertyWith "one" TileFeatureID (JsonValue.Parse "{ }")
    Assert.True(isError x2)
    