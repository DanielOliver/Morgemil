module Morgemil.Data.Tests.JsonReader.Tests


open Morgemil.Data
open Xunit

[<Fact>]
let ``Try Read Game``() =
    let basePath = "./Game/"
    let rawDtoList = JsonReader.ReadGameFiles basePath
    Assert.Empty(rawDtoList.Errors)
    Assert.True(rawDtoList.Success)
    
