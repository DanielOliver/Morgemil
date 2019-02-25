module Morgemil.Data.Tests.Translation.Tests

open Morgemil.Data
open Morgemil.Math
open Xunit

let isError = function | Result.Ok _ -> false | Result.Error _ -> true
let isOk = function | Result.Ok _ -> false | Result.Error _ -> true

[<Fact>]
let ``tryParseColor Tests`` () =
    let s1 = """{ "A": 255, "B": 253, "G": 252, "R": 249 } """
    let deserial1 = Newtonsoft.Json.JsonConvert.DeserializeObject<DTO.Color> s1            
    let expected1 = Color.From(249, 252, 253, 255) |> Translation.ColorToDto
    Assert.Equal(expected1, deserial1)
    

