module Morgemil.Data.Tests.Parser.Tests


open FSharp.Data
open Morgemil.Data
open Morgemil.Data
open Morgemil.Data.Helper
open Morgemil.Math
open Morgemil.Models
open Xunit

let isError = function | Result.Ok _ -> false | Result.Error _ -> true
let isOk = function | Result.Ok _ -> false | Result.Error _ -> true

[<Fact>]
let ``tryParseColor Tests`` () =
    let x1 = Parser.tryParseColor "one" (JsonValue.Parse "{ \"one\": [ 250, 250, 250 ] }")
    let color1 = Color.From(250)
    let expected1 = Result<Color, string>.Ok color1
    Assert.Equal(expected1, x1)
    
    let x2 = Parser.tryParseColor "one" (JsonValue.Parse "{ \"one\": [ 250, 250, 250, 255 ] }")
    Assert.Equal(expected1, x2)
    
    let x3 = Parser.tryParseColor "one" (JsonValue.Parse "{ \"one\": { \"a\": 255, \"b\": 250, \"g\": 250, \"r\": 250 } }")
    Assert.Equal(expected1, x3)
    
    let x4 = Parser.tryParseColor "one" (JsonValue.Parse "{ \"one\": { \"b\": 250, \"g\": 250, \"r\": 250 } }")
    Assert.Equal(expected1, x4)
    
    let x5 = Parser.tryParseColor "one" (JsonValue.Parse "{ }")
    Assert.True(isError x5)

